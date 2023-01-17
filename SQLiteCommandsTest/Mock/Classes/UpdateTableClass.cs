using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;

namespace SQLiteCommandsTest.Mock.Classes;

[Table("TAB_TST_TEST")]
internal class UpdateTableClass
{
    [Column("TST_INT_ID", IsPrimaryKey = true)]
    public int? Id { get; set; }

    [Column("TST_INT_IDIGNORE", IsPrimaryKey = true, IgnoreOnUpdate = true)]
    public int? IdIgnore { get; set; }

    [Column("TST_STR_PROP")]
    public string StringProperty { get; set; }

    [Column("TST_STR_PROPIGNORE", IgnoreOnUpdate = true)]
    public string StringPropertyIgnore { get; set; }

    [Column("TST_STR_PROP")]
    public string StringProperty2 { get; set; }

    [Column("TST_STR_PROP3")]
    public string StringProperty3 { get; set; }

    [ForeignKeyColumn("TST_FOR_INT_ID")]
    public UpdateTableForeignClass ForeignKeyColumn { get; set; }

    [ForeignKeyColumn("TST_FOR_INT_ID2")]
    public UpdateTableForeignClass ForeignKeyColumn2 { get; set; }

    [ForeignKeyColumn("TST_FOR_INT_ID3", IgnoreOnUpdate = true)]
    public UpdateTableForeignClass ForeignKeyColumnIgnore { get; set; }
}

[Table("TAB_FOR_FOREIGN")]
internal class UpdateTableForeignClass
{
    [Column("FOR_INT_ID", IsPrimaryKey = true)]
    public int? Id { get; set; }
}