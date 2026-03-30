using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModuloMVC.Models
{
    public class Contato
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Telefone { get; private set; }
        public bool Status { get; private set; }
        public string? Descricao { get; private set; }

        public ICollection<Tarefa> Tarefas { get; private set; }

        public Contato(string nome, string email, string telefone, string? descricao)
        {

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(telefone))
            {
                throw new ArgumentException("O Telefone e Email são obrigatórios.");
            }


            Nome = nome;
            Email = email;
            Telefone = telefone;
            Status = true;
            Descricao = descricao;

        }

        public Contato() { }

        public void AtualizarDados(string nome, string email, string telefone, bool status, string? descricao)
        {
            Validar(nome, email, telefone);

            Nome = nome;
            Email = email;
            Telefone = telefone;
            Status = status;
            Descricao = descricao;
        }


        private void Validar(string nome, string email, string telefone)
        {


            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(telefone))
                throw new ArgumentException("Dados obrigatorios não foram preenchidos.");


            if (!email.Contains("@"))
                throw new ArgumentException("Formato de E-mail inválido.");
        }
    }
}
