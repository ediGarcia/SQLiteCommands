using SQLiteCommands.Enums;
using SQLiteCommands.Helpers;

namespace SQLiteCommands.Attributes.Table;

/// <summary>
/// JOIN clause configuration.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = true)]
public class JoinAttribute : Attribute
{
    #region Properties

    /// <summary>
    /// Gets the target table alias.
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Gets and sets the JOIN clause constraints expression.
    /// </summary>
    public string Constraint { get; }

    /// <summary>
    /// Gets and sets the JOIN clause mode.
    /// </summary>
    public JoinMode Mode { get; set; } = JoinMode.Inner;

    /// <summary>
    /// Gets the main table name or alias.
    /// </summary>
    public string Table { get; }

    #endregion

    public JoinAttribute(string table, string alias, string constraint)
    {
        AttributeHelper.CheckNullProperty(table, nameof(table), "The table name");
        AttributeHelper.CheckNullProperty(constraint, nameof(constraint), "The JOIN clause constraint");

        Table = table;
        Alias = alias;
        Constraint = constraint;
    }
}