using SQLiteCommands.Enums;

namespace SQLiteCommands.Attributes.Field;

/// <summary>
/// Ordering conditions for the retrieved data.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SortingFieldAttribute : Attribute
{
    #region Properties

    /// <summary>
    /// Gets and sets the column's name (optional, to be used when no other attribute sets the column's name).
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    /// Gets and sets the sorting direction.
    /// </summary>
    public SortingDirection Direction { get; set; }

    /// <summary>
    /// Indicates whether the select column NULL values should be placed at the end of the retrieved data array.
    /// </summary>
    public bool PlaceNullsAtTheEnd { get; set; }

    /// <summary>
    /// Gets this field's position in the sorting fields list (in case of equal values, the definition position within the class will be considered).
    /// </summary>
    public int SortingIndex
    {
        get => _sortingIndex;
        set
        {
            if (value < 0)
                throw new IndexOutOfRangeException("The sorting index must be equal or greater than 0.");

            _sortingIndex = value;
        }
    }

    /// <summary>
    /// Gets and sets the parent table alias (optional, to be used when no other attribute sets the parent table's alias).
    /// </summary>
    public string TableAlias { get; set; }

    #endregion

    private int _sortingIndex = Int32.MaxValue;
}