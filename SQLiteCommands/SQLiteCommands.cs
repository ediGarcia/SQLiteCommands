using HelperExtensions;
using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;
using SQLiteCommands.Exceptions;
using SQLiteCommands.Helpers;
using SQLiteCommands.Services;
using System.Data.SQLite;
using System.Reflection;
using System.Text;

#pragma warning disable CS8604

namespace SQLiteCommands;

// ReSharper disable once InconsistentNaming
public static class SQLiteCommands
{
    #region Public Methods

    #region ExecuteNonQueryCommand
    /// <summary>
    /// Executes a SQLite command.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="commandText"></param>
    /// <param name="values"></param>
    /// <returns>The number of rows affected by the command.</returns>
    public static int ExecuteNonQueryCommand(SQLiteConnection connection, string commandText, Dictionary<string, object> values)
    {
        using SQLiteCommand command = connection.CreateCommand();

        command.CommandText = commandText;
        values.ForEach((column, value, _) => command.Parameters.AddWithValue(column, value));

        return command.ExecuteNonQuery();
    }
    #endregion

    #region Insert

    #region Insert<T>(SQLiteConnection, IEnumerable<T>, [bool], [bool])
    /// <summary>
    /// Inserts new data to the database through the specified connection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection">The SQLite database connection.</param>
    /// <param name="data">Data to be inserted into the database.</param>
    /// <param name="replaceOnConflict">Indicates whether the already existing data should be replaced when a Primary Key conflict happens.</param>
    /// <param name="updateLocalData">Indicates whether the local data should be updated after the insert (updates custom fields and related data).</param>
    public static void Insert<T>(SQLiteConnection connection, IEnumerable<T> data, bool replaceOnConflict = true, bool updateLocalData = true) =>
        data.ForEach(_ => Insert(connection, ref _, replaceOnConflict, updateLocalData));
    #endregion

    #region Insert<T>(string, T, [bool], [bool])
    /// <summary>
    /// Inserts new data to the database file.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">The SQLite database file location.</param>
    /// <param name="data">Data to be inserted into the database.</param>
    /// <param name="replaceOnConflict">Indicates whether the already existing data should be replaced when a Primary Key conflict happens.</param>
    /// <param name="updateLocalData">Indicates whether the local data should be updated after the insert (updates custom fields and related data).</param>
    public static void Insert<T>(string path, ref T data, bool replaceOnConflict = true, bool updateLocalData = true)
    {
        using SQLiteConnection connection = DatabaseService.OpenConnection(path);
        Insert(connection, ref data, replaceOnConflict, updateLocalData);
    }
    #endregion

    #region Insert<T>(string, IList<T>, [bool], [bool])
    /// <summary>
    /// Inserts new data to the database file.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">The SQLite database file location.</param>
    /// <param name="data">Data to be inserted into the database.</param>
    /// <param name="replaceOnConflict">Indicates whether the already existing data should be replaced when a Primary Key conflict happens.</param>
    /// <param name="updateLocalData">Indicates whether the local data should be updated after the insert (updates custom fields and related data).</param>
    public static void Insert<T>(string path, IList<T> data, bool replaceOnConflict = true, bool updateLocalData = true)
    {
        using SQLiteConnection connection = DatabaseService.OpenConnection(path);
        data.ForEach(_ => Insert(connection, ref _, replaceOnConflict, updateLocalData));
    }
    #endregion

    #region Insert(SQLiteConnection, T, [bool], [bool])

