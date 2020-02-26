using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class MusorPloshadka
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Название площадки")]
        public string Name { get; set; }
        [Display(Name = "УлицаID")]
        public string StreetId { get; set; }
     
        [Display(Name = "Улица")]
        public AllStreet Street;
        [Display(Name = "Объём")]
        public string Obiem { get; set; }
        [Display(Name = "Контэйнеры")]
        public string Kontainers { get; set; }
        [Display(Name = "ID площадки")]
        public string IDPloshadki { get; set; }
        [Display(Name = "Наименование ЮЛ")]
        public string NameUL { get; set; }
        [Display(Name = "Юр лицо или МКД?")]
        public string UL { get; set; }
        [Display(Name = "ТКО или КГО?")]
        public bool TKO { get; set; }

        //только для вива
        [Display(Name = "Все улицы")]
        public List<string> AllStreets;
        [Display(Name = "Все улицы")]
        public List<AllStreet> VseUlici;
        [Display(Name = "Объём по дням недели")]
        public decimal[] Obiem7 = new decimal[7];
        [Display(Name = "Контэйнеры по дням недели")]
        public int[] Kontainers7 = new int[7];
    }
}