namespace SQLiteCommands.Attributes.Field;

[AttributeUsage(AttributeTargets.Property)]
public class ForeignKeyColumnAttribute : SQLiteField
{
    #region Properties

    /// <summary>
    /// Gets or sets the target table column (needed if the target table contains more than one primary key).
    /// </summary>
    public string TargetColumn { get; set; }

    #endregion
    
    public ForeignKeyColumnAttribute(string name) : base(name) { }
}