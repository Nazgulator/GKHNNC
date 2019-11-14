using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class DefWork
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Элемент")]
        public Element Element { get; set; }
        [Display(Name = "Элемент ИД")]
        public int? ElementId { get; set; }
        [Display(Name = "Работы")]
        public string Work { get; set; }

      

    }
}