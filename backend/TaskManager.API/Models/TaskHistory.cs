using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskManager.API.Models;

public class TaskHistory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("taskId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string TaskId { get; set; } = string.Empty;

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("action")]
    public HistoryAction Action { get; set; }

    [BsonElement("field")]
    public string? Field { get; set; }

    [BsonElement("oldValue")]
    public string? OldValue { get; set; }

    [BsonElement("newValue")]
    public string? NewValue { get; set; }

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum HistoryAction
{
    Created,
    Updated,
    StatusChanged,
    Assigned,
    Commented,
    Deleted,
    Restored
}
