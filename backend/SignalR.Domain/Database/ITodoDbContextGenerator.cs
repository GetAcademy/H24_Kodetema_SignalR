namespace SignalR.Domain.Database;

public interface ITodoDbContextGenerator
{
    DocumentDbContext GetDbContext();
}