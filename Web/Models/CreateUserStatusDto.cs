using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class CreateUserStatusDto
    {
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }
    }
}
