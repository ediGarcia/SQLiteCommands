using NUnit.Framework;
using SQLiteCommands.Exceptions;

namespace SQLiteCommandsTest.Exceptions;

[TestFixture]
internal class ColumnSpecificationNeededExceptionTest
{
    #region Constructor

    [Test]
    public void ColumnSpecificationNeededException_Constructor_ShouldSetTheMessage()
    {
        // Arrange
        const string messageMock = "exceptionMessage";

        // Act
        ColumnSpecificationNeededException exception = new(messageMock);

        // Assert
        Assert.AreEqual(messageMock, exception.Message);
    }

    #endregion
}