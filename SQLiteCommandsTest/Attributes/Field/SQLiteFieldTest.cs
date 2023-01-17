using NUnit.Framework;
using SQLiteCommands.Attributes.Field;

namespace SQLiteCommandsTest.Attributes.Field;

[TestFixture]
// ReSharper disable once InconsistentNaming
internal class SQLiteFieldTest
{
    #region Constructor

    [Test]
    public void SQLiteField_Constructor_ShouldSetTheNameProperty()
    {
        const string columnNameMock = "ColumnName";
        SQLiteField field = null;

        Assert.DoesNotThrow(() => field = new(columnNameMock));
        Assert.AreEqual(columnNameMock, field.Name);
    }

    [Test]
    public void SQLiteField_Constructor_ShouldThrowException_WhenTheNameParameterIsNull()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new SQLiteField(null!));
        Assert.AreEqual("The column's name must be filled. (Parameter 'name')", exception.Message);
    }

    #endregion

    #region Properties

    [TestCase(true, false, true, false, true)]
    [TestCase(false, true, false, true, false)]
    public void SQLiteField_Setters_ShouldSetTheProperties(bool ignoreOnDelete, bool ignoreOnInsert,
        bool ignoreOnSelect, bool ignoreOnUpdate, bool isPrimaryKey)
    {
        const string tableAliasMock = "tableAlias";

    // Act
    SQLiteField field = new("fieldName")
        {
            IgnoreOnDelete = ignoreOnDelete,
            IgnoreOnInsert = ignoreOnInsert,
            IgnoreOnSelect = ignoreOnSelect,
            IgnoreOnUpdate = ignoreOnUpdate,
            IsPrimaryKey = isPrimaryKey,
            TableAlias = tableAliasMock
        };

        // Assert
        Assert.AreEqual(ignoreOnDelete, field.IgnoreOnDelete);
        Assert.AreEqual(ignoreOnInsert, field.IgnoreOnInsert);
        Assert.AreEqual(ignoreOnSelect, field.IgnoreOnSelect);
        Assert.AreEqual(ignoreOnUpdate, field.IgnoreOnUpdate);
        Assert.AreEqual(isPrimaryKey, field.IsPrimaryKey);
        Assert.AreEqual(tableAliasMock, field.TableAlias);
    }

    #endregion
}