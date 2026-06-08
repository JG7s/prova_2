public class Banco<Tmodel> : IBanco<Tmodel> where Tmodel : ModelBase
{
    private readonly string arquivo;
    private readonly List<Tmodel> Dados;

    public Banco()
    {
        string pasta = Path.Combine(
            Directory.GetCurrentDirectory(),
             "wwwroot\\Dados"
        );         

        arquivo = Path.Combine(
            pasta, 
            $"{typeof(Tmodel).Name}.json"        
        );

        
        if(File.Exists(arquivo))
        {
            string json = File.ReadAllText(arquivo);

            Dados = System.Text.Json.JsonSerializer.Deserialize<List<Tmodel>>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        else
            Dados = new List<Tmodel>();              
    }

    private void SalvarDados()
    {
        string json = System.Text.Json.JsonSerializer.Serialize(Dados);

        File.WriteAllText(arquivo, json);
    }

    public void Adicionar(Tmodel model)
    
    {
        Dados.Add(model);
        SalvarDados();
    }

    

    public void Alterar(int id, Tmodel model)
    {
        
        int index = Dados.FindIndex(p => p.Id == id);

        if (index >= 0)
        {
            Dados[index] = model;
            SalvarDados();
        }
    }

    public void Excluir(int id)
    {
        var bdModel = Dados.FirstOrDefault(p => 
            p.Id == id);
        
        Dados.Remove(bdModel);
        SalvarDados();
    }

    public List<Tmodel> Listar()
    {
        return Dados.ToList();
    }

    public Tmodel Busca(int id)
    {
        return Dados.FirstOrDefault(p => 
            p.Id == id);
    }
}