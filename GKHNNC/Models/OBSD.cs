using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class OBSD
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "АдресID")]
        public int? AdresId { get; set; }
        public Adres Adres { get; set; }
        [Display(Name = "Таблица Сервисов ID")]
        public int? TableServiceId { get; set; }
        [Display(Name = "Таблица Сервисов")]
        public TableService TableService { get; set; }
        [Display(Name = "Начисление")]
        public decimal Nachislenie { get; set; }
        [Display(Name = "Лицевой счёт")]
        public int Licevoi { get; set; }
        [Display(Name = "ФИО")]
        public string FIO { get; set; }
        [Display(Name = "Сальдо")]
        public decimal Saldo { get; set; }
        [Display(Name = "Квартира")]
        public string Kvartira { get; set; }

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

    }
}