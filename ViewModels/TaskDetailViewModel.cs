using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using TaskTracker.Models;
using TaskTracker.Services;

namespace TaskTracker.ViewModels;

public partial class TaskDetailViewModel : ObservableObject
{
    private readonly TaskService _taskService;

    [ObservableProperty]
    private TaskItem task = new();

    public TaskDetailViewModel(TaskService taskService)
    {
        _taskService = taskService;
        Debug.WriteLine("[TaskDetailViewModel] ViewModel initialized");
    }

    // Track when Task property is set
    partial void OnTaskChanged(TaskItem value)
    {
        Debug.WriteLine($"[TaskDetailViewModel] Task property changed to: ID={value?.Id}, Name={value?.Name}");
    }

    [RelayCommand]
    async Task SaveTask()
    {
        Debug.WriteLine($"[TaskDetailViewModel] SaveTask called for task: ID={Task?.Id}, Name={Task?.Name}");

        if (Task == null)
        {
            Debug.WriteLine("[TaskDetailViewModel] ERROR: Task is null");
            await Shell.Current.DisplayAlert("Error", "Cannot save null task", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Task.Id))
        {
            Debug.WriteLine("[TaskDetailViewModel] WARNING: Task has empty ID, generating new ID");
            Task.Id = Guid.NewGuid().ToString();
        }

        if (string.IsNullOrWhiteSpace(Task.Name))
        {
            Debug.WriteLine("[TaskDetailViewModel] ERROR: Task name is empty");
            await Shell.Current.DisplayAlert("Error", "Task name cannot be empty", "OK");
            return;
        }

        try
        {
            Debug.WriteLine("[TaskDetailViewModel] Loading existing tasks");
            var tasks = await _taskService.GetTasksAsync();
            Debug.WriteLine($"[TaskDetailViewModel] Loaded {tasks.Count} existing tasks");

            var existingTask = tasks.FirstOrDefault(t => t.Id == Task.Id);
            if (existingTask != null)
            {
                Debug.WriteLine($"[TaskDetailViewModel] Found existing task with ID={existingTask.Id}, Name={existingTask.Name}");
                tasks.Remove(existingTask); // Update existing task
            }
            else
            {
                Debug.WriteLine($"[TaskDetailViewModel] No existing task found with ID={Task.Id}, adding as new");
            }

            // Create a deep copy of the task to avoid reference issues
            var taskToSave = new TaskItem
            {
                Id = Task.Id,
                Name = Task.Name,
                IsComplete = Task.IsComplete,
                Notes = Task.Notes
            };

            tasks.Add(taskToSave);
            Debug.WriteLine($"[TaskDetailViewModel] Added task to collection, now saving {tasks.Count} tasks");

            await _taskService.SaveTasksAsync(tasks);
            Debug.WriteLine("[TaskDetailViewModel] Tasks saved successfully");

            Debug.WriteLine("[TaskDetailViewModel] Navigating back");
            await Shell.Current.GoToAsync(".."); // Navigate back
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[TaskDetailViewModel] ERROR saving task: {ex.Message}");
            Debug.WriteLine($"[TaskDetailViewModel] Exception details: {ex}");
            await Shell.Current.DisplayAlert("Error", $"Failed to save task: {ex.Message}", "OK");
        }
    }
}