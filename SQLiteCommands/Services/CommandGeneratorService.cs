using HelperExtensions;
using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;
using SQLiteCommands.Enums;
using SQLiteCommands.Exceptions;
using SQLiteCommands.Helpers;
using System.Data.SQLite;
using System.Reflection;
using System.Text;
#pragma warning disable CS8625
#pragma warning disable CS8601
#pragma warning disable CS8604

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
    /// <param name="connection"></param>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException"></exception>
    public static SQLiteCommand GenerateDeleteCommand<T>(T data, SQLiteConnection connection = null)
    {
        SQLiteCommand command = new(connection);
        Type type = data.GetType();
        TableAttribute table = GetTableAttribute(type);
        StringBuilder commandText = new($"DELETE FROM {table.Name} WHERE 1 = 1");
        int columnCount = 0;

        type.GetProperties().ForEach(propInfo =>
        {
            (SQLiteColumnData attribute, object propValue) = GetPropertyData(data, propInfo);

            if (attribute?.IsPrimaryKey == true && IsFieldEligible(propValue, attribute.DeleteBehaviour))
            {
                AddFilterData(commandText, command.Parameters, attribute.Name, propValue);
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
    /// Generates an INSERT command for the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="connection"></param>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException"></exception>
    public static SQLiteCommand GenerateInsertCommand<T>(T data, SQLiteConnection connection = null)
    {
        Type type = data.GetType();
        TableAttribute table = GetTableAttribute(type);
        SQLiteCommand command = new(connection);
        InsertOptionsAttribute insertOptions = AttributeHelper.GetTypeAttribute<InsertOptionsAttribute>(type);
        StringBuilder commandText =
            new($"INSERT OR {(insertOptions?.ReplaceOnConflict == true ? "REPLACE" : "ROLLBACK")} INTO {table.Name} (");
        StringBuilder valuesText = new("VALUES (");
        int columnCount = 0;

        type.GetProperties().ForEach(propInfo =>
        {
            (SQLiteColumnData attribute, object propValue) = GetPropertyData(data, propInfo);

            if (IsFieldEligible(propValue, attribute?.InsertBehaviour))
            {
                commandText.Append(attribute.Name, ",");

                string paramName = $"@{propInfo.Name}";
                valuesText.Append(paramName, ",");
                command.Parameters.AddWithValue(paramName, propValue ?? DBNull.Value);

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
    /// <param name="connection"></param>
    /// <param name="filter"></param>
    /// <param name="customFilter"></param>
    /// <param name="limitOverride">Overrides the <see cref="SelectOptionsAttribute"/> Limit properties.</param>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException"></exception>
    public static SQLiteCommand GenerateSelectCommand<T>(SQLiteConnection connection, T filter = default, string customFilter = null, int? limitOverride = null)
    {
        SQLiteCommand command = new(connection);
        Type type = filter?.GetType() ?? typeof(T);
        TableAttribute table = GetTableAttribute(type);
        SelectOptionsAttribute selectOptions = AttributeHelper.GetTypeAttribute<SelectOptionsAttribute>(type) ?? new SelectOptionsAttribute();
        StringBuilder commandText = new($"SELECT{(selectOptions.RemoveDuplicates ? " DISTINCT" : "")}");
        StringBuilder filterCommand = new();
        bool hasColumns = false;
        string primaryKeyProperty = null;
        List<string> listProperties = new();

        type.GetProperties().ForEach(propInfo =>
        {
            AttributeHelper.ValidatePropertyAttributes(propInfo);

            propInfo.GetCustomAttributes().ForEach(_ =>
            {
                switch (_)
                {
                    case SQLiteColumnData columnData:
                        if (columnData.InsertBehaviour == Behaviour.AlwaysIgnore
                            || selectOptions.PrimaryKeyFilterOnly && !columnData.IsPrimaryKey)
                            return;

                        object fieldValue = GetPropertyValue(filter, propInfo, columnData);
                        if (fieldValue is null && columnData.SelectBehaviour == Behaviour.IgnoreWhenNull)
                            return;

                        string tableAlias = columnData.TableAlias ?? table.Alias;
                        AppendColumnCommand(tableAlias, columnData.Name, propInfo.Name);
                        
                        AddFilterData(filterCommand, command.Parameters, propInfo.Name, fieldValue);

                        // Stores the first PRIMARY KEY property found.
                        if (columnData.IsPrimaryKey && primaryKeyProperty.IsNullOrWhiteSpace() && columnData.TableAlias == table.Alias)
                            primaryKeyProperty = columnData.Name;
                        break;

                    case CustomColumnAttribute customColumn:
                        commandText.Append(" (", customColumn.CustomData, ") AS ", propInfo.Name, ",");
                        break;

                    case ManyToManyDataAttribute manyToManyData:
                        if (manyToManyData.LocalColumn.IsNullOrWhiteSpace())
                            listProperties.Add(propInfo.Name);
                        else
                        {
                            AppendColumnCommand(table.Alias, manyToManyData.LocalColumn, propInfo.Name);
                            hasColumns = true;
                        }
                        break;

                    case OneToManyDataAttribute oneToManyData:
                        if (oneToManyData.LocalColumn.IsNullOrWhiteSpace())
                            listProperties.Add(propInfo.Name);
                        else
                        {
                            AppendColumnCommand(table.Alias, oneToManyData.LocalColumn, propInfo.Name);
                            hasColumns = true;
                        }
                        break;
                }
            });
        });

        // Appends the columns for OneToMany and ManyToMany lists that do not contain an specified source column.
        if (listProperties.Count > 0)
        {
            if (primaryKeyProperty.IsNullOrWhiteSpace())
                throw new InvalidAttributeCombinationException("One-to-many and many-to-many fields required a local primary key column or a specified local column");

            listProperties.ForEach(_ => AppendColumnCommand(table.Alias, primaryKeyProperty, _));
            hasColumns = true;
        }

        // No valid columns found.
        if (!hasColumns)
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

        // Filtering (WHERE).
        commandText.Append(" WHERE 1 = 1", filterCommand)
            .AppendIfNotNullOrWhiteSpace(selectOptions.Filter, " AND (", ")")
            .AppendIfNotNullOrWhiteSpace(customFilter, " AND (", ")");

        commandText.AppendIfNotNullOrWhiteSpace(selectOptions.GroupBy, " GROUP BY "); // Grouping (GROUP BY).
        commandText.AppendIfNotNullOrWhiteSpace(selectOptions.OrderBy, " ORDER BY "); // Sorting (ORDER BY).

        // Limiting (LIMIT).
        if (limitOverride.HasValue || selectOptions.Limit >= 0)
        {
            commandText.Append(" LIMIT ", limitOverride ?? selectOptions.Limit);

            if (selectOptions.Offset >= 0)
                commandText.Append(" OFFSET ", selectOptions.Offset);
        }

        command.CommandText = commandText.ToString();
        return command;

        #region Local functions

        #region AppendColumnCommand
        // Appends the specified column to the select command.
        void AppendColumnCommand(string tableAlias, string columnName, string propName)
        {
            if (columnName.IsNullOrWhiteSpace() || propName.IsNullOrWhiteSpace())
                return;

            commandText.Append(" ").AppendIfNotNullOrWhiteSpace(tableAlias, suffix: ".")
                .Append(columnName, " AS ", propName, ",");
        }
        #endregion

        #endregion
    }
    #endregion

    #region GenerateUpdateCommand
    /// <summary>
    /// Generates an UPDATE command for the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="connection"></param>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException"></exception>
    public static SQLiteCommand GenerateUpdateCommand<T>(T data, SQLiteConnection connection = null)
    {
        SQLiteCommand command = new(connection);
        Type type = data.GetType();
        TableAttribute table = GetTableAttribute(type);
        StringBuilder commandText = new($"UPDATE OR ROLLBACK {table.Name} SET");
        StringBuilder filter = new(" WHERE 1 = 1");
        int columnCount = 0;

        type.GetProperties().ForEach(propInfo =>
        {
            (SQLiteColumnData attribute, object propValue) = GetPropertyData(data, propInfo);

            if (IsFieldEligible(propValue, attribute?.UpdateBehaviour))
            {
                if (attribute.IsPrimaryKey)
                    AddFilterData(filter, command.Parameters, attribute.Name, propValue);
                else
                {
                    string paramName = GenerateParamName(attribute.Name, command.Parameters);
                    commandText.Append(" ", attribute.Name, " = ", paramName, ",");
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
        parameters.AddWithValue(paramName, propValue ?? DBNull.Value);
    }
    #endregion

    #region GetPropertyData
    /// <summary>
    /// Retrieves the <see cref="SQLiteColumnData"/> attribute and the value of the specified property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="propInfo"></param>
    /// <returns></returns>
    private static (SQLiteColumnData attribute, object value) GetPropertyData<T>(T data, PropertyInfo propInfo)
    {
        SQLiteColumnData attribute = AttributeHelper.GetPropertyAttribute<SQLiteColumnData>(propInfo);
        return (attribute, GetPropertyValue(data, propInfo, attribute));
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
    private static object GetPropertyValue<T>(T data, PropertyInfo propInfo, SQLiteColumnData propAttribute)
    {
        if (propAttribute is null)
            return null;

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
                    return
                        column is not null
                        && column.TableAlias == table.Alias
                        && (foreignKey.TargetColumn is null
                            && column.IsPrimaryKey || column.Name == foreignKey.TargetColumn);
                })?.GetValue(propValue);
        }

        return propValue;
    }
    #endregion

    #region GetTableAttribute
    /// <summary>
    /// Returns the table attribute for the specified type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException">The specified type does not contain the <see cref="TableAttribute"/> attribute.</exception>
    public static TableAttribute GetTableAttribute(Type type) =>
        AttributeHelper.GetTypeAttribute<TableAttribute>(type) ??
        throw new InvalidTypeException($"The {nameof(TableAttribute)} attribute is mandatory for SQLite commands.");
    #endregion

    #region IsFieldEligible
    /// <summary>
    /// Indicates whether the current field is eligible for the desired command according to its specified <see cref="Behaviour"/>.
    /// </summary>
    /// <param name="propValue"></param>
    /// <param name="commandBehaviour"></param>
    /// <returns></returns>
    private static bool IsFieldEligible(object propValue, Behaviour? commandBehaviour) =>
        commandBehaviour.HasValue
        && (commandBehaviour == Behaviour.AlwaysInclude
            || commandBehaviour == Behaviour.IgnoreWhenNull && propValue is not null);
    #endregion

    #endregion
}