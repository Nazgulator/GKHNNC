using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GKHNNC.Models
{
    public class WhiteList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        public string Nomer { get; set; }
        public string Marka { get; set; }
   
        public decimal Obiem { get; set; }
        public string Kontragent { get; set; }


        
    }
}