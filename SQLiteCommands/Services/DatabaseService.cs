using HelperExtensions;
using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Helpers;
using System.Data;
using System.Data.SQLite;
using System.Reflection;
using SQLiteCommands.Extensions;
#pragma warning disable CS8625
#pragma warning disable CS8601

namespace SQLiteCommands.Services;

// ReSharper disable once InconsistentNaming
public static class DatabaseService
{
    #region Public Methods

    #region Commands

    #region Select*

    #region Select(string, [T])
    /// <summary>
    /// Fetches data from the database (a new connection will be opened for the operation and closed after it has been completed).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="databasePath"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static List<T> Select<T>(string databasePath, T filter = default)
    {
        using SQLiteConnection connection = OpenConnection(databasePath);
        return SelectPrivate(connection, filter);
    }
    #endregion

    #region Select(SQLiteConnection, [T])
    /// <summary>
    /// Fetches data from the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static List<T> Select<T>(SQLiteConnection connection, T filter = default) =>
        SelectPrivate(connection, filter);
    #endregion

    #endregion

    #region SelectSingle*

    #region SelectSingle(string, [T])
    /// <summary>
    /// Fetches data from the database and returns only the first item found (a new connection will be opened for the operation and closed after it has been completed).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="databasePath"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static T SelectSingle<T>(string databasePath, T filter = default)
    {
        using SQLiteConnection connection = OpenConnection(databasePath);
        return SelectPrivate(connection, filter, limitOverride: 1).FirstOrDefault();
    }
    #endregion

    #region SelectSingle(SQLiteConnection, [T])
    /// <summary>
    /// Fetches data from the database and returns only the first item found.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static T SelectSingle<T>(SQLiteConnection connection, T filter = default) =>
        SelectPrivate(connection, filter, limitOverride: 1).FirstOrDefault();
    #endregion

    #endregion

    #endregion

    #region Connection actions

    #region GenerateConnectionString

    /// <summary>
    /// Generates the connection string.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static string GenerateConnectionString(string path, int version = 3) =>
        $"Data Source={path}; Version={version};";
    #endregion

    #region OpenConnection

    /// <summary>
    /// Opens a connection with a SQLite database file.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static SQLiteConnection OpenConnection(string path) =>
        new SQLiteConnection(GenerateConnectionString(path)).OpenAndReturn();
    #endregion

    #endregion

    #region General actions

    #region ConvertDataTable
    /// <summary>
    /// Converts the specified <see cref="DataTable"/> into a list of T by matching its property's names with the DataTable column's names.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataTable"></param>
    /// <returns></returns>
    public static List<T> ConvertDataTable<T>(DataTable dataTable)
    {
        List<T> result = new(dataTable.Rows.Count);

        foreach (DataRow row in dataTable.Rows)
        {
            T data = Activator.CreateInstance<T>();

            typeof(T).GetProperties().ForEach(propInfo =>
            {
                if (propInfo.GetSetMethod() is not null && !dataTable.Columns.Contains(propInfo.Name))
                    return;

                propInfo.SetValue(data, row[propInfo.Name]);
            });

            result.Add(data);
        }

        return result;
    }
    #endregion

    #region ExecuteNonQuery

    #region ExecuteNonQuery(SQLiteConnection, string, [Dictionary<string, object>])

    /// <summary>
    /// Executes a SQLite command and returns the number of rows inserted/updated affected by it.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="commandText"></param>
    /// <param name="parameters"></param>
    /// <returns>The number of rows affected</returns>
    public static  int ExecuteNonQuery(SQLiteConnection connection, string commandText, Dictionary<string, object> parameters = null)
    {
        using SQLiteCommand command = connection.CreateCommand();

        command.CommandText = commandText;
        parameters?.ForEach(_ => command.Parameters.AddWithValue(_.Key, _.Value));

        return command.ExecuteNonQuery();
    }
    #endregion

    #region ExecuteNonQuery(string, string, [Dictionary<string, object>])

