using NUnit.Framework;
using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Enums;
#pragma warning disable CS8625

namespace SQLiteCommandsTest.Attributes.Field;

[TestFixture]
// ReSharper disable once InconsistentNaming
internal class SQLiteColumnDataTest
{
    #region Constructor

    [Test]
    public void SQLiteField_Constructor_ShouldSetTheNameProperty()
    {
        const string columnNameMock = "ColumnName";
        SQLiteColumnData field = null;

        Assert.DoesNotThrow(() => field = new(columnNameMock));
        Assert.AreEqual(columnNameMock, field.Name);
    }

    [Test]
    public void SQLiteField_Constructor_ShouldThrowException_WhenTheNameParameterIsNull()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new SQLiteColumnData(null));
        Assert.AreEqual("The column's name must be filled. (Parameter 'name')", exception.Message);
    }

    #endregion

    #region Properties

    [Test]
    public void SQLiteField_Constructor_ShouldSetTheColumnName()
    {
        // Arrange
        const string columnName = "columnName";

        // Act
        SQLiteColumnData field = new(columnName);

        // Assert
        Assert.AreEqual(columnName, field.Name);
    }

    [Test]
    public void SQLiteField_Constructor_ShouldThrowException_WhenTheColumnNameIsEmpty() =>
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SQLiteColumnData(null), "The column's name must be filled");

    [TestCase(Behaviour.AlwaysInclude)]
    [TestCase(Behaviour.AlwaysIgnore)]
    [TestCase(Behaviour.IgnoreWhenNull)]
    public void SQLiteField_Setters_ShouldSetTheDeleteBehaviour(Behaviour behaviour)
    {
        // Act
        SQLiteColumnData field = new("fieldName") { DeleteBehaviour = behaviour };
        
        // Assert
        Assert.AreEqual(behaviour, field.DeleteBehaviour);
    }

    [TestCase(Behaviour.AlwaysInclude)]
    [TestCase(Behaviour.AlwaysIgnore)]
    [TestCase(Behaviour.IgnoreWhenNull)]
    public void SQLiteField_Setters_ShouldSetTheInsertBehaviour(Behaviour behaviour)
    {
        // Act
        SQLiteColumnData field = new("fieldName") { InsertBehaviour = behaviour };

        // Assert
        Assert.AreEqual(behaviour, field.InsertBehaviour);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void SQLiteField_Setters_ShouldSetTheIsPrimaryKeyProperty(bool isPrimaryKey)
    {
        // Act
        SQLiteColumnData field = new("fieldName") { IsPrimaryKey = isPrimaryKey };

        // Assert
        Assert.AreEqual(isPrimaryKey, field.IsPrimaryKey);
    }

    [TestCase(Behaviour.AlwaysInclude)]
    [TestCase(Behaviour.AlwaysIgnore)]
    [TestCase(Behaviour.IgnoreWhenNull)]
    public void SQLiteField_Setters_ShouldSetTheSelectBehaviour(Behaviour behaviour)
    {
        // Act
        SQLiteColumnData field = new("fieldName") { SelectBehaviour = behaviour };

        // Assert
        Assert.AreEqual(field.SelectBehaviour, behaviour);
    }

    [Test]
    public void SQLiteField_Setters_ShouldSetTheTableAlias()
    {
        // Arrange
        const string tableAliasMock = "tableAlias";

        // Act
        SQLiteColumnData field = new("fieldName") { TableAlias = tableAliasMock};

        // Assert
        Assert.AreEqual(tableAliasMock, field.TableAlias);
    }

    [TestCase(Behaviour.AlwaysInclude)]
    [TestCase(Behaviour.AlwaysIgnore)]
    [TestCase(Behaviour.IgnoreWhenNull)]
    public void SQLiteField_Setters_ShouldSetTheUpdateBehaviour(Behaviour behaviour)
    {
        // Act
        SQLiteColumnData field = new("fieldName") { UpdateBehaviour = behaviour };

        // Assert
        Assert.AreEqual(field.UpdateBehaviour, behaviour);
    }

    #endregion
}