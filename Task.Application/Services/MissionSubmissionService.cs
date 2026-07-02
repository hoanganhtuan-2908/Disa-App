using Task.Application.DTOs.Requests;
using Task.Application.Interfaces.Services;
using Task.Application.Interfaces.Repositories;
using Task.Domain.Entities;
using Task.Domain.Enums;
using Task.Application.Interfaces;

namespace Task.Application.Services;

public class MissionSubmissionService : IMissionSubmissionService
{
    private readonly IUserMissionRepository _userMissionRepository;

    private readonly IMissionSubmissionRepository _submissionRepository;

    private readonly IMissionEvidenceRepository _evidenceRepository;

    private readonly IFileStorageService _fileStorageService;
    public MissionSubmissionService(
    IUserMissionRepository userMissionRepository,
    IMissionSubmissionRepository submissionRepository,
    IMissionEvidenceRepository evidenceRepository,
    IFileStorageService fileStorageService)
    {
        _userMissionRepository = userMissionRepository;
        _submissionRepository = submissionRepository;
        _evidenceRepository = evidenceRepository;
        _fileStorageService = fileStorageService;
    }
    public MissionSubmissionService(
        IUserMissionRepository userMissionRepository,
        IMissionSubmissionRepository submissionRepository,
        IMissionEvidenceRepository evidenceRepository)
    {
        _userMissionRepository = userMissionRepository;
        _submissionRepository = submissionRepository;
        _evidenceRepository = evidenceRepository;
    }
    public async System.Threading.Tasks.Task<Guid> SubmitMissionAsync(
     Guid userMissionId,
     SubmitMissionRequest request)
    {
        var mission = await _userMissionRepository.GetByIdAsync(userMissionId);

        if (mission == null)
            throw new Exception("Mission not found.");

        if (mission.Status != MissionStatus.Assigned)
            throw new Exception("Mission cannot be submitted.");

        // Tạo Submission
        var submission = new MissionSubmission
        {
            UserMissionId = mission.Id,
            SubmittedAt = DateTime.UtcNow,
            VerificationStatus = VerificationStatus.Pending
        };

        await _submissionRepository.AddAsync(submission);

        // ===========================
        // Upload ảnh
        // ===========================
        if (request.Photo != null)
        {
            var imageUrl = await _fileStorageService.UploadImageAsync(request.Photo);

            var evidence = new MissionEvidence
            {
                SubmissionId = submission.Id,
                Type = EvidenceType.Photo,
                MediaUrl = imageUrl,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                TakenAt = request.TakenAt
            };

            await _evidenceRepository.AddAsync(evidence);
        }

        // ===========================
        // Upload video
        // ===========================
        if (request.Video != null)
        {
            var videoUrl = await _fileStorageService.UploadVideoAsync(request.Video);

            var evidence = new MissionEvidence
            {
                SubmissionId = submission.Id,
                Type = EvidenceType.Video,
                MediaUrl = videoUrl,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                TakenAt = request.TakenAt
            };

            await _evidenceRepository.AddAsync(evidence);
        }

        // Cập nhật trạng thái nhiệm vụ
        mission.Status = MissionStatus.PendingReview;
        await _userMissionRepository.UpdateAsync(mission);

        return submission.Id;
    }
}
