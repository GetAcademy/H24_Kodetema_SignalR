using Microsoft.AspNetCore.SignalR;
using SignalR.Domain.Repositories;

namespace SignalR.Api.Hubs;

public class DocumentHub : Hub
{
    private readonly IDocumentRepository _documentRepository;

    public DocumentHub(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public override async Task OnConnectedAsync()
    {
        await SendAllOpenDocuments();
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (HubStorage.ConnectedUsers.ContainsKey(Context.ConnectionId))
        {
            _documentRepository.RemoveAsOwnerOnAllOpenDocuments(HubStorage.ConnectedUsers[Context.ConnectionId]);
            await SendAllOpenDocuments();
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task RegisterId(string id)
    {
        HubStorage.ConnectedUsers.TryAdd(Context.ConnectionId, id);
    }

    public async Task SendAllOpenDocuments()
    {
        await Clients.All.SendAsync("OpenDocumentsChanged", _documentRepository.GetAllOpenItems().ToList());
    }

    public async Task UpdateOwner(int id, string ownerName)
    {
        _documentRepository.UpdateDocumentOwner(id, ownerName);
        await SendAllOpenDocuments();
    }

    public async Task ReleaseOwner(int id)
    {
        _documentRepository.UpdateDocumentOwner(id, null);
        await SendAllOpenDocuments();
    }

    public async Task MarkAsCompleted(int id)
    {
        _documentRepository.MarkAsCompleted(id);
        await SendAllOpenDocuments();
    }
}