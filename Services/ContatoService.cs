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
            var query = _context.Contato
            .OrderBy(c => c.Nome)
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
            {
                query = query.Where(c => c.Nome.Contains(nome));
            }

            if (!string.IsNullOrWhiteSpace(numero))
            {
                query = query.Where(c => c.Telefone.Contains(numero)); 
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(c => c.Email.Contains(email));
            }

      
            if (status != null && status.Any())
            {
                query = query.Where(c => status.Contains(c.Status));
            }

           
            return await query.ToListAsync();
        }

        private async Task ValidarSeJaExiste(string email, string telefone, int idDesconsiderado = 0)
        {

            bool contatoDuplicado = await _context.Contato.AnyAsync(c =>
                (c.Email == email || c.Telefone == telefone) &&
                c.Id != idDesconsiderado);

            if (contatoDuplicado)
            {
                throw new InvalidOperationException("Já existe um contato cadastrado com este E-mail ou Telefone.");
            }
        }

        public async Task CriarUm(string nome, string email, string telefone, string? descricao)
        {
           await ValidarSeJaExiste(email, telefone);

            Contato NovoContato =  new Contato(nome, email, telefone, descricao);

           await _context.Contato.AddAsync(NovoContato);
            await _context.SaveChangesAsync();
        }


        public async Task<Contato> BuscarPorId(int id)
        {
            var contato = await _context.Contato.FindAsync(id);
            if (contato == null)
            {
                throw new ArgumentException("Contato solicitado não existe");
            }

            return contato;
        }

        public async Task DeletarUm(int id)
        {

            _context.Contato.Remove( await BuscarPorId(id));
      await _context.SaveChangesAsync();
        }


        public async Task EditarUm(int id, string nome, string email, string telefone, bool status, string? descricao)
        {

            var contatodb = await BuscarPorId(id);
            await  ValidarSeJaExiste(contatodb.Nome, contatodb.Email, id);


            contatodb.AtualizarDados(nome, email, telefone, status, descricao);
           await _context.SaveChangesAsync();
        }


    }
}