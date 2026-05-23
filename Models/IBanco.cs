public interface IBanco<TModel>
{
    void Adicionar(TModel model);   
    void Alterar(int id, TModel model);    
    void Excluir(int id);    
    List<TModel> Listar();  
    TModel Busca(int id);  

}
