using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;
using SQLiteCommands.Enums;

namespace SQLiteCommandsTest.Mock.Classes;

[Table("TAB_TST_TEST")]
[Join("TAB_CRS_CROSS", "CRS", "CRS_INT_ID = TST_CRS_INT_ID", Mode = JoinMode.Cross)]
[Join("TAB_INN_INNER", "INN", "CRS_INT_ID = TST_INN_INT_ID", Mode = JoinMode.Inner)]
[Join("TAB_OUT_OUTER", "OUT", "CRS_INT_ID = TST_OUT_INT_ID", Mode = JoinMode.Outer)]
internal class TableJoinClass
{
    [Column("TST_INT_ID")]
    public int? Id { get; set; }
}