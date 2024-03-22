using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class MKDArenda
    {

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ASUId { get; set; }
     
        public decimal Nachisleno { get; set; }

        public decimal Oplacheno { get; set; }

        public decimal Vosnagragdenie { get; set; }

        public int Year { get; set; }





        //  public virtual ICollection<Sopostavlenie> Sopostavlenies { get; set; }

    }
}