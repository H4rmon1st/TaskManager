using TaskManager.Core;
using Xunit;

namespace TaskManager.Tests;

public class JsonTaskRepositoryTests : IDisposable
{
    private readonly string _tempFile;

    public JsonTaskRepositoryTests()
    {
        _tempFile = Path.Combine(Path.GetTempPath(), $"taskman-test-{Guid.NewGuid()}.json");
    }

    public void Dispose()
    {
        if (File.Exists(_tempFile))
        {
            File.Delete(_tempFile);
        }
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Load_WhenFileDoesNotExist_ShouldReturnEmptyList()
    {
        var repo = new JsonTaskRepository(_tempFile);
        var tasks = repo.Load();

        Assert.Empty(tasks);
    }

    [Fact]
    public void SaveThenLoad_ShouldRoundTripTasks()
    {
        var repo = new JsonTaskRepository(_tempFile);
        var original = new List<TodoTask>
        {
            new() { Id = 1, Title = "First", Priority = Priority.High },
            new() { Id = 2, Title = "Second", Priority = Priority.Low, IsCompleted = true }
        };

        repo.Save(original);
        var loaded = repo.Load();

        Assert.Equal(2, loaded.Count);
        Assert.Equal("First", loaded[0].Title);
        Assert.Equal(Priority.High, loaded[0].Priority);
        Assert.True(loaded[1].IsCompleted);
    }

    [Fact]
    public void Load_WhenFileIsCorrupted_ShouldReturnEmptyList()
    {
        File.WriteAllText(_tempFile, "{not valid json");
        var repo = new JsonTaskRepository(_tempFile);

        var tasks = repo.Load();

        Assert.Empty(tasks);
    }

    [Fact]
    public void TaskService_WithJsonRepository_ShouldPersistAcrossInstances()
    {
        var repo1 = new JsonTaskRepository(_tempFile);
        var service1 = new TaskService(repo1);
        service1.AddTask("Persisted task", Priority.High);
        service1.AddTask("Another task", Priority.Low);

        var repo2 = new JsonTaskRepository(_tempFile);
        var service2 = new TaskService(repo2);

        Assert.Equal(2, service2.Count);
        Assert.Equal("Persisted task", service2.FindById(1)!.Title);
        Assert.Equal(Priority.High, service2.FindById(1)!.Priority);
    }

    [Fact]
    public void TaskService_AfterReload_ShouldContinueIdSequence()
    {
        var repo1 = new JsonTaskRepository(_tempFile);
        var service1 = new TaskService(repo1);
        service1.AddTask("One");
        service1.AddTask("Two");

        var repo2 = new JsonTaskRepository(_tempFile);
        var service2 = new TaskService(repo2);
        var third = service2.AddTask("Three");

        Assert.Equal(3, third.Id);
    }

    [Fact]
    public void TaskService_CompleteAndReload_ShouldPersistCompletion()
    {
        var repo1 = new JsonTaskRepository(_tempFile);
        var service1 = new TaskService(repo1);
        var task = service1.AddTask("Do me");
        service1.CompleteTask(task.Id);

        var service2 = new TaskService(new JsonTaskRepository(_tempFile));

        Assert.True(service2.FindById(task.Id)!.IsCompleted);
    }
}
