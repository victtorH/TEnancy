using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ModuloMVC.Context;
using ModuloMVC.Models;


namespace ModuloMVC.Services
{
    public class ContatoService
    {


        protected readonly AgendaContext _context;
        public ContatoService(AgendaContext context)
        {
            _context = context;
        }


        public async Task<List<Contato>> ListarTodosAsync(string? nome, string? numero, string? email, List<bool>? status)
        {
            // 1. Inicia o IQueryable (O "caderninho de intenções" do EF Core)
            var query = _context.Contato.AsQueryable();

            // 2. Filtro por Nome (Contém a palavra)
            if (!string.IsNullOrWhiteSpace(nome))
            {
                query = query.Where(c => c.Nome.Contains(nome));
            }

            // 3. Filtro por Telefone/Número (Contém a palavra)
            if (!string.IsNullOrWhiteSpace(numero))
            {
                query = query.Where(c => c.Telefone.Contains(numero)); // Ajuste "Telefone" se sua propriedade tiver outro nome
            }

            // 4. Filtro por E-mail (Contém a palavra)
            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(c => c.Email.Contains(email));
            }

            // 5. Filtro por Status (Ativo, Inativo, ou ambos se os dois checkboxes estiverem marcados)
            if (status != null && status.Any())
            {
                // O Contains faz um "IN ('Ativo', 'Inativo')" no SQL!
                query = query.Where(c => status.Contains(c.Status));
            }

            // 6. Vai ao banco de dados e executa a query final
            return await query.ToListAsync();
        }

        private void ValidarSeJaExiste(string email, string telefone, int idDesconsiderado = 0)
        {

            bool contatoDuplicado = _context.Contato.Any(c =>
                (c.Email == email || c.Telefone == telefone) &&
                c.Id != idDesconsiderado);

            if (contatoDuplicado)
            {
                throw new InvalidOperationException("Já existe um contato cadastrado com este E-mail ou Telefone.");
            }
        }

        public void CriarUm(string nome, string email, string telefone, string? descricao)
        {
            ValidarSeJaExiste(email, telefone);

            Contato NovoContato = new Contato(nome, email, telefone, descricao);

            _context.Contato.Add(NovoContato);
            _context.SaveChanges();
        }


        public Contato BuscarPorId(int id)
        {
            var contato = _context.Contato.Find(id);
            if (contato == null)
            {
                throw new ArgumentException("Contato solicitado não existe");
            }

            return contato;
        }

        public void DeletarUm(int id)
        {

            _context.Contato.Remove(BuscarPorId(id));
            _context.SaveChanges();
        }


        public void EditarUm(int id, string nome, string email, string telefone, bool status, string? descricao)
        {

            var contatodb = BuscarPorId(id);
            ValidarSeJaExiste(contatodb.Nome, contatodb.Email, id);


            contatodb.AtualizarDados(nome, email, telefone, status, descricao);
            _context.SaveChanges();
        }


    }
}