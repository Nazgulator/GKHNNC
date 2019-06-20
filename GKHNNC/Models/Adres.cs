using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class Adres
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Адрес")]
        public string Adress { get; set; }
        [Display(Name = "ЖЭУ")]
        public string GEU { get; set; }
        //для подгруски через ID
        public ICollection<VipolnennieUslugi> VipolnennieUslugis { get; set; }
        public Adres()
        {
            VipolnennieUslugis = new List<VipolnennieUslugi>();
        }

    }
}