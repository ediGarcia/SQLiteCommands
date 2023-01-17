using NUnit.Framework;
using SQLiteCommands.Attributes.Table;

namespace SQLiteCommandsTest.Attributes.Table;

[TestFixture]
internal class SelectOptionsAttributeTest
{
    #region Constructor

    [TestCase(false)]
    [TestCase(true)]
    public void SelectOptionsAttribute_Constructor_ShouldSetTheRemoveDuplicatesProperty(bool removeDuplicates)
    {
        // Act
        SelectOptionsAttribute selectOptions = new(removeDuplicates);

        // Assert
        Assert.AreEqual(removeDuplicates, selectOptions.RemoveDuplicates);
    }

    #endregion

    #region Properties

    [Test]
    public void SelectOptionsAttribute_Setters_ShouldSetTheProperties()
    {
        // Arrange
        const string filterMock = "filter";
        const string havingMock = "having";
        const int limitMock = 5;
        const int offsetMock = 3;

        // Act
        SelectOptionsAttribute selectOptions = new(true)
        {
            Filter = filterMock,
            Having = havingMock,
            Limit = limitMock,
            Offset = offsetMock
        };

        // Assert
        Assert.AreEqual(filterMock, selectOptions.Filter);
        Assert.AreEqual(havingMock, selectOptions.Having);
        Assert.AreEqual(limitMock, selectOptions.Limit);
        Assert.AreEqual(offsetMock, selectOptions.Offset);

    }

    #endregion
}