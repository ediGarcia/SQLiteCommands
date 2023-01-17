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
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new SQLiteField(null));
        Assert.AreEqual("The column's name must be filled. (Parameter 'name')", exception.Message);
    }

    #endregion

    #region Properties

    [Test]
    public void SQLiteField_Setters_ShouldSetTheProperties()
    {
        const bool ignoreOnDeleteMock = true;
        const bool ignoreOnInsertMock = true;
        const bool ignoreOnSelectMock = true;
        const bool ignoreOnUpdateMock = true;
        const bool isPrimaryKeyMock = true;
        const string tableAliasMock = "tableAlias";

    // Act
    SQLiteField field = new("fieldName")
        {
            IgnoreOnDelete = ignoreOnDeleteMock,
            IgnoreOnInsert = ignoreOnInsertMock,
            IgnoreOnSelect = ignoreOnSelectMock,
            IgnoreOnUpdate = ignoreOnUpdateMock,
            IsPrimaryKey = isPrimaryKeyMock,
            TableAlias = tableAliasMock
        };

        // Assert
        Assert.AreEqual(ignoreOnDeleteMock, field.IgnoreOnDelete);
        Assert.AreEqual(ignoreOnInsertMock, field.IgnoreOnInsert);
        Assert.AreEqual(ignoreOnSelectMock, field.IgnoreOnSelect);
        Assert.AreEqual(ignoreOnUpdateMock, field.IgnoreOnUpdate);
        Assert.AreEqual(isPrimaryKeyMock, field.IsPrimaryKey);
        Assert.AreEqual(tableAliasMock, field.TableAlias);
    }

    #endregion
}