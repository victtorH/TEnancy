using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using ModuloMVC.Enum;
using ModuloMVC.Models;
using ModuloMVC.Services;
using ModuloMVC.ViewModels;

namespace ModuloMVC.Controllers
{
    [Authorize]
    public class TarefaController : Controller
    {
        private readonly TarefaService _service;

        public TarefaController(TarefaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? titulo, DateTime? data, List<StatusTarefa>? status, string? visao = "hoje")
        {

            ViewBag.VisaoAtual = visao;

            var listaFiltrada = await _service.ListarTodosAsync(titulo, data, status, visao);

            return View(listaFiltrada);
        }



        [HttpGet]
        public async Task<IActionResult> Criar()
        {
            var contatosDoBanco = await _service.ListarContatos();
            var contatosParaTela = contatosDoBanco.Select(c => new ContatoViewModel
            {
                Id = c.Id,
                Nome = c.Nome,
                Email = c.Email,
                Telefone = c.Telefone,
                Status = c.Status
            }).ToList();

            var viewModel = new TarefaViewModel
            {
                ContatosEnvolvidos = contatosParaTela
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Criar(string RotaDeRetorno, TarefaViewModel tarefa)
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

                if (!string.IsNullOrEmpty(RotaDeRetorno))
                    return Redirect(RotaDeRetorno);

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

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var tarefa = await _service.BuscarComContatosPorIdAsync(id);
            var contatosDoBanco = await _service.ListarContatos();


            var viewModel = new TarefaEdicaoViewModel
            {
                Id = tarefa.Id,

                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Vencimento = tarefa.Vencimento,
                Status = tarefa.Status,
                ContatosSelecionadosIds = tarefa.ContatosEnvolvidos.Select(c => c.Id).ToList(),

                TituloAtual = tarefa.Titulo,
                DescricaoAtual = tarefa.Descricao,
                VencimentoAtual = tarefa.Vencimento,
                StatusAtual = tarefa.Status,
                ContatosEnvolvidosAtuais = tarefa.ContatosEnvolvidos.Select(c => new ContatoViewModel { Nome = c.Nome }).ToList(),

                TodosContatosDisponiveis = contatosDoBanco.Select(c => new ContatoViewModel { Id = c.Id, Nome = c.Nome, Email = c.Email }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, TarefaEdicaoViewModel model, string RotaDeRetorno)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var contatosDoBanco = await _service.ListarContatos();
                    model.TodosContatosDisponiveis = contatosDoBanco.Select(c => new ContatoViewModel { Id = c.Id, Nome = c.Nome, Email = c.Email }).ToList();
                    return View(model);
                }

                var listaIds = model.ContatosSelecionadosIds ?? new List<int>();

                await _service.AtualizarUmaAsync(id, model.Titulo, model.Descricao, model.Vencimento, model.Status, listaIds);

                if (!string.IsNullOrEmpty(RotaDeRetorno))
                    return Redirect(RotaDeRetorno);

                return RedirectToAction("Index");

            }
            catch (Exception err)
            {
                var contatosDoBanco = await _service.ListarContatos();
                model.TodosContatosDisponiveis = contatosDoBanco.Select(c => new ContatoViewModel { Id = c.Id, Nome = c.Nome, Email = c.Email }).ToList();

                TempData["CriarDublicado"] = "Erro Mensagem:  " + err.Message;
                return View(model);
            }

        }


        [HttpPost]
        public async Task<IActionResult> Excluir(int id, string RotaDeRetorno)
        {
            try
            {
                await _service.ExcluirUm(id);

                TempData["ExcluirTarefa"] = "1";

                if (!string.IsNullOrEmpty(RotaDeRetorno))
                    return Redirect(RotaDeRetorno);

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