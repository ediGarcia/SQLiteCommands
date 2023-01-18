using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;

namespace SQLiteCommandsTest.Mock.Classes;

internal abstract class InsertClasses
{
    [Column("TST_INT_ID")]
    public int? Id { get; set; }

    [Column("TST_INT_ID2", IgnoreOnInsert = true)]
    public int? IdIgnore { get; set; }

    [ForeignKeyColumn("FOR_TST_INT_ID")]
    public InsertForeignClass ForeignKey { get; set; }

    [ForeignKeyColumn("FOR_TST_CHR_STRING", TargetColumn = "FOR_CHR_STRING")]
    public InsertForeignClass ForeignKey2 { get; set; }

    [ForeignKeyColumn("FOR_TST_INT_ID2", IgnoreOnInsert = true)]
    public InsertForeignClass ForeignKeyIgnore { get; set; }
}

[Table("TAB_TST_TEST")]
internal class InsertClassWithoutAttribute : InsertClasses { }

[Table("TAB_TST_TEST")]
[InsertOptions]
internal class InsertClass : InsertClasses { }

[Table("TAB_TST_TEST")]
[InsertOptions(ReplaceOnConflict = true)]
internal class InsertOrReplaceClass : InsertClasses { }

[Table("TAB_FOR_FOREIGN")]
internal class InsertForeignClass
{
    [Column("FOR_INT_ID", IsPrimaryKey = true)]
    public int? Id { get; set; }

    [Column("FOR_CHR_STRING")]
    public string StringProperty { get; set; }
}
