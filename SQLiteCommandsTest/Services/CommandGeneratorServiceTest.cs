using System.Data.SQLite;
using NUnit.Framework;
using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Exceptions;
using SQLiteCommands.Services;
using SQLiteCommandsTest.Mock.Classes;

// ReSharper disable StringLiteralTypo

namespace SQLiteCommandsTest.Services;

[TestFixture]
internal class CommandGeneratorServiceTest
{
    #region GenerateDeleteCommand

    [Test]
    public void
        CommandGeneratorService_GenerateDeleteCommand_ShouldThrowException_WhenTheClassPropertiesHaveNoValidAttributes() =>
        // Act & Assert
        Assert.Throws<InvalidTypeException>(
            () => CommandGeneratorService.GenerateDeleteCommand(new TableWithoutColumnsClass()),
            $"No {nameof(ColumnAttribute)}, {nameof(CustomColumnAttribute)} or {nameof(ForeignKeyColumnAttribute)} found among current data properties.");

    [Test]
    public void
        CommandGeneratorService_GenerateDeleteCommand_ShouldThrowException_WhenTheClassPropertiesHaveNoValue() =>
        // Act & Assert
        Assert.Throws<InvalidTypeException>(
            () => CommandGeneratorService.GenerateDeleteCommand(new TableClass()),
            $"No {nameof(ColumnAttribute)}, {nameof(CustomColumnAttribute)} or {nameof(ForeignKeyColumnAttribute)} found among current data properties.");

