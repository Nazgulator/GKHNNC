using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Web.Mvc;

namespace GKHNNC.Models
{
    public class TypeElement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "КонструкттивИД")]
        public int ConstructiveTypeId { get; set; }
        [Display(Name = "Конструктив")]
        public ConstructiveType ConstructiveType { get; set; }
        [Display(Name = "Типы конструктивных элементов")]
        public SelectList CT = null;

        [Display(Name = "АдресID")]
        public int AdresId { get; set; }
        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }

        [Display(Name = "Дата изменения")]
        public DateTime Date { get; set; }

        [Display(Name = "АдресID")]
        public int MaterialId { get; set; }
        [Display(Name = "Адрес")]
        public Material Material { get; set; }

        [Display(Name = "АдресID")]
        public int DOMPartId { get; set; }
        [Display(Name = "Адрес")]
        public DOMPart DOMPart { get; set; }

        [Display(Name = "Пользователь")]
        public string UserName { get; set; }
       
      



    }
}