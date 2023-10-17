using System.Data.SQLite;
using NUnit.Framework;
using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Exceptions;
using SQLiteCommands.Services;
using SQLiteCommandsTest.Mock.Classes;
#pragma warning disable CS8619

// ReSharper disable StringLiteralTypo

namespace SQLiteCommandsTest.Services;

[TestFixture]
internal class CommandGeneratorServiceTest
{
    #region Delete

    [TestCaseSource(nameof(ConnectionTestData))]
    public void CommandGeneratorServiceTest_GenerateDeleteCommand_ShouldGenerateTheDeleteSQLiteCommand(SQLiteConnection connection)
    {
        // Arrange
        const int primaryColumnValueMock = 1;
        const int foreignColumnValueMock = 5;
        ForeignDeleteClassMock foreignValueMock = new() { ForeignPrimaryKey = foreignColumnValueMock };
        DeleteClassMock dataMock = new()
        {
            PrimaryProperty = primaryColumnValueMock,
            PrimaryPropertyIgnore = 2,
            Property = 3,
            IgnoreProperty = 4,
            ForeignPrimaryProperty = foreignValueMock,
            ForeignPrimaryPropertyIgnore = new() { ForeignPrimaryKey = 6 },
            ForeignProperty = new() { ForeignPrimaryKey = 7 },
            ForeignIgnoreProperty = new() { ForeignPrimaryKey = 8 },
            ForeignInnerNullProperty = new(),
            NoAttributeProperty = 10
        };

        // Act
        SQLiteCommand command = CommandGeneratorService.GenerateDeleteCommand(dataMock, connection);

        // Assert
        Assert.IsNotNull(command);
        Assert.AreEqual(connection, command.Connection);
        Assert.AreEqual("DELETE FROM TAB_DEL_DELETE-TEST WHERE 1 = 1 AND DEL_INT_PRIMARYFIELD = @DEL_INT_PRIMARYFIELD AND DEL_INT_NULLFIELD = @DEL_INT_NULLFIELD AND FOR_DEL_INT_PRIMARYFIELD = @FOR_DEL_INT_PRIMARYFIELD AND FOR_DEL_INT_NULLFIELD = @FOR_DEL_INT_NULLFIELD AND FOR_DEL_INT_INNERNULLFIELD = @FOR_DEL_INT_INNERNULLFIELD", command.CommandText);
        Assert.AreEqual(5, command.Parameters.Count);
        Assert.AreEqual(primaryColumnValueMock, command.Parameters["@DEL_INT_PRIMARYFIELD"].Value);
        Assert.AreEqual(DBNull.Value, command.Parameters["@DEL_INT_NULLFIELD"].Value);
        Assert.AreEqual(foreignColumnValueMock, command.Parameters["@FOR_DEL_INT_PRIMARYFIELD"].Value);
        Assert.AreEqual(DBNull.Value, command.Parameters["@FOR_DEL_INT_NULLFIELD"].Value);
        Assert.AreEqual(DBNull.Value, command.Parameters["@FOR_DEL_INT_INNERNULLFIELD"].Value);
    }

    [Test]
    public void CommandGeneratorServiceTest_GenerateDeleteCommand_ShouldThrowError_WhenTheClassHasNoValidColumn() =>
        // Act & Assert
        Assert.Throws<InvalidTypeException>(
            () => CommandGeneratorService.GenerateDeleteCommand(new EmptyDeleteClassMock()),
            $"No eligible primary key {nameof(ColumnAttribute)} or {nameof(ForeignKeyColumnAttribute)} found among current data properties.");

    #endregion

    #region Insert

    [TestCaseSource(nameof(ConnectionTestData))]
    public void CommandGeneratorServiceTest_GenerateInsertCommand_ShouldGenerateTheDeleteSQLiteCommand(SQLiteConnection connection)
    {
        // Arrange
        const int idMock = 1;
        InsertClass dataMock = new()
        {
            Id = idMock,
            IdIgnore = 2,

        };

        // Act
        SQLiteCommand command = CommandGeneratorService.GenerateInsertCommand(dataMock, connection);

        // Assert
        Assert.IsNotNull(command);
    }

    #endregion

    private static SQLiteConnection[] ConnectionTestData =>
        new[] { new SQLiteConnection(), null };
}