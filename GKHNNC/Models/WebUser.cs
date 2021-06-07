using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GKHNNC.Models
{
    public class WebUser
    {
      
       
            public int Id { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
            public int    RoleId { get; set; }
            public object ID_MDAN { get; set; }
            public object FullName { get; set; }
            public object HouseRoleId { get; set; }
            public object DispatcherRoleId { get; set; }
            public object DispatcherDepartId { get; set; }
            public object UchastokId { get; set; }
            public object WorkRoleId { get; set; }
            public object SettingRoleId { get; set; }
            public object WorkIspId { get; set; }
            public object ChangePrice { get; set; }
            public object ChangeFrom { get; set; }
            public object ChangeTo { get; set; }
            public object LastInto { get; set; }
            public object BuildsList { get; set; }
        


        public virtual ICollection<Sopostavlenie> Sopostavlenies { get; set; }
    }
}