    /// <summary>
    /// Executes a SQLite command and returns the number of rows inserted/updated affected by it.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="commandText"></param>
    /// <param name="parameters"></param>
    /// <returns>The number of rows affected</returns>
    public static  int ExecuteNonQuery(string path, string commandText, Dictionary<string, object> parameters = null)
    {
        SQLiteConnection connection = OpenConnection(path);
         using SQLiteCommand command = connection.CreateCommand();

        command.CommandText = commandText;
        parameters.ForEach(_ => command.Parameters.AddWithValue(_.Key, _.Value));

        return command.ExecuteNonQuery();
    }
    #endregion

    #endregion

    #region ExecuteNonQueryAsync

    #region ExecuteNonQueryAsync(SQLiteConnection, string, [Dictionary<string, object>])

    /// <summary>
    /// Executes a SQLite command and returns the number of rows inserted/updated affected by it.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="commandText"></param>
    /// <param name="parameters"></param>
    /// <returns>The number of rows affected</returns>
    public static async Task<int> ExecuteNonQueryAsync(SQLiteConnection connection, string commandText, Dictionary<string, object> parameters = null)
    {
        await using SQLiteCommand command = connection.CreateCommand();

        command.CommandText = commandText;
        parameters?.ForEach(_ => command.Parameters.AddWithValue(_.Key, _.Value));

        return await command.ExecuteNonQueryAsync();
    }
    #endregion

    #region ExecuteNonQuery(string, string, [Dictionary<string, object>])

    /// <summary>
    /// Executes a SQLite command and returns the number of rows inserted/updated affected by it.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="commandText"></param>
    /// <param name="parameters"></param>
    /// <returns>The number of rows affected</returns>
    public static async Task<int> ExecuteNonQueryAsync(string path, string commandText, Dictionary<string, object> parameters = null)
    {
        SQLiteConnection connection = OpenConnection(path);
        await using SQLiteCommand command = connection.CreateCommand();

        command.CommandText = commandText;
        parameters.ForEach(_ => command.Parameters.AddWithValue(_.Key, _.Value));

        return await command.ExecuteNonQueryAsync();
    }
    #endregion

    #endregion

    #region ExecuteQuery

    #region ExecuteQuery(SQLiteConnection, string, [Dictionary<string, object>])
    /// <summary>
    /// Executes a SQLite command and returns the rows returned by it, if any.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="commandText"></param>
    /// <param name="parameters"></param>
    /// <returns>A <see cref="DataTable"/> with the data returned by the command.</returns>
    public static DataTable ExecuteQuery(SQLiteConnection connection, string commandText, Dictionary<string, object> parameters = null)
    {
        SQLiteCommand command = connection.CreateCommand();
        command.CommandText = commandText;
        parameters?.ForEach(_ => command.Parameters.AddWithValue(_.Key, _.Value));

        return command.ExecuteQuery();
    }
    #endregion

    #region ExecuteQuery(string, string, [Dictionary<string, object>])

    /// <summary>
    /// Executes a SQLite command and returns the rows returned by it, if any.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="commandText"></param>
    /// <param name="parameters"></param>
    /// <returns>A <see cref="DataTable"/> with the data returned by the command.</returns>
    public static DataTable ExecuteQuery(string path, string commandText, Dictionary<string, object> parameters = null)
    {
        using SQLiteConnection connection = OpenConnection(path);

        SQLiteCommand command = connection.CreateCommand();
        command.CommandText = commandText;
        parameters?.ForEach(_ => command.Parameters.AddWithValue(_.Key, _.Value));

        return command.ExecuteQuery();
    }
    #endregion

    #endregion

    #region ExecuteQueryAsync

    #region ExecuteQueryAsync(SQLiteConnection, string, [Dictionary<string, object>])

    /// <summary>
    /// Executes a SQLite command and returns the rows returned by it, if any.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="commandText"></param>
    /// <param name="parameters"></param>
    /// <returns>A <see cref="DataTable"/> with the data returned by the command.</returns>
    public static async Task<DataTable> ExecuteQueryAsync(
        SQLiteConnection connection,
        string commandText,
        Dictionary<string, object> parameters = null) =>
        await Task.Run(() => ExecuteQuery(connection, commandText, parameters));
    #endregion

    #region ExecuteQuery(string, string, [Dictionary<string, object>])

