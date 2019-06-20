using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class Periodichnost
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Периодичность")]
        public string PeriodichnostName { get; set; }
        //для подгрузки по айди
        public ICollection<Usluga> Uslugas { get; set; }
        public Periodichnost()
        {
            Uslugas = new List<Usluga>();
        }

    }
}