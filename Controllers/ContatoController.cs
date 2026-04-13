using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModuloMVC.Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ModuloMVC.ViewModels;


namespace ModuloMVC.Controllers
{
    [Route("[controller]")]
    public class ContatoController : Controller
    {

        private readonly ContatoService _service;
        public ContatoController(ContatoService service)
        {
            _service = service;
        }


[HttpGet]

public async Task<IActionResult> Index(string? nome, string? numero, string? email, List<bool>? status)
{

    var contatosFiltrados = await _service.ListarTodosAsync(nome, numero, email, status);
    
    return View(contatosFiltrados);
}

        [HttpGet("Criar")]
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost("Criar")]
        public async Task<IActionResult> Criar(ContatoViewModel contato)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new ValidationException("Os valores inserirdos não correspondem ao pedido no sistema");

                }
                    
                await  _service.CriarUm(contato.Nome,contato.Email,contato.Telefone,contato.Descricao);
                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                ModelState.AddModelError(string.Empty, err.Message);
                return View("Criar");
            }

        }

        [HttpGet("Deletar/{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            try
            {
                var contato = await _service.BuscarPorId(id);
                return View(contato);
            }
            catch (Exception err)
            {
                ModelState.AddModelError(string.Empty, err.Message);
                return RedirectToAction("index");
            }
        }

        [HttpPost("Deletar/{id}")]
        public async Task<IActionResult> DeletarConfirmado(int id)
        {
            try
            {
               await  _service.DeletarUm(id);
                return RedirectToAction("index");
            }
            catch (Exception err)
            {
                ModelState.AddModelError(string.Empty, err.Message);
                return RedirectToAction("index");
            }
        }

        [HttpGet("Editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {
           try
            {
                var contato = await _service.BuscarPorId(id);
                return View(contato);
            }
            catch (Exception err)
            {
                ModelState.AddModelError(string.Empty, err.Message);
                return RedirectToAction("index");
            }
        }

        [HttpPost("Editar/{id}")]
        public async Task<IActionResult> SalvarEdicao(int id, ContatoViewModel contato)
        {
            try
            {
              await  _service.EditarUm(id, contato.Nome, contato.Email, contato.Telefone, contato.Status, contato.Descricao );
                return RedirectToAction("Index");
            }
            catch(Exception err)
            {
                ModelState.AddModelError(string.Empty, err.Message);
                return RedirectToAction("index");
            }

        }

        [HttpGet("Detalhes/{id}")]
        public async Task<IActionResult> Detalhes(int id)
        {
           try
            {
                var contato = await _service.BuscarPorId(id);
                return View(contato);
            }
            catch (Exception err)
            {
                ModelState.AddModelError(string.Empty, err.Message);
                return RedirectToAction("index");
            }
        }
    }
}