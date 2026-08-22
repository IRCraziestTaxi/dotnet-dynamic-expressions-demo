namespace DotnetDynamicExpressionsDemo.Entities {
    public class User : IEntity {
        public int Id { get; set; }

        public required string Name { get; set; }

        /// <summary>
        /// Total years of experience this user has.
        /// </summary>
        public int YearsExperience { get; set; }

        public ICollection<Skill> Skills { get; } = [];
    }
}
