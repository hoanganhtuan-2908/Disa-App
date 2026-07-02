using Task.Domain.Entities;

namespace Task.Application.Interfaces.Repositories;

public interface IUserMissionRepository
{
    System.Threading.Tasks.Task AddAsync(UserMission mission);

    System.Threading.Tasks.Task<IEnumerable<UserMission>> GetByUserIdAsync(Guid userId);

    System.Threading.Tasks.Task<UserMission?> GetByIdAsync(Guid id);

    System.Threading.Tasks.Task UpdateAsync(UserMission mission);
}