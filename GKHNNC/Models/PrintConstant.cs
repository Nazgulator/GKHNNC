using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GKHNNC.Models
{
    public class PrintConstant
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameRP { get; set; }
        public string Dolgnost { get; set; }
        public string Poisk { get; set; }

    }
}