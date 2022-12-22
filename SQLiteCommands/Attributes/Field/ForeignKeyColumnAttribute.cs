using SQLiteCommands.Helpers;

namespace SQLiteCommands.Attributes.Field;

[AttributeUsage(AttributeTargets.Property)]
public class ForeignKeyColumnAttribute : SQLiteField
{
    #region Properties

    /// <summary>
    /// Indicates whether the current column contains the PRIMARY KEY constraint.
    /// </summary>
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// Gets and sets the column's name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets and sets the parent table alias.
    /// </summary>
    public string TableAlias { get; set; }

    /// <summary>
    /// Gets or sets the target table column (needed if the target table contains more than one primary key).
    /// </summary>
    public string TargetColumn { get; set; }

    #endregion
    
    public ForeignKeyColumnAttribute(string name)
    {
        AttributeHelper.CheckNullProperty(name, nameof(name), "column's name");
        Name = name;
    }
}