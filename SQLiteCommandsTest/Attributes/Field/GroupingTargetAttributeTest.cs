using NUnit.Framework;
using SQLiteCommands.Attributes.Field;

namespace SQLiteCommandsTest.Attributes.Field;

[TestFixture]
internal class GroupingTargetAttributeTest
{
    #region Properties

    [Test]
    public void GroupingTargetAttribute_Setters_ShouldSetTheProperties()
    {
        const string columnNameMock = "columnName";
        const string tableAliasMock = "tableAlias";

        // Act
        GroupingTargetAttribute groupingTarget = new()
        {
            ColumnName = columnNameMock,
            TableAlias = tableAliasMock
        };

        // Assert
        Assert.AreSame(columnNameMock, groupingTarget.ColumnName);
        Assert.AreSame(tableAliasMock, groupingTarget.TableAlias);
    }

    #endregion
};