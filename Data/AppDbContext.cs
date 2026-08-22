using DotnetDynamicExpressionsDemo.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotnetDynamicExpressionsDemo.Data {
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
        public required DbSet<User> Users { get; set; }

        public required DbSet<Skill> Skills { get; set; }
    }
}
