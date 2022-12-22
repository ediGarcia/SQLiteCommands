namespace SQLiteCommands.Enums;

public enum JoinMode
{
    /// <summary>
    /// Indicates whether the SELECT statement should use the CROSS JOIN mode.
    /// </summary>
    Cross,

    /// <summary>
    /// Indicates whether the SELECT statement should use the INNER JOIN mode.
    /// </summary>
    Inner,

    /// <summary>
    /// Indicates whether the SELECT statement should use the LEFT OUTER JOIN mode.
    /// </summary>
    Outer
}