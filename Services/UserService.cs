using DotnetDynamicExpressionsDemo.Data;
using DotnetDynamicExpressionsDemo.Entities;
using DotnetDynamicExpressionsDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetDynamicExpressionsDemo.Services {
    public class UserService(AppDbContext _dbContext) : IUserService {
        public async Task<int> AddUser(AddUser addUser) {
            var user = new User {
                Name = addUser.Name,
                YearsExperience = addUser.YearsExperience
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user.Id;
        }

        public async Task<int> UpsertSkill(int userId, AddSkill addSkill) {
            // If skill for user already exists, update YearsExperience and update. Otherwise, add.
            var existingSkill = await _dbContext.Skills.FirstOrDefaultAsync(s => s.UserId == userId && s.Name == addSkill.Name);
            Skill skill = null!;

            if (existingSkill != null) {
                existingSkill.YearsExperience = addSkill.YearsExperience;
                _dbContext.Skills.Update(existingSkill);
                skill = existingSkill;
            }
            else {
                skill = new Skill {
                    Name = addSkill.Name,
                    YearsExperience = addSkill.YearsExperience,
                    UserId = userId
                };
                _dbContext.Skills.Add(skill);
            }

            await _dbContext.SaveChangesAsync();

            return skill.Id;
        }
    }
}
