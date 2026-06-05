using System.Text.Json;

namespace TaskManager.Core;

public class JsonTaskRepository : ITaskRepository
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonTaskRepository(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be empty.", nameof(filePath));
        }

        _filePath = filePath;
    }

    public List<TodoTask> Load()
    {
        if (!File.Exists(_filePath))
        {
            return new List<TodoTask>();
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<TodoTask>();
            }

            var tasks = JsonSerializer.Deserialize<List<TodoTask>>(json, Options);
            return tasks ?? new List<TodoTask>();
        }
        catch (JsonException)
        {
            // Corrupted file — start fresh rather than crash.
            return new List<TodoTask>();
        }
    }

    public void Save(IEnumerable<TodoTask> tasks)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(tasks, Options);
        File.WriteAllText(_filePath, json);
    }
}
