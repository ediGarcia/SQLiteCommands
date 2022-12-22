namespace SQLiteCommands.Attributes.Field;

[AttributeUsage(AttributeTargets.Property)]
// ReSharper disable once InconsistentNaming
public class SQLiteField : Attribute
{
    #region Properties

    /// <summary>
    /// Gets or sets whether the current field should be ignored when an INSERT is executed.
    /// </summary>
    public bool IgnoreOnInsert { get; set; }

    /// <summary>
    /// Gets or sets whether the current field should be ignored when a SELECT is executed.
    /// </summary>
    public bool IgnoreOnSelect { get; set; }

    /// <summary>
    /// Gets or sets whether the current field should be ignored when an UPDATE is executed.
    /// </summary>
    public bool IgnoreOnUpdate { get; set; }

    #endregion
}