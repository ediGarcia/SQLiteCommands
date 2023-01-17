using NUnit.Framework;
using SQLiteCommands.Attributes.Field;

namespace SQLiteCommandsTest.Attributes.Field;

[TestFixture]
internal class ColumnAttributeTest
{
    #region Constructor

    [Test]
    public void ColumnAttribute_Constructor_ShouldSetTheNameProperty()
    {
        const string columnNameMock = "ColumnName";
        ColumnAttribute field = null;

        Assert.DoesNotThrow(() => field = new(columnNameMock));
        Assert.AreEqual(columnNameMock, field.Name);
    }

    [Test]
    public void ColumnAttribute_Constructor_ShouldThrowException_WhenTheNameParameterIsNull()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new ColumnAttribute(null!));
        Assert.AreEqual("The column's name must be filled. (Parameter 'name')", exception.Message);
    }

    #endregion

    #region Properties

    [Test]
    public void ColumnAttribute_Setters_ShouldSetTheProperties()
    {
        object defaultValueMock = new();

        // Act
        ColumnAttribute columnAttribute = new("fieldName")
        {
            DefaultValue = defaultValueMock
        };

        // Assert
        Assert.AreSame(defaultValueMock, columnAttribute.DefaultValue);
    }

    #endregion
}