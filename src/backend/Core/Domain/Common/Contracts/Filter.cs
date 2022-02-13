using System.Linq.Expressions;

namespace CodeMatrix.Mepd.Shared.DTOs.Common.Contracts;

// Todo: shall I remove in favtor to? 
public class Filter<T>
{
    public Filter(bool condition, Expression<Func<T, bool>> expression) =>
        (Condition, Expression) = (condition, expression);

    public bool Condition { get; }
    public Expression<Func<T, bool>> Expression { get; }
}