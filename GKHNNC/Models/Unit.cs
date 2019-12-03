using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GKHNNC.Models
{
    public class Unit
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Atack { get; set; }
        public int Health { get; set; }


        public virtual ICollection<Sopostavlenie> Sopostavlenies { get; set; }
    }
}