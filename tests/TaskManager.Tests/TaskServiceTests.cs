using TaskManager.Core;
using Xunit;

namespace TaskManager.Tests;

public class TaskServiceTests
{
    [Fact]
    public void AddTask_ShouldAssignIncrementingIds()
    {
        var service = new TaskService();
        var t1 = service.AddTask("First");
        var t2 = service.AddTask("Second");

        Assert.Equal(1, t1.Id);
        Assert.Equal(2, t2.Id);
        Assert.Equal(2, service.Count);
    }

    [Fact]
    public void AddTask_ShouldDefaultToMediumPriority()
    {
        var service = new TaskService();
        var t = service.AddTask("Sample");

        Assert.Equal(Priority.Medium, t.Priority);
        Assert.False(t.IsCompleted);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void AddTask_WithEmptyTitle_ShouldThrow(string? title)
    {
        var service = new TaskService();
        Assert.Throws<ArgumentException>(() => service.AddTask(title!));
    }

    [Fact]
    public void CompleteTask_ShouldMarkTaskAsCompleted()
    {
        var service = new TaskService();
        var t = service.AddTask("Do laundry");

        var result = service.CompleteTask(t.Id);

        Assert.True(result);
        Assert.True(service.FindById(t.Id)!.IsCompleted);
    }

    [Fact]
    public void CompleteTask_WithInvalidId_ShouldReturnFalse()
    {
        var service = new TaskService();
        Assert.False(service.CompleteTask(999));
    }

    [Fact]
    public void RemoveTask_ShouldDeleteTask()
    {
        var service = new TaskService();
        var t = service.AddTask("Remove me");

        Assert.True(service.RemoveTask(t.Id));
        Assert.Null(service.FindById(t.Id));
    }

    [Fact]
    public void GetPending_ShouldExcludeCompletedTasks()
    {
        var service = new TaskService();
        var t1 = service.AddTask("One");
        service.AddTask("Two");
        service.CompleteTask(t1.Id);

        var pending = service.GetPending();

        Assert.Single(pending);
        Assert.Equal("Two", pending[0].Title);
    }

    [Fact]
    public void GetSortedByPriority_ShouldOrderCriticalFirst()
    {
        var service = new TaskService();
        service.AddTask("Low task", Priority.Low);
        service.AddTask("Critical task", Priority.Critical);
        service.AddTask("Medium task", Priority.Medium);
        service.AddTask("High task", Priority.High);

        var sorted = service.GetSortedByPriority();

        Assert.Equal(Priority.Critical, sorted[0].Priority);
        Assert.Equal(Priority.High, sorted[1].Priority);
        Assert.Equal(Priority.Medium, sorted[2].Priority);
        Assert.Equal(Priority.Low, sorted[3].Priority);
    }

    [Fact]
    public void GetSortedByPriority_SamePriority_ShouldOrderByDueDate()
    {
        var service = new TaskService();
        var later = service.AddTask("Later", Priority.High, dueDate: DateTime.UtcNow.AddDays(5));
        var sooner = service.AddTask("Sooner", Priority.High, dueDate: DateTime.UtcNow.AddDays(1));

        var sorted = service.GetSortedByPriority();

        Assert.Equal(sooner.Id, sorted[0].Id);
        Assert.Equal(later.Id, sorted[1].Id);
    }

    [Fact]
    public void IsOverdue_WhenDueDateInPastAndNotCompleted_ShouldBeTrue()
    {
        var service = new TaskService();
        var t = service.AddTask("Overdue", Priority.Medium, dueDate: DateTime.UtcNow.AddDays(-1));

        Assert.True(t.IsOverdue);
    }

    [Fact]
    public void IsOverdue_WhenCompleted_ShouldBeFalse()
    {
        var service = new TaskService();
        var t = service.AddTask("Overdue but done", Priority.Medium, dueDate: DateTime.UtcNow.AddDays(-1));
        service.CompleteTask(t.Id);

        Assert.False(service.FindById(t.Id)!.IsOverdue);
    }

    [Fact]
    public void AddTask_ShouldTrimTitle()
    {
        var service = new TaskService();
        var t = service.AddTask("   Trimmed   ");
        Assert.Equal("Trimmed", t.Title);
    }
}
