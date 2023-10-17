namespace SQLiteCommands.Attributes.Table;

/// <summary>
/// SELECT statement configuration.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
public class SelectOptionsAttribute : Attribute
{
    #region Properties

    /// <summary>
    /// Fetched data SQL filter.
    /// </summary>
    public string Filter { get; set; }

    /// <summary>
    /// Gets and sets the grouping settings for the SELECT command.
    /// </summary>
    public string GroupBy { get; set; }

    /// <summary>
    /// Gets and sets the maximum number of instances returned from the database.
    /// </summary>
    public int Limit { get; set; } = -1;

    /// <summary>
    /// Gets and sets the index from which data will be fetched.
    /// </summary>
    public int Offset { get; set; } = -1;

    /// <summary>
    /// Gets and sets the sorting settings for the SELECT command.
    /// </summary>
    public string OrderBy { get; set; }

    /// <summary>
    /// Gets and sets whether the SELECT statement should only consider PRIMARY KEY properties for filtering the results.
    /// </summary>
    public bool PrimaryKeyFilterOnly { get; set; } = true;

    /// <summary>
    /// Indicates whether only distinct instances will be returned from the database (based on the UNIQUE fields).
    /// </summary>
    public bool RemoveDuplicates { get; set; }

    #endregion
}