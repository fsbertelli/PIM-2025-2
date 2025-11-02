namespace Web.Models
{
    public class TicketDto
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public int StatusId { get; set; }
        public int UserId { get; set; }
        public int DeptTargetId { get; set; }
        public DateTime OpenDateTime { get; set; }
        public int PriorityLevel { get; set; }
    }
}

