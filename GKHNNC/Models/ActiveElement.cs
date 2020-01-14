using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class ActiveElement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Элемент ИД")]
        public int ElementId { get; set; }
        [Display(Name = "Элемент")]
        public Element Element { get; set; }
        [Display(Name = "Адрес ИД")]
        public int AdresId { get; set; }
        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }
        [Display(Name = "Осмотр ИД")]
        public int OsmotrId { get; set; }
        [Display(Name = "Осмотр")]
        public Osmotr Osmotr { get; set; }
        [Display(Name = "Состояние")]
        public int Sostoyanie { get; set; }
        [Display(Name = "Фото1")]
        public string Photo1 { get; set; }
        [Display(Name = "Фото2")]
        public string Photo2 { get; set; }
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        //сюда заливаем список дефектов для view и только
        [Display(Name = "Возможные дефекты")]
        public List<Defect> Defects;
        [Display(Name = "Активные дефекты")]
        public List<ActiveDefect> ActiveDefects;
    }
}