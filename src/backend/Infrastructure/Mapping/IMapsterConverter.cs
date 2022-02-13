﻿namespace CodeMatrix.Mepd.Infrastructure.Mapping;

public interface IMapsterConverter<T, TD>
{
    public TD Convert(T item);

    public T ConvertBack(TD item);
}

public interface IMapsterConverterAsync<T, TD>
{
    public Task<TD> ConvertAsync(T item, CancellationToken cancellationToken = default);

    public Task<T> ConvertBackAsync(TD item, CancellationToken cancellationToken = default);
}