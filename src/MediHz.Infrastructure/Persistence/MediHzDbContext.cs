using MediHz.Domain.Users;
using MediHz.Domain.Projects;
using MediHz.Domain.Audio;
using MediHz.Domain.Annotations;
using Microsoft.EntityFrameworkCore;

namespace MediHz.Infrastructure.Persistence;

public class MediHzDbContext : DbContext
{
    public MediHzDbContext(DbContextOptions<MediHzDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<AudioFile> AudioFiles => Set<AudioFile>();
    
    public DbSet<Annotation>  Annotations => Set<Annotation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        //Load all configurations 
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MediHzDbContext).Assembly);
    }
}
