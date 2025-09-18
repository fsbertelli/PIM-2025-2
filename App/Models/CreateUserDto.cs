namespace App.Models
{
    public class CreateUserDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Pwd { get; set; }
        public int DeptId { get; set; }
        public int UserStatusId { get; set; }
        public int ProfileId { get; set; }
    }
}
