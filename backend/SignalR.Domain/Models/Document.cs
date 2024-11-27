namespace SignalR.Domain.Models;

public class Document
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Owner { get; set; }
}