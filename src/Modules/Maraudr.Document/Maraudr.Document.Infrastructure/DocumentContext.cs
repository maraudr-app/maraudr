using Microsoft.EntityFrameworkCore;

namespace Maraudr.Document.Infrastructure;

public class DocumentContext(DbContextOptions<DocumentContext> options) : DbContext(options)
{
    public DbSet<Domain.Document> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Document>(builder =>
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.FileName).IsRequired();
            builder.Property(d => d.Key).IsRequired();
            builder.Property(d => d.Url).IsRequired();  
            builder.Property(d => d.ContentType).IsRequired();
            builder.Property(d => d.AssociationId).IsRequired();
            builder.Property(d => d.UploadedAt).IsRequired();
        });
    }
}