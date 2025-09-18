namespace API.Models.DTO;

public class UserDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public DepartmentDto? Department { get; set; }
    public UserStatusDto? UserStatus { get; set; }
    public UserProfileDto? UserProfile { get; set; }
}