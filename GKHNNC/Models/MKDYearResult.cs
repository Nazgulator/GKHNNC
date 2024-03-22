using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class MKDYearResult
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "Адрес МКД")]
        public string AdresMKD { get; set; }

        [Display(Name = "Адрес ФГБУ")]
        public string AdresFGBU { get; set; }

        [Display(Name = "Статья")]
        public string Statya { get; set; }

        [Display(Name = "Адрес ФГБУ")]
        public int AdresId { get; set; }

        [Display(Name = "Год")]
        public int PeriodYear { get; set; }

        [Display(Name = "Старт")]
        public decimal BallStart { get; set; }

        [Display(Name = "Начислено")]
        public decimal Nachisleno { get; set; }

        [Display(Name = "Оплачено")]
        public decimal Oplacheno { get; set; }

        [Display(Name = "Балланс на конец")]
        public decimal BallEnd { get; set; }

        [Display(Name = "Выполненные работы")]
        public decimal CompleteWorks { get; set; }

    }
}