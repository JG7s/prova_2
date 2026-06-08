using Microsoft.AspNetCore.Mvc;
using prova_02.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Text;

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
        var usuario = BancoUsuario.Listar().FirstOrDefault(p => p.Login == Login);

        if (usuario == null)
        {
            ViewBag.Erro = "Usuário Invalido";
            return View();
        }

        var hash = new PasswordHasher<object>();
        var result = hash.VerifyHashedPassword(null, usuario.Senha, Senha);

        if (result == PasswordVerificationResult.Success)
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
        if (model.Senha != model.ConfirmarSenha)
        {
            ViewBag.Erro = "As senhas não coincidem!";
            return View(model);
        }

        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Login) || string.IsNullOrEmpty(model.Senha) || string.IsNullOrEmpty(model.ConfirmarSenha))
        {
            ViewBag.Erro = "Todos os campos são obrigatórios!";
            return View(model);
        }

        var existe = BancoUsuario.Listar().Any(p => p.Login == model.Login);
        if (existe)
        {
            ViewBag.Erro = "Este login já está em uso!";
            return View(model);
        }

        int maxId = BancoUsuario.Listar().Any()
            ? BancoUsuario.Listar().Max(p => p.Id)
            : 0;

        var hash = new PasswordHasher<object>();

        var usuario = new Usuario
        {
            Id    = maxId + 1,
            Email = model.Email,
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

    [HttpGet]
    public IActionResult Editar()
    {
        var loginUsuario = User.Identity.Name;

        var usuario = BancoUsuario.Listar()
            .FirstOrDefault(p => p.Login == loginUsuario);

        if (usuario == null)
            return RedirectToAction("Login");

        EditarUserViewModel model = new EditarUserViewModel
        {
            Id    = usuario.Id,
            Email = usuario.Email,
            Login = usuario.Login
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(EditarUserViewModel model)
    {
        var todos = BancoUsuario.Listar();
        var usuario = todos.FirstOrDefault(p => p.Id == model.Id);

        if (usuario == null)
        {
            ViewBag.Erro = $"Usuário Id={model.Id} não encontrado!";
            return View(model);
        }

        usuario.Email = model.Email;
        usuario.Login = model.Login;

        if (!string.IsNullOrEmpty(model.NovaSenha))
        {
            if (string.IsNullOrEmpty(model.SenhaAtual))
            {
                ViewBag.Erro = "Informe a senha atual para alterá-la.";
                return View(model);
            }

            var hasher = new PasswordHasher<object>();
            var resultado = hasher.VerifyHashedPassword(null, usuario.Senha, model.SenhaAtual);

            if (resultado == PasswordVerificationResult.Failed)
            {
                ViewBag.Erro = "Senha atual incorreta.";
                return View(model);
            }

            if (model.NovaSenha != model.ConfirmarSenha)
            {
                ViewBag.Erro = "As senhas não coincidem!";
                return View(model);
            }

            usuario.Senha = hasher.HashPassword(null, model.NovaSenha);
        }

        BancoUsuario.Alterar(usuario.Id, usuario);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Login)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        TempData["Mensagem"] = "Usuário alterado com sucesso!";
        return RedirectToAction("Editar");
    }

    [HttpPost]
    public IActionResult AlterarSenha(AlterarSenhaViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var usuario = BancoUsuario.Listar().FirstOrDefault(p => p.Id == model.Id);

        if (usuario == null)
        {
            ModelState.AddModelError("", "Usuário não encontrado.");
            return View(model);
        }

        var hasher = new PasswordHasher<object>();
        var resultado = hasher.VerifyHashedPassword(usuario, usuario.Senha, model.SenhaAtual);

        if (resultado == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("SenhaAtual", "Senha atual incorreta.");
            return View(model);
        }

        usuario.Senha = hasher.HashPassword(usuario, model.NovaSenha);

        BancoUsuario.Alterar(usuario.Id, usuario);

        TempData["Sucesso"] = "Senha alterada com sucesso!";
        return RedirectToAction("Index");
    }

    public IActionResult Exportar(int id)
    {
        var pessoal = BancoUsuario.Busca(id);
        string json = System.Text.Json.JsonSerializer.Serialize(pessoal);

        var bytes = Encoding.UTF8.GetBytes(json);

        return File(bytes, "text/json", "dados.json");
    }
}