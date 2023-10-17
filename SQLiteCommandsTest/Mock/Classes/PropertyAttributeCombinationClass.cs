using SQLiteCommands.Attributes.Field;
// ReSharper disable StringLiteralTypo

namespace SQLiteCommandsTest.Mock.Classes;

internal class InvalidAttributeCombinationClass
{
    [Column("COLUNM_NAME")]
    [CustomColumn("COLUMN_NAME > 1")]
    public int Property1 { get; set; }

    [Column("COLUNM_NAME")]
    [ForeignKeyColumn("COLUM_NAME")]
    public int Property2 { get; set; }

    [Column("COLUNM_NAME")]
    [OneToManyData("COLUMN_NAME")]
    public int Property3 { get; set; }

    [Column("COLUNM_NAME")]
    [ManyToManyData("TABLE_NAME", "SOURCE_COLUMN_NAME", "TARGET_COLUMN_NAME")]
    public int Property4 { get; set; }

    [CustomColumn("COLUMN_NAME > 1")]
    [ForeignKeyColumn("COLUM_NAME")]
    public int Property5 { get; set; }

    [CustomColumn("COLUMN_NAME > 1")]
    [OneToManyData("COLUMN_NAME")]
    public int Property6 { get; set; }

    [CustomColumn("COLUMN_NAME > 1")]
    [ManyToManyData("TABLE_NAME", "SOURCE_COLUMN_NAME", "TARGET_COLUMN_NAME")]
    public int Property7 { get; set; }

    [ForeignKeyColumn("COLUM_NAME")]
    [OneToManyData("COLUMN_NAME")]
    public int Property8 { get; set; }

    [ForeignKeyColumn("COLUM_NAME")]
    [ManyToManyData("TABLE_NAME", "SOURCE_COLUMN_NAME", "TARGET_COLUMN_NAME")]
    public int Property9 { get; set; }

    [OneToManyData("COLUMN_NAME")]
    [ManyToManyData("TABLE_NAME", "SOURCE_COLUMN_NAME", "TARGET_COLUMN_NAME")]
    public int Property10 { get; set; }
}

internal class ValidAttributeCombinationClass
{
    [Column("COLUMN_NAME")]
    public int Property1 { get; set; }

    [ForeignKeyColumn("COLUM_NAME")]
    public int Property2 { get; set; }

    [OneToManyData("COLUMN_NAME")]
    public int Property3 { get; set; }

    [ManyToManyData("TABLE_NAME", "SOURCE_COLUMN_NAME", "TARGET_COLUMN_NAME")]
    public int Property4 { get; set; }
}