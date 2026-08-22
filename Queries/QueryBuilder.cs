using System.Linq.Expressions;
using DotnetDynamicExpressionsDemo.Entities;
using DotnetDynamicExpressionsDemo.Models;

namespace DotnetDynamicExpressionsDemo.Queries {
    public class QueryBuilder : IQueryBuilder {
        private readonly ParameterExpression _userParam = Expression.Parameter(typeof(User), "u");
        private readonly ParameterExpression _skillParam = Expression.Parameter(typeof(Skill), "s");

        public Expression<Func<T, bool>> BuildQuery<T>(Query query) {
            return BuildQueryExpression<T>(query, query.Scope, query.Scope);
        }

        /// <summary>
        /// Validates the Query and returns an error message if invalid or null if valid.
        /// </summary>
        /// <param name="query">The Query to validate.</param>
        /// <returns>An error message if invalid or null if valid.</returns>
        public string? ValidateQuery(Query query) {
            // Ensure top level node specifies scope so QueryService can infer which method to call.
            if (string.IsNullOrEmpty(query.Scope)) {
                return "A top-level scope representing the queried entity is required.";
            }

            return ValidateQueryNodes(query);
        }

        private Expression<Func<T, bool>> BuildQueryCondition<T>(Query query, string scope) {
            ParameterExpression propParam;

            switch (scope) {
                case "user": {
                    propParam = _userParam;
                    break;
                }
                case "skill": {
                    propParam = _skillParam;
                    break;
                }
                default: {
                    throw new BadHttpRequestException("Invalid scope; must correspond to entity name.");
                }
            }

            var propName = char.ToUpper(scope[0]) + scope[1..];
            var propExpr = Expression.Property(propParam, propName);
            var propVal = Expression.Constant(query.IntVal.HasValue ? query.IntVal.Value : query.StrVal!);
            BinaryExpression opExpr;

            switch (query.Op) {
                case "lt": {
                    opExpr = Expression.LessThan(propExpr, propVal);
                    break;
                }
                case "lte": {
                    opExpr = Expression.LessThanOrEqual(propExpr, propVal);
                    break;
                }
                case "gt": {
                    opExpr = Expression.GreaterThan(propExpr, propVal);
                    break;
                }
                case "gte": {
                    opExpr = Expression.GreaterThanOrEqual(propExpr, propVal);
                    break;
                }
                case "eq": {
                    opExpr = Expression.Equal(propExpr, propVal);
                    break;
                }
                case "neq": {
                    opExpr = Expression.NotEqual(propExpr, propVal);
                    break;
                }
                default: {
                    throw new BadHttpRequestException("Valid values for op are \"lt\", \"lte\", \"gt\", \"gte\", \"eq\", and \"neq\".");
                }
            }

            var predicate = Expression.Lambda<Func<T, bool>>(opExpr, propParam);

            return predicate;
        }

        private Expression<Func<T, bool>> BuildQueryExpression<T>(Query query, string? currentScope, string? ambientScope) {
            bool isAnd = query.And.Count > 0;
            List<Query> subqueries = isAnd ? query.And : query.Or;

            if (subqueries.Count > 0) {
                List<Expression<Func<T, bool>>> subqueryExpressions = [];

                foreach (Query subquery in subqueries) {
                    var subqueryExpr = BuildQueryExpression<T>(subquery, subquery.Scope, subquery.Scope ?? ambientScope);
                    subqueryExpressions.Add(subqueryExpr);
                }

                Expression<Func<T, bool>>? combinedExpr = null;

                foreach (var subqueryExpr in subqueryExpressions) {
                    if (combinedExpr == null) {
                        combinedExpr = subqueryExpr;
                    }
                    else {
                        var andOrExpr = isAnd
                            ? Expression.AndAlso(combinedExpr.Body, subqueryExpr.Body)
                            : Expression.OrElse(combinedExpr.Body, subqueryExpr.Body);
                        combinedExpr = Expression.Lambda<Func<T, bool>>(andOrExpr, combinedExpr.Parameters.First());
                    }
                }

                // If crossing scope boundaries and current scope is "skill",
                // that signals this is the beginning of a User.Skills.Any expression.
                if (currentScope == "skill" && currentScope != ambientScope) {
                    var skillsProp = Expression.Property(_userParam, nameof(User.Skills));
                    var callExpr = Expression.Call(
                        typeof(Enumerable),
                        nameof(Enumerable.Any),
                        [typeof(Skill)],
                        skillsProp,
                        combinedExpr!
                    );
                    combinedExpr = Expression.Lambda<Func<T, bool>>(callExpr, _userParam);
                }

                return combinedExpr!;
            }
            else {
                return BuildQueryCondition<T>(query, currentScope ?? ambientScope!);
            }
        }

        private string? ValidateQueryNodes(Query query) {
            string validQueryHelp = "Each query object must either have prop, op, and EITHER strVal OR intVal (nutually exclusive); or it must have EITHER an and array OR an or array (mutually exclusive).";
            bool hasAnd = query.And.Count > 0;
            bool hasOr = query.Or.Count > 0;

            if (
                string.IsNullOrEmpty(query.Prop)
                && string.IsNullOrEmpty(query.Op)
                && string.IsNullOrEmpty(query.StrVal)
                && !query.IntVal.HasValue
            ) {
                // If this is not one query condition, must be either a collection of And conditions or a collection of Or conditions.

                // If neither exists; this object is empty and is not valid.
                if (!hasAnd && !hasOr) {
                    return validQueryHelp;
                }

                // If both exist, object is invalid; And and Or are mutually exclusive.
                if (hasAnd && hasOr) {
                    return validQueryHelp;
                }

                // If only one exists, validate each nested Query object.
                List<Query> subqueries = hasAnd ? query.And : query.Or;

                foreach (Query subquery in subqueries) {
                    string? errorMessage = ValidateQuery(subquery);

                    if (!string.IsNullOrEmpty(errorMessage)) {
                        return errorMessage;
                    }
                }

                return null;
            }

            // If at least one condition property exists, then all required properties must exist and And and Or must both not exist.
            if (hasAnd || hasOr) {
                return validQueryHelp;
            }

            if (!string.IsNullOrEmpty(query.StrVal) && query.IntVal.HasValue) {
                return validQueryHelp;
            }

            return (
                !string.IsNullOrEmpty(query.Prop)
                // Validate that Query.Prop is a dot-separated string, i.e. "user.yearsExperience" or "skill.name".
                && query.Prop.IndexOf('.') > 0
                && query.Prop.IndexOf('.') < query.Prop.Length - 1
                && !string.IsNullOrEmpty(query.Op)
                && (
                    !string.IsNullOrEmpty(query.StrVal)
                    || query.IntVal.HasValue
                )
            ) ? null : validQueryHelp;
        }
    }
}
