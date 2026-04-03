# 🚀 Agenda Inteligente: Meu Projeto Prático Full-Stack
Bem-vindo ao repositório do meu sistema de produtividade e gestão de contatos! Este é um projeto prático, criado como um laboratório pessoal para estudar e aplicar conceitos avançados de engenharia de software no backend, além de boas práticas de UI/UX no frontend.

Meu objetivo aqui é ir além do CRUD básico, documentando minha evolução técnica, testando diferentes versões e, futuramente, implementando testes rigorosos de segurança.\
</br>
<img width="1853" height="903" alt="Image" src="https://github.com/user-attachments/assets/4ee1cbb4-f583-481d-8fd3-cfe9391e7916" />
<img width="530" height="889" alt="Image" src="https://github.com/user-attachments/assets/a38e1d14-d7f8-4043-bb18-dbe9db2e5709" />
<img width="302" height="883" alt="Image" src="https://github.com/user-attachments/assets/685a4ac2-5b37-4f86-bbc3-907329f470e9" />
## Tecnologias e Arquitetura em Estudo
Este projeto está sendo construído para consolidar meus conhecimentos no ecossistema Microsoft e em padrões de projeto.

**Backend:** .NET 8 LTS e C# 12, utilizando o padrão ASP.NET Core MVC Razor.

**Banco de Dados:** SQL Server com Entity Framework Core 8 (Code-First).

**Arquitetura:** Estou aplicando ativamente princípios de Domain-Driven Design (DDD) e SOLID. O foco é evitar modelos anêmicos, criando Entidades ricas, uma Camada de Serviço para as regras de negócio e Controllers enxutas.

**Frontend:** Design System construído com CSS puro (usando variáveis globais) e preocupação constante com responsividade e usabilidade (UI/UX).

## Como Rodar Localmente
   
Se quiser clonar este projeto para ver o código em ação, o processo é direto:

1. Certifique-se de ter o SDK do .NET 8 instalado e acesso a uma instância do SQL Server (o LocalDB do Visual Studio funciona perfeitamente).
2. Faça o clone do repositório: git clone https://github.com/victtorH/ModuloMVC.git
3. Abra a solução no Visual Studio 2022 ou VS Code.
4. No arquivo appsettings.json, configure a DefaultConnection para apontar para o seu banco de dados.
5. Certifique-se de que possui o EF Core Tools instalado e adicione os pacotes NuGet na versão 8.0.x: `Microsoft.EntityFrameworkCore.Design` e `Microsoft.EntityFrameworkCore.SqlServer`.
6. No Package Manager Console (ou no terminal do EF), rode: Update-Database. O Entity Framework criará as tabelas via Migrations.
7. Rode o projeto (F5 ou `dotnet run`).

## Estrutura do Código
A base do sistema foi organizada com um isolamento claro de responsabilidades, algo que tenho priorizado no meu aprendizado:

**Models:** Classes blindadas com setters privados, onde a própria entidade valida suas regras.

**ViewModels:** Classes de transporte de dados para as Views, garantindo que o domínio não seja exposto acidentalmente.

**Services:** Responsáveis por coordenar as entidades e lidar com I/O assíncrono.

**Controllers:** Focadas apenas em capturar requisições HTTP, utilizando o padrão Post-Redirect-Get (PRG) e TempData para evitar reenvio acidental de formulários.

## Roadmap e Próximas Versões
Por ser um projeto de estudo em constante evolução, defini um roadmap claro para acompanhar o versionamento:

- [x] **Relações Complexas:** Implementação de conexões Muitos-para-Muitos (N:N), permitindo atrelar múltiplos contatos a uma tarefa de forma robusta.

- [ ] **Fase V1 (MVP):** Foco em finalizar um ecossistema base single-tenant (individual) com quatro raízes principais: Usuários, Contatos, Tarefas e Anotações (com suporte a textos longos) e a capacidade de gerar tarefas a partir de anotações.

- [ ] **Fase de Segurança (Testes e Refatoração):** Esta será uma etapa crucial do meu aprendizado. Planejo criar branches específicas para auditar o código, realizar testes de vulnerabilidade (ex: prevenção de SQL Injection, XSS, CSRF), blindar as rotas de autenticação/autorização e testar a resiliência da aplicação.

## Desafios Enfrentados e Aprendizados
Construir isso do zero trouxe desafios práticos excelentes para a minha evolução:

**Estado de Navegação:** Lidar com a perda de filtros em Query Strings ao voltar de uma edição. Resolvi criando uma lógica híbrida com ViewBag (C#) e history.back() (JS).

**Armadilhas de Data e Hora:** O uso inicial de DateTime.Now impedia agendamentos para o dia atual devido aos milissegundos. Aprendi a lidar com isso adotando DateTime.Today e aplicando o princípio de Fail-Fast nas validações.

**Injeção de Dependência vs. "Classe Deus":** Resisti à tentação de criar um ServiceBase genérico (anti-pattern), focando em injeção de dependência limpa para cada serviço.

**UX Mobile:** Apliquei princípios de usabilidade (como a Lei de Fitts) para criar "hitboxes" maiores em botões e links no mobile, além de isolar o CSS para evitar conflitos com o ASP.NET.

## Feedback e Contribuições
Sendo este um dos meus primeiros grandes projetos práticos, todo feedback é extremamente bem-vindo.

Se você é um desenvolvedor mais experiente e tem dicas sobre arquitetura de software, padrões SOLID, otimização de consultas LINQ ou sugestões de segurança para a minha próxima fase de testes, sinta-se à vontade para abrir uma Issue ou me chamar para um code review. Estou aqui para aprender e evoluir! 🚀
