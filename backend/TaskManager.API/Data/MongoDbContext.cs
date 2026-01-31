using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TaskManager.API.Models;

namespace TaskManager.API.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<TaskItem> Tasks => _database.GetCollection<TaskItem>("tasks");
    public IMongoCollection<Project> Projects => _database.GetCollection<Project>("projects");
    public IMongoCollection<Comment> Comments => _database.GetCollection<Comment>("comments");
    public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("notifications");
    public IMongoCollection<TaskHistory> TaskHistories => _database.GetCollection<TaskHistory>("taskHistories");

    public async Task SeedDataAsync()
    {
        // Check if admin user exists
        var adminExists = await Users.Find(u => u.Username == "admin").AnyAsync();
        
        if (!adminExists)
        {
            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@taskmanager.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FullName = "Administrador",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            await Users.InsertOneAsync(adminUser);

            // Create demo project
            var demoProject = new Project
            {
                Name = "Proyecto Demo",
                Description = "Proyecto de demostración para probar el sistema",
                Status = ProjectStatus.Active,
                OwnerId = adminUser.Id,
                Color = "#3B82F6",
                CreatedAt = DateTime.UtcNow
            };
            
            await Projects.InsertOneAsync(demoProject);

            // Create demo user
            var demoUser = new User
            {
                Username = "usuario",
                Email = "usuario@taskmanager.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("usuario123"),
                FullName = "Usuario Demo",
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            await Users.InsertOneAsync(demoUser);
        }

        // Create indexes
        await CreateIndexesAsync();
    }

    private async Task CreateIndexesAsync()
    {
        // Users indexes
        var userIndexKeys = Builders<User>.IndexKeys.Ascending(u => u.Username);
        await Users.Indexes.CreateOneAsync(new CreateIndexModel<User>(userIndexKeys, new CreateIndexOptions { Unique = true }));

        var emailIndexKeys = Builders<User>.IndexKeys.Ascending(u => u.Email);
        await Users.Indexes.CreateOneAsync(new CreateIndexModel<User>(emailIndexKeys, new CreateIndexOptions { Unique = true }));

        // Tasks indexes
        var taskStatusIndex = Builders<TaskItem>.IndexKeys.Ascending(t => t.Status);
        await Tasks.Indexes.CreateOneAsync(new CreateIndexModel<TaskItem>(taskStatusIndex));

        var taskProjectIndex = Builders<TaskItem>.IndexKeys.Ascending(t => t.ProjectId);
        await Tasks.Indexes.CreateOneAsync(new CreateIndexModel<TaskItem>(taskProjectIndex));

        var taskAssignedIndex = Builders<TaskItem>.IndexKeys.Ascending(t => t.AssignedToId);
        await Tasks.Indexes.CreateOneAsync(new CreateIndexModel<TaskItem>(taskAssignedIndex));

        // Comments indexes
        var commentTaskIndex = Builders<Comment>.IndexKeys.Ascending(c => c.TaskId);
        await Comments.Indexes.CreateOneAsync(new CreateIndexModel<Comment>(commentTaskIndex));

        // Notifications indexes
        var notificationUserIndex = Builders<Notification>.IndexKeys.Ascending(n => n.UserId);
        await Notifications.Indexes.CreateOneAsync(new CreateIndexModel<Notification>(notificationUserIndex));

        // Task history indexes
        var historyTaskIndex = Builders<TaskHistory>.IndexKeys.Ascending(h => h.TaskId);
        await TaskHistories.Indexes.CreateOneAsync(new CreateIndexModel<TaskHistory>(historyTaskIndex));
    }
}
