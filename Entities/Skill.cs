namespace DotnetDynamicExpressionsDemo.Entities {
    public class Skill : IEntity {
        public int Id { get; set; }

        public required string Name { get; set; }

        /// <summary>
        /// Years of experience this user has in this skill.
        /// </summary>
        public int YearsExperience { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
