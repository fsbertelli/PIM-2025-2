using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string? Email { get; set; }

        // Texto plano recebido no DTO; será hasheado no servidor
        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres.")]
        public string? Password { get; set; }

        [Required]
        public int DeptId { get; set; }

        [Required]
        public int UserStatusId { get; set; }

        [Required]
        public int ProfileId { get; set; }
    }
}
