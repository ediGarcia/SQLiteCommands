using NUnit.Framework;
using SQLiteCommands.Attributes.Table;

namespace SQLiteCommandsTest.Attributes.Table;

[TestFixture]
internal class InsertOptionsAttributeTest
{
    #region Properties

    [TestCase(false)]
    [TestCase(true)]
    public void InsertOptions_Setters_ShouldSetTheProperties(bool replaceOnConflict)
    {
        // Arrange & Act
        InsertOptionsAttribute insertOptions = new() { ReplaceOnConflict = replaceOnConflict };

        // Assert
        Assert.AreEqual(replaceOnConflict, insertOptions.ReplaceOnConflict);
    }

    #endregion
}