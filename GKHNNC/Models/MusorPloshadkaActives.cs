using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class MusorPloshadkaActive
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "ID базовой площадки")]
        public int PloshadkaId { get; set; }
        [Display(Name = "Площадка")]
        public MusorPloshadka Ploshadka { get; set; }
        [Display(Name = "ID активного маршрута")]
        public int MarshrutId { get; set; }
        [Display(Name = "Активный маршрут")]
        public MarshrutsALL Marshrut { get; set; }
        [Display(Name = "Объём")]
        public decimal ObiemFact { get; set; }
        [Display(Name = "Контэйнеры факт")]
        public int KontainersFact { get; set; }
    }
}