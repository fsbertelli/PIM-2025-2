using System.ComponentModel.DataAnnotations;

namespace ClientesAPI.Models;

public class StatusUser
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Status do usuário é obrigatório.")]
    [StringLength(50, ErrorMessage = "O status deve ter no máximo 50 caracteres")]
    public string? Status { get; set; }

}