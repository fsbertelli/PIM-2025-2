using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class UserStatus
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Status do usuário é obrigatório.")]
    [StringLength(50, ErrorMessage = "O status deve ter no máximo 50 caracteres")]
    public string? Name { get; set; }

}