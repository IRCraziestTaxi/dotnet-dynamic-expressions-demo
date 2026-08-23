using DotnetDynamicExpressionsDemo.Entities;

namespace DotnetDynamicExpressionsDemo.Models {
    public class SkillResult : IEntity {
        public int Id { get; set; }

        public required string Name { get; set; }

        public int YearsExperience { get; set; }

        public int UserId { get; set; }
    }
}
