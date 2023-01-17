using SQLiteCommands.Helpers;

namespace SQLiteCommands.Attributes.Field;

[AttributeUsage(AttributeTargets.Property)]
public class ManyToManyDataAttribute : Attribute
{
    #region Properties

    /// <summary>
    /// The junction table name.
    /// </summary>
    public string JunctionTable { get; }

    /// <summary>
    /// Gets and sets the junction table target column (needed if the junction table column name is different from the source table's column).
    /// </summary>
    public string JunctionTableColumn { get; set; }

    /// <summary>
    /// Gets and sets the source table column (needed if the table has more than one primary key).
    /// </summary>
    public string LocalColumn { get; set; }

    #endregion

    public ManyToManyDataAttribute(string junctionTable)
    {
        AttributeHelper.CheckNullProperty(junctionTable, nameof(junctionTable), "junction table name");
        JunctionTable = junctionTable;
    }
}