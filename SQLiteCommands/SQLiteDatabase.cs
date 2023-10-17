using SQLiteCommands.Helpers;
using SQLiteCommands.Services;
#pragma warning disable CS8601

namespace SQLiteCommands;

// ReSharper disable once InconsistentNaming
public class SQLiteDatabase
{
    #region Properties

    /// <summary>
    /// The current database file path.
    /// </summary>
    public string Path { get; }

    #endregion

    public SQLiteDatabase(string path)
    {
        AttributeHelper.ValidatePropertyValue(path, nameof(path), "SQLite database file path");
        Path = path;
    }

    #region Public Methods

    public List<T> Select<T>(T filter = default) =>
        DatabaseService.Select(Path, filter);

    public T SelectSingle<T>(T filter = default) =>
        DatabaseService.SelectSingle(Path, filter);

    #endregion
}