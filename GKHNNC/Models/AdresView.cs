using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class AdresView
    {
        public string SelectedItem { get; set; }
        public IEnumerable<Adres> Adreses { get; set; }
    }
}