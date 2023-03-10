using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;
// ReSharper disable StringLiteralTypo

namespace SQLiteCommandsTest.Mock.Classes;

[Table("TAB_TST_TEST")]
[SelectOptions(Having = "TST_INT_INTCOLUMN > 5")]
internal class SelectGroupingHavingClass
{
    [Column("TST_INT_ID")]
    public int? Id { get; set; }

    [Column("TST_INT_INTCOLUMN")]
    [GroupingTarget]
    public int? IntColumn { get; set; }

    [GroupingTarget(ColumnName = "TST_CHR_TEXTCOLUMN")]
    public string TextColumn { get; set; }
}