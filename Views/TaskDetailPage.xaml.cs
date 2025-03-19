using System.Diagnostics;
using TaskTracker.Models;
using TaskTracker.ViewModels;

namespace TaskTracker.Views;

public partial class TaskDetailPage : ContentPage
{
    public TaskDetailPage(TaskDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        Debug.WriteLine("[TaskDetailPage] Page initialized with ViewModel");
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        Debug.WriteLine("[TaskDetailPage] OnNavigatedTo called");

        try
        {
            var navParam = NavigationParameter;
            Debug.WriteLine($"[TaskDetailPage] Navigation parameter type: {navParam?.GetType().Name ?? "null"}");

            if (navParam == null)
            {
                Debug.WriteLine("[TaskDetailPage] WARNING: Navigation parameter is null");
                return;
            }

            if (navParam is Dictionary<string, object> dict)
            {
                Debug.WriteLine($"[TaskDetailPage] Navigation dictionary contains {dict.Count} items");
                foreach (var key in dict.Keys)
                {
                    Debug.WriteLine($"[TaskDetailPage] Dictionary key: {key}, Value type: {dict[key]?.GetType().Name ?? "null"}");
                }

                if (dict.TryGetValue("Task", out var taskObj))
                {
                    if (taskObj is TaskItem task)
                    {
                        Debug.WriteLine($"[TaskDetailPage] Found task in dictionary with ID: {task.Id}, Name: {task.Name}");
                        ((TaskDetailViewModel)BindingContext).Task = task;
                    }
                    else
                    {
                        Debug.WriteLine($"[TaskDetailPage] WARNING: Task object is not of type TaskItem but {taskObj?.GetType().Name ?? "null"}");
                    }
                }
                else
                {
                    Debug.WriteLine("[TaskDetailPage] WARNING: No 'Task' key found in navigation dictionary");
                }
            }
            else if (navParam is TaskItem task)
            {
                Debug.WriteLine($"[TaskDetailPage] Found task directly as parameter with ID: {task.Id}, Name: {task.Name}");
                ((TaskDetailViewModel)BindingContext).Task = task;
            }
            else
            {
                Debug.WriteLine($"[TaskDetailPage] WARNING: Navigation parameter is not a Dictionary or TaskItem but {navParam.GetType().Name}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[TaskDetailPage] ERROR processing navigation parameter: {ex.Message}");
            Debug.WriteLine($"[TaskDetailPage] Exception details: {ex}");
        }
    }

    private object? NavigationParameter
    {
        get
        {
            try
            {
                var navStack = Navigation.NavigationStack;
                Debug.WriteLine($"[TaskDetailPage] Navigation stack has {navStack.Count} items");

                if (navStack.Count == 0)
                {
                    Debug.WriteLine("[TaskDetailPage] WARNING: Navigation stack is empty");
                    return null;
                }

                var lastPage = navStack.Last();
                Debug.WriteLine($"[TaskDetailPage] Last page in stack: {lastPage.GetType().Name}");

                var paramsProp = lastPage.GetType().GetProperty("Parameters");
                if (paramsProp == null)
                {
                    Debug.WriteLine("[TaskDetailPage] WARNING: Parameters property not found on last page");
                    return null;
                }

                var paramsValue = paramsProp.GetValue(lastPage);
                Debug.WriteLine($"[TaskDetailPage] Parameters value type: {paramsValue?.GetType().Name ?? "null"}");

                return paramsValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TaskDetailPage] ERROR getting navigation parameter: {ex.Message}");
                Debug.WriteLine($"[TaskDetailPage] Exception details: {ex}");
                return null;
            }
        }
    }
}
