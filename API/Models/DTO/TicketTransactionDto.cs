using System;

namespace API.Models.DTO;

public class TicketTransactionDto
{
    public Guid Id { get; set; }
    public int UserSourceId { get; set; }
    public UserDto? UserSource { get; set; }
    public int? UserTargetId { get; set; }
    public UserDto? UserTarget { get; set; }
    public string? Body { get; set; }
    public int TicketId { get; set; }
    public TicketDto? Ticket { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? AttachUrl { get; set; }
}
