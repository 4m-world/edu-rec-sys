using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace Microsoft.EntityFrameworkCore.DataEncryption.Converters;

/// <summary>
/// Defines the internal encryption converter for string values.
/// </summary>
internal sealed class EncryptionConverter<TModel, TProvider> : ValueConverter<TModel, TProvider>, IEncryptionValueConverter
{
    /// <summary>
    /// Creates a new <see cref="EncryptionConverter{TModel,TProvider}"/> instance.
    /// </summary>
    public EncryptionConverter(
        IEncryptionProvider encryptionProvider,
        Expression<Func<TModel, TProvider>> convertToProviderExpression,
        Expression<Func<TProvider, TModel>> convertFromProviderExpression,
        ConverterMappingHints mappingHints = null)
        : base(convertToProviderExpression, convertFromProviderExpression, mappingHints)
    {
        EncryptionProvider = encryptionProvider;
    }

    /// <inheritdoc />
    public IEncryptionProvider EncryptionProvider { get; }
}
