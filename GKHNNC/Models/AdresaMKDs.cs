using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class AdresaMKDs
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int UniqueId { get; set; }
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "ОРС")]
        public string ORC { get; set; }
        [Display(Name = "АСУ")]
        public string ASU { get; set; }
        [Display(Name = "ИмяФайла")]
        public string FileName { get; set; }



    }
}