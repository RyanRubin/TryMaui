using Microsoft.Extensions.Configuration;
using System.Reflection;
using TryMaui.Dapper;
using TryMaui.DinkToPdf;
using TryMaui.Services;

namespace TryMaui;

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
            });

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<IDatabaseSyncService, SqlDataSyncService>();
        builder.Services.AddSingleton<IReportGenerationService, PdfGenerationService>();
        builder.Services.AddSingleton<IDialogsService, WindowsDialogsService>();

        var assembly = Assembly.GetExecutingAssembly();
        using (var stream = assembly.GetManifestResourceStream("TryMaui.appsettings.json"))
        {
            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
            builder.Configuration.AddConfiguration(config);
        }

        return builder.Build();
    }
}
