using SQLiteCommands.Services;
using System.Data;
using System.Data.SQLite;

namespace SQLiteCommands.Extensions;

/// <summary>
/// <see cref="SQLiteCommand"/> extension methods.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class SQLiteCommandExtensions
{
    #region Public Methods

    #region ExecuteQuery
    /// <summary>
    /// Executes the command and returns the retrieved rows, if any.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static DataTable ExecuteQuery(this SQLiteCommand command)
    {
        using SQLiteDataAdapter dataAdapter = new(command.CommandText, command.Connection);
        DataTable result = new();

        dataAdapter.Fill(result);
        return result;
    }
    #endregion

    #region ExecuteQuery<T>
    /// <summary>
    /// Executes the command and returns the retrieved data, if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <returns></returns>
    public static List<T> ExecuteQuery<T>(this SQLiteCommand command) =>
        DatabaseService.ConvertDataTable<T>(command.ExecuteQuery());
    #endregion

    #endregion
}