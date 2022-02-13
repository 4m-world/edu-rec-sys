using System.Linq.Expressions;

namespace Ardalis.Specification;

public static class ISpecificationBuilderExtensions
{
    //public static ISpecificationBuilder<T> Where<T>(
    //    this ISpecificationBuilder<T> specificationBuilder,
    //    Expression<Func<T, bool>> criteria,
    //    bool condition)
    //{
    //    if (condition)
    //    {
    //        specificationBuilder.Where(criteria);
    //    }

    //    return specificationBuilder;
    //}

    //public static IOrderedSpecificationBuilder<T> OrderBy<T>(
    //    this ISpecificationBuilder<T> specificationBuilder,
    //    Expression<Func<T, object?>> orderExpression,
    //    bool condition)
    //{
    //    if (condition)
    //    {
    //        specificationBuilder.OrderBy(orderExpression);
    //    }
    //    return new OrderedSpecificationBuilder<T>(specificationBuilder.Specification);
    //}

    //public static IOrderedSpecificationBuilder<T> OrderByDescending<T>(
    //    this ISpecificationBuilder<T> specificationBuilder,
    //    Expression<Func<T, object?>> orderExpression,
    //    bool condition)
    //{
    //    if (condition)
    //    {
    //        specificationBuilder.OrderByDescending(orderExpression);
    //    }
    //    return new OrderedSpecificationBuilder<T>(specificationBuilder.Specification);
    //}
}
