namespace APICatalogo.Repository;

public interface IUnityOfWork
{
    public IProdutoRepository ProdutoRepository { get; }
    public ICategoriaRepository CategoriaRepository { get; }
    public Task Commit();
}
