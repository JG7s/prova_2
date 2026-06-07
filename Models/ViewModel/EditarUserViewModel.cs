public class EditarUserViewModel
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Login { get; set; }
    public string? SenhaAtual { get; set; }  // <-- adiciona isso
    public string? NovaSenha { get; set; }
    public string? ConfirmarSenha { get; set; }
}