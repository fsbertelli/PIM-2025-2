using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class Profile
{
    [Key]
    public int Id { get; set; }
    
    [Required (ErrorMessage = "O perfil é obrigatório.")]
    [StringLength(50, ErrorMessage = "O perfil deve ter no máximo 50 caracteres.")]
    public string? Perfil { get; set; }

}