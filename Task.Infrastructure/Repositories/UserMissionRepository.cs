using Microsoft.EntityFrameworkCore;
using Task.Application.Interfaces.Repositories;
using Task.Domain.Entities;
using Task.Infrastructure.Persistence;

namespace Task.Infrastructure.Repositories;

public class UserMissionRepository : IUserMissionRepository
{
    private readonly TaskDbContext _context;

    public UserMissionRepository(TaskDbContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task AddAsync(UserMission mission)
    {
        await _context.UserMissions.AddAsync(mission);
        await _context.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task<IEnumerable<UserMission>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserMissions
            .Include(x => x.Template)
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task<UserMission?> GetByIdAsync(Guid id)
    {
        return await _context.UserMissions
            .Include(x => x.Template)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async System.Threading.Tasks.Task UpdateAsync(UserMission mission)
    {
        _context.UserMissions.Update(mission);
        await _context.SaveChangesAsync();
    }
}