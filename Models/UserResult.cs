using DotnetDynamicExpressionsDemo.Entities;

namespace DotnetDynamicExpressionsDemo.Models {
    public class UserResult : IEntity {
        public int Id { get; set; }

        public required string Name { get; set; }

        public int YearsExperience { get; set; }

        public IEnumerable<SkillResult> Skills { get; set; } = [];
    }
}
