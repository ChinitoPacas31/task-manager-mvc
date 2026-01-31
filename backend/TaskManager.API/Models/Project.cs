using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskManager.API.Models;

public class Project
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("status")]
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;

    [BsonElement("ownerId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string OwnerId { get; set; } = string.Empty;

    [BsonElement("memberIds")]
    public List<string> MemberIds { get; set; } = new();

    [BsonElement("startDate")]
    public DateTime? StartDate { get; set; }

    [BsonElement("endDate")]
    public DateTime? EndDate { get; set; }

    [BsonElement("color")]
    public string Color { get; set; } = "#3B82F6";

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum ProjectStatus
{
    Active,
    OnHold,
    Completed,
    Archived
}
