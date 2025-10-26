using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class CreateDepartmentDto
    {
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        public bool AcceptTicket { get; set; } = false;
    }
}
