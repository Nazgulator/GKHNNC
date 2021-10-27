using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class ChatMessage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "От кого")]
        public string From { get; set; }
        [Display(Name = "Кому")]
        public string To { get; set; }
        [Display(Name = "Текст сообщения")]
        public string Text { get; set; }
        [Display(Name = "Прочитано")]
        public bool Read { get; set; }
        [Display(Name = "Ссылка")]
        public string  Path { get; set; }

    }
}