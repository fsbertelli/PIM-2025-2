namespace Web.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserDto? User { get; set; }
        public int DeptTargetId { get; set; }
        public DepartmentDto? Department { get; set; }
        public int? UserTargetId { get; set; }
        public UserDto? UserTarget { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public CategoryDto? Category { get; set; }
        public DateTime? OpenDateTime { get; set; }
        public int StatusId { get; set; }
        public StatusTicketDto? StatusTicket { get; set; }
        public int PriorityLevel { get; set; }
    }
}
