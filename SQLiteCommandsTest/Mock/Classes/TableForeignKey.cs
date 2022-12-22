using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;
// ReSharper disable StringLiteralTypo

namespace SQLiteCommandsTest.Mock.Classes;

[Table("TAB_TST_TEST")]
internal class TableForeignKey
{
    [ForeignKeyColumn("TST_FOR_INT_ID")]
    public TableForeignKeyTargetClass ForeignKey { get; set; }

    [ForeignKeyColumn("TST_FOR_INT_NOPRIMARY", TargetColumn = "FOR_INT_NOPRIMARY")]
    public TableForeignKeyTargetClass NoPrimaryForeignKey { get; set; }

    [ForeignKeyColumn("TST_FOR_INT_NOTABLE")]
    public NoTableClass NoTable { get; set; }
}

[Table("TAB_FOR_FOREIGNTABLE")]
internal class TableForeignKeyTargetClass
{
    [Column("FOR_INT_ID", IsPrimaryKey = true)]
    public int? Id { get; set; }

    [Column("FOR_INT_NOPRIMARY")]
    public int? NonPrimaryColumn { get; set; }
}

internal class NoTableClass
{

}