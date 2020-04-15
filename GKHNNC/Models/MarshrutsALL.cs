using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using GKHNNC.DAL;

namespace GKHNNC.Models
{
    public abstract class MarshrutsALL
    {
        private WorkContext db = new WorkContext();
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int MarshrutId { get; set; }
        public Marshrut Marshrut { get; set; }
        public int Day { get; set; }
        public string MusorPloshadkas { get; set; }
        public string Avtomobils { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public decimal ObiemFact { get; set; }
        public int MassaFact { get; set; }
        public bool Modify { get; set; }



        //для выгрукзки
        public List<MusorPloshadka> MusorPloshadkas7 = new List<MusorPloshadka>();
        public List<Avtomobil> Avtomobils7 = new List<Avtomobil>();

        public string MarshrutName = ""; 


    }
}