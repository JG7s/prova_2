using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize]
public class ClientesController : Controller
{

    private readonly IBanco<Clientes> BancoClientes;
    private readonly IBanco<Cidades> BancoCidades;
    public ClientesController()
    {
        BancoClientes = new Banco<Clientes>();        
        BancoCidades = new Banco<Cidades>();
    }

    public IActionResult Index()
    {
        var teste = BancoCidades.Listar();
        var Clientes =  BancoClientes.Listar();
        return View(Clientes.Select(p => new ClientesViewModel
        {
            Id = p.Id,
            CPF = p.CPF,
            InscricaoEstatudal = p.InscricaoEstatudal,
            Nome = p.Nome,
            NomeFantasia = p.NomeFantasia,
            NumeroEndereco = p.NumeroEndereco,
            Bairro = p.Bairro,
            Cidade = p.Cidades,
            Estado = p.Estado,
            DataNascimento = p.DataNascimento
        }));
    }

    public IActionResult RegistroClientes(int id = 0)
    {
        if (id != 0)
        {
            Clientes Clientes = BancoClientes.Listar()
                .FirstOrDefault(p => p.Id == id);

            return View(new ClientesViewModel
            {
                Id = Clientes.Id,
                CPF = Clientes.CPF,
                InscricaoEstatudal = Clientes.InscricaoEstatudal,
                Nome = Clientes.Nome,
                NomeFantasia = Clientes.NomeFantasia,
                NumeroEndereco = Clientes.NumeroEndereco,
                Bairro = Clientes.Bairro,
                Cidade = Clientes.Cidades,
                Estado = Clientes.Estado,
                DataNascimento = Clientes.DataNascimento,
                
            });
        }
        return View(new ClientesViewModel 
        { 
            Id = 0,
            Cidades = BancoCidades.Listar().Select(
                p=> new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nome                    
                }
            ).ToList(),});
    }

    public IActionResult Salvar(ClientesViewModel model)
    {
        
        if (!ModelState.IsValid)
        {
            return View("RegistroClientes", model);
        }
        if (model.Id != 0)
        {
            Clientes clientes = new Clientes
            {
                Id = model.Id,
                CPF = model.CPF,
                InscricaoEstatudal = model.InscricaoEstatudal,
                Nome = model.Nome,
                NomeFantasia = model.NomeFantasia,
                NumeroEndereco = model.NumeroEndereco,
                Bairro = model.Bairro,
                Cidades = model.Cidade,
                Estado = model.Estado,
                DataNascimento = model.DataNascimento,
            };
            BancoClientes.Alterar(model.Id, clientes);
        }
        else
        {
            int maxId = 0;
            if (BancoClientes.Listar().Any())
                maxId = BancoClientes.Listar().Max(p => p.Id);

            model.Id = maxId + 1;

            Clientes clientes = new Clientes
            {
                Id = model.Id,
                CPF = model.CPF,
                InscricaoEstatudal = model.InscricaoEstatudal,
                Nome = model.Nome,
                NomeFantasia = model.NomeFantasia,
                NumeroEndereco = model.NumeroEndereco,
                Bairro = model.Bairro,
                Cidades = model.Cidade,
                Estado = model.Estado,
                DataNascimento = model.DataNascimento,
            };
            BancoClientes.Adicionar(clientes);
        }
        return RedirectToAction("Index");
    }

    public IActionResult Excluir(int id)
    {
        BancoClientes.Excluir(id);
        return RedirectToAction("Index");
    }

    public IActionResult Exportar(int id)
    {
        var pessoal = BancoClientes.Busca(id);
        string json = System.Text.Json.JsonSerializer.Serialize(pessoal);

        var bytes = Encoding.UTF8.GetBytes(json);

        return File(bytes, "text/json", "dados.json");    
    }
}
