using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using GKHNNC.Controllers;

namespace GKHNNC.Models
{
    public class DOMPart

    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Имя элемента")]
        public string Name { get; set;}
   


    }
}