    [Test]
    public void
        CommandGeneratorService_GenerateDeleteCommand_ShouldGenerateADeleteCommand_WhenTheClassPropertiesHaveValues()
    {
        // Arrange
        TableColumnForeignKey table = new()
        {
            Id = 5000,
            IdIgnore = 6000,
            NoPrimaryId = 7000,
            ForeignKey = new()
            {
                Id = 8000
        },
            ForeignKeyIgnore = new()
            {
                Id = 9000
            },
NonPrimaryForeignKey = new()
            {
Id = 1000
            }
        };

        // Act
        SQLiteCommand result = CommandGeneratorService.GenerateDeleteCommand(table);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Parameters.Count);
        Assert.AreEqual("@CFK_INT_ID", result.Parameters[0].ParameterName);
        Assert.AreEqual(table.Id, result.Parameters[0].Value);
        Assert.AreEqual("@CFK_FOR_INT_ID", result.Parameters[1].ParameterName);
        Assert.AreEqual(table.ForeignKey.Id, result.Parameters[1].Value);
        Assert.AreEqual("DELETE FROM TAB_CFK_COLUMNFOREIGNKEY WHERE 1 = 1 AND CFK_INT_ID = @CFK_INT_ID AND CFK_FOR_INT_ID = @CFK_FOR_INT_ID", result.CommandText);
    }

    #endregion

    #region GenerateSelectCommand

    [Test]
    public void
        CommandGeneratorService_GenerateSelectCommand_ShouldThrowException_WhenTheClassHasNoPropertyWithAttributes() =>
        Assert.Throws<InvalidTypeException>(
            () => CommandGeneratorService.GenerateSelectCommand(new TableWithoutColumnsClass()),
            $"No {nameof(ColumnAttribute)}, {nameof(CustomColumnAttribute)} or {nameof(ForeignKeyColumnAttribute)} found among current data properties.");

    [Test]
    public void
        CommandGeneratorService_GenerateSelectCommand_ShouldGenerateASimpleSelectCommand_WhenClassHasNoSelectAttributeAndNoValues()
    {
        // Act
        SQLiteCommand result = CommandGeneratorService.GenerateSelectCommand(new TableClass());

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Parameters);
        Assert.AreEqual("SELECT TST_INT_ID AS Id, TST_REA_DOUBLEVALUE AS DoubleValue, TST_TXT_STRINGVALUE AS StringValue, (TST_INT_ID + TST_REA_DOUBLEVALUE) AS CustomValue FROM TAB_TST_TEST WHERE 1 = 1",
            result.CommandText);
    }

    [Test]
    public void
        CommandGeneratorService_GenerateSelectCommand_ShouldGenerateASelectCommand_WhenClassHasValues()
    {
        // Arrange
        const int idMock = 3;
        const double doubleValueMock = 3.14;
        const string stringValueMock = "stringValue";
        const string idParameter = "@Id";
        const string doubleValueParameter = "@DoubleValue";
        const string stringValueParameter = "@StringValue";
        TableClass tableClass = new()
        {
            Id = idMock,
            DoubleValue = doubleValueMock,
            StringValue = stringValueMock
        };

        // Act
        SQLiteCommand result = CommandGeneratorService.GenerateSelectCommand(tableClass);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Parameters.Count);
        Assert.AreEqual(idParameter, result.Parameters[0].ParameterName);
        Assert.AreEqual(idMock, result.Parameters[0].Value);
        Assert.AreEqual(doubleValueParameter, result.Parameters[1].ParameterName);
        Assert.AreEqual(doubleValueMock, result.Parameters[1].Value);
        Assert.AreEqual(stringValueParameter, result.Parameters[2].ParameterName);
        Assert.AreEqual(stringValueMock, result.Parameters[2].Value);
        Assert.AreEqual(
            $"SELECT TST_INT_ID AS Id, TST_REA_DOUBLEVALUE AS DoubleValue, TST_TXT_STRINGVALUE AS StringValue, (TST_INT_ID + TST_REA_DOUBLEVALUE) AS CustomValue FROM TAB_TST_TEST WHERE 1 = 1 AND Id = {idParameter} AND DoubleValue = {doubleValueParameter} AND StringValue = {stringValueParameter}",
            result.CommandText);
    }

    [TestCaseSource(nameof(SelectClassTestCases))]
    public void
        CommandGeneratorService_GenerateSelectCommand_ShouldGenerateASelectCommand_WhenClassContainsTheSelectAttributeAndNoValues<
            T>(T selectClass, string expectedCommandText)
    {
        // Act
        SQLiteCommand result = CommandGeneratorService.GenerateSelectCommand(selectClass);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Parameters);
        Assert.AreEqual(expectedCommandText, result.CommandText);
    }

    [Test]
    public void
        CommandGeneratorService_GenerateSelectCommand_ShouldGenerateASelectCommandWithFilter_WhenTheSelectAttributeFilterPropertyHasValue()
    {
        // Act
        SQLiteCommand result = CommandGeneratorService.GenerateSelectCommand(new SelectFilteredClass());

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Parameters);
        Assert.AreEqual(
            "SELECT TST_INT_ID AS Id, TST_REA_DOUBLEVALUE AS DoubleValue, TST_TXT_STRINGVALUE AS StringValue FROM TAB_TST_TEST WHERE 1 = 1 AND (DoubleValue < 10)",
            result.CommandText);
    }

    [Test]
    public void
        CommandGeneratorService_GenerateSelectCommand_ShouldGenerateASimpleSelectCommand_WhenTheClassPropertiesHasNoGroupingAttributesAndSelectAttributeHavingPropertyHasValue()
    {
        // Act
        SQLiteCommand result = CommandGeneratorService.GenerateSelectCommand(new SelectHavingClass());

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Parameters);
        Assert.AreEqual("SELECT TST_INT_ID AS Id, TST_REA_DOUBLEVALUE AS DoubleValue, TST_TXT_STRINGVALUE AS StringValue FROM TAB_TST_TEST WHERE 1 = 1",
            result.CommandText);
    }

    [Test]
    public void
        CommandGeneratorService_GenerateSelectCommand_ShouldGenerateASelectCommandWithLimit_WhenTheSelectAttributeLimitPropertyHasValue()
    {
        // Act
        SQLiteCommand result = CommandGeneratorService.GenerateSelectCommand(new SelectLimitClass());

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Parameters);
        Assert.AreEqual("SELECT TST_INT_ID AS Id, TST_REA_DOUBLEVALUE AS DoubleValue, TST_TXT_STRINGVALUE AS StringValue FROM TAB_TST_TEST WHERE 1 = 1 LIMIT 7",
            result.CommandText);
    }


    [Test]
    public void
        CommandGeneratorService_GenerateSelectCommand_ShouldGenerateASelectCommandWithLimitAndOffset_WhenTheSelectAttributeLimitAndOffsetPropertiesHaveValue()
    {
        // Act
        SQLiteCommand result = CommandGeneratorService.GenerateSelectCommand(new SelectLimitOffsetClass());

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Parameters);
        Assert.AreEqual("SELECT TST_INT_ID AS Id, TST_REA_DOUBLEVALUE AS DoubleValue, TST_TXT_STRINGVALUE AS StringValue FROM TAB_TST_TEST WHERE 1 = 1 LIMIT 6 OFFSET 9",
            result.CommandText);
    }

    [Test]
    public void
        CommandGeneratorService_GenerateSelectCommand_ShouldGenerateASimpleSelectCommand_WhenTheSelectAttributeLimitPropertyHasNoValueAndTheOffsetPropertyHasValue()
    {
        // Act
        SQLiteCommand result = CommandGeneratorService.GenerateSelectCommand(new SelectOffsetClass());

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Parameters);
        Assert.AreEqual("SELECT TST_INT_ID AS Id, TST_REA_DOUBLEVALUE AS DoubleValue, TST_TXT_STRINGVALUE AS StringValue FROM TAB_TST_TEST WHERE 1 = 1",
            result.CommandText);
    }

    [Test]
    public void
        CommandGeneratorService_GenerateSelectCommand_ShouldCreateASelectCommandWithTheForeignKeyValue_WhenTheClassContainsForeignKeys()
    {
        // Arrange
        const int primaryKeyColumnValueMock = 1;
        const int regularColumnValueMock = 2;
        const string primaryKeyForeignKeyParameter = "@ForeignKey";
        const string nonPrimaryKeyForeignKeyParameter = "@NoPrimaryForeignKey";
        TableForeignKey foreignTableClass = new()
        {
            ForeignKey = new TableForeignKeyTargetClass
            {
                Id = primaryKeyColumnValueMock
            },
            NoPrimaryForeignKey = new TableForeignKeyTargetClass
            {
                NonPrimaryColumn = regularColumnValueMock
            },
            NoTable = new NoTableClass()
        };

        // Act
        SQLiteCommand result = CommandGeneratorService.GenerateSelectCommand(foreignTableClass);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Parameters.Count);
        Assert.AreEqual(primaryKeyForeignKeyParameter, result.Parameters[0].ParameterName);
        Assert.AreEqual(primaryKeyColumnValueMock, result.Parameters[0].Value);
        Assert.AreEqual(nonPrimaryKeyForeignKeyParameter, result.Parameters[1].ParameterName);
        Assert.AreEqual(regularColumnValueMock, result.Parameters[1].Value);
        Assert.AreEqual($"SELECT TST_FOR_INT_ID AS ForeignKey, TST_FOR_INT_NOPRIMARY AS NoPrimaryForeignKey, TST_FOR_INT_NOTABLE AS NoTable FROM TAB_TST_TEST WHERE 1 = 1 AND ForeignKey = {primaryKeyForeignKeyParameter} AND NoPrimaryForeignKey = {nonPrimaryKeyForeignKeyParameter}", result.CommandText);
    }

    [Test]
    public void
        CommandGeneratorService_GenerateSelectCommand_ShouldCreateASelectCommandWithTheOrderByClause_WhenTheClassContainsFieldsWithNoValueAndTheSortingFieldAttribute()
    {
        // Act
        SQLiteCommand result = CommandGeneratorService.GenerateSelectCommand(new TableSortedFieldsClass());

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(
            "SELECT TST_INT_ID AS Id, TST_INT_COLUMN AS IntColumn FROM TAB_TST_TEST WHERE 1 = 1 ORDER BY Id ASC NULLS FIRST, IntColumn DESC NULLS LAST",
            result.CommandText);
    }

    [Test]
    public void
        CommandGeneratorService_GenerateSelectCommand_ShouldCreateASelectCommandWithTheOrderByClause_WhenTheClassContainsFieldsWithValueAndTheSortingFieldAttribute()
    {
        // Arrange
        const int idValueMock = 1;
        const string idParameter = "@Id";
        TableSortedFieldsClass sortedFieldsClass = new()
        {
            Id = idValueMock,
            IntColumn = 2
        };

        // Act
        SQLiteCommand result = CommandGeneratorService.GenerateSelectCommand(sortedFieldsClass);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Parameters.Count);
        Assert.AreEqual(idParameter, result.Parameters[0].ParameterName);
        Assert.AreEqual(idValueMock, result.Parameters[0].Value);
        Assert.AreEqual(
            "SELECT TST_INT_ID AS Id, TST_INT_COLUMN AS IntColumn FROM TAB_TST_TEST WHERE 1 = 1 AND Id = @Id ORDER BY Id ASC NULLS FIRST, IntColumn DESC NULLS LAST",
            result.CommandText);
    }

    [Test]
    public void
        CommandGeneratorService_GenerateSelectCommand_ShouldCreateASelectCommandWithTheGroupByStatement_WhenTheClassContainsPropertiesWithTheGroupingAttribute()
    {
        // Act
        SQLiteCommand result = CommandGeneratorService.GenerateSelectCommand(new TableGroupingClass());

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(
            "SELECT TST_INT_ID AS Id, TST_INT_INTCOLUMN AS IntColumn, TST_CHR_TEXTCOLUMN AS TextColumn FROM TAB_TST_TEST WHERE 1 = 1 GROUP BY IntColumn, TextColumn",
            result.CommandText);
    }

    [Test]
    public void
        CommandGeneratorService_GenerateSelectCommand_ShouldCreateASelectCommandWithTheGroupByStatement_WhenTheClassContainsHavingDataAndPropertiesWithTheGroupingAttribute()
    {
        // Act
        SQLiteCommand result = CommandGeneratorService.GenerateSelectCommand(new SelectGroupingHavingClass());

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(
            "SELECT TST_INT_ID AS Id, TST_INT_INTCOLUMN AS IntColumn, TST_CHR_TEXTCOLUMN AS TextColumn FROM TAB_TST_TEST WHERE 1 = 1 GROUP BY IntColumn, TextColumn HAVING TST_INT_INTCOLUMN > 5",
            result.CommandText);
    }

    [Test]
    public void
        CommandGeneratorService_GenerateSelectCommand_ShouldGenerateJoinClauses_WhenTheClassHasTheJoinAttribute()
    {
        // Act
        SQLiteCommand result = CommandGeneratorService.GenerateSelectCommand(new TableJoinClass());

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(
            "SELECT TST_INT_ID AS Id FROM TAB_TST_TEST CROSS JOIN TAB_CRS_CROSS AS CRS ON CRS_INT_ID = TST_CRS_INT_ID INNER JOIN TAB_INN_INNER AS INN ON CRS_INT_ID = TST_INN_INT_ID LEFT OUTER JOIN TAB_OUT_OUTER AS OUT ON CRS_INT_ID = TST_OUT_INT_ID WHERE 1 = 1",
            result.CommandText);
    }

    [Test]
    public void
        CommandGeneratorService_GenerateSelectCommand_ShouldUseTheCorrectAliasForEachColumn_WhenTheTableAliasAreDefined()
    {
        // Act
        SQLiteCommand result = CommandGeneratorService.GenerateSelectCommand(new TableAliasClass());

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(
            "SELECT TST.TST_INT_ID AS Id, ALI.TST_INT_COLUMNALIAS AS ColumnWithAlias, TST.TST_FOR_INT_ID AS ForeignKey, FOR.TST_FOR_INT_ID2 AS ForeignKey2, TST.TST_INT_SORTING AS SortingColumn, SOR.TST_INT_SORTING2 AS SortingColumn2, TST.TST_INT_GROUPCOLUMN AS GroupingColumn, GRP.TST_INT_GROUPCOLUMN2 AS GroupingColumn2 FROM TAB_TST_TEST AS TST WHERE 1 = 1 GROUP BY GroupingColumn, GroupingColumn2 ORDER BY SortingColumn ASC NULLS FIRST, SortingColumn2 ASC NULLS FIRST",
            result.CommandText);
    }

    #endregion

    #region Test Cases Sources

    #region SelectClassTestCases
    /// <summary>
    /// Generates test cases to test the "RemoveDuplicates" property of the Select attribute.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<TestCaseData> SelectClassTestCases()
    {
        yield return new TestCaseData(new SelectClass(),
            "SELECT TST_INT_ID AS Id, TST_REA_DOUBLEVALUE AS DoubleValue, TST_TXT_STRINGVALUE AS StringValue FROM TAB_TST_TEST WHERE 1 = 1");
        yield return new TestCaseData(new SelectDistinctClass(),
            "SELECT DISTINCT TST_INT_ID AS Id, TST_REA_DOUBLEVALUE AS DoubleValue, TST_TXT_STRINGVALUE AS StringValue FROM TAB_TST_TEST WHERE 1 = 1");
    }
    #endregion

    #endregion
}