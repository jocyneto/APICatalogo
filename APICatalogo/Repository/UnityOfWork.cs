using APICatalogo.Context;

namespace APICatalogo.Repository;

public class UnityOfWork : IUnityOfWork
{

    private ProdutoRepository _produtoRepository;
    private CategoriaRepository _categoriaRepository;
    public AppDbContext _context;

    public UnityOfWork(AppDbContext context)
    {
        this._context = context;
    }


    public IProdutoRepository ProdutoRepository
    {
        get
        {
            return this._produtoRepository = this._produtoRepository ?? new ProdutoRepository(_context);
            //Verifica se uma instância de repositório é nula:
                //-> Se ela for nula eu passo a instância do contexto que foi injetada no construtor;
                //-> Se não, eu passo a instância que já existe do reppositório.
        }
    }

    public ICategoriaRepository CategoriaRepository 
    { 
        get
        {
            return this._categoriaRepository = this._categoriaRepository ?? new CategoriaRepository(_context);
        }
    }


    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
