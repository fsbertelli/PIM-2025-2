namespace API.Models.DTO;

public record RequestTexto(string Texto);

public record RespostaGPT(Choice[] choices);
public record Choice(Message message);
public record Message(string role, string content);