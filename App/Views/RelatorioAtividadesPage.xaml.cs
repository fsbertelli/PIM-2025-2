using System.Net.Http.Headers;
using System.Text.Json;

namespace App.Views;

public partial class RelatorioAtividadesPage : ContentPage
{
    private readonly HttpClient _http;

    public RelatorioAtividadesPage()
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

            var resp = await _http.GetAsync("/tickets");
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                await DisplayAlert("Erro", $"Falha: {(int)resp.StatusCode}", "OK");
                return;
            }

            var list = new List<AtividadeItem>();
            using var doc = JsonDocument.Parse(body);

            // aceita array direto ou { data: [] }
            var root = doc.RootElement;
            var arr = root.ValueKind == JsonValueKind.Array
                ? root
                : (root.TryGetProperty("data", out var dataProp) && dataProp.ValueKind == JsonValueKind.Array ? dataProp : default);

            if (arr.ValueKind == JsonValueKind.Array)
            {
                foreach (var el in arr.EnumerateArray())
                    list.Add(MapAtividade(el));
            }

            ListAtv.ItemsSource = list;
            LblTotal.Text = list.Count.ToString();
            LblAbertas.Text = list.Count(a => string.Equals(a.Status, "Aberto", StringComparison.OrdinalIgnoreCase)).ToString();
            LblFechadas.Text = list.Count(a =>
                a.Status?.StartsWith("Conclu", StringComparison.OrdinalIgnoreCase) == true ||
                string.Equals(a.Status, "Fechado", StringComparison.OrdinalIgnoreCase)).ToString();
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
    // 🔸 MAPEAMENTO FLEXÍVEL DE CAMPOS (OS, jornadas, eventos…)
    // ======================================================

    private static AtividadeItem MapAtividade(JsonElement el)
    {
        // tenta vários nomes comuns
        string titulo = GetString(el, "titulo", "title", "descricao", "description", "os", "task", "numero", "id");
        string tipo   = GetString(el, "tipo", "type", "categoria", "category", "origem", "source");
        string status = GetString(el, "status", "statusOS", "situacao", "state");
        DateTime? ini = GetDate(el, "dataInicio", "startTime", "inicio", "startedAt", "data_hr_Inicio");
        DateTime? fim = GetDate(el, "dataFim", "endTime", "fim", "finishedAt", "data_hr_Fim");

        // normalizações comuns
        if (string.Equals(status, "R", StringComparison.OrdinalIgnoreCase)) status = "Aberto";
        if (string.Equals(status, "A", StringComparison.OrdinalIgnoreCase)) status = "Aberto";
        if (string.Equals(status, "C", StringComparison.OrdinalIgnoreCase)) status = "Concluído";
        if (string.Equals(status, "F", StringComparison.OrdinalIgnoreCase)) status = "Fechado";

        return new AtividadeItem
        {
            Titulo = string.IsNullOrWhiteSpace(titulo) ? "(Sem título)" : titulo,
            Tipo = tipo,
            Status = status,
            DataInicio = ini,
            DataFim = fim
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
public sealed class AtividadeItem
{
    public string Titulo { get; set; }
    public string Tipo { get; set; }
    public string Status { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}
