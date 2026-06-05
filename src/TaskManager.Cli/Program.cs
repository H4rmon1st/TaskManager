using TaskManager.Core;

namespace TaskManager.Cli;

public static class Program
{
    public static void Main()
    {
        var dataPath = GetDataFilePath();
        var repository = new JsonTaskRepository(dataPath);
        var service = new TaskService(repository);

        Console.WriteLine("=== Task Manager CLI ===");
        Console.WriteLine($"Data file: {dataPath}");
        Console.WriteLine($"Loaded {service.Count} task(s).");
        PrintHelp();

        while (true)
        {
            Console.Write("\n> ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            var parts = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLowerInvariant();
            var arg = parts.Length > 1 ? parts[1] : string.Empty;

            try
            {
                switch (cmd)
                {
                    case "add":
                        HandleAdd(service, arg);
                        break;
                    case "list":
                        HandleList(service);
                        break;
                    case "done":
                        HandleDone(service, arg);
                        break;
                    case "remove":
                        HandleRemove(service, arg);
                        break;
                    case "help":
                        PrintHelp();
                        break;
                    case "exit":
                    case "quit":
                        return;
                    default:
                        Console.WriteLine($"Unknown command: {cmd}. Type 'help'.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static string GetDataFilePath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var folder = Path.Combine(appData, "TaskManager");
        return Path.Combine(folder, "tasks.json");
    }

    private static void HandleAdd(TaskService service, string arg)
    {
        if (string.IsNullOrWhiteSpace(arg))
        {
            Console.WriteLine("Usage: add <title> [|priority]  e.g. add Buy milk |High");
            return;
        }

        var pieces = arg.Split('|', 2);
        var title = pieces[0].Trim();
        var priority = Priority.Medium;

        if (pieces.Length > 1 && Enum.TryParse<Priority>(pieces[1].Trim(), true, out var p))
        {
            priority = p;
        }

        var task = service.AddTask(title, priority);
        Console.WriteLine($"Added #{task.Id} [{task.Priority}] {task.Title}");
    }

    private static void HandleList(TaskService service)
    {
        var tasks = service.GetSortedByPriority();
        if (tasks.Count == 0)
        {
            Console.WriteLine("(no tasks)");
            return;
        }

        Console.WriteLine($"{"ID",-4} {"Status",-8} {"Priority",-10} Title");
        Console.WriteLine(new string('-', 50));
        foreach (var t in tasks)
        {
            var status = t.IsCompleted ? "DONE" : (t.IsOverdue ? "OVERDUE" : "OPEN");
            Console.WriteLine($"{t.Id,-4} {status,-8} {t.Priority,-10} {t.Title}");
        }
    }

    private static void HandleDone(TaskService service, string arg)
    {
        if (!int.TryParse(arg, out var id))
        {
            Console.WriteLine("Usage: done <id>");
            return;
        }

        Console.WriteLine(service.CompleteTask(id) ? $"Task #{id} marked complete." : $"Task #{id} not found.");
    }

    private static void HandleRemove(TaskService service, string arg)
    {
        if (!int.TryParse(arg, out var id))
        {
            Console.WriteLine("Usage: remove <id>");
            return;
        }

        Console.WriteLine(service.RemoveTask(id) ? $"Task #{id} removed." : $"Task #{id} not found.");
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Commands:");
        Console.WriteLine("  add <title> [|Priority]   - add a task (Priority: Low|Medium|High|Critical)");
        Console.WriteLine("  list                      - list tasks sorted by priority");
        Console.WriteLine("  done <id>                 - mark task complete");
        Console.WriteLine("  remove <id>               - delete task");
        Console.WriteLine("  help                      - show this help");
        Console.WriteLine("  exit                      - quit");
    }
}
