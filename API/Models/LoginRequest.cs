namespace API.Models.DTO
{
    public class LoginRequest
    {
        // Login será feito por Email e Password
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
