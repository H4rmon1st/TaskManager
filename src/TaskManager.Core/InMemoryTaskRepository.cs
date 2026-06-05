namespace TaskManager.Core;

public class InMemoryTaskRepository : ITaskRepository
{
    private List<TodoTask> _store = new();

    public List<TodoTask> Load() => _store.Select(Clone).ToList();

    public void Save(IEnumerable<TodoTask> tasks) =>
        _store = tasks.Select(Clone).ToList();

    private static TodoTask Clone(TodoTask t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        Priority = t.Priority,
        IsCompleted = t.IsCompleted,
        CreatedAt = t.CreatedAt,
        DueDate = t.DueDate
    };
}
