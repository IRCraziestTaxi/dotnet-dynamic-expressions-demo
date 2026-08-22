using System.ComponentModel.DataAnnotations;

namespace DotnetDynamicExpressionsDemo.Models {
    public class Query {
        /// <summary>
        /// Used to signal which entity should be used for the current or nested conditions.
        /// </summary>
        [AllowedValues("user", "skill", ErrorMessage = "Allowed scopes are \"user\" and \"skill\".")]
        public string? Scope { get; set; }

        /// <summary>
        /// Must be used in conjunction with Op and either IntVal or StrVal; these properties compose one Query condition.
        /// If these properties exist, And and Or cannot be used.
        /// </summary>
        public string? Prop { get; set; }

        /// <summary>
        /// Must be used in conjunction with Prop and either IntVal or StrVal; these properties compose one Query condition.
        /// If these properties exist, And and Or cannot be used. Value must be one of the following:
        /// "lt", "gt", "lte", "gte", "eq", or "neq".
        /// </summary>
        [AllowedValues("lt", "gt", "lte", "gte", "eq", "neq", ErrorMessage = "Allowed operations are \"lt\", \"gt\", \"lte\", \"gte\", \"eq\", and \"neq\".")]
        public string? Op { get; set; }

        /// <summary>
        /// Used if the property for this condition is an integer property.
        /// </summary>
        public int? IntVal { get; set; }

        /// <summary>
        /// Used if the property for this condition is a string property.
        /// </summary>
        public string? StrVal { get; set; }

        /// <summary>
        /// If this property exists, no other properties can exist on the same Query object (except for Scope).
        /// </summary>
        public List<Query> And { get; set; } = [];

        /// <summary>
        /// If this property exists, no other properties can exist on the same Query object (except for Scope).
        /// </summary>
        public List<Query> Or { get; set; } = [];
    }
}
