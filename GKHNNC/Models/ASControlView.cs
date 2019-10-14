using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class ASControlView 
    {
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "АвтоID")]
        public int? AvtoId { get; set; }

        [Display(Name = "Автомобиль")]
        public Avtomobil Avto { get; set; }

        [Display(Name = "Водитель")]
        public int? VoditelId { get; set; }

        [Display(Name = "Водитель")]
        public Voditel Voditel { get; set; }


        [Display(Name = "ЗаказчикИД")]
        public int? ZakazchikId { get; set; }

        [Display(Name = "Заказчик")]
        public Zakazchik Zakazchik { get; set; }

        [Display(Name = "Прицеп")]
        public bool Pricep { get; set; }

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Display(Name = "Дата Завершения")]
        public DateTime DateClose { get; set; }

        [Display(Name = "Выехал?")]
        public bool Go { get; set; }

        [Display(Name = "Выезд открыт контролем?")]
        public bool Kontrol { get; set; }

        [Display(Name = "Примечание")]
        public string Primech { get; set; }

        [Display(Name = "Километраж Автоскан")]
        public decimal KMAS { get; set; }

        [Display(Name = "Километраж водитель")]
        public int KM { get; set; }

        [Display(Name = "Потрачено по ДУТ")]
        public decimal DUT { get; set; }

        [Display(Name = "Начальный уровень")]
        public decimal Start { get; set; }

        [Display(Name = "Конечный уровень")]
        public decimal End { get; set; }

        [Display(Name = "Заправлено")]
        public decimal Zapravleno { get; set; }

        [Display(Name = "Слито")]
        public decimal Sliv { get; set; }

        [Display(Name = "Потеря связи")]
        public bool Warning { get; set; }

        [Display(Name = "Места")]
        public List<string> Mesta { get; set; }

        [Display(Name = "Потери связи")]
        public List<string> NoSvaz { get; set; }

        [Display(Name = "Подтверждено")]
        public bool Podtvergdeno { get; set; }

        //то чего нет в базе а используется только для обозревания
        [Display(Name = "Количество наблюдений")]
        public int Nabludenii { get; set; }

        [Display(Name = "Количество не наблюдений")]
        public int NoNabludenii { get; set; }

        [Display(Name = "Все ДУТ")]
        public List<decimal> ALLDut { get; set; }

        [Display(Name = "Все пробеги АС")]
        public List<decimal> ALLKm { get; set; }

        [Display(Name = "Час реального выезда")]
        public int RealGo { get; set; }

        [Display(Name = "Час реального въезда")]
        public int RealEnd { get; set; }

        [Display(Name = "Чья машина АС или ФС")]
        public string Zag { get; set; }

        [Display(Name = "Время ДУТ")]
        public List<int> TimeDut { get; set; }

        [Display(Name = "Марка авто")]
        public string MarkaAvto { get; set; }

        [Display(Name = "Тип авто")]
        public string TypeAvto { get; set; }

        [Display(Name = "Все действия за день")]
        public List<string> AllActions { get; set; }

        [Display(Name = "100 часов расходов ")]
        public List<string> AllRashod { get; set; }

        [Display(Name = "Средний расход")]
        public decimal SredniiRashod { get; set; }

        [Display(Name = "Максимальный расход")]
        public decimal MaxRashod { get; set; }

        [Display(Name = "Средний расход вчера")]
        public decimal SredniiRashodVchera { get; set; }

        [Display(Name = "Средний расход за день")]
        public decimal SredniiRashodDay { get; set; }

        [Display(Name = "Заправлено по документам")]
        public decimal ZapravlenoFact { get; set; }

        [Display(Name = "Старт АС24")]
        public decimal StartAS24 { get; set; }

        [Display(Name = "Энд АС24")]
        public decimal EndAS24 { get; set; }
    }
}