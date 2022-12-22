using SQLiteCommands.Attributes.Table;

namespace SQLiteCommandsTest.Mock.Classes;

[Table("TAB_TST_TEST")]
internal class TableWithoutColumnsClass
{
    public int? Id { get; set; }

    public string StringValue { get; set; }
}