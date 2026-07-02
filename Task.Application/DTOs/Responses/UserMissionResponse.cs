using Task.Domain.Enums;

namespace Task.Application.DTOs.Responses;

public class UserMissionResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public MissionStatus Status { get; set; }

    public int RewardXP { get; set; }

    public int RewardCoins { get; set; }

    public DateTime StartAt { get; set; }

    public DateTime? CompletedAt { get; set; }
}