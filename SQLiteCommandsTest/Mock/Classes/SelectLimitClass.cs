using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;
// ReSharper disable StringLiteralTypo

namespace SQLiteCommandsTest.Mock.Classes;

[Table("TAB_TST_TEST")]
[SelectOptions(Limit = 7)]
internal class SelectLimitClass
{
    [Column("TST_INT_ID")]
    public int? Id { get; set; }

    [Column("TST_REA_DOUBLEVALUE")]
    public double? DoubleValue { get; set; }

    [Column("TST_TXT_STRINGVALUE")]
    public string StringValue { get; set; }
}