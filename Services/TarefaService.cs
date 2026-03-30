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
            // 1. Inicia a consulta base (O caderninho de anotações do EF Core)
            var query = _context.Tarefa
            .OrderBy(t => t.Vencimento)
            .Include(t => t.ContatosEnvolvidos)
            .AsQueryable();

            // 2. Filtro 1: Título (Procura se a palavra digitada existe no título)
            if (!string.IsNullOrWhiteSpace(titulo))
            {
                query = query.Where(t => t.Titulo.Contains(titulo));
            }

            // 3. Filtro 2: Data de Vencimento
            if (data.HasValue)
            {
                // Comparamos apenas a "Date" (dia/mês/ano), ignorando a hora (00:00:00) para evitar bugs de fuso horário
                query = query.Where(t => t.Vencimento.HasValue && t.Vencimento.Value.Date == data.Value.Date);
            }

            // 4. Filtro 3: Status (Pode vir 1, 2 ou 3 checkboxes marcados)
            if (status != null && status.Any())
            {
                // O Contains aqui funciona magicamente como um "IN (1, 2)" no SQL
                query = query.Where(t => status.Contains(t.Status));
            }

            // 5. Só agora ele vai ao banco de dados e traz os resultados refinados!
            var hoje = DateTime.Today;

            if (visao == "atrasadas")
            {
                // Vencimento menor que hoje (ontem para trás)
                query = query.Where(t => t.Vencimento.HasValue && t.Vencimento.Value.Date < hoje);
            }
            else if (visao == "hoje")
            {
                // Vencimento maior ou igual a hoje (hoje e os próximos dias)
                // OBS: Traz também as tarefas sem vencimento para não sumirem da tela principal
                query = query.Where(t => !t.Vencimento.HasValue || t.Vencimento.Value.Date >= hoje);
            }
            // Se for "todas", não fazemos nenhum If. O EF Core vai trazer o banco inteiro!

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
                                       .Include(t => t.ContatosEnvolvidos) // A MÁGICA ACONTECE AQUI
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
            var contatos = await _context.Contato
                                         .Where(c => Ids.Contains(c.Id))
                                         .ToListAsync();

            await ValidarSeJaExite(titulo, descricao, vencimento);

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
            // 1. Busca a tarefa original no banco (já com os contatos atuais usando o .Include)
            var tarefa = await BuscarComContatosPorIdAsync(id);

            await ValidarSeJaExite(titulo, descricao, vencimento, id);

            tarefa.AtualizarTarefa(titulo, descricao, vencimento, status);

            // 3. Busca os novos contatos que o usuário marcou nos checkboxes
            var novosContatos = await _context.Contato
                                              .Where(c => idsContatos.Contains(c.Id))
                                              .ToListAsync();

            // 4. Usa o novo método para trocar os contatos
            tarefa.AtualizarContatos(novosContatos);

            // 5. Salva a mágica toda no banco de dados
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