using System.Linq.Expressions;
using DotnetDynamicExpressionsDemo.Models;

namespace DotnetDynamicExpressionsDemo.Queries {
    public interface IQueryBuilder {
        Expression<Func<T, bool>> BuildQuery<T>(Query query);

        string? ValidateQuery(Query query);
    }
}
