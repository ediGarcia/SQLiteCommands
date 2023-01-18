using NUnit.Framework;
using SQLiteCommands.Attributes.Table;

namespace SQLiteCommandsTest.Attributes.Table;

[TestFixture]
internal class SelectOptionsAttributeTest
{
    #region Properties

    [TestCase(false)]
    [TestCase(true)]
    public void SelectOptionsAttribute_Setters_ShouldSetTheProperties(bool removeDuplicates)
    {
        // Arrange
        const string filterMock = "filter";
        const string havingMock = "having";
        const int limitMock = 5;
        const int offsetMock = 3;

        // Act
        SelectOptionsAttribute selectOptions = new()
        {
            Filter = filterMock,
            Having = havingMock,
            Limit = limitMock,
            Offset = offsetMock,
            RemoveDuplicates = removeDuplicates
        };

        // Assert
        Assert.AreEqual(filterMock, selectOptions.Filter);
        Assert.AreEqual(havingMock, selectOptions.Having);
        Assert.AreEqual(limitMock, selectOptions.Limit);
        Assert.AreEqual(offsetMock, selectOptions.Offset);
        Assert.AreEqual(removeDuplicates, selectOptions.RemoveDuplicates);
    }

    #endregion
}