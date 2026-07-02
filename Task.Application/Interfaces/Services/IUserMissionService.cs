using Task.Application.DTOs.Requests;
using Task.Application.DTOs.Responses;

namespace Task.Application.Interfaces.Services;

public interface IUserMissionService
{
    Task<Guid> AssignMissionAsync(AssignMissionRequest request);

    Task<IEnumerable<UserMissionResponse>> GetUserMissionsAsync(Guid userId);

    Task<UserMissionResponse?> GetMissionByIdAsync(Guid id);
}