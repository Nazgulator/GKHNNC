using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class TelefonsSpravochnik
    {
        [Key]
        [Display(Name = "Num")]
        public int Number  { get; set; }

        [Display(Name = "Улица")]
        public string Улица { get; set; }
        [Display(Name = "Dom")]
        public string Дом { get; set; }
        [Display(Name = "Etag")]
        public string Этаж { get; set; }
        [Display(Name = "Pomeshenie")]
        public string Помещение { get; set; }
        [Display(Name = "FIO")]
        public string ФИО { get; set; }
        [Display(Name = "Telephone")]
        public string Телефон { get; set; }
        [Display(Name = "Gilaya")]
        public float Жилая { get; set; }
        [Display(Name = "Obshaya")]
        public float Общая { get; set; }
        [Display(Name = "Vid")]
        public string Вид { get; set; }
        [Display(Name = "Number")]
        public float Колво { get; set; }




    }
}