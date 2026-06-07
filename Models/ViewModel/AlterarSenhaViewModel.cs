using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

public class AlterarSenhaViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe a senha atual")]
    [DataType(DataType.Password)]
    public string SenhaAtual { get; set; }

    [Required(ErrorMessage = "Informe a nova senha")]
    [DataType(DataType.Password)]
    public string NovaSenha { get; set; }

    [Compare("NovaSenha", ErrorMessage = "As senhas não conferem")]
    [DataType(DataType.Password)]
    public string ConfirmarSenha { get; set; }
}