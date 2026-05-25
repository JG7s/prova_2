using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using prova_02.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace prova_02.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IBanco<Usuario> BancoUsuario;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
        BancoUsuario = new Banco<Usuario>();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    

    [HttpPost]
    public async Task<IActionResult> Login(string Login, string Senha)
    {
        var usuario = BancoUsuario.Listar().FirstOrDefault(p=> p.Login == Login);
        
        if(usuario == null)
        {
            ViewBag.Erro = "Usuário Invalido";
            return View();
        }
        

        var hash = new PasswordHasher<object>();

        var result = hash.VerifyHashedPassword(null, usuario.Senha, Senha);

        if(result == PasswordVerificationResult.Success)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Login)
            };

            var indentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(indentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ViewBag.Erro = "Usuário Invalido";
            return View();
        }

        
        

        
    }  

    [HttpGet]
    public IActionResult Cadastro()
    {
        return View(new UsuarioViewModel());
    }

    [HttpPost]
    public IActionResult Cadastro(UsuarioViewModel model)
    {
        // Valida se as senhas coincidem
        if (model.Senha != model.ConfirmarSenha)
        {
            ViewBag.Erro = "As senhas não coincidem!";
            return View(model);
        }

        // Verifica se o login já existe
        var existe = BancoUsuario.Listar().Any(p => p.Login == model.Login);
        if (existe)
        {
            ViewBag.Erro = "Este login já está em uso!";
            return View(model);
        }

        // Gera o próximo Id
        int maxId = BancoUsuario.Listar().Any()
            ? BancoUsuario.Listar().Max(p => p.Id)
            : 0;

        // Hash da senha
        var hash = new PasswordHasher<object>();

        var usuario = new Usuario
        {
            Id    = maxId + 1,
            Nome  = model.Nome,
            Login = model.Login,
            Senha = hash.HashPassword(null, model.Senha)
        };

        BancoUsuario.Adicionar(usuario);

        TempData["Mensagem"] = "Conta criada com sucesso!";
        return RedirectToAction("Login");
    }

    
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return View("Login");
    }    
}
