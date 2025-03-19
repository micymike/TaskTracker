using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using TaskTracker.Models;

namespace TaskTracker.Services;

public class TaskService
{
    private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "tasks.json");

    public async Task<List<TaskItem>> GetTasksAsync()
    {
        try
        {
            Debug.WriteLine($"[TaskService] Attempting to load tasks from: {_filePath}");

            if (!File.Exists(_filePath))
            {
                Debug.WriteLine("[TaskService] Tasks file does not exist, returning empty list");
                return new List<TaskItem>();
            }

            var json = await File.ReadAllTextAsync(_filePath);
            Debug.WriteLine($"[TaskService] Read JSON data: {json}");

            var tasks = JsonConvert.DeserializeObject<List<TaskItem>>(json) ?? new List<TaskItem>();
            Debug.WriteLine($"[TaskService] Deserialized {tasks.Count} tasks");

            // Validate task IDs for uniqueness
            var taskIds = new HashSet<string>();
            var duplicateIds = new List<string>();

            foreach (var task in tasks)
            {
                if (!taskIds.Add(task.Id))
                {
                    duplicateIds.Add(task.Id);
                }
            }

            if (duplicateIds.Count > 0)
            {
                Debug.WriteLine($"[TaskService] WARNING: Found {duplicateIds.Count} duplicate task IDs: {string.Join(", ", duplicateIds)}");
            }

            return tasks;
        }
        catch (JsonException jsonEx)
        {
            Debug.WriteLine($"[TaskService] JSON deserialization error: {jsonEx.Message}");
            Debug.WriteLine($"[TaskService] JSON error details: {jsonEx}");
            // Create a backup of the corrupted file for analysis
            if (File.Exists(_filePath))
            {
                var backupPath = $"{_filePath}.corrupted";
                File.Copy(_filePath, backupPath, true);
                Debug.WriteLine($"[TaskService] Created backup of corrupted file at: {backupPath}");
            }
            return new List<TaskItem>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[TaskService] Error loading tasks: {ex.Message}");
            Debug.WriteLine($"[TaskService] Exception details: {ex}");
            return new List<TaskItem>();
        }
    }

    public async Task SaveTasksAsync(List<TaskItem> tasks)
    {
        try
        {
            Debug.WriteLine($"[TaskService] Saving {tasks.Count} tasks to: {_filePath}");

            // Check for duplicate IDs before saving
            var taskIds = new HashSet<string>();
            var duplicateIds = new List<string>();

            foreach (var task in tasks)
            {
                if (!taskIds.Add(task.Id))
                {
                    duplicateIds.Add(task.Id);
                }
            }

            if (duplicateIds.Count > 0)
            {
                Debug.WriteLine($"[TaskService] WARNING: Found {duplicateIds.Count} duplicate task IDs before saving: {string.Join(", ", duplicateIds)}");
            }

            var json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
            Debug.WriteLine($"[TaskService] Serialized JSON data: {json}");

            // Ensure directory exists
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory) && directory != null)
            {
                Directory.CreateDirectory(directory);
                Debug.WriteLine($"[TaskService] Created directory: {directory}");
            }

            await File.WriteAllTextAsync(_filePath, json);
            Debug.WriteLine($"[TaskService] Successfully saved tasks to file");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[TaskService] Error saving tasks: {ex.Message}");
            Debug.WriteLine($"[TaskService] Exception details: {ex}");
        }
    }
}
