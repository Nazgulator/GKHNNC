using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class ContainersType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Адрес")]
        public string Name { get; set; }
        [Display(Name = "Примечание")]
        public string Primech { get; set; }
        [Display(Name = "Картинка")]
        public string Ico { get; set; }
      
      
    }
}