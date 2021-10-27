using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using GKHNNC.Models;
using System.Data.Entity;
using System.Runtime.Serialization.Formatters.Binary;

namespace GKHNNC.Models
{

    public class VehicleRegistrationLog
    {
        
        [Display(Name = "Идентификатор")]
        public long Id { get; set; }

        [Display(Name = "Время")]
        public DateTime TimeStamp { get; set; }
        //для подгрузки через айди
       // [Display(Name = "Картинка")]
      //  public byte[] Picture { get; set; }

        [Display(Name = "Номер авто")]
        public string Plate { get; set; }

        [Display(Name = "Номер авто")]
        public int RecognitionStatus { get; set; }

        [Display(Name = "Удалена?")]
        public bool IsDeleted { get; set; }

        //   [Display(Name = "LogId")]
        //  public long LogId { get; set; }
    }
}