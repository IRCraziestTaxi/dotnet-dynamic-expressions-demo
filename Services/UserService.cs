using DotnetDynamicExpressionsDemo.Data;
using DotnetDynamicExpressionsDemo.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotnetDynamicExpressionsDemo.Services {
    public class UserService(AppDbContext _dbContext) : IUserService {
        public async Task<int> AddUser(User user) {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user.Id;
        }

        public async Task<int> UpsertSkill(int userId, Skill skill) {
            // If skill for user already exists, update YearsExperience and update. Otherwise, add.
            var existingSkill = await _dbContext.Skills.FirstOrDefaultAsync(s => s.UserId == userId && s.Name == skill.Name);

            if (existingSkill != null) {
                existingSkill.YearsExperience = skill.YearsExperience;
                _dbContext.Skills.Update(existingSkill);
            }
            else {
                skill.UserId = userId;
                _dbContext.Skills.Add(skill);
            }

            await _dbContext.SaveChangesAsync();

            return existingSkill?.Id ?? skill.Id;
        }
    }
}
