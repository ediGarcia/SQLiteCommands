using NUnit.Framework;
using SQLiteCommands.Attributes.Field;
#pragma warning disable CS8625

namespace SQLiteCommandsTest.Attributes.Field;

[TestFixture]
internal class CustomColumnAttributeTest
{
    #region Constructor

    [Test]
    public void CustomColumnAttribute_Constructor_ShouldSetTheCustomDataProperty()
    {
        // Arrange
        const string customDataMock = "1 + 1";

        // Act
        CustomColumnAttribute columnAttribute = new(customDataMock);

        // Assert
        Assert.AreEqual(customDataMock, columnAttribute.CustomData);
    }

    [Test]
    public void CustomColumnAttribute_Constructor_ShouldThrowException_WhenTheCustomDataParameterIsNull()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new CustomColumnAttribute(null));
        Assert.AreEqual("The custom column's data must be filled. (Parameter 'customData')", exception.Message);
    }

    #endregion
}