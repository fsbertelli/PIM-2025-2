namespace App.Services
{
    public class SettingsService
    {
        // --- Valores Padrão ---
        private const string DefaultBaseUrl = "http://localhost:5185";
        private const string DefaultGetAll = "/users";
        private const string DefaultGetById = "/users/{id}";
        private const string DefaultPost = "/users";
        private const string DefaultPut = "/users/{id}";
        private const string DefaultDelete = "/users/{id}";

        // --- Propriedades ---
        public string BaseUrl { get; set; } = DefaultBaseUrl;
        public string GetAllEndpoint { get; set; } = DefaultGetAll;
        public string GetByIdEndpoint { get; set; } = DefaultGetById;
        public string CreateEndpoint { get; set; } = DefaultPost;
        public string UpdateEndpoint { get; set; } = DefaultPut;
        public string DeleteEndpoint { get; set; } = DefaultDelete;

        // --- Endpoints de Departamentos ---
        public string DeptGetAllEndpoint { get; set; } = "/departments";
        public string DeptGetByIdEndpoint { get; set; } = "/departments/{id}";
        public string DeptCreateEndpoint { get; set; } = "/departments";
        public string DeptUpdateEndpoint { get; set; } = "/departments/{id}";
        public string DeptDeleteEndpoint { get; set; } = "/departments/{id}";

        // --- Endpoints de Usuários ---
        public string UserCreateEndpoint { get; set; } = "/users";
        public string UserUpdateEndpoint { get; set; } = "/users/{id}";
        public string UserDeleteEndpoint { get; set; } = "/users/{id}";

        // --- Endpoints de User DTO ---
        public string UserDtoGetAllEndpoint { get; set; } = "/users";
        public string UserDtoGetByIdEndpoint { get; set; } = "/users/{id}";


        // --- Endpoints de Perfis de Usuário ---
        public string UserProfileGetAllEndpoint { get; set; } = "/profiles";
        public string UserProfileGetByIdEndpoint { get; set; } = "/profiles/{id}";
        public string UserProfileCreateEndpoint { get; set; } = "/profiles";
        public string UserProfileUpdateEndpoint { get; set; } = "/profiles/{id}";
        public string UserProfileDeleteEndpoint { get; set; } = "/profiles/{id}";

        // --- Endpoints de Status de Usuário ---
        public string UserStatusGetAllEndpoint { get; set; } = "/userstatuses";
        public string UserStatusGetByIdEndpoint { get; set; } = "/userstatuses/{id}";
        public string UserStatusCreateEndpoint { get; set; } = "/userstatuses";
        public string UserStatusUpdateEndpoint { get; set; } = "/userstatuses/{id}";
        public string UserStatusDeleteEndpoint { get; set; } = "/userstatuses/{id}";
    }
}
