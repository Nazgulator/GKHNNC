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
        [Display(Name = "Билдинг")]
        public int BuildId { get; set; }
        [Display(Name = "Площадь")]
        public decimal Ploshad { get; set; }
        [Display(Name = "Активная Площадь")]
        public decimal ActivePloshad { get; set; }
        [Display(Name = "Жилая Площадь")]
        public decimal PloshadGilaya { get; set; }
        [Display(Name = "Нежилая Площадь")]
        public decimal PloshadNegilaya { get; set; }
        [Display(Name = "Площадь подвала")]
        public decimal PloshadPodval { get; set; }
        [Display(Name = "Площадь лестниц")]
        public decimal PloshadLestnica { get; set; }
        [Display(Name = "Площадь кровля")]
        public decimal PloshadKrovlya { get; set; }
        [Display(Name = "Площадь мусор")]
        public decimal PloshadMusor { get; set; }
        [Display(Name = "Площадь земля")]
        public decimal PloshadZemlya { get; set; }
        [Display(Name = "Количество этажей")]
        public int Etagi { get; set; }
        [Display(Name = "Количество подъездов")]
        public int Podezds { get; set; }
        [Display(Name = "Количество квартир")]
        public int Kvartirs { get; set; }
        [Display(Name = "Количество лифтов")]
        public int Lifts { get; set; }
        [Display(Name = "Количество жильцов")]
        public int Peoples { get; set; }
        [Display(Name = "Подрядчик")]
        public string IP { get; set; }
        [Display(Name = "Теплота 1/12")]
        public decimal Teplota12 { get; set; }
        [Display(Name = "Улица")]
        public string Ulica { get; set; }
        [Display(Name = "Дом")]
        public string Dom { get; set; }
        [Display(Name = "ЖЭУ")]
        public string GEU { get; set; }
        [Display(Name = "Код УЭВ")]
        public int UEV { get; set; }
        [Display(Name = "Код ОБСД")]
        public int OBSD { get; set; }
        //для подгрузки через ID
        public ICollection<VipolnennieUslugi> VipolnennieUslugis { get; set; }
        public Adres()
        {
            VipolnennieUslugis = new List<VipolnennieUslugi>();
        }

    }
}