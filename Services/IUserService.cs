using DotnetDynamicExpressionsDemo.Entities;

namespace DotnetDynamicExpressionsDemo.Services {
    public interface IUserService {
        Task<int> AddUser(User user);

        Task<int> UpsertSkill(int userId, Skill skill);
    }
}
