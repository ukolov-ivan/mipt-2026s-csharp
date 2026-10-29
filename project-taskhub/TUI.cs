namespace TaskHub.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TaskHub.Models;
    using TaskHub.Services;

    public static class TUI
    {
        public static void ShowMenu()
        {
            Console.WriteLine("=== TASK HUB ===");
            Console.WriteLine("1. Create task | 2. View all | 3. View completed | 4. View uncompleted");
            Console.WriteLine("5. View high Priority | 6. Edit task | 7. Delete task | 8. Search tasks");
            Console.WriteLine("9. Stats | 10. Save | 11. Load | 0. Exit");
            Console.Write("Choice: ");
        }

        public static async Task CreateTaskAsync(TaskManager manager)
        {
            Console.Write("Name: ");
            var name = Console.ReadLine() ?? "Unnamed Task";
            
            Console.Write("Description: ");
            var desc = Console.ReadLine() ?? string.Empty;
            
            Priority priority;
            while (true)
            {
                Console.Write("Priority (Low/Medium/High): ");
                if (Enum.TryParse(Console.ReadLine(), true, out priority)) break;
                Console.WriteLine("Invalid priority. Please try again.");
            }
            
            DateTime deadline;
            while (true)
            {
                Console.Write("Deadline (yyyy-MM-dd HH:mm) [Leave empty for 24h from now]: ");
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    deadline = DateTime.Now.AddDays(1);
                    break;
                }
                
                if (DateTime.TryParse(input, out deadline))
                {
                    break;
                }

                Console.WriteLine("Invalid date format. Please use yyyy-MM-dd HH:mm.");
            }

            Status status;
            while (true)
            {
                Console.Write("Status (New/InProgress/Done): ");
                if (Enum.TryParse(Console.ReadLine(), true, out status)) break;
                Console.WriteLine("Invalid status. Please try again.");
            }

            var task = new TaskItem
            {
                Name = name,
                Description = desc,
                Priority = priority,
                Deadline = deadline,
                Status = status
            };

            manager.AddTask(task);
            Console.WriteLine("Task created successfully! Press any key to continue.");
            Console.ReadKey();
        }
        public static async Task EditTaskAsync(TaskManager manager)
        {
            Console.Write("ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            manager.EditTask(id, task =>
            {
                Console.Write($"New Name [{task.Name}]: ");
                string? n = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(n)) task.Name = n;

                Console.Write($"New Description [{task.Description}]: ");
                string? d = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(d)) task.Description = d;
            });
            Console.WriteLine("Updated!");
            Console.ReadKey();
        }

        public static async Task DeleteTaskAsync(TaskManager manager)
        {
            Console.Write("Id: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                if (manager.DeleteTask(id)) Console.WriteLine("Deleted!");
            }
            Console.ReadKey();
        }

        public static async Task SearchTasksAsync(TaskManager manager)
        {
            Console.Clear();
            Console.WriteLine("Search Tasks by");
            Console.WriteLine("1 - Name");
            Console.WriteLine("2 - Status");
            Console.WriteLine("3 - Priority");
            Console.Write("Search by: ");
            
            var option = Console.ReadLine()?.Trim();
            List<TaskItem> results = new List<TaskItem>();

            switch (option)
            {
                case "1":
                    Console.Write("Name keyword: ");
                    var nameInput = Console.ReadLine() ?? string.Empty;
                    results = manager.GetTasksByPredicate(t => 
                        t.Name.Contains(nameInput, StringComparison.OrdinalIgnoreCase));
                    break;

                case "2":
                    Console.Write("Status (New/InProgress/Done): ");
                    if (Enum.TryParse(Console.ReadLine(), true, out Status s))
                    {
                        results = manager.GetTasksByPredicate(t => t.Status == s);
                    }
                    else
                    {
                        Console.WriteLine("Invalid status entered.");
                        Console.ReadKey();
                        return;
                    }
                    break;

                case "3":
                    Console.Write("Priority (Low/Medium/High): ");
                    if (Enum.TryParse(Console.ReadLine(), true, out Priority p))
                    {
                        results = manager.GetTasksByPredicate(t => t.Priority == p);
                    }
                    else
                    {
                        Console.WriteLine("Invalid priority entered.");
                        Console.ReadKey();
                        return;
                    }
                    break;

                default:
                    Console.WriteLine("Invalid search option selected.");
                    Console.ReadKey();
                    return;
            }

            ShowTasks(results, "Search Results");
        }

        public static void ShowTasks(List<TaskItem> list, string title)
        {
            Console.WriteLine($"--- {title} ---");
            if (!list.Any()) { Console.WriteLine("No tasks."); }
            else { foreach(var t in list) Console.WriteLine(t); }
            Console.ReadKey();
        }

        public static void ShowStatistics(Dictionary<string, int> stats)
        {
            Console.Clear();
            Console.WriteLine("--- Stats ---");
            foreach(var item in stats)
            {
                Console.WriteLine($"{item.Key,-15}: {item.Value}");
            }
        }

        public static async Task SaveTasksAsync(TaskManager manager, string path)
        {
            var tasks = manager.GetAllTasks().ToArray();
            await FileService.SaveToFileAsync(path, tasks);
            Console.ReadKey();
        }
    }
}
