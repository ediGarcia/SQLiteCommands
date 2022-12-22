using System.Data.SQLite;
using System.Text;
using HelperExtensions;
using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;
using SQLiteCommands.Enums;
using SQLiteCommands.Exceptions;
using SQLiteCommands.Helpers;

namespace SQLiteCommands.Services;

// ReSharper disable once InconsistentNaming
internal static class CommandGeneratorService
{
    #region GenerateSelectCommand
    /// <summary>
    /// Generates a Select command for the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException"></exception>
    public static SQLiteCommand GenerateSelectCommand<T>(T data)
    {
        SQLiteCommand command = new();
        Type currentType = typeof(T);
        TableAttribute table = AttributeHelper.GetTypeAttribute<TableAttribute>(currentType);
        SelectOptionsAttribute select = AttributeHelper.GetTypeAttribute<SelectOptionsAttribute>(currentType);
        StringBuilder commandText = new("SELECT");
        List<(string propName, SortingFieldAttribute sortingField)> sortingFields = new();
        StringBuilder filterCommand = new();
        StringBuilder groupingCommand = new();

        if (select?.RemoveDuplicates == true)
            commandText.Append(" DISTINCT");

        int columnCount = 0;
        currentType.GetProperties().ForEach(propInfo =>
        {
            bool columnAdded = false;
            AttributeHelper.CheckPropertyAttributes(propInfo);

            if (AttributeHelper.GetPropertyAttribute<ColumnAttribute>(propInfo) is { IgnoreOnSelect: false } column)
            {
                AppendColumnCommand(column.TableAlias ?? table.Alias, column.Name, propInfo.Name);

                if (propInfo.GetValue(data) is { } columnValue)
                    AddFilterData(propInfo.Name, columnValue);

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
                    AddFilterData(propInfo.Name, foreignKeyValue);

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
                $"No {nameof(ColumnAttribute)}, {nameof(CustomColumnAttribute)} or {nameof(ForeignKeyColumnAttribute)} found among current data properties.");

        commandText.Length--; //Removes the trailing comma.
        commandText.Append(" FROM ", table.Name).AppendIfNotNullOrWhiteSpace(table.Alias, " AS ");

        // Join (JOIN).
        Attribute.GetCustomAttributes(currentType).Where(_ => _ is JoinAttribute).ForEach(attr =>
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

        #region AddFilterData
        // Adds the property's data to the command filter.
        void AddFilterData(string propName, object propValue)
        {
                string paramName = $"@{propName}";

                filterCommand.Append(" AND ", propName, " = ", paramName);
                command.Parameters.AddWithValue(paramName, propValue);
        }
        #endregion

        #region AppendColumnCommand
        // Appends the specified column to the select command.
        void AppendColumnCommand(string tableAlias, string columnName, string propName) =>
            commandText.Append(" ").AppendIfNotNullOrWhiteSpace(tableAlias, null, ".")
                .Append(columnName, " AS ", propName, ",");
        #endregion

        #endregion
    }
    #endregion

    #region Private Methods

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