namespace SQLiteCommands.Attributes.Field;

[AttributeUsage(AttributeTargets.Property)]
public class OneToManyDataAttribute : Attribute
{
    #region Properties

    /// <summary>
    /// Gets and sets the local column name that contains the identifying data (needed only if the parent class contains more than one primary key).
    /// </summary>
    public string LocalColumn { get; set; }

    /// <summary>
    /// Gets the column name from the target table (needed if the target columns name is different from the source table's column).
    /// </summary>
    public string TargetColumn { get; set; }

    #endregion
}