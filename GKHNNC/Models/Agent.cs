using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GKHNNC.Models
{
    public class Agent
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AgentID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }


        public virtual ICollection<Sopostavlenie> Sopostavlenies { get; set; }
    }
}