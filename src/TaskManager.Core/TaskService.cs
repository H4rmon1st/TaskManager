namespace TaskManager.Core;

public class TaskService
{
    private readonly ITaskRepository _repository;
    private readonly List<TodoTask> _tasks;
    private int _nextId;

    public TaskService() : this(new InMemoryTaskRepository())
    {
    }

    public TaskService(ITaskRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tasks = _repository.Load();
        _nextId = _tasks.Count == 0 ? 1 : _tasks.Max(t => t.Id) + 1;
    }

    public TodoTask AddTask(string title, Priority priority = Priority.Medium, string? description = null, DateTime? dueDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        }

        var task = new TodoTask
        {
            Id = _nextId++,
            Title = title.Trim(),
            Description = description,
            Priority = priority,
            DueDate = dueDate
        };

        _tasks.Add(task);
        Persist();
        return task;
    }

    public bool CompleteTask(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task is null)
        {
            return false;
        }

        task.IsCompleted = true;
        Persist();
        return true;
    }

    public bool RemoveTask(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task is null || !_tasks.Remove(task))
        {
            return false;
        }

        Persist();
        return true;
    }

    public IReadOnlyList<TodoTask> GetAll() => _tasks.AsReadOnly();

    public IReadOnlyList<TodoTask> GetPending() =>
        _tasks.Where(t => !t.IsCompleted).ToList().AsReadOnly();

    public IReadOnlyList<TodoTask> GetSortedByPriority()
    {
        return _tasks
            .OrderByDescending(t => (int)t.Priority)
            .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
            .ThenBy(t => t.CreatedAt)
            .ToList()
            .AsReadOnly();
    }

    public TodoTask? FindById(int id) => _tasks.FirstOrDefault(t => t.Id == id);

    public int Count => _tasks.Count;

    private void Persist() => _repository.Save(_tasks);
}
