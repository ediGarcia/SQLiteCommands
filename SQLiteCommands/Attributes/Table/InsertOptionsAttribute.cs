namespace SQLiteCommands.Attributes.Table;

/// <summary>
/// The options for the INSERT statement.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
internal class InsertOptionsAttribute : Attribute
{
    #region Properties

    /// <summary>
    /// Indicates whether the current data should overwrite any already existing in the database on FOREIGN KEY conflict.
    /// </summary>
    public bool ReplaceOnConflict { get; set; }

    #endregion
}