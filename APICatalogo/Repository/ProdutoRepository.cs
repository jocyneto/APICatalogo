using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace APICatalogo.Repository;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Produto>> GetProdutoPorPreco()
    {
        return await Get().OrderBy(p => p.Preco).ToListAsync();
    }

    public async Task<PagedList<Produto>> GetProdutos(ProdutosParameters produtosParameters)
    {
        //return Get()
        //    .OrderBy(on => on.Nome)
        //    .Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
        //    .Take(produtosParameters.PageSize)
        //    .ToList();

        return await PagedList<Produto>.ToPagedList(Get().OrderBy(on => on.ProdutoId), produtosParameters.PageNumber, produtosParameters.PageSize);
    }
}
