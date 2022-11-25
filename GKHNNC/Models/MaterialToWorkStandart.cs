using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;


namespace GKHNNC.Models
{
    public class MaterialToWorkStandart
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "Материал")]
        public int MaterialStandartId { get; set; }

        [Display(Name = "Материал")]
        public MaterialStandart MaterialStandart { get; set; }

        [Display(Name = "Работа")]
        public int WorkStandartId { get; set; }

        [Display(Name = "Работа")]
        public WorkStandart WorkStandart { get; set; }

        [Display(Name = "Количество затраченого материала")]
        public decimal QTY { get; set; }

    


    }
}