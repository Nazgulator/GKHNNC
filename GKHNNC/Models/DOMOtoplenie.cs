using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using GKHNNC.Controllers;

namespace GKHNNC.Models
{
    public class DOMOtoplenie
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Адрес ИД")]
        public int? AdresId { get; set; }
        [Display(Name = "Адрес")]
        public Adres Adress { get; set; }
        [Display(Name = "Износ отопления")]
        public int IznosOtop { get; set; }
        [Display(Name = "Количество вводов отопления")]
        public int VvodsOtop { get; set; }
        [Display(Name = "Материал изоляции отопления 1 ИД")]
        public int? MaterialOtop1Id { get; set; }
        [Display(Name = "Материал изоляции отопления 1")]
        public Material MaterialOtop1 { get; set; }
        [Display(Name = "Материал изоляции отопления 2 ИД")]
        public int? MaterialOtop2Id { get; set; }
        [Display(Name = "Материал изоляции отопления 2")]
        public Material MaterialOtop2 { get; set; }
        [Display(Name = "Материал труб и стояков отопления  ИД")]
        public int? MaterialOtopTrubId { get; set; }
        [Display(Name = "Материал труб и стояков отопления")]
        public Material MaterialOtopTrub { get; set; }
        [Display(Name = "Стояки материал ID")]
        public int? MaterialTeploId { get; set; }
        [Display(Name = "Стояки материал")]
        public Material MaterialTeplo { get; set; }
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }




    }
}