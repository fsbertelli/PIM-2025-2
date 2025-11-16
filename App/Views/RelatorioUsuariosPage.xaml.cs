using System.Net.Http.Headers;
using System.Text.Json;

namespace App.Views;

public partial class RelatorioUsuariosPage : ContentPage
{
    private readonly HttpClient _http;

    public RelatorioUsuariosPage()
    {
        InitializeComponent();
        _http = new HttpClient { BaseAddress = new Uri(GetBaseUrl()) };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    private static string GetBaseUrl()
        => DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5185" : "http://localhost:5185";

    private async Task LoadAsync()
    {
        try
        {
            Busy.IsVisible = Busy.IsRunning = true;
            Refresh.IsRefreshing = true;

            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrWhiteSpace(token))
            {
                await DisplayAlert("Sessão", "Faça login novamente.", "OK");
                await Shell.Current.GoToAsync("//login");
                return;
            }

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // 🔹 TROQUE AQUI PELO ENDPOINT REAL DA SUA API
            // exemplos: "/api/users", "/usuarios", "/users/list"
            var resp = await _http.GetAsync("/api/users");
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                await DisplayAlert("Erro", $"Falha: {(int)resp.StatusCode}", "OK");
                return;
            }

            var list = new List<UsuarioItem>();
            using var doc = JsonDocument.Parse(body);

            // aceita array direto ou { data: [] }
            var root = doc.RootElement;
            var arr = root.ValueKind == JsonValueKind.Array
                ? root
                : (root.TryGetProperty("data", out var dataProp) && dataProp.ValueKind == JsonValueKind.Array ? dataProp : default);

            if (arr.ValueKind == JsonValueKind.Array)
            {
                foreach (var el in arr.EnumerateArray())
                    list.Add(MapUsuario(el));
            }

            ListUsers.ItemsSource = list;
            LblTotal.Text = list.Count.ToString();
            LblAtivos.Text = list.Count(u => string.Equals(u.Status, "Ativo", StringComparison.OrdinalIgnoreCase)).ToString();
            LblSync.Text = DateTime.Now.ToString("dd/MM HH:mm");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
        finally
        {
            Busy.IsVisible = Busy.IsRunning = false;
            Refresh.IsRefreshing = false;
        }
    }

    private async void OnRefreshClicked(object sender, EventArgs e) => await LoadAsync();
    private async void OnRefreshing(object sender, EventArgs e) => await LoadAsync();

    // ======================================================
    // 🔸 MAPEAMENTO FLEXÍVEL DE CAMPOS
    // ======================================================

    private static UsuarioItem MapUsuario(JsonElement el)
    {
        string nome = GetString(el, "nome", "name", "fullName", "usuario");
        string email = GetString(el, "email", "mail", "e_mail");
        string status = GetString(el, "status") ?? (GetBool(el, "ativo", "isActive") ? "Ativo" : "Inativo");
        DateTime? last = GetDate(el, "ultimoAcesso", "lastLogin", "lastAccess", "updatedAt");

        return new UsuarioItem
        {
            Nome = nome ?? "(Sem nome)",
            Email = email ?? "-",
            Status = status ?? "-",
            UltimoAcesso = last
        };
    }

    private static string GetString(JsonElement el, params string[] keys)
    {
        foreach (var k in keys)
            if (el.TryGetProperty(k, out var p) && p.ValueKind == JsonValueKind.String)
                return p.GetString();
        return null;
    }

    private static bool GetBool(JsonElement el, params string[] keys)
    {
        foreach (var k in keys)
        {
            if (el.TryGetProperty(k, out var p))
            {
                if (p.ValueKind == JsonValueKind.True) return true;
                if (p.ValueKind == JsonValueKind.False) return false;
                if (p.ValueKind == JsonValueKind.String && bool.TryParse(p.GetString(), out var b)) return b;
                if (p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out var n)) return n != 0;
            }
        }
        return false;
    }

    private static DateTime? GetDate(JsonElement el, params string[] keys)
    {
        foreach (var k in keys)
        {
            if (el.TryGetProperty(k, out var p) && p.ValueKind == JsonValueKind.String)
            {
                var s = p.GetString();
                if (DateTime.TryParse(s, out var dt)) return dt;
                if (DateTime.TryParse(s?.Replace("Z", ""), out dt)) return dt;
            }
        }
        return null;
    }
}

// ======================================================
// 🔹 DTO usado na CollectionView
// ======================================================
public sealed class UsuarioItem
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }
    public DateTime? UltimoAcesso { get; set; }
}
