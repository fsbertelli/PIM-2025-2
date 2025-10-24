namespace API.Models.DTO
{
    public class UpdateUserDto
    {
        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public int DeptId { get; set; }
        public int UserStatusId { get; set; }
        public int ProfileId { get; set; }
    }
}

