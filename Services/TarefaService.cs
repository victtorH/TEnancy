using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModuloMVC.Context;
using ModuloMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using Humanizer;
using ModuloMVC.ViewModels;
using System.ComponentModel.DataAnnotations;
using ModuloMVC.Enum;

namespace ModuloMVC.Services
{
    public class TarefaService
    {
        protected readonly AgendaContext _context;
        public TarefaService(AgendaContext context)
        {
            _context = context;
        }

        public async Task<List<Tarefa>> ListarTodosAsync(string? titulo, DateTime? data, List<StatusTarefa>? status, string? visao)
        {
            var query = _context.Tarefa
            .OrderBy(t => t.Vencimento)
            .Include(t => t.ContatosEnvolvidos)
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(titulo))
            {
                query = query.Where(t => t.Titulo.Contains(titulo));
            }

            if (data.HasValue)
            {

                query = query.Where(t => t.Vencimento.HasValue && t.Vencimento.Value.Date == data.Value.Date);
            }

            if (status != null && status.Any())
            {

                query = query.Where(t => status.Contains(t.Status));
            }

            var hoje = DateTime.Today;

            if (visao == "atrasadas")
            {
                query = query.Where(t => t.Vencimento.HasValue && t.Vencimento.Value.Date < hoje);
            }
            else if (visao == "hoje")
            {

                query = query.Where(t => !t.Vencimento.HasValue || t.Vencimento.Value.Date >= hoje);
            }

            return await query.ToListAsync();
        }

        public async Task<List<Contato>> ListarContatos()
        {
            return await _context.Contato.ToListAsync();
        }



        public async Task<Tarefa> BuscarPorId(int id)
        {
            var tarefa = await _context.Tarefa.FindAsync(id);
            if (tarefa == null)
            {
                throw new ArgumentException("Tarefa solicitado não existe");
            }

            return tarefa;
        }

        public async Task<Tarefa> BuscarComContatosPorIdAsync(int id)
        {
            var tarefa = await _context.Tarefa
                                       .Include(t => t.ContatosEnvolvidos)
                                       .FirstOrDefaultAsync(t => t.Id == id);

            if (tarefa == null) throw new ArgumentException("Tarefa não encontrada.");

            return tarefa;
        }


        private async Task ValidarSeJaExite(string? titulo, string? descricao, DateTime? vencimento, int IgnorarId = 0)
        {

            var validar = await _context.Tarefa.Where(t => t.Titulo == titulo && t.Descricao == descricao && t.Vencimento == vencimento && t.Id != IgnorarId).AnyAsync();
            if (validar)
            {
                throw new InvalidOperationException("Está tarefa já foi criada, por genilezar verificar listagem de tarefas");
            }
        }


        public async Task<Tarefa> CriarUmaAsync(string? titulo, string? descricao, DateTime? vencimento, List<int> Ids)
        {
            await ValidarSeJaExite(titulo, descricao, vencimento);

            if (vencimento < DateTime.Today)
            {
                throw new Exception("A data não pode ser anterior a de hoje");
            }

            var contatos = await _context.Contato
                                         .Where(c => Ids.Contains(c.Id))
                                         .ToListAsync();

            Tarefa novaTarefa = new Tarefa(titulo, descricao, vencimento);
            foreach (var contato in contatos)
            {
                novaTarefa.AdicionarContato(contato);
            }

            await _context.Tarefa.AddAsync(novaTarefa);
            await _context.SaveChangesAsync();

            return novaTarefa;
        }

        public async Task AtualizarUmaAsync(int id, string? titulo, string? descricao, DateTime? vencimento, StatusTarefa status, List<int> idsContatos)
        {

            await ValidarSeJaExite(titulo, descricao, vencimento, id);
            if (vencimento < DateTime.Today)
            {
                throw new Exception("A data não pode ser anterior a de hoje");
            }

            var tarefa = await BuscarComContatosPorIdAsync(id);


            tarefa.AtualizarTarefa(titulo, descricao, vencimento, status);

            var novosContatos = await _context.Contato
                                              .Where(c => idsContatos.Contains(c.Id))
                                              .ToListAsync();

            tarefa.AtualizarContatos(novosContatos);

            _context.Tarefa.Update(tarefa);
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirUm(int id)
        {
            var tarefa = await BuscarPorId(id);
            _context.Tarefa.Remove(tarefa);
            await _context.SaveChangesAsync();
        }

    }
}