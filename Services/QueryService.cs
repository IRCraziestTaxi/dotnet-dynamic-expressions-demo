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

            IEnumerable<IEntity> results = query.Scope switch {
                "user" => await QueryUsers(query),
                _ => throw new BadHttpRequestException("Invalid scope.")
            };

            return results;
        }

        private async Task<IEnumerable<UserResult>> QueryUsers(Query query) {
            var queryExpr = _queryBuilder.BuildQuery<User>(query);
            var users = await _dbContext.Users.Include(u => u.Skills).Where(queryExpr).ToListAsync();
            var results = users.Select(u => new UserResult {
                Id = u.Id,
                Name = u.Name,
                YearsExperience = u.YearsExperience,
                Skills = u.Skills.Select(s => new SkillResult {
                    Id = s.Id,
                    Name = s.Name,
                    YearsExperience = s.YearsExperience,
                    UserId = s.UserId
                })
            });

            return results;
        }
    }
}
