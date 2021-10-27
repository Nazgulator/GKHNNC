using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class PlanoviStatiView
    {
        

        [Display(Name = "Рекомендованные работы")]
        public List<OsmotrRecommendWork> ORW { get; set; }
        public List<ActiveOsmotrWork> AOW { get; set; }
        public List<Stati> Statis;



    }
}