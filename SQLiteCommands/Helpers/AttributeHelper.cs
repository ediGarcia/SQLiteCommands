using SQLiteCommands.Attributes.Field;
using SQLiteCommands.Attributes.Table;
using SQLiteCommands.Exceptions;
using System.Reflection;

namespace SQLiteCommands.Helpers;

/// <summary>
/// Attribute related methods.
/// </summary>
internal static class AttributeHelper
{
    #region Public Methods

    #region CheckNullProperty

    /// <summary>
    /// Throws an exception if the selected property is not filled.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="propertyName"></param>
    /// <param name="fieldName"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void CheckNullProperty(string value, string propertyName, string fieldName)
    {
        if (String.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(propertyName, $"The {fieldName} must be filled.");
    }
    #endregion

    #region CheckPropertyAttributes
    /// <summary>
    /// Verifies if the current property contains any invalid attribute combination.
    /// </summary>
    /// <param name="propInfo"></param>
    /// <exception cref="InvalidAttributeCombinationException"></exception>
    public static void CheckPropertyAttributes(PropertyInfo propInfo)
    {
        if (propInfo.GetCustomAttributes().Count(_ =>
                _ is ColumnAttribute
                    or CustomColumnAttribute
                    or ForeignKeyColumnAttribute
                    or OneToManyDataAttribute
                    or ManyToManyDataAttribute) > 1)
            throw new InvalidAttributeCombinationException("Invalid property attribute combination.");
    }
    #endregion

    #region GetPropertyAttribute
    /// <summary>
    /// Retrieves the property's attribute of the specified type, if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propInfo"></param>
    /// <returns></returns>
    public static T GetPropertyAttribute<T>(PropertyInfo propInfo) where T : Attribute =>
        propInfo.GetCustomAttribute(typeof(T)) as T;
    #endregion

    #region GetTableAttribute
    /// <summary>
    /// Returns the table attribute for the specified type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException">The specified type does not contain the <see cref="TableAttribute"/> attribute.</exception>
    public static TableAttribute GetTableAttribute(Type type) =>
        GetTypeAttribute<TableAttribute>(type) ??
        throw new InvalidTypeException($"The {nameof(TableAttribute)} attribute is mandatory for SQLite commands.");
    #endregion

    #region GetTypeAttribute
    /// <summary>
    /// Retrieves the type's attribute of the specified type, if any.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static T GetTypeAttribute<T>(Type type) where T : Attribute =>
        Attribute.GetCustomAttribute(type, typeof(T)) as T;
    #endregion

    #endregion
}