using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class CompleteWork
    {

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

      //  [Display(Name = "Наименование работы")]
      //  public string WorkName { get; set; }
        [Display(Name = "Количество")]
        //[DisplayFormat(DataFormatString = "{0:#,#}")]
        //[RegularExpression(@"^\d+,\d{0,1}$", ErrorMessage = "Десятичное число округляется до сотых X.XX")]
        public decimal WorkNumber { get; set; }
        [Display(Name = "Группа")]
        public string WorkGroup { get; set; }
        [Display(Name = "Адрес")]
        public string WorkAdress { get; set; }
        [Display(Name = "Код")]
        public string WorkCode { get; set; }
        [Display(Name = "Измерение")]
        public string WorkIzmerenie { get; set; }
        [Display(Name = "Наименование работы")]
        public string WorkName { get; set; }
        [Display(Name = "Индекс работы")]
        public int? WorkWorkId { get; set; }
        [Display(Name = "Агент")]
        public string Agent { get; set; }
        [Display(Name = "Сохранил данные")]
        public string KtoSohranil { get; set; }
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy'/'MM'/'dd}")]
        [Display(Name = "Дата выполнения работы")]
        public DateTime WorkDate { get; set; }


        //  public virtual ICollection<Sopostavlenie> Sopostavlenies { get; set; }

    }
}