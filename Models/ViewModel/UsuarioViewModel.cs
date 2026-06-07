using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
public class UsuarioViewModel
{       
    [Required(ErrorMessage = "O E-mail é obrigatório")]
    public string Email { get; set; }

    [Required(ErrorMessage = "O Login é obrigatório")]
    public string Login { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória")]
    public string Senha { get; set; }

    [Required(ErrorMessage = "A confirmação da senha é obrigatória")]
    public string ConfirmarSenha { get; set; }
}