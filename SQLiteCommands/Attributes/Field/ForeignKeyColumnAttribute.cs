namespace SQLiteCommands.Attributes.Field;

public class ForeignKeyColumnAttribute : SQLiteColumnData
{
    #region Properties

    /// <summary>
    /// Gets or sets whether the target entries should be removed from their tables when the parent entry is deleted.
    /// </summary>
    public bool CascadeDelete { get; set; }

    /// <summary>
    /// Gets or sets whether the target entries should be insert or updated from their tables when the parent entry is inserted or updated.
    /// </summary>
    public bool CascadeInsertOrUpdate { get; set; }

    /// <summary>
    /// Gets or sets whether the target entries should be fetched from their tables when the parent entry is fetched.
    /// </summary>
    public bool CascadeSelect { get; set; } = true;

    /// <summary>
    /// Gets or sets the target table column (needed if the target table contains more than one primary key).
    /// </summary>
    public string TargetColumn { get; set; }

    #endregion
    
    public ForeignKeyColumnAttribute(string name) : base(name) { }
}