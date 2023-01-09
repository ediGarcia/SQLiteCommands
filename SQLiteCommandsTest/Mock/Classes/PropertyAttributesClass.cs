using SQLiteCommands.Attributes.Field;

namespace SQLiteCommandsTest.Mock;

internal class PropertyAttributesClass
{
    [Column("TAB_CHR_PROP1")]
    public string Prop1 { get; set; }

    [CustomColumn("TAB_CHR_PROP2")]
    public string Prop2 { get; set; }
}