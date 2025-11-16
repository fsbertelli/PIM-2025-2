using App.Models;
using App.Services;

namespace App.Views;

[QueryProperty(nameof(UserId), "UserId")]
public partial class CreateUser : ContentPage
{
    private readonly UserService _userService;
    private int _userId;
    private User? _user;

    private List<Department> _departments = new();
    private List<UserStatus> _userStatuses = new();
    private List<UserProfile> _userProfiles = new();

    public int UserId
    {
        get => _userId;
        set
        {
            _userId = value;
            if (_userId > 0)
            {
                LoadUser();
            }
        }
    }
    public CreateUser() : this(ServiceHelper.GetService<UserService>()) { }

    public CreateUser(UserService userService)
    {
        InitializeComponent();
        _userService = userService;
        _user = new User();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPickersAsync();
        if (_userId > 0)
            LoadUser();
    }

    private async Task LoadPickersAsync()
    {
        _departments = await _userService.GetDepartmentsAsync();
        DepartmentPicker.ItemsSource = _departments;
        _userStatuses = await _userService.GetUserStatusesAsync();
        UserStatusPicker.ItemsSource = _userStatuses;
        _userProfiles = await _userService.GetUserProfilesAsync();
        UserProfilePicker.ItemsSource = _userProfiles;

        // Debug: mostrar perfis carregados
        var perfis = string.Join(", ", _userProfiles.Select(p => $"{p.Id}:{p.Name}"));
        await DisplayAlert("Perfis carregados", perfis.Length > 0 ? perfis : "Nenhum perfil carregado", "OK");

        // Forçar seleção do primeiro item se não estiver vazio
        if (_userProfiles.Count > 0 && UserProfilePicker.SelectedItem == null)
            UserProfilePicker.SelectedItem = _userProfiles[0];
    }

    private async void LoadUser()
    {
        _user = await _userService.GetUserAsync(_userId);
        if (_user != null)
        {
            NameEntry.Text = _user.Name;
            EmailEntry.Text = _user.Email;
            // Do not pre-fill password for security
            // PasswordEntry.Text = _user.Password;
            DepartmentPicker.SelectedItem = _departments.FirstOrDefault(d => d.Id == _user.Department?.Id);
            UserStatusPicker.SelectedItem = _userStatuses.FirstOrDefault(s => s.Id == _user.UserStatus?.Id);
            UserProfilePicker.SelectedItem = _userProfiles.FirstOrDefault(p => p.Id == _user.UserProfile?.Id);
        }
    }

    private async void SalvarButton_Clicked(object sender, System.EventArgs e)
    {
        var deptId = (DepartmentPicker.SelectedItem as Department)?.Id ?? 0;
        var statusId = (UserStatusPicker.SelectedItem as UserStatus)?.Id ?? 0;
        var profileId = (UserProfilePicker.SelectedItem as UserProfile)?.Id ?? 0;

        await DisplayAlert("Debug IDs", $"DeptId: {deptId}\nStatusId: {statusId}\nProfileId: {profileId}", "OK");

        if (string.IsNullOrWhiteSpace(NameEntry.Text) || string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Campos Obrigatórios", "Preencha Nome, Email e Senha.", "OK");
            return;
        }
        if (PasswordEntry.Text.Length < 6 || PasswordEntry.Text.Length > 100)
        {
            await DisplayAlert("Senha Inválida", "A senha deve ter entre 6 e 100 caracteres.", "OK");
            return;
        }
        if (!EmailEntry.Text.Contains("@") || !EmailEntry.Text.Contains("."))
        {
            await DisplayAlert("Email Inválido", "Digite um email válido.", "OK");
            return;
        }
        if (DepartmentPicker.SelectedItem == null || UserStatusPicker.SelectedItem == null || UserProfilePicker.SelectedItem == null || deptId <= 0 || statusId <= 0 || profileId <= 0)
        {
            await DisplayAlert("Seleção Inválida", "Selecione Departamento, Status e Perfil válidos.", "OK");
            return;
        }

        var dto = new CreateUserDto
        {
            Name = NameEntry.Text,
            Email = EmailEntry.Text,
            Password = PasswordEntry.Text,
            DeptId = deptId,
            UserStatusId = statusId,
            ProfileId = profileId
        };

        (bool success, string? errorMessage) result;
        if (_userId > 0)
            result = await _userService.UpdateUserAsync(_userId, dto);
        else
            result = await _userService.SaveUserAsync(dto);

        if (result.success)
        {
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await DisplayAlert("Erro ao salvar", $"{result.errorMessage}", "OK");
        }
    }

    private async void CancelarButton_Clicked(object sender, System.EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
