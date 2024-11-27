using SignalR.Domain.Database;
using SignalR.Domain.Models;

namespace SignalR.Domain.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly DocumentDbContext _documentDbContext;

    public DocumentRepository(ITodoDbContextGenerator todoDbContextGenerator)
    {
        _documentDbContext = todoDbContextGenerator.GetDbContext();
    }

    public IEnumerable<Document> GetAllOpenItems()
    {
        return _documentDbContext.Documents.Where(x => !x.Completed);
    }

    public void UpdateDocumentOwner(int id, string ownerName)
    {
        var item = _documentDbContext.Documents.FirstOrDefault(x => x.Id == id);
        if (item == null)
            return;
        
        item.Owner = ownerName;
        
        _documentDbContext.SaveChanges();
    }

    public void MarkAsCompleted(int id)
    {
        var item = _documentDbContext.Documents.FirstOrDefault(x => x.Id == id);
        if (item == null)
            return;

        item.Completed = true;
        item.CompletedAt = DateTime.Now;
        
        _documentDbContext.SaveChanges();
    }

    public void RemoveAsOwnerOnAllOpenDocuments(string ownerName)
    {
        var documents = _documentDbContext.Documents.Where(x => x.Owner == ownerName && !x.Completed).ToList();
        foreach (var document in documents)
        {
            document.Owner = null;
        }
        
        _documentDbContext.SaveChanges();
    }
}