using DotnetDynamicExpressionsDemo.Entities;
using DotnetDynamicExpressionsDemo.Models;

namespace DotnetDynamicExpressionsDemo.Services {
    public interface IQueryService {
        Task<IEnumerable<IEntity>> ExecuteQuery(Query query);
    }
}
