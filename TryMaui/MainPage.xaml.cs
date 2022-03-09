using TryMaui.Dapper;

namespace TryMaui;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnGoToGitHubUsersPageClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GitHubUsersPage());

        var syncService = new DatabaseSyncService(DatabaseSyncService.LocalDatabaseConnectionString, DatabaseSyncService.ServerDatabaseConnectionString);
        await syncService.ExecuteSync();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;
        CounterLabel.Text = $"Current count: {count}";

        SemanticScreenReader.Announce(CounterLabel.Text);
    }
}

