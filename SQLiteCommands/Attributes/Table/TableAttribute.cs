using SQLiteCommands.Helpers;

namespace SQLiteCommands.Attributes.Table;

/// <summary>
/// Database table information.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
public class TableAttribute : Attribute
{
    #region Properties

    /// <summary>
    /// Gets and sets the table alias for JOIN clauses.
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Gets and sets the table's id.
    /// </summary>
    public string Name { get; }

    #endregion

    public TableAttribute(string name)
    {
        AttributeHelper.ValidatePropertyValue(name, nameof(name), "table name");
        Name = name;
    }
}