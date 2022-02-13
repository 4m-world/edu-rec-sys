using System.Security;
using System.Text;

namespace Microsoft.EntityFrameworkCore.DataEncryption.Converters;

public static class ConverterBuilderExtensions
{
    /// <summary>
    /// Builds a converter for a property with a custom model type.
    /// </summary>
    public static ConverterBuilder<TModelType> From<TModelType>(
            this IEncryptionProvider encryptionProvider,
            Func<TModelType, byte[]> decoder,
            Func<Stream, TModelType> encoder)
    {
        if (decoder is null)
        {
            throw new ArgumentNullException(nameof(decoder));
        }

        if (encoder is null)
        {
            throw new ArgumentNullException(nameof(encoder));
        }

        return new ConverterBuilder<TModelType>(encryptionProvider, decoder, encoder);
    }

    /// <summary>
    /// Builds a converter for a binary property.
    /// </summary>
    public static ConverterBuilder<byte[]> FromBinary(this IEncryptionProvider encryptionProvider)
    {
        return new ConverterBuilder<byte[]>(encryptionProvider, b => b, StandardConverters.StreamToBytes);
    }

    /// <summary>
    /// Builds a converter for a string property.
    /// </summary>
    public static ConverterBuilder<string> FromString(this IEncryptionProvider encryptionProvider)
    {
        return new ConverterBuilder<string>(encryptionProvider, Encoding.UTF8.GetBytes, StandardConverters.StreamToString);
    }

    /// <summary>
    /// Builds a converter for a <see cref="SecureString"/> property.
    /// </summary>
    public static ConverterBuilder<SecureString> FromSecureString(this IEncryptionProvider encryptionProvider)
    {
        return new ConverterBuilder<SecureString>(encryptionProvider, Encoding.UTF8.GetBytes, StandardConverters.StreamToSecureString);
    }

    /// <summary>
    /// Specifies that the property should be stored in the database using a custom format.
    /// </summary>
    public static ConverterBuilder<TModelType, TStoreType> To<TModelType, TStoreType>(
                ConverterBuilder<TModelType> modelType,
                Func<TStoreType, byte[]> decoder,
                Func<Stream, TStoreType> encoder)
    {
        if (modelType.IsEmpty)
        {
            throw new ArgumentNullException(nameof(modelType));
        }

        if (decoder is null)
        {
            throw new ArgumentNullException(nameof(decoder));
        }

        if (encoder is null)
        {
            throw new ArgumentNullException(nameof(encoder));
        }

        return new ConverterBuilder<TModelType, TStoreType>(modelType, decoder, encoder);
    }

    /// <summary>
    /// Specifies that the property should be stored in the database in binary.
    /// </summary>
    public static ConverterBuilder<TModelType, byte[]> ToBinary<TModelType>(this ConverterBuilder<TModelType> modelType)
    {
        if (modelType.IsEmpty)
        {
            throw new ArgumentNullException(nameof(modelType));
        }

        return new ConverterBuilder<TModelType, byte[]>(modelType, b => b, StandardConverters.StreamToBytes);
    }

    /// <summary>
    /// Specifies that the property should be stored in the database in a Base64-encoded string.
    /// </summary>
    public static ConverterBuilder<TModelType, string> ToBase64<TModelType>(this ConverterBuilder<TModelType> modelType)
    {
        if (modelType.IsEmpty)
        {
            throw new ArgumentNullException(nameof(modelType));
        }

        return new ConverterBuilder<TModelType, string>(modelType, Convert.FromBase64String, StandardConverters.StreamToBase64String);
    }
}
