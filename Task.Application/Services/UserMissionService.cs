using Task.Application.DTOs.Requests;
using Task.Application.DTOs.Responses;
using Task.Application.Interfaces.Repositories;
using Task.Application.Interfaces.Services;
using Task.Domain.Entities;

namespace Task.Application.Services;

public class UserMissionService : IUserMissionService
{
    private readonly IUserMissionRepository _userMissionRepository;
    private readonly IMissionTemplateRepository _missionTemplateRepository;

    public UserMissionService(
        IUserMissionRepository userMissionRepository,
        IMissionTemplateRepository missionTemplateRepository)
    {
        _userMissionRepository = userMissionRepository;
        _missionTemplateRepository = missionTemplateRepository;
    }

    public async System.Threading.Tasks.Task<Guid> AssignMissionAsync(AssignMissionRequest request)
    {
        var template = await _missionTemplateRepository.GetByIdAsync(request.TemplateId);

        if (template == null)
            throw new Exception("Mission template not found.");

        var mission = new UserMission
        {
            UserId = request.UserId,
            TripId = request.TripId,
            PlaceId = request.PlaceId,
            TemplateId = template.Id,

            Title = template.Name,

            RewardXP = template.RewardXP,
            RewardCoins = template.RewardCoins,

            Status = Domain.Enums.MissionStatus.Assigned,

            StartAt = DateTime.UtcNow
        };

        await _userMissionRepository.AddAsync(mission);

        return mission.Id;
    }

    public async System.Threading.Tasks.Task<IEnumerable<UserMissionResponse>> GetUserMissionsAsync(Guid userId)
    {
        var missions = await _userMissionRepository.GetByUserIdAsync(userId);

        return missions.Select(x => new UserMissionResponse
        {
            Id = x.Id,
            Title = x.Title,
            Status = x.Status,
            RewardXP = x.RewardXP,
            RewardCoins = x.RewardCoins,
            StartAt = x.StartAt,
            CompletedAt = x.CompletedAt
        });
    }

    public async System.Threading.Tasks.Task<UserMissionResponse?> GetMissionByIdAsync(Guid id)
    {
        var mission = await _userMissionRepository.GetByIdAsync(id);

        if (mission == null)
            return null;

        return new UserMissionResponse
        {
            Id = mission.Id,
            Title = mission.Title,
            Status = mission.Status,
            RewardXP = mission.RewardXP,
            RewardCoins = mission.RewardCoins,
            StartAt = mission.StartAt,
            CompletedAt = mission.CompletedAt
        };
    }
}