using HelperExtensions;
using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;
using SQLiteCommands.Enums;
using SQLiteCommands.Exceptions;
using SQLiteCommands.Helpers;
using System.Data.SQLite;
using System.Reflection;
using System.Text;

namespace SQLiteCommands.Services;

// ReSharper disable once InconsistentNaming
internal static class CommandGeneratorService
{
    #region Public Methods

    #region GenerateDeleteCommand
    /// <summary>
    /// Generates a DELETE command for the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException"></exception>
    public static SQLiteCommand GenerateDeleteCommand<T>(T data)
    {
        SQLiteCommand command = new();
        Type type = typeof(T);
        TableAttribute table = AttributeHelper.GetTableAttribute(type);
        StringBuilder commandText = new($"DELETE FROM {table.Name} WHERE 1 = 1");
        int columnCount = 0;

        type.GetProperties().ForEach(propInfo =>
        {
            if (AttributeHelper.GetPropertyAttribute<SQLiteField>(propInfo) is
                { IsPrimaryKey: true, IgnoreOnDelete: false } field
                && GetPropertyValue(data, propInfo, field) is { } propValue)
            {
                AddFilterData(commandText, command.Parameters, field.Name, propValue);
                columnCount++;
            }
        });

        // No valid columns found.
        if (columnCount == 0)
            throw new InvalidTypeException(
                $"No eligible primary key {nameof(ColumnAttribute)} or {nameof(ForeignKeyColumnAttribute)} found among current data properties.");

        command.CommandText = commandText.ToString();
        return command;
    }
    #endregion

    #region GenerateInsertCommand
    /// <summary>
    /// Generates a INSERT command for the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException"></exception>
    public static SQLiteCommand GenerateInsertCommand<T>(T data)
    {
        Type type = typeof(T);
        TableAttribute table = AttributeHelper.GetTableAttribute(type);
        SQLiteCommand command = new();
        InsertOptionsAttribute insertOptions = AttributeHelper.GetTypeAttribute<InsertOptionsAttribute>(type);
        StringBuilder commandText =
            new($"INSERT OR {(insertOptions?.ReplaceOnConflict == true ? "REPLACE" : "ROLLBACK")} INTO {table.Name} (");
        StringBuilder valuesText = new("VALUES (");
        int columnCount = 0;

        type.GetProperties().ForEach(propInfo =>
        {
            if (AttributeHelper.GetPropertyAttribute<SQLiteField>(propInfo) is { IgnoreOnInsert: false } field &&
                GetPropertyValue(data, propInfo, field) is { } propValue)
            {
                commandText.Append(field.Name, ",");

                string paramName = $"@{propInfo.Name}";
                valuesText.Append(paramName, ",");
                command.Parameters.AddWithValue(paramName, propValue);

                columnCount++;
            }
        });

        // No valid columns found.
        if (columnCount == 0)
            throw new InvalidTypeException(
                $"No eligible {nameof(ColumnAttribute)} or {nameof(ForeignKeyColumnAttribute)} found among current data properties.");

        commandText.Length -= 1;
        valuesText.Length -= 1;

        commandText.Append(") ", valuesText, ")");

        command.CommandText = commandText.ToString();
        return command;
    }
    #endregion

