using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
   public class cReporteSolicitudPorEstatus
    {

       public string Estado { get; set; }
       public string AreaTecnica { get; set; }
       public string Anio{ get; set; }
       public string Folio { get; set; }
       public string NombreImputado { get; set; }   
       public string Actividad { get; set; }
       public string FechaSolicitud { get; set; }

       public string FechaCita{ get; set; }
       public string Total { get; set; }

       public string GrupoEstatus_AreaTecn { get; set; }

    }
}
