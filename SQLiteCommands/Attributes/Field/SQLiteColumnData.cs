using SQLiteCommands.Enums;
using SQLiteCommands.Helpers;

namespace SQLiteCommands.Attributes.Field;

[AttributeUsage(AttributeTargets.Property)]
// ReSharper disable once InconsistentNaming
public class SQLiteColumnData : Attribute
{
    #region Properties

    /// <summary>
    /// Gets or sets this fields behaviour on DELETE commands.
    /// </summary>
    public Behaviour DeleteBehaviour { get; set; } = Behaviour.IgnoreWhenNull;

    /// <summary>
    /// Gets or sets this fields behaviour on INSERT commands.
    /// </summary>
    public Behaviour InsertBehaviour { get; set; } = Behaviour.IgnoreWhenNull;

    /// <summary>
    /// Indicates whether the current column contains the PRIMARY KEY constraint.
    /// </summary>
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// Gets and sets the column's name.
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Gets or sets this fields behaviour on SELECT commands.
    /// </summary>
    public Behaviour SelectBehaviour { get; set; } = Behaviour.IgnoreWhenNull;

    /// <summary>
    /// Gets and sets the parent table alias.
    /// </summary>
    public string TableAlias { get; set; }

    /// <summary>
    /// Gets or sets this fields behaviour on UPDATE commands.
    /// </summary>
    public Behaviour UpdateBehaviour { get; set; } = Behaviour.IgnoreWhenNull;

    #endregion

    public SQLiteColumnData(string name)
    {
        AttributeHelper.ValidatePropertyValue(name, nameof(name), "column's name");
        Name = name;
    }
}