using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
public class ClientesViewModel
{
    public int Id { get; set; }    

    [Required(ErrorMessage = "O CPF/CNPJ é obrigatório")]
    public string CPF { get; set; }

    [Required(ErrorMessage = "A Inscricao Estatudal é obrigatório")]
    public string InscricaoEstatudal { get; set; }

    [Required(ErrorMessage = "O Nome do cliente é obrigatório")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O Nome Fantasia é obrigatório")]
    public string NomeFantasia { get; set; }

    [Required(ErrorMessage = "O Numero do Endereco é obrigatório")]
    public string NumeroEndereco { get; set; }

    [Required(ErrorMessage = "O Bairro é obrigatório")]
    public string Bairro { get; set; }

    [Required(ErrorMessage = "A Cidade é obrigatória")]
    public string Cidade { get; set; }

    [Required(ErrorMessage = "O Estado é obrigatório")]
    public string Estado { get; set; }

    [Required(ErrorMessage = "A Data de Nascimento ou de abertura de CNPJ é obrigatória")]
    public DateTime DataNascimento { get; set; }

    public List<SelectListItem> Cidades { get; set; }
}
