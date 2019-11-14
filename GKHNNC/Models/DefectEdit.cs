using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class DefectEdit
    {

        [Display(Name = "Элемент")]
        public Element Element { get; set; }
       // [Display(Name = "Элемент ИД")]
       // public int? ElementId { get; set; }
        [Display(Name = "Дефект")]
        public List<Defect> Defect { get; set; }
      
        [Display(Name = "Работа")]
        public List<DefWork> DefWork { get; set; }
      

    }
}