using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class MKDStatya
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Позиция")]
        public int Pos { get; set; }
        [Display(Name = "Наименование в WORD")]
        public string WordName { get; set; }
        [Display(Name = "Наименование в МКД")]
        public string MKDName { get; set; }

        [Display(Name = "MainId")]
        public int? MainId { get; set; }


    }
}