using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class UpdateUserDto
    {
        [StringLength(50)]
        public string? Name { get; set; }

        [EmailAddress]
        [StringLength(50)]
        public string? Email { get; set; }

        public string? Password { get; set; }

        public int DeptId { get; set; }

        public int UserStatusId { get; set; }

        public int ProfileId { get; set; }
    }
}
