using DotnetDynamicExpressionsDemo.Models;

namespace DotnetDynamicExpressionsDemo.Services {
    public interface IUserService {
        Task<int> AddUser(AddUser user);

        Task<int> UpsertSkill(int userId, AddSkill skill);
    }
}
