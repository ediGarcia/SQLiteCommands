#pragma warning disable CS8618
namespace SQLiteCommands.Attributes.Field;

/// <summary>
/// Conditions for grouping related data returned from the database.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class GroupingTargetAttribute : Attribute
{
    #region Properties

    /// <summary>
    /// Gets and sets the column's name (optional, to be used when no other attribute sets the column's name).
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    /// Gets and sets the parent table alias (optional, to be used when no other attribute sets the parent table's alias).
    /// </summary>
    public string TableAlias { get; set; }

    #endregion
}