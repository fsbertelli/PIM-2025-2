using System;

namespace API.Models.DTO;

public class CreateTicketTransactionDto
{
    public int UserSourceId { get; set; }
    public int? UserTargetId { get; set; }
    public string? Body { get; set; }
    public int TicketId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? AttachBase64 { get; set; }
}
