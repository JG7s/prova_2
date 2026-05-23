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
        var hash = new PasswordHasher<object>();

        var senhaHash = hash.HashPassword(null, "123"); 

        var usr = new Usuario
        {
            Login = "Admin",
            Senha = senhaHash            
        }; 

        BancoUsuario.Adicionar(usr);

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

    
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return View("Login");
    }    
}
