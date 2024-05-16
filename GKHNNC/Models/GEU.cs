using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class GEU
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "Начальник РП")]
        public string Director { get; set; }
        [Display(Name = "Начальник ИП")]
        public string DirectorIP { get; set; }
        [Display(Name = "Доверенность №")]
        public string Doverennost { get; set; }
        [Display(Name = "Инженер ПТО")]
        public string IngenerPTO { get; set; }
        [Display(Name = "Инженер ОЭЖФ")]
        public string IngenerOEGF { get; set; }
        [Display(Name = "ЭУ")]
        public int EU { get; set; }
        [Display(Name = "Номер ЖЭУ")]
        public int GEUN { get; set; }
        [Display(Name = "Должность начальника ЖЭУ")]
        public string DirectorDolgnost { get; set; }
        [Display(Name = "Должность инженера ПТО")]
        public string IngenerPTODolgnost { get; set; }
        [Display(Name = "Должность инженера ОЭЖФ")]
        public string IngenerOEGFDolgnost { get; set; }

    }
}