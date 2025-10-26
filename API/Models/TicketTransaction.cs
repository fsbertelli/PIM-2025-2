using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

public class TicketTransaction
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("UserSource")]
    public int UserSourceId { get; set; }
    public User? UserSource { get; set; }

    [ForeignKey("UserTarget")]
    public int? UserTargetId { get; set; }
    public User? UserTarget { get; set; }

    public string? Body { get; set; }

    [ForeignKey("Ticket")]
    public int TicketId { get; set; }
    public Ticket? Ticket { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? AttachUrl { get; set; }
}
