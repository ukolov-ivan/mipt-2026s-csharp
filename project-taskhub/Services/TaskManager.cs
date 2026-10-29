namespace TaskHub.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TaskHub.Models;

    // Requirement: Делегаты (Delegates)
    public delegate void TaskOverdueHandler(TaskItem task);

    public class TaskManager : IDisposable
    {
        private List<TaskItem> _tasks = new List<TaskItem>();
        private int _nextId = 1;
        private readonly object _lock = new object();

        public event TaskOverdueHandler? TaskOverdue;

        public void AddTask(TaskItem task)
        {
            lock (_lock)
            {
                task.Id = _nextId++;
                _tasks.Add(task);
            }
        }

        public List<TaskItem> GetAllTasks()
        {
            lock (_lock)
            {
                return _tasks.ToList();
            }
        }

        // Requirement: Generic / Func
        public List<TaskItem> GetTasksByPredicate(Func<TaskItem, bool> predicate)
        {
            lock (_lock)
            {
                return _tasks.Where(predicate).ToList();
            }
        }

        public bool EditTask(int id, Action<TaskItem> editAction)
        {
            lock (_lock)
            {
                var task = _tasks.FirstOrDefault(t => t.Id == id);
                if (task == null) return false;
                editAction(task);
                return true;
            }
        }

        public bool DeleteTask(int id)
        {
            lock (_lock)
            {
                var task = _tasks.FirstOrDefault(t => t.Id == id);
                if (task == null) return false;
                return _tasks.Remove(task);
            }
        }

        // FIX: Return Dictionary instead of Tuple for Requirement compliance
        public Dictionary<string, int> GetStatistics()
        {
            lock (_lock)
            {
                var now = DateTime.Now;
                return new Dictionary<string, int>
                {
                    { "Total", _tasks.Count },
                    { "Completed", _tasks.Count(t => t.Status == Status.Done) },
                    { "Overdue", _tasks.Count(t => t.Deadline < now && t.Status != Status.Done) },
                    { "LowPriority", _tasks.Count(t => t.Priority == Priority.Low) },
                    { "MediumPriority", _tasks.Count(t => t.Priority == Priority.Medium) },
                    { "HighPriority", _tasks.Count(t => t.Priority == Priority.High) }
                };
            }
        }

        public void CheckOverdueTasks()
        {
            List<TaskItem> overdueList;
            lock (_lock)
            {
                var now = DateTime.Now;
                overdueList = _tasks.Where(t => t.Deadline < now && t.Status != Status.Done).ToList();
            }
            foreach (var task in overdueList)
            {
                TaskOverdue?.Invoke(task);
            }
        }

        public List<TaskItem> GetTasksForSerialization()
        {
            lock (_lock)
            {
                return _tasks.ToList();
            }
        }

        public void LoadTasks(List<TaskItem> tasks)
        {
            lock (_lock)
            {
                _tasks = tasks;
                if (_tasks.Any())
                {
                    _nextId = _tasks.Max(t => t.Id) + 1;
                }
                else
                {
                    _nextId = 1;
                }
            }
        }

        public void Dispose() { }

        // Requirement: Static Methods
        public static List<TaskItem> ValidateAndFixTasks(List<TaskItem> tasks)
        {
            if (tasks == null) return new List<TaskItem>();

            var validTasks = new List<TaskItem>();
            var usedIds = new HashSet<int>();

            foreach (var task in tasks)
            {
                if (task == null) continue;
                if (string.IsNullOrWhiteSpace(task.Name)) task.Name = "Unnamed Task";
                if (task.Description == null) task.Description = string.Empty;
                if (!Enum.IsDefined(typeof(Priority), task.Priority)) task.Priority = Models.Priority.Medium;
                if (!Enum.IsDefined(typeof(Models.Status), task.Status)) task.Status = Models.Status.New;
                if (task.Deadline == DateTime.MinValue || task.Deadline == DateTime.MaxValue) task.Deadline = DateTime.Now.AddDays(1);
                
                validTasks.Add(task);
            }
            
            validTasks.Sort((t1, t2) => t1.Id.CompareTo(t2.Id));
            return validTasks;
        }
    }
}