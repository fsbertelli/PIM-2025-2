using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class Department
{
    [Key] public int Id { get; set; }
    
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [StringLength(50, ErrorMessage = "Nome deve ter no máximo 50 caracteres.")]
    public string? Name { get; set; }
    
    public bool AcceptTicket { get; set; }
}