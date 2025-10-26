// ...existing code...
using System;

namespace API.Models.DTO;

public class CreateTicketSimpleDto
{
    // Id do usuário que está abrindo o chamado. If you have authenticated user, you can ignore this and use the auth user id instead.
    public int UserSourceId { get; set; }

    // Optional textual description / message to be stored in the ticket and as the initial transaction body
    public string? Body { get; set; }

    // Optional base64-encoded attachment (data URI or raw base64)
    public string? AttachBase64 { get; set; }

    // Optional department target id. If provided, the service will attempt to use this department (must exist and accept tickets).
    public int? DeptTargetId { get; set; }

    // NOTE: Priority is not provided by the mobile user; it is set by the server/triage team.
    // public int? PriorityLevel { get; set; }
}

// ...existing code...
