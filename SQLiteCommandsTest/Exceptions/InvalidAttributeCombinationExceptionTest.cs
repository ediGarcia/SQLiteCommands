using NUnit.Framework;
using SQLiteCommands.Exceptions;

namespace SQLiteCommandsTest.Exceptions;

[TestFixture]
internal class InvalidAttributeCombinationExceptionTest
{
    #region Constructor

    [Test]
    public void InvalidAttributeCombinationException_Constructor_ShouldSetTheMessage()
    {
        // Arrange
        const string messageMock = "exceptionMessage";

        // Act
        InvalidAttributeCombinationException exception = new(messageMock);

        // Assert
        Assert.AreEqual(messageMock, exception.Message);
    }

    #endregion
}