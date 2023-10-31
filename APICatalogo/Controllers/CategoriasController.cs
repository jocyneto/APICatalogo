using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using APICatalogo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace APICatalogo.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/[controller]")]
[ApiController]
//[Authorize(AuthenticationSchemes = "Bearer")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CategoriasController : ControllerBase
{
    public readonly IUnityOfWork _uof;
    public readonly IMapper _mapper;
    public CategoriasController(IUnityOfWork context, IMapper mapper)
    {
        _uof = context;
        _mapper = mapper;
    }

    [HttpGet("saudacao/{nome}")]
    public ActionResult<string> GetSaudacao([FromServices] IMeuServico ms, string nome)
    {
        return ms.GetSaudacao(nome);
    }

    /* # COMENTADO TEMPORARIAMENTE PARA UTILIZAR O UNITY OF WORK APENAS COM METODOS SINCRONOS.
     * 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetAsync()
    {
        var categorias = await _uof.Categorias.AsNoTracking().ToListAsync();

        if (categorias is null)
            return NotFound("Lista de categorias não existente...");

        return categorias;
    }
    */

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasParameters categoriasParameters)
    {
        var categorias = await _uof.CategoriaRepository.GetCategorias(categoriasParameters);

        var metadata = new
        {
            categorias.TotalCount,
            categorias.PageSize,
            categorias.CurrentPage,
            categorias.TotalPages,
            categorias.HasPrevious,
            categorias.HasNext
        };
        //NET 5.0 ~~> JsonConvert.SerializeObject(metadata)
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));



        var categoriaDto = _mapper.Map<List<CategoriaDTO>>(categorias);


        if (categoriaDto is null)
            return NotFound("Lista de categorias não existente...");

        return categoriaDto;
    }

    [HttpGet("produtos")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutos()
    {
        /*O metodo de extensão INCLUDE permite carregar entidades relacionadas*/
        var categorias = await _uof.CategoriaRepository.GetCategoriasProdutos();
        var categoriaDto = _mapper.Map<List<CategoriaDTO>>(categorias);

        if (categorias is null)
            return NotFound("Lista de categorias não existente...");

        return categoriaDto;
    }


    /* # COMENTADO TEMPORARIAMENTE PARA UTILIZAR O UNITY OF WORK APENAS COM METODOS SINCRONOS.
     * 
    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task<ActionResult<Categoria>> GetAsync(int id)
    {
        var categoria = await _uof.Categorias.FirstOrDefaultAsync(cat => cat.CategoriaId == id);

        if (categoria is null)
            return NotFound("Categoria procurada não existente...");

        return categoria;
    }
    */

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task<ActionResult<CategoriaDTO>> Get(int id)
    {
        var categoria = await _uof.CategoriaRepository.GetById(cat => cat.CategoriaId == id);
        var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

        if (categoria is null)
            return NotFound("Categoria procurada não existente...");

        return categoriaDto;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CategoriaDTO categoriaDto)
    {
        var categoria = _mapper.Map<Categoria>(categoriaDto);
        if (categoria is null)
            return BadRequest();

        _uof.CategoriaRepository.Add(categoria);
        await _uof.Commit();

        var viewCategoriaDto = _mapper.Map<CategoriaDTO>(categoria);


        return new CreatedAtRouteResult("ObterCategoria", new {id=categoria.CategoriaId}, viewCategoriaDto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, CategoriaDTO categoriaDto)
    {
        if(id != categoriaDto.CategoriaId)
            return BadRequest();

        var categoria = _mapper.Map<Categoria>(categoriaDto);

        _uof.CategoriaRepository.Update(categoria);
        await _uof.Commit();

        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<CategoriaDTO>> Delete(int id)
    {
        var categoria = await _uof.CategoriaRepository.GetById(cat => cat.CategoriaId==id);

        if (categoria is null)
            return NotFound("Produto não encontrado...");

        _uof.CategoriaRepository.Delete(categoria);
        await _uof.Commit();

        var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

        return categoriaDto;
    }

}
