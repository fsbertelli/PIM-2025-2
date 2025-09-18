using App.Services;

namespace App.Views;

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
        // Departamentos
        DeptGetAllEntry.Text = _settingsService.DeptGetAllEndpoint;
        DeptGetByIdEntry.Text = _settingsService.DeptGetByIdEndpoint;
        DeptCreateEntry.Text = _settingsService.DeptCreateEndpoint;
        DeptUpdateEntry.Text = _settingsService.DeptUpdateEndpoint;
        DeptDeleteEntry.Text = _settingsService.DeptDeleteEndpoint;
        // Usuários
        UserCreateEntry.Text = _settingsService.UserCreateEndpoint;
        UserUpdateEntry.Text = _settingsService.UserUpdateEndpoint;
        UserDeleteEntry.Text = _settingsService.UserDeleteEndpoint;
        // User DTO
        UserDtoGetAllEntry.Text = _settingsService.UserDtoGetAllEndpoint;
        UserDtoGetByIdEntry.Text = _settingsService.UserDtoGetByIdEndpoint;
        // Perfis de Usuário
        UserProfileGetAllEntry.Text = _settingsService.UserProfileGetAllEndpoint;
        UserProfileGetByIdEntry.Text = _settingsService.UserProfileGetByIdEndpoint;
        UserProfileCreateEntry.Text = _settingsService.UserProfileCreateEndpoint;
        UserProfileUpdateEntry.Text = _settingsService.UserProfileUpdateEndpoint;
        UserProfileDeleteEntry.Text = _settingsService.UserProfileDeleteEndpoint;
        // Status de Usuário
        UserStatusGetAllEntry.Text = _settingsService.UserStatusGetAllEndpoint;
        UserStatusGetByIdEntry.Text = _settingsService.UserStatusGetByIdEndpoint;
        UserStatusCreateEntry.Text = _settingsService.UserStatusCreateEndpoint;
        UserStatusUpdateEntry.Text = _settingsService.UserStatusUpdateEndpoint;
        UserStatusDeleteEntry.Text = _settingsService.UserStatusDeleteEndpoint;
    }

    private async void SalvarButton_Clicked(object sender, System.EventArgs e)
    {
        _settingsService.BaseUrl = BaseUrlEntry.Text;
        // Departamentos
        _settingsService.DeptGetAllEndpoint = DeptGetAllEntry.Text;
        _settingsService.DeptGetByIdEndpoint = DeptGetByIdEntry.Text;
        _settingsService.DeptCreateEndpoint = DeptCreateEntry.Text;
        _settingsService.DeptUpdateEndpoint = DeptUpdateEntry.Text;
        _settingsService.DeptDeleteEndpoint = DeptDeleteEntry.Text;
        // Usuários
        _settingsService.UserCreateEndpoint = UserCreateEntry.Text;
        _settingsService.UserUpdateEndpoint = UserUpdateEntry.Text;
        _settingsService.UserDeleteEndpoint = UserDeleteEntry.Text;
        // User DTO
        _settingsService.UserDtoGetAllEndpoint = UserDtoGetAllEntry.Text;
        _settingsService.UserDtoGetByIdEndpoint = UserDtoGetByIdEntry.Text;
        // Perfis de Usuário
        _settingsService.UserProfileGetAllEndpoint = UserProfileGetAllEntry.Text;
        _settingsService.UserProfileGetByIdEndpoint = UserProfileGetByIdEntry.Text;
        _settingsService.UserProfileCreateEndpoint = UserProfileCreateEntry.Text;
        _settingsService.UserProfileUpdateEndpoint = UserProfileUpdateEntry.Text;
        _settingsService.UserProfileDeleteEndpoint = UserProfileDeleteEntry.Text;
        // Status de Usuário
        _settingsService.UserStatusGetAllEndpoint = UserStatusGetAllEntry.Text;
        _settingsService.UserStatusGetByIdEndpoint = UserStatusGetByIdEntry.Text;
        _settingsService.UserStatusCreateEndpoint = UserStatusCreateEntry.Text;
        _settingsService.UserStatusUpdateEndpoint = UserStatusUpdateEntry.Text;
        _settingsService.UserStatusDeleteEndpoint = UserStatusDeleteEntry.Text;

        await DisplayAlert("Sucesso", "Configurações salvas com sucesso!", "OK");
        await Shell.Current.GoToAsync("..");
    }
}
