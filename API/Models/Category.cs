using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "A descrição da categoria é obrigatória.")]
    [StringLength(100, ErrorMessage = "A descrição deve ter no máximo 100 caracteres.")]
    public string? Description { get; set; }

    [ForeignKey("Department")]
    public int DeptId { get; set; }
    public Department? Department { get; set; }
}

