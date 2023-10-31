using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace APICatalogo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    public readonly IUnityOfWork _uof;
    public readonly IMapper _mapper;

    public object JsonConvert { get; private set; }

    public ProdutosController(IUnityOfWork context, IMapper mapper)
    {
        _uof = context;
        _mapper = mapper;
    }

    /* # COMENTADO TEMPORARIAMENTE PARA UTILIZAR O UNITY OF WORK APENAS COM METODOS SINCRONOS.
     * 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> GetAsync()
    {
        var produtos = await _uof.Produtos.AsNoTracking().ToListAsync();
        if(produtos is null)
        {
            return NotFound("Lista de produtos Vazia ou não encontrada...");
        }
        
        return produtos;
    }
    */
    [HttpGet("menorpreco")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutoPrecos()
    {
        var produtos = await _uof.ProdutoRepository.GetProdutoPorPreco();
        var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);
       
        return produtosDto;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
    {
        var produtos = await _uof.ProdutoRepository.GetProdutos(produtosParameters);

        var metadata = new
        {
            produtos.TotalCount,
            produtos.PageSize,
            produtos.CurrentPage,
            produtos.TotalPages,
            produtos.HasPrevious,
            produtos.HasNext
        };
                                //NET 5.0 ~~> JsonConvert.SerializeObject(metadata)
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

        var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);


        if (produtosDto is null)
        {
            return NotFound("Lista de produtos Vazia ou não encontrada...");
        }

        return produtosDto;
    }

    /* # COMENTADO TEMPORARIAMENTE PARA UTILIZAR O UNITY OF WORK APENAS COM METODOS SINCRONOS.
    
    [HttpGet("{id:int}", Name="ObterProduto")]
    public async Task<ActionResult<Produto>> GetAsync(int id)
    {
        var produto = await _uof.Produtos.FirstOrDefaultAsync(p => p.ProdutoId == id);
        if(produto is null)
        {
            return NotFound("Produto não encontrado...");
        }
        return produto;
    }
    */

    [HttpGet("{id:int}", Name = "ObterProduto")]
    public async Task<ActionResult<ProdutoDTO>> Get(int id)
    {
        var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);
        var produtoDto = _mapper.Map<ProdutoDTO>(produto);

        if (produtoDto is null)
        {
            return NotFound("Produto não encontrado...");
        }
        return produtoDto;
    }

    [HttpPost]
    public async Task<ActionResult> Post(ProdutoDTO produtoDto)
    {
        var produto = _mapper.Map<Produto>(produtoDto);

        if (produto is null)
            return BadRequest();

        _uof.ProdutoRepository.Add(produto);
        await _uof.Commit();

        var viewProdutoDto = _mapper.Map<ProdutoDTO>(produto);

        return new CreatedAtRouteResult("ObterProduto", new {id = produto.ProdutoId}, viewProdutoDto);

    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, ProdutoDTO produtoDto)
    {
        if (id != produtoDto.ProdutoId)
            return BadRequest();

        var produto = _mapper.Map<Produto>(produtoDto);

        _uof.ProdutoRepository.Update(produto);
        await _uof.Commit();

        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProdutoDTO>> Delete(int id)
    {
        var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

        if(produto is null)
        {
            return NotFound("Produto não encontrado...");
        }

        _uof.ProdutoRepository.Delete(produto);
        await _uof.Commit();

        var produtoDto = _mapper.Map<ProdutoDTO>(produto);

        return produtoDto;
    }

}
