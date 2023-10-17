using SQLiteCommands.Helpers;

namespace SQLiteCommands.Attributes.Field;

[AttributeUsage(AttributeTargets.Property)]
public class ManyToManyDataAttribute : Attribute
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
    /// The join table name.
    /// </summary>
    public string JoinTable { get; }

    /// <summary>
    /// Gets or sets the join table key that points to the current table.
    /// </summary>
    public string JoinTableSourcePrimaryKey { get; }

    /// <summary>
    /// Gets or sets the join table key that points to the target type table.
    /// </summary>
    public string JoinTableTargetPrimaryKey { get; }

    /// <summary>
    /// Gets or sets the current table column that is pointed by the join table primary key (if empty, the first primary key of the current table will be used).
    /// </summary>
    public string LocalColumn { get; set; }

    /// <summary>
    /// Gets or sets the target table column that is pointed by the join table primary key (if empty, the first primary key of the target table will be used).
    /// </summary>
    public string TargetTableColumn { get; set; }

    #endregion

    public ManyToManyDataAttribute(string joinTable, string joinTableSourcePrimaryKey, string joinTableTargetPrimaryKey)
    {
        AttributeHelper.ValidatePropertyValue(joinTable, nameof(joinTable), "join table name");

        AttributeHelper.ValidatePropertyValue(
            joinTableSourcePrimaryKey,
            nameof(joinTableSourcePrimaryKey),
            "join table source primary key name");

        AttributeHelper.ValidatePropertyValue(
            joinTableTargetPrimaryKey, 
            nameof(joinTableTargetPrimaryKey),
            "join table target primary key name");

        JoinTable = joinTable;
        JoinTableSourcePrimaryKey = joinTableSourcePrimaryKey;
        JoinTableTargetPrimaryKey = joinTableTargetPrimaryKey;
    }
}