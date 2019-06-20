using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;


namespace GKHNNC.Models
{
    public class MenuLogs
    {
        [Display(Name = "Год")]
        public int Year { get; set; }

        [Display(Name = "Месяц")]
        public int Month { get; set; }

        [Display(Name = "День")]
        public int Day { get; set; }


    }
}