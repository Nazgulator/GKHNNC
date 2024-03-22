using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class MKDCompleteWork
    {

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "АдресID")]
        public int AdresMKDID { get; set; }
        [Display(Name = "Адрес")]
        public AdresaMKDs AdresMKD;
        //  [Display(Name = "Наименование работы")]
        //  public string WorkName { get; set; }
        //  [Display(Name = "Улица")]
        //[DisplayFormat(DataFormatString = "{0:#,#}")]
        //[RegularExpression(@"^\d+,\d{0,1}$", ErrorMessage = "Десятичное число округляется до сотых X.XX")]
        //  public string Street { get; set; }
        //   [Display(Name = "Дом")]
        //   public string House { get; set; }
        //  [Display(Name = "Код")]
        //   public string Prefix { get; set; }
        [Display(Name = "Измерение")]
        public string WorkIzmerenie { get; set; }
        [Display(Name = "Наименование работы")]
        public string WorkName { get; set; }
        [Display(Name = "Тип работы")]
        public string WorkTip { get; set; }
        [Display(Name = "Цена за единицу")]
        public decimal WorkCena { get; set; }
        [Display(Name = "Сумма")]
        public decimal WorkSumma { get; set; }
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy'/'MM'/'dd}")]
        [Display(Name = "Дата выполнения работы")]
        public DateTime WorkDate { get; set; }

        public int Count =0;
     


        //  public virtual ICollection<Sopostavlenie> Sopostavlenies { get; set; }

    }
}