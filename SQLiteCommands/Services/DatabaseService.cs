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
    /// <param name="versionNumber"></param>
    /// <returns></returns>
    public static SQLiteConnection OpenConnection(string path, int versionNumber = 3) =>
        new SQLiteConnection(GenerateConnectionString(path, versionNumber)).OpenAndReturn();
    #endregion

    #region GenerateConnectionString
    /// <summary>
    /// Generates the connection string
    /// </summary>
    /// <param name="path"></param>
    /// <param name="versionNumber"></param>
    /// <returns></returns>
    public static string GenerateConnectionString(string path, int versionNumber = 3) =>
        $"Data Source={path}; Version={versionNumber};";
    #endregion

    #endregion
}