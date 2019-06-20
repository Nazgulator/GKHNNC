using GKHNNC.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKHNNC.Utilites
{

    public class SessionObjects
    {
       
        public static List<HouseToAkt> HouseToAktsGet( HttpSessionStateBase session )
        {

            return (List<HouseToAkt>)session["Act2House"];
        }

        public static void  HouseToAktsSet(HttpSessionStateBase session,List<HouseToAkt> listwork )
        {

            session["Act2House"] = listwork;
        }
    }
}