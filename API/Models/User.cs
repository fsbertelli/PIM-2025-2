using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
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
    public Department? Department { get; set; }
    
    [ForeignKey("UserStaus") ]
    public int UserStatusId { get; set; }
    public UserStatus? UserStatus { get; set; }
    
    [ForeignKey("Profile") ]
    public int ProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
}
