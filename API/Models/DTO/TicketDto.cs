namespace API.Models.DTO;

public class TicketDto
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int StatusId { get; set; }
    public int UserId { get; set; }
    public int DeptTargetId { get; set; }
    public System.DateTime OpenDateTime { get; set; }
    public int PriorityLevel { get; set; }
}

