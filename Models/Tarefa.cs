using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModuloMVC.Enum;

namespace ModuloMVC.Models
{
    public class Tarefa
    {
        public int Id { get; private set; }
        public string? Titulo { get; private set; }
        public string? Descricao { get; private set; }
        public DateTime? Vencimento { get; private set; }
        public StatusTarefa Status { get; private set; }

        private readonly List<Contato> _contatosEnvolvidos;
        public IReadOnlyCollection<Contato> ContatosEnvolvidos => _contatosEnvolvidos.AsReadOnly();

        public Tarefa(string? titulo, string? descricao, DateTime? vencimento)
        {
            Validar(titulo, descricao);

            Titulo = titulo;
            Descricao = descricao;
            Vencimento = vencimento;
            Status = StatusTarefa.Pendente;
            _contatosEnvolvidos = new List<Contato>();
        }

        public void AtualizarTarefa(string? titulo, string? descricao, DateTime? vencimento, StatusTarefa status)
        {
            Validar(titulo, descricao); 
            
            Titulo = titulo;
            Descricao = descricao;
            Vencimento = vencimento;
            Status = status;
        }


        public Tarefa()
        {
            _contatosEnvolvidos = new List<Contato>();
        }

        public void AdicionarContato(Contato contato)
        {
            if (contato == null) throw new ArgumentNullException("Os contatos passados não podem ser nulos");

            if (!_contatosEnvolvidos.Contains(contato))
            {
                _contatosEnvolvidos.Add(contato);
            }
        }

        public void AtualizarContatos(IEnumerable<Contato> novosContatos)
        {
            _contatosEnvolvidos.Clear(); 
            foreach (var contato in novosContatos)
            {
                _contatosEnvolvidos.Add(contato); 
            }
        }

        private void Validar(string? titulo, string? descricao)
        {
            if (string.IsNullOrWhiteSpace(titulo) && string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("O Título ou descrição precisão ser preenchidos.");

    
        }
    }
}