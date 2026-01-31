namespace TaskManager.API.DTOs;

public class NotificationResponse
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? RelatedTaskId { get; set; }
    public string? RelatedProjectId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotificationCountResponse
{
    public int Total { get; set; }
    public int Unread { get; set; }
}
