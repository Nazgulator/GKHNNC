using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class Element
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Элемент")]
        public string Name { get; set; }
        [Display(Name = "Элемент ИД")]
        public int ElementId { get; set; }
        [Display(Name = "Тип")]
        public string ElementType { get; set; }



    }
}