    #region GenerateSelectCommand
    /// <summary>
    /// Generates a SELECT command for the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException"></exception>
    public static SQLiteCommand GenerateSelectCommand<T>(T data)
    {
        SQLiteCommand command = new();
        Type type = typeof(T);
        TableAttribute table = AttributeHelper.GetTableAttribute(type);
        SelectOptionsAttribute selectOptions = AttributeHelper.GetTypeAttribute<SelectOptionsAttribute>(type);
        StringBuilder commandText = new($"SELECT{(selectOptions?.RemoveDuplicates == true ? " DISTINCT" : "")}");
        List<(string propName, SortingFieldAttribute sortingField)> sortingFields = new();
        StringBuilder filterCommand = new();
        StringBuilder groupingCommand = new();
        int columnCount = 0;

        type.GetProperties().ForEach(propInfo =>
        {
            bool columnAdded = false;
            AttributeHelper.CheckPropertyAttributes(propInfo);

            if (AttributeHelper.GetPropertyAttribute<SQLiteField>(propInfo) is { IgnoreOnSelect: false } field)
            {
                AppendColumnCommand(field.TableAlias ?? table.Alias, field.Name, propInfo.Name);

                if (GetPropertyValue(data, propInfo, field) is { } fieldValue)
                    AddFilterData(filterCommand, command.Parameters, propInfo.Name, fieldValue);

                columnAdded = true;
                columnCount++;
            }

            else if (AttributeHelper.GetPropertyAttribute<CustomColumnAttribute>(propInfo) is { } customColumn)
            {
                commandText.Append(" (", customColumn.CustomData, ") AS ", propInfo.Name, ",");
                columnAdded = true;
                columnCount++;
            }

            // Saves the sorting data.
            if (AttributeHelper.GetPropertyAttribute<SortingFieldAttribute>(propInfo) is { } sortingField)
            {
                // Adds the target column if it does not exist.
                if (!columnAdded)
                {
                    AppendColumnCommand(sortingField.TableAlias ?? table.Alias, sortingField.ColumnName, propInfo.Name);
                    columnAdded = true;
                }

                sortingFields.Add((propInfo.Name, sortingField));
            }

            // Saves the grouping commands.
            if (AttributeHelper.GetPropertyAttribute<GroupingTargetAttribute>(propInfo) is { } groupingTarget)
            {
                // Adds the target column if it does not exist.
                if (!columnAdded)
                    AppendColumnCommand(groupingTarget.TableAlias ?? table.Alias, groupingTarget.ColumnName, propInfo.Name);

                groupingCommand.Append(propInfo.Name, ", ");
            }
        });

        // No valid columns found.
        if (columnCount == 0)
            throw new InvalidTypeException(
                $"No eligible {nameof(ColumnAttribute)}, {nameof(CustomColumnAttribute)} or {nameof(ForeignKeyColumnAttribute)} found among current data properties.");

        commandText.Length--; //Removes the trailing comma.
        commandText.Append(" FROM ", table.Name).AppendIfNotNullOrWhiteSpace(table.Alias, " AS ");

        // Join (JOIN).
        Attribute.GetCustomAttributes(type).Where(_ => _ is JoinAttribute).ForEach(attr =>
        {
            JoinAttribute join = attr as JoinAttribute;

            commandText.Append(join.Mode switch
            {
                JoinMode.Cross => " CROSS",
                JoinMode.Outer => " LEFT OUTER",
                _ => " INNER"
            });

            commandText.Append(" JOIN ", join.Table, " AS ", join.Alias).AppendIfNotNullOrWhiteSpace(join.Constraint, " ON ");
        });

        commandText.Append(" WHERE 1 = 1", filterCommand).AppendIfNotNullOrWhiteSpace(selectOptions?.Filter, " AND (", ")"); // Filtering (WHERE).

        // Grouping (GROUP BY).
        if (groupingCommand.Length > 0)
        {
            groupingCommand.Length -= 2; // Removes the trailing comma.
            commandText.Append(" GROUP BY ", groupingCommand);

            // Filtering (HAVING).
            if (!String.IsNullOrWhiteSpace(selectOptions?.Having))
                commandText.Append(" HAVING ", selectOptions.Having);
        }

        // Sorting (ORDER BY).
        if (sortingFields.Any())
        {
            commandText.Append(" ORDER BY");
            sortingFields.OrderBy(_ => _.sortingField.SortingIndex).ForEach(_ =>
                commandText.Append(" ", _.propName,
                    $" {(_.sortingField.Direction == SortingDirection.Ascending ? "ASC" : "DESC")} NULLS {(_.sortingField.PlaceNullsAtTheEnd ? "LAST" : "FIRST")},"));

            commandText.Length--; //Removes trailing comma.
        }

        // Limiting (LIMIT).
        if (selectOptions?.Limit >= 0)
        {
            commandText.Append(" LIMIT ", selectOptions.Limit);

            if (selectOptions.Offset >= 0)
                commandText.Append(" OFFSET ", selectOptions.Offset);
        }

        command.CommandText = commandText.ToString();
        return command;

        #region Local functions

        #region AppendColumnCommand
        // Appends the specified column to the select command.
        void AppendColumnCommand(string tableAlias, string columnName, string propName) =>
            commandText.Append(" ").AppendIfNotNullOrWhiteSpace(tableAlias, null, ".")
                .Append(columnName, " AS ", propName, ",");
        #endregion

        #endregion
    }
    #endregion

