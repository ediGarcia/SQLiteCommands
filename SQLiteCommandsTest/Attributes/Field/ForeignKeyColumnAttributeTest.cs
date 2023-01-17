using NUnit.Framework;
using SQLiteCommands.Attributes.Field;

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
    }

    [Test]
    public void ForeignKeyColumnAttribute_Constructor_ShouldThrowException_WhenTheNameParameterIsNull()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new ForeignKeyColumnAttribute(null!));
        Assert.AreEqual("The column's name must be filled. (Parameter 'name')", exception.Message);
    }

    #endregion

    #region Properties

    [Test]
    public void ForeignKeyColumnAttribute_Setters_ShouldSetTheProperties()
    {
        const string targetColumnMock = "targetColumnName";

        // Act
        ForeignKeyColumnAttribute columnAttribute = new("fieldName")
        {
            TargetColumn = targetColumnMock
        };

        // Assert
        Assert.AreEqual(targetColumnMock, columnAttribute.TargetColumn);
    }

    #endregion
}