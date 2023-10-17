using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;
using SQLiteCommands.Enums;

namespace SQLiteCommandsTest.Mock.Classes;

[Table("TAB_DEL_DELETE-TEST")]
public class DeleteClassMock
{
    [Column("DEL_INT_PRIMARYFIELD", IsPrimaryKey = true)]
    public int? PrimaryProperty { get; set; }

    [Column("DEL_INT_PRIMARYFIELD", IsPrimaryKey = true, DeleteBehaviour = Behaviour.AlwaysIgnore)]
    public int? PrimaryPropertyIgnore { get; set; }

    [Column("DEL_INT_FIELD")]
    public int? Property { get; set; }

    [Column("DEL_INT_IGNOREFIELD", IsPrimaryKey = true, DeleteBehaviour = Behaviour.AlwaysIgnore)]
    public int? IgnoreProperty { get; set; }

    [Column("DEL_INT_NULLFIELD", IsPrimaryKey = true, DeleteBehaviour = Behaviour.AlwaysInclude)]
    public object NullProperty { get; set; }

    [Column("DEL_INT_NULLIGNOREFIELD", IsPrimaryKey = true)]
    public object NullIgnoreProperty { get; set; }

    [ForeignKeyColumn("FOR_DEL_INT_PRIMARYFIELD", IsPrimaryKey = true)]
    public ForeignDeleteClassMock ForeignPrimaryProperty { get; set; }

    [ForeignKeyColumn("FOR_DEL_INT_PRIMARYFIELD", IsPrimaryKey = true, DeleteBehaviour = Behaviour.AlwaysIgnore)]
    public ForeignDeleteClassMock ForeignPrimaryPropertyIgnore { get; set; }

    [ForeignKeyColumn("FOR_DEL_INT_FIELD")]
    public ForeignDeleteClassMock ForeignProperty { get; set; }

    [ForeignKeyColumn("FOR_DEL_INT_IGNOREFIELD", IsPrimaryKey = true, DeleteBehaviour = Behaviour.AlwaysIgnore)]
    public ForeignDeleteClassMock ForeignIgnoreProperty { get; set; }

    [ForeignKeyColumn("FOR_DEL_INT_NULLFIELD", IsPrimaryKey = true, DeleteBehaviour = Behaviour.AlwaysInclude)]
    public ForeignDeleteClassMock ForeignNullProperty { get; set; }

    [ForeignKeyColumn("FOR_DEL_INT_INNERNULLFIELD", IsPrimaryKey = true, DeleteBehaviour = Behaviour.AlwaysInclude)]
    public ForeignDeleteClassMock ForeignInnerNullProperty { get; set; }

    [ForeignKeyColumn("FOR_DEL_INT_NULLIGNOREFIELD", IsPrimaryKey = true)]
    public ForeignDeleteClassMock ForeignNullIgnoreProperty { get; set; }

    [ForeignKeyColumn("FOR_DEL_INT_INNERNULLIGNOREFIELD", IsPrimaryKey = true)]
    public ForeignDeleteClassMock ForeignInnerNullIgnoreProperty { get; set; }

    public int NoAttributeProperty { get; set; }
}

[Table("TAB_FOR_FOREIGN")]
public class ForeignDeleteClassMock
{
    [Column("FOR_INT_PRIMARY", IsPrimaryKey = true)]
    public int? ForeignPrimaryKey { get; set; }
}

[Table("TAB_FOR_FOREIGN")]
public class EmptyDeleteClassMock { }