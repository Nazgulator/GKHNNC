using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using GKHNNC.Models;

namespace GKHNNC.Models
{
    public class AutomarshallView
    {

        [Display(Name = "Идентификатор")]
        public long Id { get; set; }

        [Display(Name = "Время")]
        public DateTime TimeStamp { get; set; }
        //для подгрузки через айди
         [Display(Name = "Картинка")]
          public byte[] Picture { get; set; }

        [Display(Name = "Картинка номера")]
        public byte[] PlateShot { get; set; }

        [Display(Name = "Номер авто")]
        public string Plate { get; set; }


        [Display(Name = "LogId")]
        public long LogId { get; set; }
    }
}