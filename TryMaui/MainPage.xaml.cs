using TryMaui.Dapper;
using TryMaui.DinkToPdf;

namespace TryMaui;

public partial class MainPage : ContentPage
{
    private readonly IDatabaseSyncService databaseSyncService;
    private readonly IReportGenerationService reportGenerationService;

    int count = 0;

    public MainPage(IDatabaseSyncService databaseSyncService, IReportGenerationService reportGenerationService)
    {
        this.databaseSyncService = databaseSyncService;
        this.reportGenerationService = reportGenerationService;

        InitializeComponent();
    }

    private async void OnGitHubUsersClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GitHubUsersPage());
    }

    private async void OnLocalToServerSync_Clicked(object sender, EventArgs e)
    {
        await databaseSyncService.ExecuteSync("LocalDatabase", "ServerDatabase");
    }

    private async void OnServerToLocalSync_Clicked(object sender, EventArgs e)
    {
        await databaseSyncService.ExecuteSync("ServerDatabase", "LocalDatabase");
    }

    private void OnGenerateAndOpenPdf_Clicked(object sender, EventArgs e)
    {
        reportGenerationService.GenerateAndOpenReport("PersonReport", $"<title>Person Report - {DateTime.Now}</title>");
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;
        CounterLabel.Text = $"Current count: {count}";

        SemanticScreenReader.Announce(CounterLabel.Text);
    }
}

