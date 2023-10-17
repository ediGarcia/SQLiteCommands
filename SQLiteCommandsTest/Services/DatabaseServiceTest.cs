using System.Data;
using HelperMethods;
using NUnit.Framework;
using SQLiteCommands.Services;
using System.Data.SQLite;

namespace SQLiteCommandsTest.Services;

internal class DatabaseServiceTest
{
    private const string MockDatabaseDirectoryPath = "Mock";
    private const string MockDatabaseFilePath = @"Mock\Test.dat";

    #region SetUps and TearDowns

    [OneTimeSetUp]
    public void OneTimeSetUp() =>
        SystemMethods.CreateDirectory(MockDatabaseDirectoryPath);

    [OneTimeTearDown]
    public void OneTimeTearDown() =>
        SystemMethods.Delete(MockDatabaseDirectoryPath);

    [TearDown]
    public void TearDown() =>
        SystemMethods.Delete(MockDatabaseFilePath);

    #endregion

    #region GenerateConnectionString

    [Test]
    public void DatabaseService_GenerateConnectionString_ShouldGenerateAConnectionString()
    {
        // Arrange
        const string testPathMock = "testPath";

        // Act
        string result = DatabaseService.GenerateConnectionString(testPathMock);

        // Assert
        Assert.AreEqual($"Data Source={testPathMock}; Version=3;", result);
    }

    [Test]
    public void DatabaseService_GenerateConnectionString_ShouldGenerateAConnectionString_WhenTheDatabaseVersionIsSpecified()
    {
        // Arrange
        const string testPathMock = "testPath";
        const int versionNumberMock = 4;

        // Act
        string result = DatabaseService.GenerateConnectionString(testPathMock, versionNumberMock);

        // Assert
        Assert.AreEqual($"Data Source={testPathMock}; Version={versionNumberMock};", result);
    }

    #endregion

    #region OpenConnection
    [Test]
    public void DatabaseService_OpenConnection_ShouldCreateADatabaseAndConnectToIt()
    {
        Assert.IsFalse(SystemMethods.CheckFileExists(MockDatabaseFilePath));

        // Act
        using SQLiteConnection connection = DatabaseService.OpenConnection(MockDatabaseFilePath);

        // Assert
        Assert.IsNotNull(connection);
        Assert.AreEqual(SystemMethods.GetFullPath(MockDatabaseFilePath), connection.FileName);
        Assert.IsTrue(File.Exists(connection.FileName));
        Assert.AreEqual(0, SystemMethods.GetSize(MockDatabaseFilePath));
        Assert.IsTrue(connection.ServerVersion.StartsWith("3."));
        Assert.AreEqual(ConnectionState.Open, connection.State);

        connection.Close();
    }
    #endregion
}