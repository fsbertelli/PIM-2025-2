namespace App.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Department? Department { get; set; }
        public UserStatus? UserStatus { get; set; }
        public UserProfile? UserProfile { get; set; }
    }
}