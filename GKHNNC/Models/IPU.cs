using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class IPU
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "АдресID")]
        public int? AdresId { get; set; }
        public Adres Adres { get; set; }
        [Display(Name = "Норматив")]
        public decimal Normativ { get; set; }
        [Display(Name = "Счетчик")]
        public decimal Schetchik { get; set; }
        [Display(Name = "Номер счетчика")]
        public string NomerSchetchika { get; set; }

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
    }
}