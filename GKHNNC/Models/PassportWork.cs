using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class PassportWork
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Есть паспорт или нет")]
        public bool Est { get; set; }
        [Display(Name = "Место хранения файла")]
        public string FilePath { get; set; }

        [Display(Name = "Адрес")]
        public int AdresId { get; set; }
        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }

        [Display(Name = "Тип работы")]
        public int PassportWorkTypeId { get; set; }
        [Display(Name = "Тип работы")]
        public PassportWorkType PassportWorkType { get; set; }

    }
}