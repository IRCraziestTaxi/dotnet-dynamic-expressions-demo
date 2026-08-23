using System.Linq.Expressions;
using DotnetDynamicExpressionsDemo.Entities;
using DotnetDynamicExpressionsDemo.Models;

namespace DotnetDynamicExpressionsDemo.Queries {
    public class QueryBuilder : IQueryBuilder {
        private readonly ParameterExpression _userParam = Expression.Parameter(typeof(User), "u");
        private readonly ParameterExpression _skillParam = Expression.Parameter(typeof(Skill), "s");

        public Expression<Func<T, bool>> BuildQuery<T>(Query query) {
            var expr = BuildQueryExpression(query, query.Scope, query.Scope);
            var param = GetParameter(query.Scope!);

            return Expression.Lambda<Func<T, bool>>(expr, param);
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

        private BinaryExpression BuildQueryCondition(Query query, string scope) {
            var propParam = GetParameter(scope);
            var propName = char.ToUpper(query.Prop![0]) + query.Prop[1..];
            var propExpr = Expression.Property(propParam, propName);
            var propVal = Expression.Constant(query.IntVal.HasValue ? query.IntVal.Value : query.StrVal!);

            return query.Op switch {
                "lt" => Expression.LessThan(propExpr, propVal),
                "lte" => Expression.LessThanOrEqual(propExpr, propVal),
                "gt" => Expression.GreaterThan(propExpr, propVal),
                "gte" => Expression.GreaterThanOrEqual(propExpr, propVal),
                "eq" => Expression.Equal(propExpr, propVal),
                "neq" => Expression.NotEqual(propExpr, propVal),
                _ => throw new BadHttpRequestException("Valid values for op are \"lt\", \"lte\", \"gt\", \"gte\", \"eq\", and \"neq\".")
            };
        }

        private Expression BuildQueryExpression(Query query, string? currentScope, string? parentScope) {
            bool isAnd = query.And.Count > 0;
            List<Query> subqueries = isAnd ? query.And : query.Or;

            if (subqueries.Count > 0) {
                List<Expression> subqueryExpressions = [];

                foreach (Query subquery in subqueries) {
                    var subqueryExpr = BuildQueryExpression(subquery, subquery.Scope ?? currentScope, currentScope);
                    subqueryExpressions.Add(subqueryExpr);
                }

                Expression? combinedExpr = null;

                foreach (var subqueryExpr in subqueryExpressions) {
                    if (combinedExpr == null) {
                        combinedExpr = subqueryExpr;
                    }
                    else {
                        combinedExpr = isAnd
                            ? Expression.AndAlso(combinedExpr, subqueryExpr)
                            : Expression.OrElse(combinedExpr, subqueryExpr);
                    }
                }

                // If crossing scope boundaries and current scope is "skill",
                // that signals this is the beginning of a User.Skills.Any expression.
                if (currentScope == "skill" && currentScope != parentScope) {
                    var skillsProp = Expression.Property(_userParam, nameof(User.Skills));
                    var skillsPredicate = Expression.Lambda<Func<Skill, bool>>(
                        combinedExpr!,
                        _skillParam
                    );
                    var callExpr = Expression.Call(
                        typeof(Enumerable),
                        nameof(Enumerable.Any),
                        [typeof(Skill)],
                        skillsProp,
                        skillsPredicate
                    );
                    combinedExpr = Expression.Lambda(callExpr, _userParam).Body;
                }

                return combinedExpr!;
            }
            else {
                return BuildQueryCondition(query, currentScope ?? parentScope!);
            }
        }

        private ParameterExpression GetParameter(string scope) {
            return scope switch {
                "user" => _userParam,
                "skill" => _skillParam,
                _ => throw new BadHttpRequestException("Invalid scope.")
            };
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
                    string? errorMessage = ValidateQueryNodes(subquery);

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
                && !string.IsNullOrEmpty(query.Op)
                && (
                    !string.IsNullOrEmpty(query.StrVal)
                    || query.IntVal.HasValue
                )
            ) ? null : validQueryHelp;
        }
    }
}
