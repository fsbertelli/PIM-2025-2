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
    public string? Password { get; set; }
    
    [ForeignKey("Department")]
    public int DeptId { get; set; }
    public Department? Department { get; set; }
    
    [ForeignKey("UserStatus")]
    public int UserStatusId { get; set; }
    public UserStatus? UserStatus { get; set; }
    
    [ForeignKey("UserProfile")]
    public int ProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
}
