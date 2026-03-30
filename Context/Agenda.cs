using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuloMVC.Models;

namespace ModuloMVC.Context
{
    public class AgendaContext : DbContext
    {
        public AgendaContext(DbContextOptions<AgendaContext> options) : base(options)
        {
        }

        public DbSet<Contato> Contato { get; set; }
        public DbSet<Tarefa> Tarefa { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- CONFIGURAÇÃO DA TAREFA DIRETO NO DBCONTEXT ---

            var tarefaBuilder = modelBuilder.Entity<Tarefa>();

            tarefaBuilder.HasKey(t => t.Id);

            tarefaBuilder.Property(t => t.Titulo).IsRequired(false).HasMaxLength(200);
            tarefaBuilder.Property(t => t.Descricao).IsRequired(false).HasMaxLength(2000);

            // A trava de segurança no banco que conversamos
            tarefaBuilder.HasCheckConstraint("CK_Tarefa_TituloOuDescricao_Requerido",
                "([Titulo] IS NOT NULL AND [Titulo] <> '') OR ([Descricao] IS NOT NULL AND [Descricao] <> '')");

            // Ensinando o EF Core a ler a sua lista privada (Backing Field)
            tarefaBuilder.Navigation(t => t.ContatosEnvolvidos)
                         .HasField("_contatosEnvolvidos")
                         .UsePropertyAccessMode(PropertyAccessMode.Field);

            tarefaBuilder.Property(t => t.Status)
                        .HasConversion<string>();
        }
    }
}