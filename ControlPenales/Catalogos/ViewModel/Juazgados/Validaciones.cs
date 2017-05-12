using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
   partial class CatalogoJuzgadosViewModel
    {

       private void setValidationRules()
       {
           base.ClearRules();
           base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCION ES REQUERIDA!");
           base.AddRule(() => Domicilio, () => !string.IsNullOrEmpty(Domicilio), "DOMICILIO ES REQUERIDO!");
           base.AddRule(() => Pais, () => Pais != null && Pais.ID_PAIS_NAC > 0, "PAIS ES REQUERIDO!");
           base.AddRule(() => Entidad, () => Entidad!= null && Entidad.ID_ENTIDAD > 0, "ENTIDAD ES REQUERIDA!");
           base.AddRule(() => SelectMunicipio, () => SelectMunicipio != null && SelectMunicipio.ID_ENTIDAD > 0, "MUNICIPIO ES REQUERIDO!");
           base.AddRule(() => SelectJuzgadoTipo, () => SelectJuzgadoTipo != null && SelectJuzgadoTipo.ID_TIPO_JUZGADO > 0, "TIPO JUZGADO ES REQUERIDO!");
           base.AddRule(() => SelectedFuero, () => SelectedFuero != null , "FUERO ES REQUERIDO!");
           base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "ESTATUS REQUERIDO!");
       }

    }
}
