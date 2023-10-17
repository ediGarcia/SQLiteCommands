using NUnit.Framework;
using SQLiteCommands.Attributes.Table;

namespace SQLiteCommandsTest.Attributes.Table;

[TestFixture]
internal class SelectOptionsAttributeTest
{
    #region Constructor

    [Test]
    public void SelectOptionsAttribute_Constructor_ShouldSetThePropertiesWithTheirDefaultValues()
    {
        // Act
        SelectOptionsAttribute selectOptions = new();

        // Assert
        Assert.IsNull(selectOptions.Filter);
        Assert.IsNull(selectOptions.GroupBy);
        Assert.AreEqual(-1, selectOptions.Limit);
        Assert.AreEqual(-1, selectOptions.Offset);
        Assert.IsNull(selectOptions.OrderBy);
        Assert.IsTrue(selectOptions.PrimaryKeyFilterOnly);
        Assert.IsFalse(selectOptions.RemoveDuplicates);
    }

    #endregion

    #region Properties

    [TestCase(false, true)]
    [TestCase(true, false)]
    public void SelectOptionsAttribute_Setters_ShouldSetTheProperties(bool primaryKeyFilterOnly, bool removeDuplicates)
    {
        // Arrange
        const string filterMock = "filter";
        const string groupByMock = "group by";
        const int limitMock = 5;
        const int offsetMock = 3;
        const string orderByMock = "order by";

        // Act
        SelectOptionsAttribute selectOptions = new()
        {
            Filter = filterMock,
            GroupBy = groupByMock,
            Limit = limitMock,
            Offset = offsetMock,
            OrderBy = orderByMock,
            PrimaryKeyFilterOnly = primaryKeyFilterOnly,
            RemoveDuplicates = removeDuplicates
        };

        // Assert
        Assert.AreEqual(filterMock, selectOptions.Filter);
        Assert.AreEqual(groupByMock, selectOptions.GroupBy);
        Assert.AreEqual(limitMock, selectOptions.Limit);
        Assert.AreEqual(offsetMock, selectOptions.Offset);
        Assert.AreEqual(orderByMock, selectOptions.OrderBy);
        Assert.AreEqual(primaryKeyFilterOnly, selectOptions.PrimaryKeyFilterOnly);
        Assert.AreEqual(removeDuplicates, selectOptions.RemoveDuplicates);
    }

    #endregion
}