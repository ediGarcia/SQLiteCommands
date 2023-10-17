using NUnit.Framework;
using SQLiteCommands.Attributes.Table;
using SQLiteCommands.Enums;

#pragma warning disable CS8625

namespace SQLiteCommandsTest.Attributes.Table;

[TestFixture]
internal class JoinAttributeTest
{
    #region Constructor

    [Test]
    public void JoinAttribute_Constructor_ShouldSetTheTableAliasAndConstraintPropertiesProperties()
    {
        // Arrange
        const string tableNameMock = "TableName";
        const string aliasMock = "Alias";
        const string constraintMock = "Constraint";

        // Act
        JoinAttribute joinAttribute = new(tableNameMock, aliasMock, constraintMock);

        // Assert
        Assert.AreEqual(tableNameMock, joinAttribute.Table);
        Assert.AreEqual(aliasMock, joinAttribute.Alias);
        Assert.AreEqual(constraintMock, joinAttribute.Constraint);
        Assert.AreEqual(JoinMode.Inner, joinAttribute.Mode);
    }

    [Test]
    public void JoinAttribute_Constructor_ShouldThrowException_WhenTheTableParameterIsNull()
    {
        // Act & Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new JoinAttribute(null, "alias", "constraint"));
        Assert.AreEqual("The table name must be filled. (Parameter 'table')", exception.Message);
    }

    [Test]
    public void JoinAttribute_Constructor_ShouldNotThrowException_WhenTheAliasParameterIsNull() =>
        // Act & Assert
        Assert.DoesNotThrow(() => new JoinAttribute("table", null, "constraint"));

    [Test]
    public void JoinAttribute_Constructor_ShouldThrowException_WhenTheConstraintParameterIsNull()
    {
        // Act & Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new JoinAttribute("table", "alias", null));
        Assert.AreEqual("The JOIN clause constraint must be filled. (Parameter 'constraint')", exception.Message);
    }

    #endregion

    #region Setters

    [TestCase(JoinMode.Cross)]
    [TestCase(JoinMode.Inner)]
    [TestCase(JoinMode.Outer)]
    public void JoinAttribute_Setters_ShouldSetTheJoinMode(JoinMode mode)
    {
        // Act
        JoinAttribute joinAttribute = new("TableName", "TableAlias", "JoinConstraint")
        {
            Mode = mode
        };

        // Assert
        Assert.AreEqual(mode, joinAttribute.Mode);
    }

    #endregion
}