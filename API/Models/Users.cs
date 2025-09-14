using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

public class Users
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(50, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string? Name { get; set; }
    
    [EmailAddress]
    [StringLength(50, ErrorMessage = "O endereço de email deve ter no máximo 50 caracteres.")]
    public string? Email { get; set; }

    [PasswordPropertyText]
    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(12, ErrorMessage = "A senha deve ter no máximo 12 caracteres.")]
    public string? Pwd { get; set; }
    
    [ForeignKey("Dept") ]
    public int DeptId { get; set; }
    public Dept? Dept { get; set; }
    
    [ForeignKey("StatusUser") ]
    public int StatusUserId { get; set; }
    public StatusUser? StatusUser { get; set; }
    
    [ForeignKey("Profile") ]
    public int ProfileId { get; set; }
    public Profile? Profile { get; set; }
    
}
