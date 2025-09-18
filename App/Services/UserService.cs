using System.Net.Http.Json;
using App.Models;

namespace App.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly SettingsService _settingsService;

        public UserService(SettingsService settingsService)
        {
            _httpClient = new HttpClient();
            _settingsService = settingsService;
        }

        // Buscar todos os usuários
        public async Task<List<User>> GetUsersAsync()
        {
            if (string.IsNullOrWhiteSpace(_settingsService.BaseUrl) || string.IsNullOrWhiteSpace(_settingsService.GetAllEndpoint))
                return new List<User>();

            var url = _settingsService.BaseUrl + _settingsService.GetAllEndpoint;
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<User>>() ?? new List<User>();
            }
            return new List<User>();
        }

        // Buscar usuário por ID
        public async Task<User?> GetUserAsync(int id)
        {
            if (string.IsNullOrWhiteSpace(_settingsService.BaseUrl) || string.IsNullOrWhiteSpace(_settingsService.GetByIdEndpoint))
                return null;

            var url = (_settingsService.BaseUrl + _settingsService.GetByIdEndpoint).Replace("{id}", id.ToString());
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }
            return null;
        }

        public async Task<bool> SaveUserAsync(User user)
        {
            HttpResponseMessage response;
            if (user.Id == 0)
            {
                // Criar
                if (string.IsNullOrWhiteSpace(_settingsService.BaseUrl) || string.IsNullOrWhiteSpace(_settingsService.CreateEndpoint))
                    return false;
                var url = _settingsService.BaseUrl + _settingsService.CreateEndpoint;
                response = await _httpClient.PostAsJsonAsync(url, user);
            }
            else
            {
                // Atualizar
                if (string.IsNullOrWhiteSpace(_settingsService.BaseUrl) || string.IsNullOrWhiteSpace(_settingsService.UpdateEndpoint))
                    return false;
                var url = (_settingsService.BaseUrl + _settingsService.UpdateEndpoint).Replace("{id}", user.Id.ToString());
                response = await _httpClient.PutAsJsonAsync(url, user);
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<(bool Success, string? ErrorMessage)> SaveUserAsync(CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(_settingsService.BaseUrl) || string.IsNullOrWhiteSpace(_settingsService.CreateEndpoint))
                return (false, "Endpoint de cadastro não configurado.");
            var url = _settingsService.BaseUrl + _settingsService.CreateEndpoint;
            var response = await _httpClient.PostAsJsonAsync(url, dto);
            if (response.IsSuccessStatusCode)
                return (true, null);
            var error = await response.Content.ReadAsStringAsync();
            var status = (int)response.StatusCode;
            return (false, $"Status: {status}\n{error}");
        }

        public async Task DeleteUserAsync(int id)
        {
            if (string.IsNullOrWhiteSpace(_settingsService.BaseUrl) || string.IsNullOrWhiteSpace(_settingsService.DeleteEndpoint))
                return;

            var url = (_settingsService.BaseUrl + _settingsService.DeleteEndpoint).Replace("{id}", id.ToString());
            await _httpClient.DeleteAsync(url);
        }

        // Buscar todos os departamentos
        public async Task<List<Department>> GetDepartmentsAsync()
        {
            var url = _settingsService.BaseUrl + _settingsService.DeptGetAllEndpoint;
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Department>>() ?? new List<Department>();
            }
            return new List<Department>();
        }

        // Buscar todos os status de usuário
        public async Task<List<UserStatus>> GetUserStatusesAsync()
        {
            var url = _settingsService.BaseUrl + _settingsService.UserStatusGetAllEndpoint;
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<UserStatus>>() ?? new List<UserStatus>();
            }
            return new List<UserStatus>();
        }

        // Buscar todos os perfis de usuário
        public async Task<List<UserProfile>> GetUserProfilesAsync()
        {
            var url = _settingsService.BaseUrl + _settingsService.UserProfileGetAllEndpoint;
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<UserProfile>>() ?? new List<UserProfile>();
            }
            return new List<UserProfile>();
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateUserAsync(int id, CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(_settingsService.BaseUrl) || string.IsNullOrWhiteSpace(_settingsService.UpdateEndpoint))
                return (false, "Endpoint de edição não configurado.");
            var url = (_settingsService.BaseUrl + _settingsService.UpdateEndpoint).Replace("{id}", id.ToString());
            var response = await _httpClient.PutAsJsonAsync(url, dto);
            if (response.IsSuccessStatusCode)
                return (true, null);
            var error = await response.Content.ReadAsStringAsync();
            var status = (int)response.StatusCode;
            return (false, $"Status: {status}\n{error}");
        }
    }
}
