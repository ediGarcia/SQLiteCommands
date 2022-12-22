using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;
// ReSharper disable StringLiteralTypo

namespace SQLiteCommandsTest.Mock.Classes;

[Table("TAB_TST_TEST", Alias = "TST")]
internal class TableAliasClass
{
    [Column("TST_INT_ID")]
    public int? Id { get; set; }

    [Column("TST_INT_COLUMNALIAS", TableAlias = "ALI")]
    public int? ColumnWithAlias { get; set; }

    [ForeignKeyColumn("TST_FOR_INT_ID", TargetColumn = "FOR_INT_ID")]
    public TableForeignKeyTargetAliasClass ForeignKey { get; set; }

    [ForeignKeyColumn("TST_FOR_INT_ID2", TableAlias = "FOR", TargetColumn = "FOR_INT_ID2")]
    public TableForeignKeyTargetAliasClass ForeignKey2 { get; set; }

    [SortingField(ColumnName = "TST_INT_SORTING")]
    public int? SortingColumn { get; set; }

    [SortingField(ColumnName = "TST_INT_SORTING2", TableAlias = "SOR")]
    public int? SortingColumn2 { get; set; }

    [GroupingTarget(ColumnName = "TST_INT_GROUPCOLUMN")]
    public int? GroupingColumn { get; set; }

    [GroupingTarget(ColumnName = "TST_INT_GROUPCOLUMN2", TableAlias = "GRP")]
    public int? GroupingColumn2 { get; set; }
}
    
[Table("TAB_FOR_FOREIGNKEYTARGET")]
internal class TableForeignKeyTargetAliasClass
{
    [Column("FOR_INT_ID")]
    public int Id { get; set; }

    [Column("FOR_INT_ID2")]
    public int Id2 { get; set; }
}