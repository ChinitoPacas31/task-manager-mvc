using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskManager.API.Models;

public class Notification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("message")]
    public string Message { get; set; } = string.Empty;

    [BsonElement("type")]
    public NotificationType Type { get; set; } = NotificationType.Info;

    [BsonElement("relatedTaskId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? RelatedTaskId { get; set; }

    [BsonElement("relatedProjectId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? RelatedProjectId { get; set; }

    [BsonElement("isRead")]
    public bool IsRead { get; set; } = false;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum NotificationType
{
    Info,
    Warning,
    Success,
    Error,
    TaskAssigned,
    TaskCompleted,
    TaskDueSoon,
    CommentAdded,
    ProjectUpdate
}
