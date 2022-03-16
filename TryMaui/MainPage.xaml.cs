using TryMaui.Dapper;
using TryMaui.DinkToPdf;
using TryMaui.Services;

namespace TryMaui;

public partial class MainPage : ContentPage
{
    private readonly IDatabaseSyncService databaseSyncService;
    private readonly IReportGenerationService reportGenerationService;
    private readonly IDialogsService dialogsService;

    int count = 0;

    public MainPage(IDatabaseSyncService databaseSyncService, IReportGenerationService reportGenerationService, IDialogsService dialogsService)
    {
        this.databaseSyncService = databaseSyncService;
        this.reportGenerationService = reportGenerationService;
        this.dialogsService = dialogsService;

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
        reportGenerationService.GenerateAndOpenReport("PersonReport", @"
            <firstName>John</firstName>
            <lastName>Doe</lastName>
            <gender>Male</gender>
            <streetAddress>123 Some Street</streetAddress>
            <city>Some City</city>
            <state>Some State</state>
            <zip>123456</zip>
        ");
    }

    private void OnShowSaveFileDialog_Clicked(object sender, EventArgs e)
    {
        string ret = dialogsService.ShowSaveFileDialog("Report.pdf", "PDF Files (*.pdf)|*.pdf");

        DialogBoxCommandID result = dialogsService.ShowMessageBox("Test MB_OK", "title", MessageBoxCheckFlags.MB_OK);

        result = dialogsService.ShowMessageBox("Test MB_OKCANCEL", "title", MessageBoxCheckFlags.MB_OKCANCEL);

        result = dialogsService.ShowMessageBox("Test MB_YESNO", "title", MessageBoxCheckFlags.MB_YESNO);
    }

    private void OnShowOpenFileDialog_Clicked(object sender, EventArgs e)
    {
        string ret = dialogsService.ShowOpenFileDialog("All Files (*.*)|*.*");
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;
        CounterLabel.Text = $"Current count: {count}";

        SemanticScreenReader.Announce(CounterLabel.Text);
    }
}

