using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GESAL.Clases.Misc
{
    public class FuncionesFechas
    {
        public static int mesToInt(string mes)
        {
            switch(mes.ToUpper())
            {
                case "ENERO":
                    return 1;
                case "FEBRERO":
                    return 2;
                case "MARZO":
                    return 3;
                case "ABRIL":
                    return 4;
                case "MAYO":
                    return 5;
                case "JUNIO":
                    return 6;
                case "JULIO":
                    return 7;
                case "AGOSTO":
                    return 8;
                case "SEPTIEMBRE":
                    return 9;
                case "OCTUBRE":
                    return 10;
                case "NOVIEMBRE":
                    return 11;
                case "DICIEMBRE":
                    return 12;
                default:
                    return -1;
            }
        }

    }
}
