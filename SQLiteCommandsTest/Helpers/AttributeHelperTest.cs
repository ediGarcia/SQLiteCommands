using NUnit.Framework;
using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;
using SQLiteCommands.Exceptions;
using SQLiteCommands.Helpers;
using SQLiteCommandsTest.Mock.Classes;
using System.Reflection;

namespace SQLiteCommandsTest.Helpers;

internal class AttributeHelperTest
{
    #region GetPropertyAttribute

    [Test]
    public void
        AttributeHelper_GetPropertyAttribute_ShouldReturnThePropertyAttribute_WhenThePropertyContainsAnAttributeOfTheSpecifiedType()
    {
        // Act
        ColumnAttribute column =
            AttributeHelper.GetPropertyAttribute<ColumnAttribute>(
                typeof(ValidAttributeCombinationClass).GetProperties()[0]);

        // Assert
        Assert.IsNotNull(column);
    }

    [Test]
    public void
        AttributeHelper_GetPropertyAttribute_ShouldReturnNull_WhenThePropertyDoesNotContainAnAttributeOfTheSpecifiedType()
    {
        // Act
        ForeignKeyColumnAttribute foreignKey =
            AttributeHelper.GetPropertyAttribute<ForeignKeyColumnAttribute>(
                typeof(ValidAttributeCombinationClass).GetProperties()[0]);

        // Assert
        Assert.IsNull(foreignKey);
    }

    #endregion

    #region GetTypeAttribute

    [Test]
    public void AttributeHelper_GetTypeAttribute_ShouldReturnTheAttribute_WhenTheClassContainsTheSpecifiedAttributeType()
    {
        // Act
        TableAttribute table = AttributeHelper.GetTypeAttribute<TableAttribute>(typeof(NoPropertyTableAttributeClass));

        // Assert
        Assert.IsNotNull(table);
        Assert.AreEqual("TAB_TST_TEST", table.Name);
    }

    [Test]
    public void AttributeHelper_GetTypeAttribute_ShouldReturnNull_WhenTheClassDoesNotContainTheSpecifiedAttributeType()
    {
        // Act
        TableAttribute table = AttributeHelper.GetTypeAttribute<TableAttribute>(typeof(NoPropertyNoTableAttributeClass));

        // Assert
        Assert.IsNull(table);
    }

    #endregion

    #region ValidatePropertyAttributes

    [TestCaseSource(nameof(GenerateInvalidAttributeCombinationPropertyInfos))]
    public void
        AttributeHelper_ValidatePropertyAttributes_ShouldThrowException_WhenThePropertyAttributeCombinationIsInvalid(
            PropertyInfo propInfo)
    {
        // Act & Assert
        InvalidAttributeCombinationException exception =
            Assert.Throws<InvalidAttributeCombinationException>(() =>
                AttributeHelper.ValidatePropertyAttributes(propInfo));
        Assert.AreEqual("Invalid property attribute combination.", exception.Message);
    }

    [TestCaseSource(nameof(GenerateValidAttributeCombinationPropertyInfos))]
    public void
        AttributeHelper_ValidatePropertyAttributes_ShouldNotThrowException_WhenThePropertyAttributeCombinationIsValid(
            PropertyInfo propInfo) =>
        // Act & Assert
        Assert.DoesNotThrow(() => AttributeHelper.ValidatePropertyAttributes(propInfo));

    #endregion

    #region ValidatePropertyValue

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void AttributeHelper_ValidatePropertyValue_ShouldThrowException_WhenTheValueIsEmpty(string value)
    {
        const string fieldName = "fieldName";
        const string propertyName = "parameterName";

        // Act & Assert
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() =>
                AttributeHelper.ValidatePropertyValue(value, propertyName, fieldName));
        Assert.AreEqual($"The {fieldName} must be filled. (Parameter '{propertyName}')", exception.Message);
    }

    [Test]
    public void AttributeHelper_ValidatePropertyValue_ShouldNotThrowException_WhenTheValueIsFilled() =>
        // Act & Assert
        Assert.DoesNotThrow(() => AttributeHelper.ValidatePropertyValue("value", "propertyName", "fieldName"));

    #endregion

    #region Test cases sources

    private static IEnumerable<PropertyInfo> GenerateInvalidAttributeCombinationPropertyInfos()
    {
        PropertyInfo[] propInfos = typeof(InvalidAttributeCombinationClass).GetProperties();

        yield return propInfos[0];
        yield return propInfos[1];
        yield return propInfos[2];
        yield return propInfos[3];
        yield return propInfos[4];
        yield return propInfos[5];
        yield return propInfos[6];
        yield return propInfos[7];
        yield return propInfos[8];
        yield return propInfos[9];
    }

    private static IEnumerable<PropertyInfo> GenerateValidAttributeCombinationPropertyInfos()
    {
        PropertyInfo[] propInfos = typeof(ValidAttributeCombinationClass).GetProperties();

        yield return propInfos[0];
        yield return propInfos[1];
        yield return propInfos[2];
        yield return propInfos[3];
    }

    #endregion
}