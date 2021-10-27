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
    public class Poligon
    {
        
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "Гос. Номер")]
        [RegularExpression(@"^[А-Я0-9]+$", ErrorMessage = "Только заглавные русские буквы!")]
        public string Number { get; set; }

        [Display(Name = "Автомобиль ID")]
        public int? AvtomobilId { get; set; }
        [Display(Name = "Автомобиль")]
        public Avtomobil Avtomobil { get; set; }


        [Display(Name = "Марка автомобиля ИД")]
        public int? MarkaId { get; set; }
        //для подгрузки через айди
        [Display(Name = "Марка автомобиля")]
        public MarkaAvtomobil Marka { get; set; }

        [Display(Name = "Тип автомобиля ИД")]
        public int? TypeId { get; set; }
        //для подгрузки через айди
        [Display(Name = "Тип автомобиля")]
        public TypeAvto Type { get; set; }

        [Display(Name = "Дата и время заезда")]
        public DateTime Date { get; set; }

        [Display(Name = "Масса на въезде")]
        public decimal MassIn { get; set; }

        [Display(Name = "Масса на выезде")]
        public decimal MassOut { get; set; }

        [Display(Name = "Масса мусора")]
        public decimal MassMusor { get; set; }

        [Display(Name = "Описание груза")]
        public string Description { get; set; }

        [Display(Name = "Пользователь")]
        public string User { get; set; }

        [Display(Name = "КонтрагентИД")]
        public int KontrAgentId { get; set; }

        [Display(Name = "Контрагента выбрал человек?")]
        public bool VibralRab { get; set; }

        [Display(Name = "Камера подтвердила?")]
        public bool CameraFix { get; set; }

        [Display(Name = "Изображение")]
        public string Picture;

        [Display(Name = "Изображение номера")]
        public string PlateShot;

        [Display(Name = "Камера ид")]
        public long IdCam;




        //Теперь только для jquery




        [Display(Name = "Контрагент Имя")]
        public string KontrAgentName { get; set; }


        [Display(Name = "Гаражный номер")]
        public int Garage = 0;

        [Display(Name = "Глонасс")]
        public bool Glonass = false;

        [Display(Name = "Въездов на полигон")]
        public int PoligonIn = 0;






    }
}