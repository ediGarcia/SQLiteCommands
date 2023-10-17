using SQLiteCommands.Helpers;

namespace SQLiteCommands.Attributes.Field;

[AttributeUsage(AttributeTargets.Property)]
public class OneToManyDataAttribute : Attribute
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
    /// Gets and sets the local column name that contains the identifying data (if null, the first primary key found in the parent class will be used).
    /// </summary>
    public string LocalColumn { get; set; }

    /// <summary>
    /// Gets the column name from the target table.
    /// </summary>
    public string TargetColumn { get; }

    #endregion

    public OneToManyDataAttribute(string targetColumn)
    {
        AttributeHelper.ValidatePropertyValue(targetColumn, nameof(targetColumn), "target table column name");
        TargetColumn = targetColumn;
    }
}