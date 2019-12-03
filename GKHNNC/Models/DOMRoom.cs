using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class DOMRoom
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "АдресId")]
        public int? AdresId { get; set; }
        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }

        [Display(Name = "Тип внутренних стен")]
        public int? TypeId { get; set; }
        [Display(Name = "Тип внутренних стен")]
        public RoomType Type { get; set; }
        [Display(Name = "Тип внутренних перекрытий")]
        public int? OverlapId { get; set; }
        [Display(Name = "Тип внутренних перекрытий")]
        public RoomOverlap Overlap { get; set; }
        [Display(Name = "Тип окон")]
        public int? WindowId { get; set; }
        [Display(Name = "Тип окон")]
        public RoomWindow Window { get; set; }
        [Display(Name = "Тип дверей")]
        public int? DoorId { get; set; }
        [Display(Name = "Тип дверей")]
        public RoomDoor Door { get; set; }

        [Display(Name = "Количество лоджий")]
        public int Lodgi { get; set; }
        [Display(Name = "Количество балконов")]
        public int Balkon { get; set; }

        [Display(Name = "Дата последнего изменения")]
        public DateTime Date { get; set; }
        [Display(Name = "Состояние")]
        public int Sostoyanie { get; set; }
    }
}