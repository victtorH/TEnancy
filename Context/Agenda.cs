using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ModuloMVC.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ModuloMVC.Context
{
    public class AgendaContext : IdentityDbContext<IdentityUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AgendaContext(
            DbContextOptions<AgendaContext> options,
            IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // 2. A MÁGICA: Uma propriedade dinâmica. 
        // Toda vez que alguém chamar isso, ela faz a leitura AO VIVO do usuário atual.
        public string UserIdLogado
        {
            get
            {
                var id = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return id ?? string.Empty;
            }
        }

        public DbSet<Tarefa> Tarefa { get; set; }
        public DbSet<Contato> Contato { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // 3. O Entity Framework é inteligente e vai usar a propriedade dinâmica!
            modelBuilder.Entity<Tarefa>().HasQueryFilter(t => t.UserId == UserIdLogado);
            modelBuilder.Entity<Contato>().HasQueryFilter(c => c.UserId == UserIdLogado);

            // Aquela nossa trava de segurança contra o erro de Cascata
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

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

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entradas = ChangeTracker.Entries().Where(e => e.State == EntityState.Added);

            foreach (var entrada in entradas)
            {
                var propUserId = entrada.Entity.GetType().GetProperty("UserId");
                if (propUserId != null)
                {
                    // 4. Lemos o ID ao vivo na hora H de salvar
                    var idAoVivo = UserIdLogado;

                    if (string.IsNullOrEmpty(idAoVivo))
                    {
                        throw new Exception("ALERTA: Tentativa de salvar dados sem um usuário logado validado pelo sistema.");
                    }

                    // Carimbamos a tarefa com o ID correto!
                    propUserId.SetValue(entrada.Entity, idAoVivo);
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}