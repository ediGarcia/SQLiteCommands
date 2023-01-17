using NUnit.Framework;
using SQLiteCommands.Attributes.Field;

namespace SQLiteCommandsTest.Attributes.Field;

internal class ManyToManyDataAttributeTest
{
    #region Constructor

    [Test]
    public void ManyToManyDataAttribute_Constructor_ShouldSetTheNameProperty()
    {
        // Arrange
        const string junctionTableMock = "ColumnName";

        // Act
        ManyToManyDataAttribute manyToManyDataAttribute = new(junctionTableMock);

        // Assert
        Assert.AreEqual(junctionTableMock, manyToManyDataAttribute.JunctionTable);
    }

    [Test]
    public void ManyToManyDataAttribute_Constructor_ShouldThrowException_WhenTheNameParameterIsNull()
    {
        // Act & Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new ManyToManyDataAttribute(null!));
        Assert.AreEqual("The junction table name must be filled. (Parameter 'junctionTable')", exception.Message);
    }

    #endregion

    #region Properties

    [Test]
    public void ManyToManyDataAttribute_Setters_ShouldSetTheProperties()
    {
        const string junctionTableColumnMock = "junctionTableColumn";
        const string localColumnMock = "localColumn";

        // Act
        ManyToManyDataAttribute manyToManyData = new("junctionTable")
        {
            JunctionTableColumn = junctionTableColumnMock,
            LocalColumn = localColumnMock
        };

        // Assert
        Assert.AreEqual(junctionTableColumnMock, manyToManyData.JunctionTableColumn);
        Assert.AreEqual(localColumnMock, manyToManyData.LocalColumn);
    }

    #endregion
}