namespace SQLiteCommands.Attributes.Field;

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute : SQLiteField
{
    #region Properties

    /// <summary>
    /// Gets or sets the field's default value for the INSERT/UPDATE commands when the property is null.
    /// </summary>
    public object DefaultValue { get; set; }

    #endregion

    public ColumnAttribute(string name) : base(name) { }
}