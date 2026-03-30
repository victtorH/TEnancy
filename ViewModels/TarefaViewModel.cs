using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Linq;
using System.Threading.Tasks;
using ModuloMVC.Enum;

namespace ModuloMVC.ViewModels
{


    public class TarefaViewModel
    {
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public DateTime? Vencimento { get; set; }
 

      [ValidateNever] 
    public List<ContatoViewModel> ContatosEnvolvidos { get; set; } = new();
       
        public List<int> ContatosSelecionadosIds { get; set; } = new();

    }
}