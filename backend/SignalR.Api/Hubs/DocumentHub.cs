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
        await SendAllOpenDocumentsToCaller();
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (HubStorage.ConnectedUsers.TryRemove(Context.ConnectionId, out var userId))
        {
            _documentRepository.RemoveAsOwnerOnAllOpenDocuments(userId);
            await NotifyAllExceptCaller($"User with id {userId} has been disconnected. All documents assigned has been released");
            await SendAllOpenDocuments();
            await SendAllConnectedUsers();
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task RegisterId(string id)
    {
        HubStorage.ConnectedUsers.TryAdd(Context.ConnectionId, id);
        await SendAllConnectedUsers();
        await NotifyAllExceptCaller($"User with id {id} connected");
    }

    public async Task SendAllOpenDocuments()
    {
        await Clients.All.SendAsync("OpenDocumentsChanged", _documentRepository.GetAllOpenItems().ToList());
    }

    public async Task SendAllOpenDocumentsToCaller()
    {
        await Clients.Caller.SendAsync("OpenDocumentsChanged", _documentRepository.GetAllOpenItems().ToList());
    }

    public async Task SendAllConnectedUsers()
    {
        await Clients.All.SendAsync("ConnectedUsersChanged", HubStorage.ConnectedUsers
            .Select(x => x.Value)
            .ToList());
    }

    public async Task NudgeUser(string userId)
    {
        var userContextId = HubStorage.ConnectedUsers
            .FirstOrDefault(x => x.Value == userId).Key;

        if (!string.IsNullOrEmpty(userContextId))
        {
            await NotifySpecificUser(userContextId, "Hurry up my man");
        }
    }

    private async Task NotifySpecificUser(string userContextId, string message)
    {
        var callerUserId = HubStorage.ConnectedUsers[Context.ConnectionId];
        await Clients.Client(userContextId).SendAsync("ShowNotification", $"{callerUserId} says: {message}");
    }

    public async Task NotifyAllExceptCaller(string message)
    {
        await Clients.Others.SendAsync("ShowNotification", message);
    }

    public async Task UpdateOwner(int id, string ownerName)
    {
        _documentRepository.UpdateDocumentOwner(id, ownerName);
        await NotifyAllExceptCaller($"Document with id {id} was assigned to {ownerName}");
        await SendAllOpenDocuments();
    }

    public async Task ReleaseOwner(int id)
    {
        _documentRepository.UpdateDocumentOwner(id, null);
        await NotifyAllExceptCaller($"Document with id {id} was released");
        await SendAllOpenDocuments();
    }

    public async Task MarkAsCompleted(int id)
    {
        _documentRepository.MarkAsCompleted(id);
        await NotifyAllExceptCaller($"Document with id {id} completed");
        await SendAllOpenDocuments();
    }
}