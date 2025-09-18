using System.Collections.ObjectModel;
using App.Models;
using App.Services;

namespace App.Views;

public partial class UserPage : ContentPage
{
    private readonly UserService _userService;

    public UserPage(UserService userService)
    {
        InitializeComponent();
        _userService = userService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await DisplayAlert("Debug", "OnAppearing chamado", "OK");
        await LoadUsers();
    }

    private async Task LoadUsers()
    {
        try
        {
            var users = await _userService.GetUsersAsync();
            var ids = string.Join(", ", users.Select(u => u.Id));
            await DisplayAlert("Diagnóstico", $"Usuários retornados: {users.Count}\nIDs: {ids}", "OK");
            BindableLayout.SetItemsSource(UsersList, new ObservableCollection<User>(users));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }



    private async void AdicionarButton_Clicked(object sender, System.EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CreateUser));
    }

    private User? GetUserFromSender(object sender)
    {
        if (sender is Button button && button.CommandParameter is User user)
        {
            return user;
        }
        return null;
    }

    private async void EditButton_Clicked_Item(object sender, System.EventArgs e)
    {
        var user = GetUserFromSender(sender);
        if (user != null)
        {
            var navigationParameters = new Dictionary<string, object>
            {
                { "UserId", user.Id }
            };
            await Shell.Current.GoToAsync(nameof(CreateUser), navigationParameters);
        }
    }

    private async void DeleteButton_Clicked_Item(object sender, System.EventArgs e)
    {
        var user = GetUserFromSender(sender);
        if (user != null)
        {
            bool confirm = await DisplayAlert("Confirmar Exclusão", $"Você tem certeza que deseja excluir o usuário '{user.Name}'?", "Sim", "Não");
            if (confirm)
            {
                await _userService.DeleteUserAsync(user.Id);
                await LoadUsers();
            }
        }
    }

    private async void SettingsButton_Clicked(object sender, System.EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }
}
