using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class DOMFundament
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Площадь отмостки")]
        public decimal Ploshad { get; set; }
        [Display(Name = "Материал фундамента")]
        public int? MaterialId { get; set; }
        public FundamentMaterial Material { get; set; }
        [Display(Name = "Тип фундамента")]
        public int? TypeId { get; set; }
        public FundamentType Type { get; set; }
        [Display(Name = "АдресId")]
        public int? AdresId { get; set; }
        public Adres Adres { get; set; }
        [Display(Name = "Дата последнего изменения")]
        public DateTime Date { get; set; }
    }
}