    /// <summary>
    /// Executes a SQLite command and returns the rows returned by it, if any.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="commandText"></param>
    /// <param name="parameters"></param>
    /// <returns>A <see cref="DataTable"/> with the data returned by the command.</returns>
    public static async Task<DataTable> ExecuteQueryAsync(
        string path,
        string commandText,
        Dictionary<string, object> parameters = null) =>
        await Task.Run(() => ExecuteQuery(path, commandText, parameters));
    #endregion

    #endregion

    #region RunTransaction
    /// <summary>
    /// Runs multiple database commands within a single transaction.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="command"></param>
    public static void RunTransaction(SQLiteConnection connection, params Action[] command)
    {
        using SQLiteTransaction transaction = connection.BeginTransaction();
        try
        {
            command.ForEach(_ => _());
            transaction.Commit();
        }
        finally
        {
            transaction.Rollback();
        }
    }
    #endregion

    #endregion

    #endregion

    #region Private Methods

    #region SelectPrivate

    /// <summary>
    /// Runs the SELECT commands and returns the results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="filter"></param>
    /// <param name="customFilter"></param>
    /// <param name="limitOverride">Overrides the maximum number of rows returned.</param>
    /// <returns></returns>
    private static List<T> SelectPrivate<T>(SQLiteConnection connection, T filter = default, string customFilter = null, int? limitOverride = null)
    {
        SQLiteCommand command = CommandGeneratorService.GenerateSelectCommand(connection, filter, customFilter, limitOverride);
        DataTable dataTable = command.ExecuteQuery();
        List<T> result = new(dataTable.Rows.Count);

        foreach (DataRow row in dataTable.Rows)
        {
            T data = Activator.CreateInstance<T>();

            /*foreach (DataColumn dataColumn in dataTable.Columns)
            {
                object value = row[dataColumn];
                PropertyInfo propInfo = typeof(T).GetProperty(dataColumn.ColumnName);

                if (value is null or DBNull || propInfo is null)
                    continue;

                if (AttributeHelper.GetPropertyAttribute<ColumnAttribute>(propInfo) is { IgnoreOnSelect: false })
                    propInfo.SetValue(data, value);

                else if (AttributeHelper.GetPropertyAttribute<CustomColumnAttribute>(propInfo) is { })
                    propInfo.SetValue(data, value);

                else if (AttributeHelper.GetPropertyAttribute<ForeignKeyColumnAttribute>(propInfo) is { IgnoreOnSelect: false } foreignKey)
                    propInfo.SetValue(
                        data,
                        SelectPropertyValue(
                            connection,
                            propInfo.PropertyType,
                            $"{foreignKey.TargetColumn} = '{value}'").FirstOrDefault());

                else if (AttributeHelper.GetPropertyAttribute<ManyToManyDataAttribute>(propInfo) is { } manyToManyData)
                    propInfo.SetValue(
                        data,
                        SelectPropertyValue( //Recursively calls the Select method to fill the property.
                            connection,
                            propInfo.PropertyType,
                            $"{manyToManyData.TargetTableColumn} in (SELECT {manyToManyData.JoinTableSourcePrimaryKey} FROM {manyToManyData.JoinTable} WHERE {manyToManyData.JoinTableSourcePrimaryKey} = '{row[propInfo.Name]}')"));

                else if (AttributeHelper.GetPropertyAttribute<OneToManyDataAttribute>(propInfo) is { } oneManyToManyData)
                    propInfo.SetValue( //Recursively calls the Select method to fill the property.
                        data,
                        SelectPropertyValue(
                            connection,
                            propInfo.PropertyType,
                            $"{oneManyToManyData.TargetColumn} = '{row[propInfo.Name]}'"));
            }*/

            result.Add(data);
        }

        return result;
    }
    #endregion

    #region SelectPropertyValue
    /// <summary>
    /// Calls the select command for the specified class type.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="targetType"></param>
    /// <param name="customFilter"></param>
    /// <returns></returns>
    private static List<object> SelectPropertyValue(SQLiteConnection connection, Type targetType, string customFilter) =>
            typeof(DatabaseService)
                .GetMethod(nameof(SelectPrivate))
                .MakeGenericMethod(targetType)
                .Invoke(
                    null,
                    new object[]
                    {
                    connection,
                    null,
                    customFilter,
                    null
                    }) as List<object> ?? new List<object>();
    #endregion

    #endregion
}