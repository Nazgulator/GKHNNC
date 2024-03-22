using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class MKDOstatki
    {
   [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Adres { get; set; }
        public string Schet { get; set; }
        public decimal OstatokJan  { get; set; }
        public decimal Sobrano  { get; set; }
        public decimal Dohod  { get; set; }
        public decimal Inie  { get; set; }
        public decimal Rashod { get; set; }
        public decimal OstatokDec  { get; set; }
        public int Year { get; set; }


    }
}