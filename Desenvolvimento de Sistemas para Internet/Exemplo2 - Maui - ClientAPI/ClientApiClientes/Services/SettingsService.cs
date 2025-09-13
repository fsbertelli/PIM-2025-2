namespace ClientApiClientes.Services
{
    public class SettingsService
    {
        // --- Valores Padrão ---
        private const string DefaultBaseUrl = "http://localhost:5185";
        private const string DefaultGetAll = "/clientes";
        private const string DefaultGetById = "/clientes/{id}";
        private const string DefaultPost = "/clientes";
        private const string DefaultPut = "/clientes/{id}";
        private const string DefaultDelete = "/clientes/{id}";

        // --- Propriedades ---
        public string BaseUrl { get; set; } = DefaultBaseUrl;
        public string GetAllEndpoint { get; set; } = DefaultGetAll;
        public string GetByIdEndpoint { get; set; } = DefaultGetById;
        public string CreateEndpoint { get; set; } = DefaultPost;
        public string UpdateEndpoint { get; set; } = DefaultPut;
        public string DeleteEndpoint { get; set; } = DefaultDelete;
    }
}
