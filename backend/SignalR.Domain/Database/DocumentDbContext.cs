using Microsoft.EntityFrameworkCore;
using SignalR.Domain.Models;

namespace SignalR.Domain.Database;

public class DocumentDbContext : DbContext
{
    public DocumentDbContext(DbContextOptions options) : base(options) { }
    
    public DbSet<Document> Documents { get; set; }
}