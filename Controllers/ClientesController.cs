using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize]
public class ClientesController : Controller
{
    private readonly IBanco<Clientes> BancoClientes;
    private readonly IBanco<Cidade> BancoCidades;
    public ClientesController()
    {
        BancoClientes = new Banco<Clientes>();        
        BancoCidades = new Banco<Cidade>();
    }
    public IActionResult Index()
    {
        var teste = BancoCidades.Listar();
        var clientes = BancoClientes.Listar();

        return View(clientes.Select(p => new ClientesViewModel
        {
            Id = p.Id,
            CPF = p.CPF,
            InscricaoEstatudal = p.InscricaoEstatudal,
            Nome = p.Nome,
            NomeFantasia = p.NomeFantasia,
            NumeroEndereco = p.NumeroEndereco,
            Bairro = p.Bairro,
            Cidade = p.Cidade,
            Estado = p.Estado,
            DataNascimento = p.DataNascimento
        }));
    }

    public IActionResult RegistroClientes(int id = 0)
    {
        var cidades = BancoCidades.Listar().Select(
            p => new SelectListItem
            {
                Value = p.Nome,
                Text = p.Nome
            }
        ).ToList();

        // EDITAR
        if (id != 0)
        {
            Clientes clientes = BancoClientes.Listar()
                .FirstOrDefault(p => p.Id == id);

            return View(new ClientesViewModel
            {
                Id = clientes.Id,
                CPF = clientes.CPF,
                CaminhoImagem = clientes.CaminhoImagem,
                InscricaoEstatudal = clientes.InscricaoEstatudal,
                Nome = clientes.Nome,
                NomeFantasia = clientes.NomeFantasia,
                NumeroEndereco = clientes.NumeroEndereco,
                Bairro = clientes.Bairro,
                Cidade = clientes.Cidade,
                Estado = clientes.Estado,
                DataNascimento = clientes.DataNascimento,
                Cidades = cidades
            });
        }

        // NOVO
        return View(new ClientesViewModel
        {
            Id = 0,
            Cidades = cidades
        });
    }

    [HttpPost]
    public async Task<IActionResult> SalvarAsync(ClientesViewModel model)
    {
        // Se tiver erro no formulário
        if (!ModelState.IsValid)
        {
            // Recarrega as cidades
            model.Cidades = BancoCidades.Listar().Select(
                p => new SelectListItem
                {
                    Value = p.Nome,
                    Text = p.Nome
                }
            ).ToList();

            return View("RegistroClientes", model);
        }

        // ALTERAR
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
                Cidade = model.Cidade,
                Estado = model.Estado,
                DataNascimento = model.DataNascimento,
            };

            

            string pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads");
            string nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(model.Imagem.FileName);
            string caminho = Path.Combine(pasta, nomeArquivo);
        
            using(var strem = new FileStream(caminho, FileMode.Create))
            {
                await model.Imagem.CopyToAsync(strem);
            }   

            clientes.CaminhoImagem = $"/uploads/{nomeArquivo}";

            BancoClientes.Alterar(model.Id, clientes);

            TempData["Mensagem"] = "Cliente alterado com sucesso!";
        }

        // NOVO CADASTRO
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
                Cidade = model.Cidade,
                Estado = model.Estado,
                DataNascimento = model.DataNascimento,
            };

            string pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads");
            string nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(model.Imagem.FileName);
            string caminho = Path.Combine(pasta, nomeArquivo);
        
            using(var strem = new FileStream(caminho, FileMode.Create))
            {
                await model.Imagem.CopyToAsync(strem);
            }   

            clientes.CaminhoImagem = $"/uploads/{nomeArquivo}";
            BancoClientes.Adicionar(clientes);

            TempData["Mensagem"] = "Cliente cadastrado com sucesso!";
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
