using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;

namespace SQLiteCommandsTest.Mock.Classes;

[Table("TAB_TST_TEST")]
public class InsertClass
{
    [Column("TST_INT_ID")]
    public int? Id { get; set; }

    [Column("TST_INT_ID2")]
    public int? IdIgnore { get; set; }

    [ForeignKeyColumn("FOR_TST_INT_ID")]
    public InsertForeignClass ForeignKey { get; set; }

    [ForeignKeyColumn("FOR_TST_CHR_STRING", TargetColumn = "FOR_CHR_STRING")]
    public InsertForeignClass ForeignKey2 { get; set; }

    [ForeignKeyColumn("FOR_TST_INT_ID2")]
    public InsertForeignClass ForeignKeyIgnore { get; set; }
}

[InsertOptions(ReplaceOnConflict = true)]
public class InsertOrReplaceClass : InsertClass { }

[Table("TAB_FOR_FOREIGN")]
public class InsertForeignClass
{
    [Column("FOR_INT_ID", IsPrimaryKey = true)]
    public int? Id { get; set; }

    [Column("FOR_CHR_STRING")]
    public string StringProperty { get; set; }
}
