using NUnit.Framework;
using SQLiteCommands.Attributes.Table;

namespace SQLiteCommandsTest.Attributes.Table;

[TestFixture]
internal class JoinAttributeTest
{
    #region Constructor

    [Test]
    public void JoinAttribute_Constructor_ShouldSetTheTableAliasAndConstraintPropertiesProperties()
    {
        const string tableNameMock = "TableName";
        const string aliasMock = "Alias";
        const string constraintMock = "Constraint";
        JoinAttribute joinAttribute = null;

        Assert.DoesNotThrow(() => joinAttribute = new(tableNameMock, aliasMock, constraintMock));
        Assert.AreEqual(tableNameMock, joinAttribute.Table);
        Assert.AreEqual(aliasMock, joinAttribute.Alias);
        Assert.AreEqual(constraintMock, joinAttribute.Constraint);
    }

    [Test]
    public void JoinAttribute_Constructor_ShouldThrowException_WhenTheTableParameterIsNull()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new JoinAttribute(null, "alias", "constraint"));
        Assert.AreEqual("The table name must be filled. (Parameter 'table')", exception.Message);
    }

    [Test]
    public void JoinAttribute_Constructor_ShouldNotThrowException_WhenTheAliasParameterIsNull() =>
        Assert.DoesNotThrow(() => new JoinAttribute("table", null, "constraint"));

    [Test]
    public void JoinAttribute_Constructor_ShouldThrowException_WhenTheConstraintParameterIsNull()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new JoinAttribute("table", "alias", null));
        Assert.AreEqual("The JOIN clause constraint must be filled. (Parameter 'constraint')", exception.Message);
    }

    #endregion
}