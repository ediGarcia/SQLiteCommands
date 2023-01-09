using SQLiteCommands.Attributes.Field;

namespace SQLiteCommandsTest.Mock.Classes;

internal class ColumnAndCustomColumnCombinationClass
{
    [Column("TAB_CHR_PROPNAME")]
    [CustomColumn("TAB_CHR_PROPNAME = 'PropName'")]
    public string PropName { get; set; }
}

internal class ColumnAndForeignKeyColumnCombinationClass
{
    [Column("TAB_CHR_PROPNAME")]
    [ForeignKeyColumn("TAB_TST_CHR_PROPNAME")]
    public string PropName { get; set; }
}
internal class ColumnAndOneToManyDataCombinationClass
{
    [Column("TAB_CHR_PROPNAME")]
    [OneToManyData]
    public string PropName { get; set; }
}

internal class ColumnAndManyToManyDataCombinationClass
{
    [Column("TAB_CHR_PROPNAME")]
    [ManyToManyData("TAB_TST_TEST")]
    public string PropName { get; set; }
}

internal class ColumnAndGroupingTargetCombinationClass
{
    [Column("TAB_CHR_PROPNAME")]
    [GroupingTarget]
    public string PropName { get; set; }
}

internal class ColumnAndSortingFieldCombinationClass
{
    [Column("TAB_CHR_PROPNAME")]
    [SortingField]
    public string PropName { get; set; }
}

internal class CustomAndForeignKeyColumnCombinationClass
{
    [CustomColumn("TAB_CHR_PROPNAME = 'PropName'")]
    [ForeignKeyColumn("TAB_TST_CHR_PROPNAME")]
    public string PropName { get; set; }
}

internal class CustomColumnAndOneToManyDataCombinationClass
{
    [CustomColumn("TAB_CHR_PROPNAME = 'PropName'")]
    [OneToManyData]
    public string PropName { get; set; }
}

internal class CustomColumnAndManyToManyDataCombinationClass
{
    [CustomColumn("TAB_CHR_PROPNAME = 'PropName'")]
    [ManyToManyData("TAB_TST_TEST")]
    public string PropName { get; set; }
}

internal class CustomColumnAndGroupingTargetCombinationClass
{
    [CustomColumn("TAB_CHR_PROPNAME = 'PropName'")]
    [GroupingTarget]
    public string PropName { get; set; }
}

internal class CustomColumnAndSortingFieldCombinationClass
{
    [CustomColumn("TAB_CHR_PROPNAME = 'PropName'")]
    [SortingField]
    public string PropName { get; set; }
}

internal class ForeignKeyColumnAndOneToManyDataCombinationClass
{
    [ForeignKeyColumn("TAB_TST_CHR_PROPNAME")]
    [OneToManyData]
    public string PropName { get; set; }
}

internal class ForeignKeyColumnAndManyToManyDataCombinationClass
{
    [ForeignKeyColumn("TAB_TST_CHR_PROPNAME")]
    [ManyToManyData("TAB_TST_TEST")]
    public string PropName { get; set; }
}

internal class ForeignKeyColumnAndGroupingTargetCombinationClass
{
    [ForeignKeyColumn("TAB_TST_CHR_PROPNAME")]
    [GroupingTarget]
    public string PropName { get; set; }
}

internal class ForeignKeyColumnAndSortingFieldCombinationClass
{
    [ForeignKeyColumn("TAB_TST_CHR_PROPNAME")]
    [SortingField]
    public string PropName { get; set; }
}

internal class OneToManyAndManyToManyDataCombinationClass
{
    [OneToManyData]
    [ManyToManyData("TAB_TST_TEST")]
    public string PropName { get; set; }
}

internal class OneToManyAndGroupingTargetCombinationClass
{
    [OneToManyData]
    [GroupingTarget]
    public string PropName { get; set; }
}

internal class OneToManyAndSortingFieldCombinationClass
{
    [OneToManyData]
    [SortingField]
    public string PropName { get; set; }
}

internal class GroupingTargetAndSortingFieldCombinationClass
{
    [GroupingTarget]
    [SortingField]
    public string PropName { get; set; }
}