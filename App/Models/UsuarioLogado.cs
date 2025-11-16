namespace App.Models
{
    public class UsuarioLogado
    {
        public int IdUsuario { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Perfil { get; set; }
        public bool EstaAutenticado { get; set; }
        public DateTime DataLogin { get; set; }
    }
}