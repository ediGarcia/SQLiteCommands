using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;
using SQLiteCommands.Enums;

namespace SQLiteCommandsTest.Mock.Classes;

[Table("TAB_TST_TEST")]
internal class TableSortedFieldsClass
{
    [Column("TST_INT_ID")]
    [SortingField(ColumnName = "Id")]
    public int? Id { get; set; }

    [SortingField(ColumnName = "TST_INT_COLUMN", Direction = SortingDirection.Descending, PlaceNullsAtTheEnd = true)]
    public int IntColumn { get; set; }
}