using Microsoft.EntityFrameworkCore;
using SignalR.Domain.Models;

namespace SignalR.Domain.Database;

public class InMemoryDbContextGenerator : ITodoDbContextGenerator
{
    private readonly Lazy<DocumentDbContext> _documentDbContext;

    public InMemoryDbContextGenerator()
    {
        _documentDbContext = new Lazy<DocumentDbContext>(CreateInMemoryDbContext);
    }

    public DocumentDbContext GetDbContext() => _documentDbContext.Value;

    private DocumentDbContext CreateInMemoryDbContext()
    {
        var dbOptions = new DbContextOptionsBuilder<DocumentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var dbContext = new DocumentDbContext(dbOptions);
        dbContext.Database.EnsureCreated();
        SeedDatabase(dbContext);

        return dbContext;
    }

    private void SeedDatabase(DocumentDbContext dbContext)
    {
        if (dbContext.Documents.Any())
            return;

        dbContext.Documents.AddRange(new Document
        {
            Completed = false,
            CreatedAt = DateTime.Now,
            Id = 1,
            Owner = null,
            Content = "",
            FileName = "Scanned_Document_1.pdf"
        }, new Document
        {
            Completed = false,
            CreatedAt = DateTime.Now.AddDays(-1),
            Id = 2,
            Owner = null,
            Content = "",
            FileName = "Scanned_Document_2.pdf"
        }, new Document
        {
            Completed = false,
            CreatedAt = DateTime.Now.AddDays(-2),
            Id = 3,
            Owner = null,
            Content = "",
            FileName = "Scanned_Document_3.pdf"
        }, new Document
        {
            Completed = false,
            CreatedAt = DateTime.Now.AddDays(-3),
            Id = 4,
            Owner = null,
            Content = "",
            FileName = "Scanned_Document_4.pdf"
        }, new Document
        {
            Completed = false,
            CreatedAt = DateTime.Now.AddDays(-4),
            Id = 5,
            Owner = null,
            Content = "",
            FileName = "Scanned_Document_5.pdf"
        }, new Document
        {
            Completed = false,
            CreatedAt = DateTime.Now.AddDays(-5),
            Id = 6,
            Owner = null,
            Content = "",
            FileName = "Scanned_Document_6.pdf"
        }, new Document
        {
            Completed = false,
            CreatedAt = DateTime.Now.AddDays(-6),
            Id = 7,
            Owner = null,
            Content = "",
            FileName = "Scanned_Document_7.pdf"
        }, new Document
        {
            Completed = false,
            CreatedAt = DateTime.Now.AddDays(-7),
            Id = 8,
            Owner = null,
            Content = "",
            FileName = "Scanned_Document_8.pdf"
        }, new Document
        {
            Completed = false,
            CreatedAt = DateTime.Now.AddDays(-8),
            Id = 9,
            Owner = null,
            Content = "",
            FileName = "Scanned_Document_9.pdf"
        }, new Document
        {
            Completed = false,
            CreatedAt = DateTime.Now.AddDays(-9),
            Id = 10,
            Owner = null,
            Content = "",
            FileName = "Scanned_Document_10.pdf"
        });

        dbContext.SaveChanges();
    }
}