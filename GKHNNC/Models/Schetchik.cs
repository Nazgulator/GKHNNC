using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class Schetchik
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Тип")]
        public int TipId { get; set; }
        public SchetchikTip Tip { get; set; }
        [Display(Name = "Группа")]
        public int GroupId{ get; set; }
        public Group Group { get; set; }
        [Display(Name = "Адрес")]
        public int AdresId { get; set; }
        public Adres Adres { get; set; }
        [Display(Name = "Сумма")]
        public decimal Summa { get; set; }
        [Display(Name = "Диаметр")]
        public int Diameter { get; set; }
        //public string Agent { get; set; }
        [Display(Name = "Дата старта")]
        public DateTime DateStart { get; set; }
        [Display(Name = "Дата окончания")]
        public DateTime DateEnd { get; set; }
        [Display(Name = "Имя")]
        public string Name { get; set; }
        [Display(Name = "Номер счетчика")]
        public string Number { get; set; }

 

     
    }
}