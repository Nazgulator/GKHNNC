using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace GKHNNC.Models
{
    public class Podpisant
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Код")]
        public string StringId { get; set; }
        [Display(Name = "Должность для отчета")]
        public string Dolgnost { get; set; }
        [Display(Name = "ФИО для отчета")]
        public string Name { get; set; }
    }
}