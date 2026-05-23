using System.ComponentModel.DataAnnotations;
public class Pessoa : ModelBase
{
    
    public string Nome { get; set; }
    public string Sobrenome { get; set; }
    public string Endereco { get; set; }
    public string Cidade { get; set; }
    public string Cep { get; set; }
    public string Telefone { get; set; }
}
