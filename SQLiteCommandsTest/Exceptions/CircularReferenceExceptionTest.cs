using NUnit.Framework;
using SQLiteCommands.Exceptions;

namespace SQLiteCommandsTest.Exceptions;

[TestFixture]
internal class CircularReferenceExceptionTest
{
    #region Constructor

    [Test]
    public void CircularReferenceException_Constructor_ShouldSetTheMessage()
    {
        // Arrange
        const string messageMock = "exceptionMessage";

        // Act
        CircularReferenceException exception = new(messageMock);

        // Assert
        Assert.AreEqual(messageMock, exception.Message);
    }

    #endregion
}