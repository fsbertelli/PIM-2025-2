using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace App.Views;

public partial class LoginPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public LoginPage()
    {
        InitializeComponent();
        // Cliente simples para testes. Se preferir usar HttpClient via DI, altere a inicialização.
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5185") };
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Erro", "Preencha email e senha.", "OK");
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
                await DisplayAlert("Sucesso", "Login efetuado com sucesso.", "OK");
                // Aqui você pode navegar para outra página ou salvar token/usuário.
            }
            else if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await DisplayAlert("Falha", "Credenciais inválidas.", "OK");
            }
            else
            {
                var body = await resp.Content.ReadAsStringAsync();
                await DisplayAlert("Erro", $"Resposta: {resp.StatusCode}\n{body}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }
}

