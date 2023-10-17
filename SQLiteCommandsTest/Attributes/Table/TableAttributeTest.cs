using NUnit.Framework;
using SQLiteCommands.Attributes.Table;

namespace SQLiteCommandsTest.Attributes.Table;

internal class TableAttributeTest
{
    #region Constructor

    [Test]
    public void TableAttribute_Constructor_ShouldSetTheTableName()
    {
        // Arrange
        const string tableNameMock = "TableName";

        // Act
        TableAttribute table = new(tableNameMock);

        // Assert
        Assert.AreEqual(tableNameMock, table.Name);
        Assert.IsNull(table.Alias);

    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void TableAttribute_Constructor_ShouldThrowException_WhenTableNameIsEmpty(string tableName)
    {
        // Act & Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new TableAttribute(tableName));
        Assert.AreEqual("The table name must be filled. (Parameter 'name')", exception.Message);
    }

    #endregion

    #region Setters

    [Test]
    public void TableAttribute_Setters_ShouldSetTheProperties()
    {
        // Arrange
        const string aliasMock = "TableAlias";

        // Act
        TableAttribute table = new("tableName")
        {
            Alias = aliasMock
        };

        // Assert
        Assert.AreEqual(aliasMock, table.Alias);
    }

    #endregion
}