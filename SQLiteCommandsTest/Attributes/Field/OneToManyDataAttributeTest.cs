using NUnit.Framework;
using SQLiteCommands.Attributes.Field;

namespace SQLiteCommandsTest.Attributes.Field;

internal class OneToManyDataAttributeTest
{
    #region Properties

    [Test]
    public void OneToManyDataAttribute_Setters_ShouldSetTheProperties()
    {
        const string localColumnMock = "localColumnName";
        const string targetColumnMock = "targetColumnMock";

        // Act
        OneToManyDataAttribute oneToManyData = new()
        {
            LocalColumn = localColumnMock,
            TargetColumn = targetColumnMock
        };

        // Assert
        Assert.AreEqual(localColumnMock, oneToManyData.LocalColumn);
        Assert.AreEqual(targetColumnMock, oneToManyData.TargetColumn);
    }

    #endregion
}