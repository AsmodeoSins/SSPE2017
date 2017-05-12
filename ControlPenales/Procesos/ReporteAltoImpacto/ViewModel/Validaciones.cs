using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
  public  partial class ReporteAltoImpactoViewModel
    {
      public void ValidacionesFiltros()
      {
          base.ClearRules();
          
          base.AddRule(() => TextFechaInicio, () => TextFechaInicio!=null, "FECHA INICIO ES REQUERIDA!");
          base.AddRule(() => TextFechaFin, () => TextFechaFin != null, "FECHA FIN ES REQUERIDA!");
          base.AddRule(() => SelectFuero, () => SelectFuero != "SELECCIONE", "FUERO ES REQUERIDO!");
          base.AddRule(() => SelectTitulo, () => SelectTitulo >-1, "TITULO ES REQUERIDO!");
      }
    }
}
