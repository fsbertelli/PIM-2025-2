using ClientApiClientes.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace ClientApiClientes.Services
{
    public class ClienteService
    {
        private readonly HttpClient _httpClient;
        private readonly SettingsService _settingsService;

        public ClienteService(SettingsService settingsService)
        {
            _httpClient = new HttpClient();
            _settingsService = settingsService;
        }

        public async Task<List<Cliente>> GetClientesAsync()
        {
            if (string.IsNullOrWhiteSpace(_settingsService.BaseUrl) || string.IsNullOrWhiteSpace(_settingsService.GetAllEndpoint))
                return new List<Cliente>();

            var url = _settingsService.BaseUrl + _settingsService.GetAllEndpoint;
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Cliente>>() ?? new List<Cliente>();
            }
            return new List<Cliente>();
        }

        public async Task<Cliente?> GetClienteAsync(int id)
        {
            if (string.IsNullOrWhiteSpace(_settingsService.BaseUrl) || string.IsNullOrWhiteSpace(_settingsService.GetByIdEndpoint))
                return null;

            var url = (_settingsService.BaseUrl + _settingsService.GetByIdEndpoint).Replace("{id}", id.ToString());
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Cliente>();
            }
            return null;
        }

        public async Task SaveClienteAsync(Cliente cliente)
        {
            if (cliente.Id == 0)
            {
                // Criar
                if (string.IsNullOrWhiteSpace(_settingsService.BaseUrl) || string.IsNullOrWhiteSpace(_settingsService.CreateEndpoint))
                    return;
                var url = _settingsService.BaseUrl + _settingsService.CreateEndpoint;
                await _httpClient.PostAsJsonAsync(url, cliente);
            }
            else
            {
                // Atualizar
                if (string.IsNullOrWhiteSpace(_settingsService.BaseUrl) || string.IsNullOrWhiteSpace(_settingsService.UpdateEndpoint))
                    return;
                var url = (_settingsService.BaseUrl + _settingsService.UpdateEndpoint).Replace("{id}", cliente.Id.ToString());
                await _httpClient.PutAsJsonAsync(url, cliente);
            }
        }

        public async Task DeleteClienteAsync(int id)
        {
            if (string.IsNullOrWhiteSpace(_settingsService.BaseUrl) || string.IsNullOrWhiteSpace(_settingsService.DeleteEndpoint))
                return;

            var url = (_settingsService.BaseUrl + _settingsService.DeleteEndpoint).Replace("{id}", id.ToString());
            await _httpClient.DeleteAsync(url);
        }
    }
}