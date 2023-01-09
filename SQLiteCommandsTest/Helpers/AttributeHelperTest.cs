using SQLiteCommands.Exceptions;
using SQLiteCommands.Helpers;
using SQLiteCommandsTest.Mock.Classes;
using System.Reflection;
using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;
using SQLiteCommandsTest.Mock;

namespace SQLiteCommandsTest.Helpers;

[TestFixture]
internal class AttributeHelperTest
{
    #region CheckNullProperty

    [Test]
    public void SqliteCommandGenerator_CheckNullProperty_ShouldNotThrowException_WhenThePropertyValueIsNotNull() =>
        // Act & Assert
        Assert.DoesNotThrow(() => AttributeHelper.CheckNullProperty("propValue", "propName", "fieldName"));

    [Test]
    public void SqliteCommandGenerator_CheckNullProperty_ShouldThrowException_WhenThePropertyValueIsNull()
    {
        // Arrange
        const string propName = "propName";
        const string fieldName = "fieldName";

        // Act & Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => AttributeHelper.CheckNullProperty(null, propName, fieldName));
        Assert.AreEqual($"The {fieldName} be must be filled. (Parameter '{propName}')", exception.Message);
    }

    #endregion

    #region CheckPropertyAttributes

    [TestCaseSource(nameof(InvalidAttributeCombinationTestCases))]
    public void
        SqliteCommandGenerator_CheckPropertyAttributes_ShouldThrowException_WhenThePropertyAttributeCombinationIsInvalid(
            PropertyInfo propInfo)
    {
        // Act & Assert
        InvalidAttributeCombinationException exception =
            Assert.Throws<InvalidAttributeCombinationException>(() =>
                AttributeHelper.CheckPropertyAttributes(propInfo));

        Assert.AreEqual("Invalid property attribute combination.", exception.Message);
    }

    [TestCaseSource(nameof(ValidAttributeCombinationTestCases))]
    public void
        SqliteCommandGenerator_CheckPropertyAttributes_ShouldNotThrowException_WhenThePropertyAttributeCombinationIsValid(
            PropertyInfo propInfo) =>
        // Act & Assert
            Assert.DoesNotThrow(() => AttributeHelper.CheckPropertyAttributes(propInfo));

    #endregion

    #region GetPropertyAttribute

    [Test]
    public void SqliteCommandGenerator_GetPropertyAttribute_ShouldReturnTheAttribute_WhenThePropertyContainsTheSpecifiedAttribute()
    {
        // Act
        ColumnAttribute result =
            AttributeHelper.GetPropertyAttribute<ColumnAttribute>(new PropertyAttributesClass().GetType()
                .GetProperties()[0]);

        // Assert
        Assert.IsInstanceOf<ColumnAttribute>(result);
    }

    [Test]
    public void SqliteCommandGenerator_GetPropertyAttribute_ShouldReturnNull_WhenThePropertyDoesNotContainTheSpecifiedAttribute()
    {
        // Act
        CustomColumnAttribute result =
            AttributeHelper.GetPropertyAttribute<CustomColumnAttribute>(new PropertyAttributesClass().GetType()
                .GetProperties()[0]);

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region GetTableAttribute

    [Test]
    public void
        SqliteCommandGenerator_GetTableAttribute_ShouldThrowException_WhenTheSpecifiedClassDoesNotContainTheTableAttribute()
    {
        InvalidTypeException exception = Assert.Throws<InvalidTypeException>(() => AttributeHelper.GetTableAttribute(typeof(object)));
        Assert.AreEqual($"The {nameof(TableAttribute)} attribute is mandatory for SQLite commands.", exception.Message);
    }

    [Test]
    public void
        SqliteCommandGenerator_GetTableAttribute_ShouldNotThrowException_WhenTheSpecifiedClassContainsTheTableAttribute() =>
        Assert.DoesNotThrow(() => AttributeHelper.GetTableAttribute(typeof(TableClass)));

    #endregion

    #region GetTypeAttribute

    [Test]
    public void SqliteCommandGenerator_GetTypeAttribute_ShouldReturnTheAttribute_WhenTheClassContainsTheSpecifiedAttribute()
    {
        // Act
        TableAttribute result = AttributeHelper.GetTypeAttribute<TableAttribute>(typeof(TableClass));

        // Assert
        Assert.IsInstanceOf<TableAttribute>(result);
    }

    [Test]
    public void SqliteCommandGenerator_GetTypeAttribute_ShouldReturnNull_WhenTheClassDoesNotContainTheSpecifiedAttribute()
    {
        // Act
        TableAttribute result =
            AttributeHelper.GetTypeAttribute<TableAttribute>(typeof(object));

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region Test cases sources

    #region InvalidAttributeCombinationTestCases
    /// <summary>
    /// Generates invalid property attributes combination test cases.
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<TestCaseData> InvalidAttributeCombinationTestCases()
    {
        yield return new TestCaseData(new ColumnAndCustomColumnCombinationClass().GetType().GetProperties()[0]);
        yield return new TestCaseData(new ColumnAndForeignKeyColumnCombinationClass().GetType().GetProperties()[0]);
        yield return new TestCaseData(new ColumnAndOneToManyDataCombinationClass().GetType().GetProperties()[0]);
        yield return new TestCaseData(new ColumnAndManyToManyDataCombinationClass().GetType().GetProperties()[0]);
        yield return new TestCaseData(new CustomAndForeignKeyColumnCombinationClass().GetType().GetProperties()[0]);
        yield return new TestCaseData(new CustomColumnAndOneToManyDataCombinationClass().GetType().GetProperties()[0]);
        yield return new TestCaseData(new CustomColumnAndManyToManyDataCombinationClass().GetType().GetProperties()[0]);
        yield return new TestCaseData(new ForeignKeyColumnAndOneToManyDataCombinationClass().GetType().GetProperties()[0]);
        yield return new TestCaseData(new ForeignKeyColumnAndManyToManyDataCombinationClass().GetType().GetProperties()[0]);
        yield return new TestCaseData(new OneToManyAndManyToManyDataCombinationClass().GetType().GetProperties()[0]);
    }
    #endregion

    #region ValidAttributeCombinationTestCases
    /// <summary>
    /// Generates valid property attributes combination test cases.
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<TestCaseData> ValidAttributeCombinationTestCases()
    {
        yield return new TestCaseData(new ColumnAndGroupingTargetCombinationClass().GetType().GetProperties()[0]);
        yield return new TestCaseData(new ColumnAndSortingFieldCombinationClass().GetType().GetProperties()[0]);
        yield return new TestCaseData(new CustomColumnAndGroupingTargetCombinationClass().GetType().GetProperties()[0]);
        yield return new TestCaseData(new CustomColumnAndSortingFieldCombinationClass().GetType().GetProperties()[0]);
        yield return new TestCaseData(new ForeignKeyColumnAndSortingFieldCombinationClass().GetType().GetProperties()[0]);
        yield return new TestCaseData(new OneToManyAndGroupingTargetCombinationClass().GetType().GetProperties()[0]);
        yield return new TestCaseData(new OneToManyAndSortingFieldCombinationClass().GetType().GetProperties()[0]);
        yield return new TestCaseData(new GroupingTargetAndSortingFieldCombinationClass().GetType().GetProperties()[0]);
    }
    #endregion

    #endregion
}