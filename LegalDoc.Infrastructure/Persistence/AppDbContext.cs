using LegalDoc.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LegalDoc.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<LegalDocument> LegalDocuments => Set<LegalDocument>();

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
        });
        
        base.OnModelCreating(modelBuilder);
    }
}