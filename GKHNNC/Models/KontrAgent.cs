using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GKHNNC.Models
{
    public class KontrAgent
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}