using NUnit.Framework;
using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Enums;

namespace SQLiteCommandsTest.Attributes.Field;

[TestFixture]
internal class SortingFieldAttributeTest
{
    #region Properties

    [TestCase(SortingDirection.Ascending, false)]
    [TestCase(SortingDirection.Descending, true)]
    public void SortingFieldAttribute_Setters_ShouldSetTheProperties(SortingDirection sortingDirection, bool placeNullsAtEnd)
    {
        const string tableAliasMock = "tableAlias";
        const string columnNameMock = "columnName";
        const int sortingIndexMock = 7;

        // Act
        SortingFieldAttribute sortingField = new()
        {
            TableAlias = tableAliasMock,
            ColumnName = columnNameMock,
            Direction = sortingDirection,
            PlaceNullsAtTheEnd = placeNullsAtEnd,
            SortingIndex = sortingIndexMock
        };

        // Assert
        Assert.AreEqual(tableAliasMock, sortingField.TableAlias);
        Assert.AreEqual(columnNameMock, sortingField.ColumnName);
        Assert.AreEqual(sortingDirection, sortingField.Direction);
        Assert.AreEqual(placeNullsAtEnd, sortingField.PlaceNullsAtTheEnd);
        Assert.AreEqual(sortingIndexMock, sortingField.SortingIndex);
    }

    [Test]
    public void SortingFieldAttribute_Setters_ShouldThrowException_WhenSortingFieldIsLessThanZero()
    {
        // Act & Assert
IndexOutOfRangeException exception = Assert.Throws<IndexOutOfRangeException>(() => new SortingFieldAttribute
        {
            SortingIndex = -1
        });
Assert.AreEqual("The sorting index must be equal or greater than 0.", exception.Message);
    }

    #endregion
}