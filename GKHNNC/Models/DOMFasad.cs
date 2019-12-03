using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class DOMFasad
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "АдресId")]
        public int? AdresId { get; set; }
        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }

        [Display(Name = "Износ фасада")]
        public decimal Iznos { get; set; }
        [Display(Name = "Год последнего капремонта")]
        public int Year { get; set; }
        [Display(Name = "Материал фасада")]
        public int? MaterialId { get; set; }
        [Display(Name = "Материал фасада")]
        public FasadMaterial Material { get; set; }
        [Display(Name = "Тип фасада")]
        public int? TypeId { get; set; }
        [Display(Name = "Тип фасада")]
        public FasadType Type { get; set; }
        [Display(Name = "Утепление фасада")]
        public int? UteplenieId { get; set; }
        public FasadUteplenie Uteplenie { get; set; }

        [Display(Name = "Дата последнего изменения")]
        public DateTime Date { get; set; }
        [Display(Name = "Состояние")]
        public int Sostoyanie { get; set; }
    }
}