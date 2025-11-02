using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class CreateUserProfileDto
    {
        [StringLength(50)]
        public string? Name { get; set; }
    }
}
