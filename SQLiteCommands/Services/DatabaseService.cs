using System.Data.SQLite;

namespace SQLiteCommands.Services;

// ReSharper disable once InconsistentNaming
public static class DatabaseService
{
    #region Public Methods

    #region OpenConnection

    /// <summary>
    /// Opens a connection with a SQLite database file.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static SQLiteConnection OpenConnection(string path) =>
        new SQLiteConnection(GenerateConnectionString(path)).OpenAndReturn();
    #endregion

    #region GenerateConnectionString

    /// <summary>
    /// Generates the connection string
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GenerateConnectionString(string path) =>
        $"Data Source={path}; Version=3;";
    #endregion

    #endregion
}