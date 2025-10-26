using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class StatusTicket
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O status do ticket é obrigatório.")]
    [StringLength(50, ErrorMessage = "O status deve ter no máximo 50 caracteres.")]
    public string? Name { get; set; }
}

