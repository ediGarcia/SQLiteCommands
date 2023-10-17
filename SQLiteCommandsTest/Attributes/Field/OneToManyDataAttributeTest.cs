using NUnit.Framework;
using SQLiteCommands.Attributes.Field;

namespace SQLiteCommandsTest.Attributes.Field;

internal class OneToManyDataAttributeTest
{
    #region Constructor

    [Test]
    public void OneToManyDataAttribute_Constructor_ShouldSetTheTargetColumn()
    {
        // Arrange
        const string targetColumnMock = "TargetMock";

        // Act
        OneToManyDataAttribute field = new(targetColumnMock);

        // Assert
        Assert.AreEqual(targetColumnMock, field.TargetColumn);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void OneToManyDataAttribute_Constructor_ShouldThrowExceptionWhenTargetColumnIsEmpty(string targetColumn)
    {
        // Act & Assert
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() => new OneToManyDataAttribute(targetColumn));
        Assert.AreEqual("The target table column name must be filled. (Parameter 'targetColumn')", exception.Message);
    }

    #endregion

    #region Properties

    [TestCase(true, false, true)]
    [TestCase(false, true, false)]
    public void OneToManyDataAttribute_Setters_ShouldSetTheProperties(
        bool cascadeDelete,
        bool cascadeInsertOrUpdate,
        bool cascadeSelect)
    {
        const string localColumnMock = "localColumnName";

        // Act
        OneToManyDataAttribute oneToManyData = new("TargetColumn")
        {
            CascadeDelete = cascadeDelete,
            CascadeInsertOrUpdate = cascadeInsertOrUpdate,
            CascadeSelect = cascadeSelect,
            LocalColumn = localColumnMock
        };

        // Assert
        Assert.AreEqual(cascadeDelete, oneToManyData.CascadeDelete);
        Assert.AreEqual(cascadeInsertOrUpdate, oneToManyData.CascadeInsertOrUpdate);
        Assert.AreEqual(cascadeSelect, oneToManyData.CascadeSelect);
        Assert.AreEqual(localColumnMock, oneToManyData.LocalColumn);
    }

    #endregion
}