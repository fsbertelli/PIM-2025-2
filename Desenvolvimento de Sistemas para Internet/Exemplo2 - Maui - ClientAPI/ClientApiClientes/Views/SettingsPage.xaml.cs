using ClientApiClientes.Services;

namespace ClientApiClientes.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsService _settingsService;

	public SettingsPage(SettingsService settingsService)
	{
		InitializeComponent();
        _settingsService = settingsService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadSettings();
    }

    private void LoadSettings()
    {
        BaseUrlEntry.Text = _settingsService.BaseUrl;
        GetAllEntry.Text = _settingsService.GetAllEndpoint;
        GetByIdEntry.Text = _settingsService.GetByIdEndpoint;
        CreateEntry.Text = _settingsService.CreateEndpoint;
        UpdateEntry.Text = _settingsService.UpdateEndpoint;
        DeleteEntry.Text = _settingsService.DeleteEndpoint;
    }

    private async void SalvarButton_Clicked(object sender, System.EventArgs e)
    {
        _settingsService.BaseUrl = BaseUrlEntry.Text;
        _settingsService.GetAllEndpoint = GetAllEntry.Text;
        _settingsService.GetByIdEndpoint = GetByIdEntry.Text;
        _settingsService.CreateEndpoint = CreateEntry.Text;
        _settingsService.UpdateEndpoint = UpdateEntry.Text;
        _settingsService.DeleteEndpoint = DeleteEntry.Text;

        await DisplayAlert("Sucesso", "Configurações salvas com sucesso!", "OK");
        await Shell.Current.GoToAsync("..");
    }
}