    #region GenerateUpdateCommand
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException"></exception>
    public static SQLiteCommand GenerateUpdateCommand<T>(T data)
    {
        SQLiteCommand command = new();
        Type type = typeof(T);
        TableAttribute table = AttributeHelper.GetTableAttribute(type);
        StringBuilder commandText = new($"UPDATE OR ROLLBACK {table.Name} SET");
        StringBuilder filter = new(" WHERE 1 = 1");
        int columnCount = 0;

        type.GetProperties().ForEach(propInfo =>
        {
            if (AttributeHelper.GetPropertyAttribute<SQLiteField>(propInfo) is { IgnoreOnUpdate: false } field &&
                GetPropertyValue(data, propInfo, field) is { } propValue)
            {
                if (field.IsPrimaryKey)
                    AddFilterData(filter, command.Parameters, field.Name, propValue);
                else
                {
                    string paramName = GenerateParamName(field.Name, command.Parameters);
                    commandText.Append(" ", field.Name, " = ", paramName, ",");
                    command.Parameters.AddWithValue(paramName, propValue);
                }

                columnCount++;
            }
        });

        // No valid columns found.
        if (columnCount == 0)
            throw new InvalidTypeException(
                $"No eligible {nameof(ColumnAttribute)} or {nameof(ForeignKeyColumnAttribute)} found among current data properties.");

        commandText.Length -= 1; // Removes the trailing comma.
        commandText.Append(filter);

        command.CommandText = commandText.ToString();
        return command;

        #region Local Methods

        #region GenerateParamName
        // Generates a new unique parameter name based on the specified column name.
        string GenerateParamName(string columnName, SQLiteParameterCollection parameters)
        {
            string originalParamName = $"@{columnName}";
            string paramName = originalParamName;
            List<string> paramNames = new(from SQLiteParameter parameter in parameters select parameter.ParameterName);
            int index = 0;

            while (paramNames.Contains(paramName))
                paramName = $"{originalParamName}_{++index}";

            return paramName;
        }
        #endregion

        #endregion
    }
    #endregion

    #endregion

    #region Private Methods

    #region AddFilterData

    /// <summary>
    /// Adds the property's data to the specified command text.
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="columnName"></param>
    /// <param name="propValue"></param>
    /// <param name="commandText"></param>
    private static void AddFilterData(StringBuilder commandText, SQLiteParameterCollection parameters, string columnName, object propValue)
    {
        string paramName = $"@{columnName}";

        commandText.Append(" AND ", columnName, " = ", paramName);
        parameters.AddWithValue(paramName, propValue);
    }
    #endregion

    #region GetPropertyValue
    /// <summary>
    /// Retrieves the specified property's value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="propInfo"></param>
    /// <param name="propAttribute"></param>
    /// <returns></returns>
    private static object GetPropertyValue<T>(T data, PropertyInfo propInfo, SQLiteField propAttribute)
    {
        object propValue = propInfo.GetValue(data);

        // Special treatment for Foreign Key properties.
        if (propValue is not null && propAttribute is ForeignKeyColumnAttribute foreignKey)
        {
            Type type = propValue.GetType();
            TableAttribute table = AttributeHelper.GetTypeAttribute<TableAttribute>(type);

            return table is null
                ? null
                : type.GetProperties().FirstOrDefault(_ =>
                {
                    ColumnAttribute column = AttributeHelper.GetPropertyAttribute<ColumnAttribute>(_);
                    return column is not null &&
                           column.TableAlias == table.Alias &&
                           (foreignKey.TargetColumn is null && column.IsPrimaryKey ||
                            column.Name == foreignKey.TargetColumn);
                })?.GetValue(propValue);
        }

        return propValue;
    }
    #endregion

    #endregion
}