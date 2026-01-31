using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskManager.API.Models;

public class TaskItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("status")]
    public TaskStatus Status { get; set; } = TaskStatus.Pending;

    [BsonElement("priority")]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    [BsonElement("projectId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ProjectId { get; set; }

    [BsonElement("assignedToId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? AssignedToId { get; set; }

    [BsonElement("createdById")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string CreatedById { get; set; } = string.Empty;

    [BsonElement("dueDate")]
    public DateTime? DueDate { get; set; }

    [BsonElement("estimatedHours")]
    public decimal? EstimatedHours { get; set; }

    [BsonElement("actualHours")]
    public decimal? ActualHours { get; set; }

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("completedAt")]
    public DateTime? CompletedAt { get; set; }
}

public enum TaskStatus
{
    Pending,
    InProgress,
    Review,
    Completed,
    Cancelled
}

public enum TaskPriority
{
    Low,
    Medium,
    High,
    Critical
}
