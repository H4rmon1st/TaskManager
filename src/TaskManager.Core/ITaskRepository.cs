namespace TaskManager.Core;

public interface ITaskRepository
{
    List<TodoTask> Load();
    void Save(IEnumerable<TodoTask> tasks);
}
