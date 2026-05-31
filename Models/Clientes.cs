using System.ComponentModel.DataAnnotations;

public class Clientes : ModelBase
{
    public string CPF { get; set; }
    public string InscricaoEstatudal { get; set; }
    public string Nome { get; set; }
    public string NomeFantasia { get; set; }
    public string NumeroEndereco { get; set; }
    public string Bairro { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public string CaminhoImagem { get; set; }
    public DateTime DataNascimento { get; set; }
}