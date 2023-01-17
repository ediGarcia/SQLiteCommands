using HelperExtensions;
using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;
using SQLiteCommands.Enums;
using SQLiteCommands.Exceptions;
using SQLiteCommands.Helpers;
using System.Data.SQLite;
using System.Text;

namespace SQLiteCommands.Services;

// ReSharper disable once InconsistentNaming
internal static class CommandGeneratorService
{
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
                { IsPrimaryKey: true, IgnoreOnDelete: false } field)
            {
                object propValue = propInfo.GetValue(data);

                if (field is ForeignKeyColumnAttribute foreignKey)
                    propValue = GetForeignKeyValue(propValue, foreignKey.TargetColumn);

                if (propValue is not null)
                {
                    AddFilterData(commandText, command.Parameters, field.Name, propValue);
                    columnCount++;
                }
            }
        });

        // No valid columns found.
        if (columnCount == 0)
            throw new InvalidTypeException(
                $"No eligible {nameof(ColumnAttribute)} or {nameof(ForeignKeyColumnAttribute)} found among current data properties.");

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
        SelectOptionsAttribute select = AttributeHelper.GetTypeAttribute<SelectOptionsAttribute>(type);
        StringBuilder commandText = new("SELECT");
        List<(string propName, SortingFieldAttribute sortingField)> sortingFields = new();
        StringBuilder filterCommand = new();
        StringBuilder groupingCommand = new();

        if (select?.RemoveDuplicates == true)
            commandText.Append(" DISTINCT");

        int columnCount = 0;
        type.GetProperties().ForEach(propInfo =>
        {
            bool columnAdded = false;
            AttributeHelper.CheckPropertyAttributes(propInfo);

            if (AttributeHelper.GetPropertyAttribute<ColumnAttribute>(propInfo) is { IgnoreOnSelect: false } column)
            {
                AppendColumnCommand(column.TableAlias ?? table.Alias, column.Name, propInfo.Name);

                if (propInfo.GetValue(data) is { } columnValue)
                    AddFilterData(filterCommand, command.Parameters, propInfo.Name, columnValue);

                columnAdded = true;
                columnCount++;
            }

            else if (AttributeHelper.GetPropertyAttribute<CustomColumnAttribute>(propInfo) is { } customColumn)
            {
                commandText.Append(" (", customColumn.CustomData, ") AS ", propInfo.Name, ",");
                columnAdded = true;
                columnCount++;
            }

            else if (AttributeHelper.GetPropertyAttribute<ForeignKeyColumnAttribute>(propInfo) is { IgnoreOnSelect: false } foreignKeyColumn)
            {
                AppendColumnCommand(foreignKeyColumn.TableAlias ?? table.Alias, foreignKeyColumn.Name, propInfo.Name);

                if (GetForeignKeyValue(propInfo.GetValue(data), foreignKeyColumn.TargetColumn) is { } foreignKeyValue)
                    AddFilterData(filterCommand, command.Parameters, propInfo.Name, foreignKeyValue);

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

        commandText.Append(" WHERE 1 = 1", filterCommand).AppendIfNotNullOrWhiteSpace(select?.Filter, " AND (", ")"); // Filtering (WHERE).

        // Grouping (GROUP BY).
        if (groupingCommand.Length > 0)
        {
            groupingCommand.Length -= 2; // Removes the trailing comma.
            commandText.Append(" GROUP BY ", groupingCommand);

            // Filtering (HAVING).
            if (!String.IsNullOrWhiteSpace(select?.Having))
                commandText.Append(" HAVING ", select.Having);
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
        if (select?.Limit >= 0)
        {
            commandText.Append(" LIMIT ", select.Limit);

            if (select.Offset >= 0)
                commandText.Append(" OFFSET ", select.Offset);
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
            if (AttributeHelper.GetPropertyAttribute<SQLiteField>(propInfo) is { IgnoreOnUpdate: false } field)
            {
                object propValue = propInfo.GetValue(data);

                if (field is ForeignKeyColumnAttribute foreignKey)
                    propValue = GetForeignKeyValue(propValue, foreignKey.TargetColumn);

                if (propValue is null)
                    return;

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

    #region GetForeignKeyValue
    /// <summary>
    /// Gets the specified foreign key value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="foreignKeyData"></param>
    /// <param name="columnName"></param>
    /// <returns></returns>
    private static object GetForeignKeyValue<T>(T foreignKeyData, string columnName)
    {
        if (foreignKeyData == null)
            return null;

        Type type = foreignKeyData.GetType();
        TableAttribute table = AttributeHelper.GetTypeAttribute<TableAttribute>(type);

        if (table is null)
            return null;

        return type.GetProperties().FirstOrDefault(_ =>
        {
            ColumnAttribute column = AttributeHelper.GetPropertyAttribute<ColumnAttribute>(_);
            return column is not null &&
                   (column.TableAlias is null && table.Alias is null || column.TableAlias == table.Alias) &&
                   (columnName is null && column.IsPrimaryKey || column.Name == columnName);
        })?.GetValue(foreignKeyData);
    }
    #endregion

    #endregion
}