//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GKHNNC
{
    using System;
    using System.Collections.Generic;
    
    public partial class AutoScans
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<decimal> KM { get; set; }
        public Nullable<System.DateTime> TimeInMove { get; set; }
        public Nullable<System.DateTime> MotoHours { get; set; }
        public Nullable<decimal> MaxSpeed { get; set; }
        public Nullable<decimal> Poesdki { get; set; }
        public Nullable<decimal> DUT { get; set; }
        public Nullable<decimal> Start { get; set; }
        public Nullable<decimal> End { get; set; }
        public Nullable<decimal> Zapravleno { get; set; }
        public string Zagrugeno { get; set; }
        public Nullable<int> AvtoId { get; set; }
        public decimal Sliv { get; set; }
    
        public virtual Avtomobils Avtomobils { get; set; }
    }
}
