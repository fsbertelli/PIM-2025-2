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
        Shell.SetNavBarIsVisible(this, true);
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

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

            // Envia 'Email' e 'Password' (login por email)
            var payload = new { Email = email, Password = password };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var resp = await _httpClient.PostAsync("/login", content);
                if (resp.IsSuccessStatusCode)
                {
                    await DisplayAlert("Sucesso", "Login realizado com sucesso.", "OK");

// habilita o flyout depois de logar (opcional)
                    Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;

// navegação ABSOLUTA para a rota de Shell "reports" (limpa a pilha)
                    await Shell.Current.GoToAsync("//reports");

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