using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModuloMVC.Enum;
using ModuloMVC.Models;
using ModuloMVC.Services;
using ModuloMVC.ViewModels;

namespace ModuloMVC.Controllers
{
    [Route("[controller]")]
    public class TarefaController : Controller
    {
        private readonly TarefaService _service;

        public TarefaController(TarefaService service)
        {
            _service = service;
        }

[HttpGet]
public async Task<IActionResult> Index(string? titulo, DateTime? data, List<StatusTarefa>? status,string? visao = "hoje")
{
    
    ViewBag.VisaoAtual = visao;

    var listaFiltrada = await _service.ListarTodosAsync(titulo, data, status, visao);
    
    return View(listaFiltrada);
}



        [HttpGet("Criar")]
        // 1. O método precisa ser async porque vamos no banco buscar os contatos
        public async Task<IActionResult> Criar()
        {
            // 2. Busca os contatos originais (Entidades ricas) lá do banco de dados
            var contatosDoBanco = await _service.ListarContatos(); // Ou _context.Contatos.ToListAsync();

            // 3. A Tradução: Transforma a lista de 'Contato' em 'ContatoViewModel'
            var contatosParaTela = contatosDoBanco.Select(c => new ContatoViewModel
            {
                Id = c.Id,
                Nome = c.Nome,
                Email = c.Email,
                Telefone = c.Telefone,
                Status = c.Status
            }).ToList();

            // 4. Cria a "mala" que vai viajar para a View, já com os contatos dentro
            var viewModel = new TarefaViewModel
            {
                ContatosEnvolvidos = contatosParaTela
            };


            return View(viewModel);
        }


        [HttpPost("Criar")]
        public async Task<IActionResult> Criar(TarefaViewModel tarefa)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var contatosDoBanco = await _service.ListarContatos();
                    tarefa.ContatosEnvolvidos = contatosDoBanco.Select(c => new ContatoViewModel { Id = c.Id, Nome = c.Nome }).ToList();

                    return View(tarefa);
                }
                var ListaIds = tarefa.ContatosSelecionadosIds ?? new List<int>();

                var TarefaNova = await _service.CriarUmaAsync(tarefa.Titulo, tarefa.Descricao, tarefa.Vencimento, ListaIds);
                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                var contatosDoBanco = await _service.ListarContatos();
                tarefa.ContatosEnvolvidos = contatosDoBanco.Select(c => new ContatoViewModel { Id = c.Id, Nome = c.Nome }).ToList();
                TempData["CriarDublicado"] = "Erro Mensagem:  " + err.Message;
                return View(tarefa);
            }


        }

        [HttpGet("Editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {
            // Busca a tarefa e os contatos possíveis
            var tarefa = await _service.BuscarComContatosPorIdAsync(id);
            var contatosDoBanco = await _service.ListarContatos();

            // Monta a mala com os dois lados da tela
            var viewModel = new TarefaEdicaoViewModel
            {
                Id = tarefa.Id,

                // Dados para preencher os Inputs (O que pode mudar)
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Vencimento = tarefa.Vencimento,
                Status = tarefa.Status,
                ContatosSelecionadosIds = tarefa.ContatosEnvolvidos.Select(c => c.Id).ToList(),

                // Dados para o painel esquerdo (A foto de como estava)
                TituloAtual = tarefa.Titulo,
                DescricaoAtual = tarefa.Descricao,
                VencimentoAtual = tarefa.Vencimento,
                StatusAtual = tarefa.Status,
                ContatosEnvolvidosAtuais = tarefa.ContatosEnvolvidos.Select(c => new ContatoViewModel { Nome = c.Nome }).ToList(),

                // Contatos para desenhar os checkboxes
                TodosContatosDisponiveis = contatosDoBanco.Select(c => new ContatoViewModel { Id = c.Id, Nome = c.Nome, Email = c.Email }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost("Editar/{id}")]
        public async Task<IActionResult> Editar(int id, TarefaEdicaoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Se der erro, recarregamos as listas para a tela não quebrar
                    var contatosDoBanco = await _service.ListarContatos();
                    model.TodosContatosDisponiveis = contatosDoBanco.Select(c => new ContatoViewModel { Id = c.Id, Nome = c.Nome, Email = c.Email }).ToList();
                    return View(model);
                }

                var listaIds = model.ContatosSelecionadosIds ?? new List<int>();

                await _service.AtualizarUmaAsync(id, model.Titulo, model.Descricao, model.Vencimento, model.Status, listaIds);

                return RedirectToAction("Index");
            }
            catch(Exception err)
            {
                var contatosDoBanco = await _service.ListarContatos();
                model.TodosContatosDisponiveis = contatosDoBanco.Select(c => new ContatoViewModel { Id = c.Id, Nome = c.Nome, Email = c.Email }).ToList();

                TempData["CriarDublicado"] = "Erro Mensagem:  " + err.Message;
                return View(model);
            }

        }


        [HttpPost("Excluir")]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                await _service.ExcluirUm(id);

                TempData["ExcluirTarefa"] = "1";
                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                TempData["ExcluirTarefa"] = "Não foi possível excluir: " + err.Message;
                return RedirectToAction("Editar", new { id = id });
            }


        }
    }
}