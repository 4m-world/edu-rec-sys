using System.Diagnostics;

namespace Microsoft.EntityFrameworkCore.DataEncryption.Converters;

/// <summary>
/// A converter builder class which has the model type specified.
/// </summary>
/// <typeparam name="TModelType">
/// The model type.
/// </typeparam>
public readonly struct ConverterBuilder<TModelType>
{
    private readonly IEncryptionProvider EncryptionProvider;
    private readonly Func<TModelType, byte[]> Decoder;
    private readonly Func<Stream, TModelType> Encoder;

    internal bool IsEmpty => Decoder is null || Encoder is null;

    internal ConverterBuilder(IEncryptionProvider encryptionProvider, Func<TModelType, byte[]> decoder, Func<Stream, TModelType> encoder)
    {
        Debug.Assert(decoder is not null);
        Debug.Assert(encoder is not null);

        EncryptionProvider = encryptionProvider;
        Decoder = decoder;
        Encoder = encoder;
    }

    internal void Deconstruct(out IEncryptionProvider encryptionProvider, out Func<TModelType, byte[]> decoder, out Func<Stream, TModelType> encoder)
    {
        encryptionProvider = EncryptionProvider;
        decoder = Decoder;
        encoder = Encoder;
    }
}
