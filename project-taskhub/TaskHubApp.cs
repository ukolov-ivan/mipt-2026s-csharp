using System;
using TaskHub.Models;
using TaskHub.Services;
using TaskHub.UI;

namespace TaskHub
{
    class App
    {
        static TaskManager _taskManager = new TaskManager();
        static OverdueMonitor? _overdueMonitor;
        // This file is probably stored besides an executable
        static readonly string _dataFilename = ".tasks.json";

        static async Task Main()
        {
            Console.WriteLine("Welcome to TaskHub!");
            
            // Setup background monitor + event handler
            _overdueMonitor = new OverdueMonitor(_taskManager, 5);
            _taskManager.TaskOverdue += task => 
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n[BACKGROUND ALERT] Task #{task.Id} '{task.Name}' is overdue! Deadline: {task.Deadline:HH:mm}");
                Console.ResetColor();
            };

            bool running = true;
            while (running)
            {
                Console.Clear();

                var overdueTasks = _taskManager.GetTasksByPredicate(t => t.Deadline < DateTime.Now && t.Status != Status.Done);
                if (overdueTasks.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("=== OVERDUE TASKS ===");
                    foreach (var task in overdueTasks)
                    {
                        Console.WriteLine($"[ALERT] ID: {task.Id} | {task.Name}");
                    }
                    Console.ResetColor();
                    Console.WriteLine();
                }

                // Load initial data if empty
                if (!_taskManager.GetAllTasks().Any())
                {
                    await LoadTasksAsync();
                }

                TUI.ShowMenu();
                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        await TUI.CreateTaskAsync(_taskManager);
                        break;
                    case "2": TUI.ShowTasks(_taskManager.GetAllTasks(), "All Tasks"); break;
                    case "3": TUI.ShowTasks(_taskManager.GetTasksByPredicate(t => t.Status == Status.Done), "Completed Tasks"); break;
                    case "4": TUI.ShowTasks(_taskManager.GetTasksByPredicate(t => t.Status != Status.Done), "Uncompleted Tasks"); break;
                    case "5": TUI.ShowTasks(_taskManager.GetTasksByPredicate(t => t.Priority == Priority.High), "High Priority Tasks"); break;
                    case "6": await TUI.EditTaskAsync(_taskManager); break;
                    case "7": await TUI.DeleteTaskAsync(_taskManager); break;
                    case "8": await TUI.SearchTasksAsync(_taskManager); break;
                    case "9": TUI.ShowStatistics(_taskManager.GetStatistics()); break;
                    case "10": await TUI.SaveTasksAsync(_taskManager, _dataFilename); break;
                    case "11": await LoadTasksAsync(); break;
                    case "0":
                        running = false;
                        break;
                    default: Console.WriteLine("Invalid choice."); Console.ReadKey(); break;
                }
            }

            // Cleanup
            _overdueMonitor?.Dispose();
            _taskManager.Dispose();
            Console.WriteLine("Goodbye!");
        }

        static async Task LoadTasksAsync()
        {
            var tasks = await FileService.LoadFromFileAsync(_dataFilename);
            var validTasks = TaskManager.ValidateAndFixTasks(tasks);
            _taskManager.LoadTasks(validTasks);
            Console.WriteLine($"Loaded {validTasks.Count} tasks from file.");
            Console.ReadKey();
        }
    }
}