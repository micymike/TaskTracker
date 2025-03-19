using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskTracker.Models;
using TaskTracker.Services;
using TaskTracker.Views;

namespace TaskTracker.ViewModels;

public partial class TaskListViewModel : ObservableObject
{
    private readonly TaskService _taskService;
    private readonly WeatherService _weatherService;

    [ObservableProperty]
    private List<TaskItem> tasks = new();

    [ObservableProperty]
    private string weatherInfo = string.Empty;

    public TaskListViewModel(TaskService taskService, WeatherService weatherService)
    {
        _taskService = taskService;
        _weatherService = weatherService;
        LoadDataAsync();
    }

    private async void LoadDataAsync()
    {
        Tasks = await _taskService.GetTasksAsync();
        WeatherInfo = await _weatherService.GetWeatherAsync("London"); // Change city if desired
    }

    [RelayCommand]
    async Task AddTask()
    {
        await Shell.Current.GoToAsync(nameof(TaskDetailPage), true, new Dictionary<string, object>
        {
            { "Task", new TaskItem() }
        });
    }

    [RelayCommand]
    async Task EditTask(TaskItem task)
    {
        await Shell.Current.GoToAsync(nameof(TaskDetailPage), true, new Dictionary<string, object>
        {
            { "Task", task }
        });
    }

    [RelayCommand]
    async Task DeleteTask(TaskItem task)
    {
        Tasks.Remove(task);
        await _taskService.SaveTasksAsync(Tasks);
    }
}