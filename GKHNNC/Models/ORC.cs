using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class ORC
    {
   [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string DISTRICT { get; set; }
        public string DEPARTMENT { get; set; }

        public string ADDRESS { get; set; }
        public string SERVICE { get; set; }
        public string PROVIDER { get; set; }
        public decimal SID{ get; set; }
        public decimal SIC { get; set; }
        public decimal SI  { get; set; }
        public decimal CHARGE  { get; set; }
        public decimal PAYS  { get; set; }
        public decimal CORR  { get; set; }
        public decimal SOD  { get; set; }
        public decimal SOC  { get; set; }
        public decimal SO  { get; set; }
        public int Year { get; set; }


    }
}