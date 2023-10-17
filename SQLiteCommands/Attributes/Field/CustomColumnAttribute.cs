using SQLiteCommands.Helpers;

namespace SQLiteCommands.Attributes.Field;

[AttributeUsage(AttributeTargets.Property)]
public class CustomColumnAttribute : Attribute
{
    #region Properties

    /// <summary>
    /// The column's custom data. It can be a literal value or a valid SQL operation.
    /// </summary>
    public string CustomData { get; }

    #endregion

    public CustomColumnAttribute(string customData)
    {
        AttributeHelper.ValidatePropertyValue(customData, nameof(customData), "custom column's data");
        CustomData = customData;
    }
}