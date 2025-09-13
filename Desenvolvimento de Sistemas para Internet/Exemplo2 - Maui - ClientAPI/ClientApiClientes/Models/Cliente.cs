namespace ClientApiClientes.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Endereco { get; set; }
        public DateTime DataDeNascimento { get; set; }
        public string? CEP { get; set; }
        public string? Cidade { get; set; }
        public string? CPF { get; set; }
        public string? Foto { get; set; }
        public string? Sexo { get; set; }
    }
}