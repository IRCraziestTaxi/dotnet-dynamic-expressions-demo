using DotnetDynamicExpressionsDemo.Data;
using DotnetDynamicExpressionsDemo.Entities;
using DotnetDynamicExpressionsDemo.Models;
using DotnetDynamicExpressionsDemo.Queries;
using Microsoft.EntityFrameworkCore;

namespace DotnetDynamicExpressionsDemo.Services {
    public class QueryService(AppDbContext _dbContext, IQueryBuilder _queryBuilder) : IQueryService
    {
        public async Task<IEnumerable<IEntity>> ExecuteQuery(Query query) {
            var errorMessage = _queryBuilder.ValidateQuery(query);

            if (!string.IsNullOrEmpty(errorMessage)) {
                throw new BadHttpRequestException(errorMessage);
            }

            IEnumerable<IEntity> results;

            switch (query.Scope) {
                case "user": {
                    results = await QueryUsers(query);
                    break;
                }
                default: {
                    throw new BadHttpRequestException("Invalid scope.");
                }
            }

            return results;
        }

        private async Task<IEnumerable<User>> QueryUsers(Query query) {
            var queryExpr = _queryBuilder.BuildQuery<User>(query);
            var results = await _dbContext.Users.Include(u => u.Skills).Where(queryExpr).ToListAsync();

            return results;
        }
    }
}
