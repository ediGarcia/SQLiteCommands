using SQLiteCommands.Helpers;

namespace SQLiteCommands.Attributes.Field;

[AttributeUsage(AttributeTargets.Property)]
// ReSharper disable once InconsistentNaming
public class SQLiteField : Attribute
{
    #region Properties

    /// <summary>
    /// Gets o sets whether the current field should be ignored when a DELETE command is executed.
    /// </summary>
    public bool IgnoreOnDelete { get; set; }

    /// <summary>
    /// Gets or sets whether the current field should be ignored when an INSERT command is executed.
    /// </summary>
    public bool IgnoreOnInsert { get; set; }

    /// <summary>
    /// Gets or sets whether the current field should be ignored when a SELECT command is executed.
    /// </summary>
    public bool IgnoreOnSelect { get; set; }

    /// <summary>
    /// Gets or sets whether the current field should be ignored when an UPDATE command is executed.
    /// </summary>
    public bool IgnoreOnUpdate { get; set; }

    /// <summary>
    /// Indicates whether the current column contains the PRIMARY KEY constraint.
    /// </summary>
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// Gets and sets the column's name.
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Gets and sets the parent table alias.
    /// </summary>
    public string TableAlias { get; set; }

    #endregion

    public SQLiteField(string name)
    {
        AttributeHelper.CheckNullProperty(name, nameof(name), "column's name");
        Name = name;
    }
}