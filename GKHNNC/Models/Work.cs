using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class Work
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int WorkId { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "Группа")]
        public string Group { get; set; }
        [Display(Name = "Измерение")]
        public string Izmerenie { get; set; }
        [Display(Name = "Код")]
        public string Code { get; set; }
        //public string Agent { get; set; }
        //public DateTime Date { get; set; }

        public virtual ICollection<Sopostavlenie> Sopostavlenies { get; set; }
    }
}