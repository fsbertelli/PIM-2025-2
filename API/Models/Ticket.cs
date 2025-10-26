using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

public class Ticket
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User? User { get; set; }

    // Department target (where the ticket is directed)
    [ForeignKey("Department")]
    public int DeptTargetId { get; set; }
    public Department? Department { get; set; }
    
    [Required(ErrorMessage = "A descrição do ticket é obrigatória.")]
    public string? Description { get; set; }

    [ForeignKey("Category")]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public DateTime OpenDateTime { get; set; } = DateTime.UtcNow;

    [ForeignKey("StatusTicket")]
    public int StatusId { get; set; }
    public StatusTicket? StatusTicket { get; set; }

    public int PriorityLevel { get; set; }
}