    /// <summary>
    /// Inserts new data to the database through the specified connection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection">The SQLite database connection.</param>
    /// <param name="data">Data to be inserted into the database.</param>
    /// <param name="replaceOnConflict">Indicates whether the already existing data should be replaced when a Primary Key conflict happens.</param>
    /// <param name="updateLocalData">Indicates whether the local data should be updated after the insert (updates custom fields and related data).</param>
    public static void Insert<T>(SQLiteConnection connection, ref T data, bool replaceOnConflict = true, bool updateLocalData = true)
    {
        Type type = typeof(T);
        TableAttribute tableAttribute = AttributeHelper.GetTableAttribute(type);

        CheckForCircularReference(type);

        StringBuilder commandText = new("INSERT");

        if (replaceOnConflict)
            commandText.Append(" OR REPLACE");

        commandText.Append(" INTO ", tableAttribute.Name, "(");

        List<(PropertyInfo PropertyInfo, Attribute attribute)> innerData = new();
        PropertyInfo[] properties = type.GetProperties().Where(_ => _.CanRead && _.CanWrite).ToArray();
        Dictionary<string, object> columns = new(properties.Length);

        foreach (PropertyInfo propInfo in properties)
        {
            AttributeHelper.CheckPropertyAttributes(propInfo);

            if (AttributeHelper.GetPropertyAttribute<ColumnAttribute>(propInfo) is { } column)
                columns.Add(column.Name, propInfo.GetValue(data) ?? column.DefaultValue);

            else if (AttributeHelper.GetPropertyAttribute<ForeignKeyColumnAttribute>(propInfo) is { } foreignKey)
            {
                innerData.Add((propInfo, foreignKey));
                //columns.Add(foreignKey.Name, AttributeHelper.GetForeignKeyValue(propInfo.GetValue(data), foreignKey));
            }

            else if (AttributeHelper.GetPropertyAttribute<OneToManyDataAttribute>(propInfo) is { } oneToMany)
                innerData.Add((propInfo, oneToMany));

            else if (AttributeHelper.GetPropertyAttribute<ManyToManyDataAttribute>(propInfo) is { } manyToMany)
                innerData.Add((propInfo, manyToMany));
        }

        //TODO: método separado para gerar comandos em string.

        if (columns.Count == 0)
            throw new InvalidTypeException($"No {nameof(ColumnAttribute)} found in current data type.");

        commandText.Append(String.Join(',', columns.Keys), ")VALUES(");

        columns.Keys.ForEach(_ => commandText.Append('@', _, ','));
        commandText.Length -= 1;
        commandText.Append(')');

        ExecuteNonQueryCommand(connection, commandText.ToString(), columns);
    }
    #endregion

    #endregion

    #endregion

    #region Private Methods

    #region CheckForCircularReference

    #region CheckForCircularReference(Type)
    /// <summary>
    /// Indicates whether the specified type contains any SQLite circular references.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static void CheckForCircularReference(Type type) =>
        CheckForCircularReference(new List<Type> { type });
    #endregion

    #region CheckForCircularReference(List<Type>)
    /// <summary>
    /// Indicates whether any of the specified types contain any SQLite circular references.
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    private static void CheckForCircularReference(List<Type> types)
    {
        types = types?.Where(_ => !IsSystemType(_)).ToList()!;

        if (types?.Any() != true)
            return;

        IEnumerable<PropertyInfo> properties = types.Last().GetProperties().Where(_ =>
            _.CanRead &&
            _.CanWrite &&
            !IsSystemType(_.PropertyType) &&
            Attribute.GetCustomAttributes(_).Any(__ =>
                __ is ForeignKeyColumnAttribute or OneToManyDataAttribute or ManyToManyDataAttribute));

        foreach (PropertyInfo propInfo in properties)
        {
            Type propType = propInfo.PropertyType.GetGenericArguments().FirstOrDefault() ?? propInfo.PropertyType;

            if (types.Contains(propType))
                ThrowsCircularReferenceException();

            List<Type> innerTypes = new(types);
            types.Add(propType);

            CheckForCircularReference(innerTypes);
        }

        return;

        #region Local functions

        #region IsSystemType
        // Indicates whether the specified type is a System type.
        bool IsSystemType(Type type) =>
            type.Namespace == "System";
        #endregion

        #region ThrowsCircularReferenceException
        // Throws the CircularReferenceException exception.
        void ThrowsCircularReferenceException() =>
            throw new CircularReferenceException(
                "The specified type contains one or more circular references that will lead to an infinite loop.");
        #endregion

        #endregion
    }
    #endregion

    #endregion

    

    

    

    

    #endregion
}