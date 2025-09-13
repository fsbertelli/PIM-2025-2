using System.ComponentModel.DataAnnotations;

namespace ClientesAPI.Models;

public class Cliente
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string? Nome { get; set; }

    [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres.")]
    public string? Endereco { get; set; }

    public DateTime DataDeNascimento { get; set; }

    [StringLength(9, ErrorMessage = "O CEP deve ter no máximo 9 caracteres.")]
    public string? CEP { get; set; }

    [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres.")]
    public string? Cidade { get; set; }

    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [StringLength(14, ErrorMessage = "O CPF deve ter no máximo 14 caracteres.")]
    public string? CPF { get; set; }

    public string? Foto { get; set; } // URL para a foto

    [StringLength(20, ErrorMessage = "O sexo deve ter no máximo 20 caracteres.")]
    public string? Sexo { get; set; }
}
