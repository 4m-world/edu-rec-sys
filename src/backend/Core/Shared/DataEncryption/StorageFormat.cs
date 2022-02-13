namespace System.ComponentModel.DataAnnotations;

public enum StorageFormat
{
    /// <summary>
    /// THe format determined by the model data type
    /// </summary>
    Default,

    /// <summary>
    /// The value stored in binary
    /// </summary>
    Binary,

    /// <summary>
    /// The value is stored in a Base64-encided string
    /// </summary>
    Base64
}