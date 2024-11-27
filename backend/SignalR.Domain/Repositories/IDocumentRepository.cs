using SignalR.Domain.Models;

namespace SignalR.Domain.Repositories;

public interface IDocumentRepository
{
    IEnumerable<Document> GetAllOpenItems();
    void UpdateDocumentOwner(int id, string ownerName);
    void MarkAsCompleted(int id);
    void RemoveAsOwnerOnAllOpenDocuments(string ownerName);
}