namespace API.Models.DTO;

public class CreateTicketSimpleDto
{
    // Id do usuário (autenticado) que está abrindo o chamado.
    public int UserSourceId { get; set; }

    // Descrição / corpo da mensagem no chat
    public string? Body { get; set; }

    // Base64 do anexo
    public string? AttachBase64 { get; set; }

    // ID do departamento - bollean acceptTicket.
    public int? DeptTargetId { get; set; }

    //Nao adicionar categoria nem prioridade aqui (deixar para a IA)
}

