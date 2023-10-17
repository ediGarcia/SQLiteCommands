using NUnit.Framework;
using SQLiteCommands.Attributes.Field;

namespace SQLiteCommandsTest.Attributes.Field;

internal class ManyToManyDataAttributeTest
{
    #region Constructor

    [Test]
    public void ManyToManyDataAttribute_Constructor_ShouldSetTheJoinTableData()
    {
        // Arrange
        const string joinTableMock = "JoinTable";
        const string joinTableSourcePrimaryKeyMock = "SourceColumn";
        const string joinTableTargetPrimaryKeyMock = "TargetColumn";

        // Act
        ManyToManyDataAttribute field = new(
            joinTableMock,
            joinTableSourcePrimaryKeyMock,
            joinTableTargetPrimaryKeyMock);

        // Assert
        Assert.AreEqual(joinTableMock, field.JoinTable);
        Assert.AreEqual(joinTableSourcePrimaryKeyMock, field.JoinTableSourcePrimaryKey);
        Assert.AreEqual(joinTableTargetPrimaryKeyMock, field.JoinTableTargetPrimaryKey);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void ManyToManyDataAttribute_Constructor_ShouldThrowException_WhenJoinTableIsNullOrEmpty(string joinTable)
    {
        // Act & Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => 
            new ManyToManyDataAttribute(
                joinTable,
                "SourceColumn",
                "TargetColumn"));
        Assert.AreEqual(
            "The join table name must be filled. (Parameter 'joinTable')",
            exception.Message);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void ManyToManyDataAttribute_Constructor_ShouldThrowException_WhenJoinTableSourcePrimaryKeyIsNull(string joinTableSourcePrimaryKey)
    {
        // Act & Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
            new ManyToManyDataAttribute(
                "JoinTable",
                joinTableSourcePrimaryKey,
                "TargetColumn"));
        Assert.AreEqual(
            "The join table source primary key name must be filled. (Parameter 'joinTableSourcePrimaryKey')",
            exception.Message);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void ManyToManyDataAttribute_Constructor_ShouldThrowException_WhenJoinTableTargetPrimaryKeyIsNull(string joinTableTargetPrimaryKey)
    {
        // Act & Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
            new ManyToManyDataAttribute(
                "JoinTable",
                "SourceColumn",
                joinTableTargetPrimaryKey));
        Assert.AreEqual(
            "The join table target primary key name must be filled. (Parameter 'joinTableTargetPrimaryKey')",
            exception.Message);
    }

    #endregion

    #region Properties

    [TestCase(true, false, true)]
    [TestCase(false, true, false)]
    public void ManyToManyDataAttribute_Setters_ShouldSetTheProperties(
        bool cascadeDelete,
        bool cascadeInsertOrUpdate,
        bool cascadeSelect)
    {
        // Arrange
        const string localColumn = "LocalColumn";
        const string targetTableColumn = "TargetTableColumn";

        // Act
        ManyToManyDataAttribute field = new(
            "JoinTable",
            "JoinTableSourcePrimaryKey",
            "JoinTableTargetPrimaryKey")
        {
            CascadeDelete = cascadeDelete,
            CascadeInsertOrUpdate = cascadeInsertOrUpdate,
            CascadeSelect = cascadeSelect,
            LocalColumn = localColumn,
            TargetTableColumn = targetTableColumn
        };

        // Assert
        Assert.AreEqual(cascadeDelete, field.CascadeDelete);
        Assert.AreEqual(cascadeInsertOrUpdate, field.CascadeInsertOrUpdate);
        Assert.AreEqual(cascadeSelect, field.CascadeSelect);
        Assert.AreEqual(localColumn, field.LocalColumn);
        Assert.AreEqual(targetTableColumn, field.TargetTableColumn);
    }

    #endregion
}