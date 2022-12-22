using SQLiteCommands.Helpers;

#pragma warning disable CS8618

namespace SQLiteCommands.Attributes.Field;

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute : SQLiteField
{
    #region Properties

    /// <summary>
    /// Gets or sets the field's default value for the INSERT/UPDATE commands when the property is null.
    /// </summary>
    public object DefaultValue { get; set; }

    /// <summary>
    /// Indicates whether the field contains the PRIMARY KEY constraint.
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

    #endregion

    public ColumnAttribute(string name)
    {
        AttributeHelper.CheckNullProperty(name, nameof(name), "column's name");
        Name = name;
    }
}