using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class CreateUserStatusDto
    {
        [StringLength(50)]
        public string? Name { get; set; }
    }
}
