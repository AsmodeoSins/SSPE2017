using MVVMShared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GESAL.ViewModels
{
    public partial class EntradasAlmacenesViewModel:ValidationViewModelBase,IDataErrorInfo
    {
        private void setValidationRulesAsignaLotes()
        {
            base.ClearRules();
            base.AddRule(()=>Recibido,()=>(Recibido>0 && Recibido<=Restante),"LA CANTIDAD RECIBIDA ES OBLIGATORIA Y NO PUEDE SER MAYOR A LA CANTIDAD RESTANTE ORDENADA");
            base.AddRule(() => Lote, () => (Lote.HasValue && Lote.Value>0 && !Recepcion_Producto_Detalle.Select(s=>s.LOTE).Contains(Lote.Value)), "EL NUMERO DE LOTE DEBE DE SER MAYOR A 0 Y NO DEBE DE HABER SIDO CAPTURADO YA");
            if (SelectedAlmacenPrincipal.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO == "M")
                base.AddRule(()=> IsFechaCaducidadValid, ()=> IsFechaCaducidadValid, MensajeFechaCaducidad);
            RaisePropertyChanged("Recibido");
            RaisePropertyChanged("Lote");
            RaisePropertyChanged("IsFechaCaducidadValid");
        }
        private void setValidationRulesEntradasAlmacen()
        {
            base.ClearRules();
            base.AddRule(() => IsGridValido, () => IsGridValido, "CAPTURAR TODOS LOS CAMPOS REQUERIDOS DE LA RECEPCION DE PRODUCTOS!");
            RaisePropertyChanged("IsGridValido");
        }

        private void setValidacionRulesRechazoProducto()
        {
            base.ClearRules();
            base.AddRule(() => SelectedIncidencia_Tipo, () => (SelectedIncidencia_Tipo != null && SelectedIncidencia_Tipo.ID_TIPO_INCIDENCIA !=-1), "TIPO DE INCIDENCIA ES OBLIGATORIO!");
            base.AddRule(()=>Observacion_Incidencia,()=>(!string.IsNullOrWhiteSpace(Observacion_Incidencia)),"OBSERVACION ES OBLIGATORIA!");
            RaisePropertyChanged("SelectedIncidencia_Tipo");
            RaisePropertyChanged("Observacion_Incidencia");
        }

        private void setValidacionRulesRechazoEntrada()
        {
            base.ClearRules();
            base.AddRule(() => IsRechazo_Entrada_Valido, () => (IsRechazo_Entrada_Valido), "ES REQUISITO MINIMO RECHAZAR UNA ENTRADA DE PRODUCTO!");
            RaisePropertyChanged("IsRechazo_Entrada_Valido");
        }
    }
}
