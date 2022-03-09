using TryMaui.Dapper;

namespace TryMaui;

public partial class MainPage : ContentPage
{
    private readonly IDatabaseSyncService databaseSyncService;


    int count = 0;

    public MainPage(IDatabaseSyncService databaseSyncService)
    {
        this.databaseSyncService = databaseSyncService;

        InitializeComponent();
    }

    private async void OnGoToGitHubUsersPageClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GitHubUsersPage());

        await databaseSyncService.ExecuteSync("LocalDatabase", "ServerDatabase");
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;
        CounterLabel.Text = $"Current count: {count}";

        SemanticScreenReader.Announce(CounterLabel.Text);
    }
}

