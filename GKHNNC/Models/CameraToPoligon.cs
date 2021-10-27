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
    public class CameraToPoligon 
    {
        
        [Display(Name = "Идентификатор камеры")]
        public int Id { get; set; }

        [Display(Name = "Марка автомобиля ИД")]
        public int? MarkaId { get; set; }

        [Display(Name = "Тип автомобиля ИД")]
        public int? TypeId { get; set; }

        [Display(Name = "Контрагент ИД")]
        public int KontrAgentId  { get; set; }

        [Display(Name = "Гос. Номер")]
        public string Number { get; set; }

        [Display(Name = "Год выпуска")]
        [Range(1700, 10000, ErrorMessage = "Недопустимый год")]
        public int Date { get; set; }

        [Display(Name = "Объём бункера")]
        public decimal ObiemBunkera { get; set; }

        [Display(Name = "Объём бункера")]
        public decimal KoefficientSgatiya { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }




    }
}