using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class Musor
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "НомерАвто")]
        public string Name { get; set; }

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Display(Name = "НомерАвто")]
        public int AvtoID { get; set; }

        [Display(Name = "Места")]
        public string Mesta { get; set; }

        [Display(Name = "Объём загрузки")]
        public decimal ObiemIn { get; set; }

        [Display(Name = "Объём выгрузки")]
        public decimal ObiemOut { get; set; }

        [Display(Name = "Объём выгрузки")]
        public decimal KgOut { get; set; }

        [Display(Name = "Водитель")]
        public string Driver { get; set; }

        [Display(Name = "Пробег")]
        public int Probeg { get; set; }

    }
}