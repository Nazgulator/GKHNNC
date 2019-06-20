using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GKHNNC.Models
{
    public class OtchetModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "ЖЭУ")]
        public int GEU { get; set; }
        [Display(Name = "Месяц")]
        public string Month { get; set; }

    }
}