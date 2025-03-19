namespace TaskTracker.Models;

public class TaskItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
    public string Notes { get; set; } = string.Empty;
}