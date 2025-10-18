using System.Text;
using System.Text.Json;

namespace App.Views;

public partial class LoginPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public LoginPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5185") };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var remember = Preferences.Get("remember_me", false);
        RememberCheckBox.IsChecked = remember;

        if (remember)
        {
            var savedEmail = Preferences.Get("saved_email", string.Empty);
            if (string.IsNullOrEmpty(savedEmail)) return;
            EmailEntry.Text = savedEmail;
            PasswordEntry.Focus();
        }
        else
        {
            EmailEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // Hide previous error
        ErrorLabel.IsVisible = false;

        // Disable UI while request is in-flight
        LoginButton.IsEnabled = false;
        LoginActivity.IsRunning = true;
        LoginActivity.IsVisible = true;

        try
        {
            var email = EmailEntry.Text.Trim();
            var password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ErrorLabel.Text = "Preencha email e senha.";
                ErrorLabel.IsVisible = true;
                return;
            }

            var payload = new { Email = email, Password = password };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var resp = await _httpClient.PostAsync("/login", content);
                if (resp.IsSuccessStatusCode)
                {
                    var respJson = await resp.Content.ReadAsStringAsync();
                    // Show server response to the user as feedback
                    await DisplayAlert("Sucesso", $"Login efetuado com sucesso.\n{respJson}", "OK");

                    var remember = RememberCheckBox.IsChecked;
                    if (remember)
                    {
                        Preferences.Set("remember_me", true);
                        Preferences.Set("saved_email", email);
                    }
                    else
                    {
                        Preferences.Remove("remember_me");
                        Preferences.Remove("saved_email");
                    }

                    await Shell.Current.GoToAsync($"//{nameof(UserPage)}");
                }
                else if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ErrorLabel.Text = "Credenciais inválidas.";
                    ErrorLabel.IsVisible = true;
                }
                else
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    ErrorLabel.Text = $"Resposta: {resp.StatusCode}\n{body}";
                    ErrorLabel.IsVisible = true;
                }
            }
            catch (HttpRequestException httpEx)
            {
                ErrorLabel.Text = "Erro de rede: " + httpEx.Message;
                ErrorLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
        finally
        {
            // Restore UI
            LoginButton.IsEnabled = true;
            LoginActivity.IsRunning = false;
            LoginActivity.IsVisible = false;
        }
    }

    private void OnEmailCompleted(object sender, EventArgs e)
    {
        PasswordEntry.Focus();
    }

    private void OnPasswordCompleted(object sender, EventArgs e)
    {
        OnLoginClicked(sender, e);
    }
}
