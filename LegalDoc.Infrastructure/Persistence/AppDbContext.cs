using LegalDoc.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LegalDoc.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<LegalDocument> LegalDocuments => Set<LegalDocument>();
    public DbSet<Registry> Registries => Set<Registry>();
    public DbSet<Lawyer> Lawyers => Set<Lawyer>();
    public DbSet<ReviewTask> ReviewTasks => Set<ReviewTask>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LegalDocument>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.Title).IsRequired().HasMaxLength(200);
            e.Property(d => d.FileName).IsRequired().HasMaxLength(255);
            e.Property(d => d.StoragePath).IsRequired();
            e.Property(d => d.CreatedAt).IsRequired();
            e.Property(d => d.Status).IsRequired();
            e.Property(d => d.Summary).HasColumnType("text");
            e.Property(d => d.Clauses).HasColumnType("text");
            e.Property(d => d.Risks).HasColumnType("text");
            e.Property(d => d.RegistryId).IsRequired();
        });
        
        modelBuilder.Entity<Registry>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.Name).IsRequired().HasMaxLength(150);
            e.Property(d => d.Capacity).IsRequired();
            e.Property(d => d.Availability).IsRequired();
        });
        
        modelBuilder.Entity<Lawyer>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.Name).IsRequired().HasMaxLength(200);
            e.Property(l => l.BarNumber).IsRequired().HasMaxLength(50);
            e.Property(l => l.Email).HasMaxLength(150);
        });
        
        modelBuilder.Entity<ReviewTask>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Description).HasMaxLength(500);
            e.Property(t => t.AssignedAt).IsRequired();
            
            e.HasOne<LegalDocument>().WithMany().HasForeignKey(t => t.DocumentId);
            e.HasOne<Lawyer>().WithMany().HasForeignKey(t => t.LawyerId);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}