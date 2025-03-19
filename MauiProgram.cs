using Microsoft.Extensions.Logging;
using TaskTracker.Services;
using TaskTracker.ViewModels;
using TaskTracker.Views;

namespace TaskTracker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<TaskService>();
        builder.Services.AddSingleton<WeatherService>();
        builder.Services.AddTransient<TaskListViewModel>();
        builder.Services.AddTransient<TaskListPage>();
        builder.Services.AddTransient<TaskDetailViewModel>();
        builder.Services.AddTransient<TaskDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}