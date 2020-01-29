using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class BuildElement

    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "ИД дома")]
        public int BuildId { get; set; }
        [Display(Name = "ИД элемента")]
        public int ElementId { get; set; }
        [Display(Name = "Материал")]
        public int Material { get; set; }
        [Display(Name = "Ед изм")]
        public int EdIzm { get; set; }
        [Display(Name = "Количество")]
        public string Count { get; set; }




    }
}