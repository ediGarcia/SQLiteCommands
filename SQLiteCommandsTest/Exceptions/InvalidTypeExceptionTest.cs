using NUnit.Framework;
using SQLiteCommands.Exceptions;

namespace SQLiteCommandsTest.Exceptions;

[TestFixture]
internal class InvalidTypeExceptionTest
{
    #region Constructor

    [Test]
    public void InvalidTypeException_Constructor_ShouldSetTheMessage()
    {
        // Arrange
        const string messageMock = "exceptionMessage";

        // Act
        InvalidTypeException exception = new(messageMock);

        // Assert
        Assert.AreEqual(messageMock, exception.Message);
    }

    #endregion
}