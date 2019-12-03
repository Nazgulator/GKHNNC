using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class Osmotr
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }
        [Display(Name = "Адрес ИД")]
        public int? AdresId { get; set; }

        [Display(Name = "Фасад")]
        public DOMFasad DOMFasad { get; set; }
        [Display(Name = "Фасад ИД")]
        public int? DOMFasadId { get; set; }

        [Display(Name = "Фундамент")]
        public DOMFundament DOMFundament { get; set; }
        [Display(Name = "Фундамент ИД")]
        public int? DOMFundamentId { get; set; }

        [Display(Name = "Электросеть")]
        public DOMElectro DOMElectro { get; set; }
        [Display(Name = "Электросеть ИД")]
        public int? DOMElectroId { get; set; }

        [Display(Name = "Система холодного водоснабжения")]
        public DOMCW DOMCW { get; set; }
        [Display(Name = "Система холодного водоснабжения ИД")]
        public int? DOMCWId { get; set; }

        [Display(Name = "Система горячего водоснабжения")]
        public DOMHW DOMHW { get; set; }
        [Display(Name = "Система горячего водоснабжения ИД")]
        public int? DOMHWId { get; set; }

        [Display(Name = "Система отопления")]
        public DOMOtoplenie DOMOtoplenie { get; set; }
        [Display(Name = "Система отопления ИД")]
        public int? DOMOtoplenieId { get; set; }

        [Display(Name = "Крыша")]
        public DOMRoof DOMRoof { get; set; }
        [Display(Name = "Крыша ИД")]
        public int? DOMRoofId { get; set; }

        [Display(Name = "Комната")]
        public DOMRoom DOMRoom { get; set; }
        [Display(Name = "Комната ИД")]
        public int? DOMRoomId { get; set; }

        [Display(Name = "Система водоотведения")]
        public DOMVodootvod DOMVodootvod { get; set; }
        [Display(Name = "Система водоотведения ИД")]
        public int? DOMVodootvodId { get; set; }

        [Display(Name = "Состояние")]
        public int Sostoyanie { get; set; }

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Display(Name = "Дефекты")]
        public List<ActiveDefect> Defects { get; set; }

        [Display(Name = "Элементы ")]
        public List<ActiveElement> Elements;
    }
}