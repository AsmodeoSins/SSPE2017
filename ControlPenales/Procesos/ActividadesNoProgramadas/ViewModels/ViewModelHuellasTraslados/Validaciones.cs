using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public partial class BusquedaHuellaTraslado
    {
        private void ValidacionNIP()
        {
            base.ClearRules();
            base.AddRule(() => IncidenciaNIP, () => !string.IsNullOrEmpty(IncidenciaNIP) ? IncidenciaNIP.Length == 13 : false, "El NIP es requerido y debe contener 13 caracteres numéricos.");
            OnPropertyChanged("IncidenciaNIP");
        }

        private void ValidacionNIPInexistente(IMPUTADO imputado)
        {
            base.ClearRules();
            base.AddRule(() => IncidenciaNIP, () => imputado != null, "Interno inexistente.");
            OnPropertyChanged("IncidenciaNIP");
        }

        private void ValidacionImputadoActivo(IMPUTADO imputado)
        {
            var Ingreso = imputado.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
            var ID_ESTATUS_ADMINISTRATIVO = Ingreso != null ? Ingreso.ID_ESTATUS_ADMINISTRATIVO : 0;
            base.ClearRules();
            base.AddRule(() => IncidenciaNIP, () =>
                (Ingreso != null &&
                Ingreso.ID_UB_CENTRO.HasValue &&
                Ingreso.ID_UB_CENTRO.Value == GlobalVar.gCentro &&
                ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.LIBERADO &&
                ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.TRASLADADO &&
                ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.SUJETO_A_PROCESO_EN_LIBERTAD &&
                ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.DISCRECIONAL), "Interno no permitido.");
            OnPropertyChanged("IncidenciaNIP");
        }

        private void ValidacionResponsable()
        {
            base.ClearRules();
            base.AddRule(() => NombreResponsableTraslado, () => !string.IsNullOrEmpty(NombreResponsableTraslado), "El nombre del responsable es requerido.");
            OnPropertyChanged("NombreResponsableTraslado");
            base.AddRule(() => ApellidoPaternoResponsableTraslado, () => !string.IsNullOrEmpty(ApellidoPaternoResponsableTraslado), "El apellido paterno del responsable es requerido.");
            OnPropertyChanged("ApellidoPaternoResponsableTraslado");
            base.AddRule(() => ApellidoMaternoResponsableTraslado, () => !string.IsNullOrEmpty(ApellidoPaternoResponsableTraslado), "El apellido materno del responsable es requerido.");
            OnPropertyChanged("ApellidoMaternoResponsableTraslado");
        }
    }
}
