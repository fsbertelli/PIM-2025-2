using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class LoginModel
    {
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? Email { get; set; }

        public string? Password { get; set; }

        public bool Remember { get; set; }
    }
}

