using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKHNNC.Models
{
    public class Sopostavlenie
    {
        public int SopostavlenieId { get; set; }
        public int WorkId { get; set; }
        public int AgentId { get; set; }
        

        public virtual Work Work { get; set; }
        public virtual Agent Agent { get; set; }
       
    }
}