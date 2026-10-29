namespace TaskHub.Services
{
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;
    using TaskHub.Models;

    public static class FileService
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { WriteIndented = true };

        public static async Task SaveToFileAsync(string filePath, TaskHub.Models.TaskItem[] tasks)
        {
            try
            {
                string json = JsonSerializer.Serialize(tasks, Options);
                await File.WriteAllTextAsync(filePath, json);
                Console.WriteLine("Saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save Error: {ex.Message}");
            }
        }

        public static async Task<List<TaskItem>> LoadFromFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return new List<TaskItem>();
                string json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load Error: {ex.Message}");
                return new List<TaskItem>();
            }
        }
    }
}