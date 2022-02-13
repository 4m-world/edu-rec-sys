namespace Microsoft.EntityFrameworkCore.DataEncryption.Converters;

/// <summary>
/// Interface for an encryption value converter.
/// </summary>
public interface IEncryptionValueConverter
{
    /// <summary>
    /// Returns the encryption provider, if any.
    /// </summary>
    /// <value>
    /// The <see cref="IEncryptionProvider"/> for this converter, if any.
    /// </value>
    IEncryptionProvider EncryptionProvider { get; }
}
