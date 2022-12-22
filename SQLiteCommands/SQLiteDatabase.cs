using SQLiteCommands.Helpers;

namespace SQLiteCommands;

// ReSharper disable once InconsistentNaming
public class SQLiteDatabase
{
    private readonly string _path;

    // ReSharper disable once IdentifierTypo
    public SQLiteDatabase(string path)
    {
        AttributeHelper.CheckNullProperty(path, nameof(path), "SQLite database file path");
        _path = path;
    }

    #region Public Methods

    #region Insert
    /// <summary>
    /// Insert new data into the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    public void Insert<T>(params T[] data) =>
        SQLiteCommands.Insert(_path, data);
    #endregion

    #endregion
}