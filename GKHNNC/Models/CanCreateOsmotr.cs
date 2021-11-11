using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class CanCreateOsmotr

    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Можно ли создавать")]
        public bool Sozdanie { get; set; }
        [Display(Name = "Дата")]
        public DateTime DateTime { get; set; }
     
        
       

    }
}