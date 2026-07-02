using Microsoft.EntityFrameworkCore;
using Task.Domain.Entities;
namespace Task.Infrastructure.Persistence
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options)
            : base(options)
        {
        }
        public DbSet<MissionTemplate> MissionTemplates => Set<MissionTemplate>();

        public DbSet<UserMission> UserMissions => Set<UserMission>();

        public DbSet<MissionSubmission> MissionSubmissions => Set<MissionSubmission>();

        public DbSet<MissionEvidence> MissionEvidences => Set<MissionEvidence>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskDbContext).Assembly);
        }
    }
}
