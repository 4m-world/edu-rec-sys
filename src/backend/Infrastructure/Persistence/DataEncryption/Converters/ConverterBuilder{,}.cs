﻿using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics;

namespace Microsoft.EntityFrameworkCore.DataEncryption.Converters;

/// <summary>
/// A converter builder class which has both the model type and store type specified.
/// </summary>
/// <typeparam name="TModelType">
/// The model type.
/// </typeparam>
/// <typeparam name="TStoreType">
/// The store type.
/// </typeparam>
public readonly struct ConverterBuilder<TModelType, TStoreType>
{
    private readonly ConverterBuilder<TModelType> ModelType;
    private readonly Func<TStoreType, byte[]> Decoder;
    private readonly Func<Stream, TStoreType> Encoder;

    internal ConverterBuilder(ConverterBuilder<TModelType> modelType, Func<TStoreType, byte[]> decoder, Func<Stream, TStoreType> encoder)
    {
        Debug.Assert(!modelType.IsEmpty);
        Debug.Assert(decoder is not null);
        Debug.Assert(encoder is not null);

        ModelType = modelType;
        Decoder = decoder;
        Encoder = encoder;
    }

    public ValueConverter<TModelType, TStoreType> Build(ConverterMappingHints mappingHints = null)
    {
        var (encryptionProvider, modelDecoder, modelEncoder) = ModelType;
        var storeDecoder = Decoder;
        var storeEncoder = Encoder;

        if (modelDecoder is null || modelEncoder is null || storeDecoder is null || storeEncoder is null)
        {
            return null;
        }

        if (encryptionProvider is null)
        {
            return new ValueConverter<TModelType, TStoreType>(
                m => storeEncoder(StandardConverters.BytesToStream(modelDecoder(m))),
                s => modelEncoder(StandardConverters.BytesToStream(storeDecoder(s))),
                mappingHints);
        }

        return new EncryptionConverter<TModelType, TStoreType>(
            encryptionProvider,
            m => encryptionProvider.Encrypt(m, modelDecoder, storeEncoder),
            s => encryptionProvider.Decrypt(s, storeDecoder, modelEncoder),
            mappingHints);
    }
}
