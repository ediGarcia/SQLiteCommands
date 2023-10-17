using NUnit.Framework;
using SQLiteCommands.Attributes.Field;
#pragma warning disable CS8625

namespace SQLiteCommandsTest.Attributes.Field;

[TestFixture]
internal class ForeignKeyColumnAttributeTest
{
    #region Constructor

    [Test]
    public void ForeignKeyColumnAttribute_Constructor_ShouldSetTheNameProperty()
    {
        const string columnNameMock = "ColumnName";
        ForeignKeyColumnAttribute field = null;

        Assert.DoesNotThrow(() => field = new(columnNameMock));
        Assert.AreEqual(columnNameMock, field.Name);
        Assert.IsNull(field.TargetColumn);
    }

    [Test]
    public void ForeignKeyColumnAttribute_Constructor_ShouldThrowException_WhenTheNameParameterIsNull()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new ForeignKeyColumnAttribute(null));
        Assert.AreEqual("The column's name must be filled. (Parameter 'name')", exception.Message);
    }

    #endregion

    #region Properties

    [TestCase(true, false, true)]
    [TestCase(false, true, false)]
    public void ForeignKeyColumnAttribute_Setters_ShouldSetTheProperties(
        bool cascadeDelete,
        bool cascadeInsertOrUpdate,
        bool cascadeSelect)
    {
        const string targetColumnMock = "targetColumnName";

        // Act
        ForeignKeyColumnAttribute columnAttribute = new("fieldName")
        {
            CascadeDelete = cascadeDelete,
            CascadeInsertOrUpdate = cascadeInsertOrUpdate,
            CascadeSelect = cascadeSelect,
            TargetColumn = targetColumnMock
        };

        // Assert
        Assert.AreEqual(cascadeDelete, columnAttribute.CascadeDelete);
        Assert.AreEqual(cascadeInsertOrUpdate, columnAttribute.CascadeInsertOrUpdate);
        Assert.AreEqual(cascadeSelect, columnAttribute.CascadeSelect);
        Assert.AreEqual(targetColumnMock, columnAttribute.TargetColumn);
    }

    #endregion
}