using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public partial class BusquedaHuellaExcarcelaciones
    {
        private void ValidacionResponsable()
        {
            base.ClearRules();
            base.AddRule(() => NombreResponsableExcarcelacion, () => !string.IsNullOrEmpty(NombreResponsableExcarcelacion), "El nombre del responsable es requerido.");
            OnPropertyChanged("NombreResponsableExcarcelacion");
            base.AddRule(() => ApellidoPaternoResponsableExcarcelacion, () => !string.IsNullOrEmpty(ApellidoPaternoResponsableExcarcelacion), "El apellido paterno del responsable es requerido.");
            OnPropertyChanged("ApellidoPaternoResponsableExcarcelacion");
            base.AddRule(() => ApellidoMaternoResponsableExcarcelacion, () => !string.IsNullOrEmpty(ApellidoMaternoResponsableExcarcelacion), "El apellido materno del responsable es requerido.");
            OnPropertyChanged("ApellidoMaternoResponsableExcarcelacion");
        }

        private void ValidacionNIP()
        {
            base.ClearRules();
            base.AddRule(() => IncidenciaNIP, () => !string.IsNullOrEmpty(IncidenciaNIP) ? IncidenciaNIP.Length == 13 : false, "El NIP es requerido y debe contener 13 caracteres numéricos.");
            OnPropertyChanged("IncidenciaNIP");
        }

        private void ValidacionNIPAcceso()
        {
            base.ClearRules();
            base.AddRule(() => NIPBuscar, () => !string.IsNullOrEmpty(NIPBuscar) ? NIPBuscar.Length == 13 : false, "El NIP es requerido y debe contener 13 caracteres numéricos.");
            OnPropertyChanged("NIPBuscar");
        }

        private void ValidacionImputadoActivo(IMPUTADO imputado)
        {
            var ID_ESTATUS_ADMINISTRATIVO = imputado.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO;
            base.ClearRules();
            base.AddRule(() => IncidenciaNIP, () =>
                (ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.LIBERADO &&
                ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.TRASLADADO &&
                ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.SUJETO_A_PROCESO_EN_LIBERTAD &&
                ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.DISCRECIONAL), "Interno no permitido.");
            OnPropertyChanged("IncidenciaNIP");
        }

        private void ValidacionNIPInexistente(IMPUTADO imputado)
        {
            var Ingreso = imputado.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
            var ID_ESTATUS_ADMINISTRATIVO = Ingreso != null ? Ingreso.ID_ESTATUS_ADMINISTRATIVO : 0;
            base.ClearRules();
            base.AddRule(() => IncidenciaNIP, () => imputado != null &&
                (Ingreso != null &&
                Ingreso.ID_UB_CENTRO.HasValue &&
                Ingreso.ID_UB_CENTRO.Value == GlobalVar.gCentro &&
                ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.LIBERADO &&
                ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.TRASLADADO &&
                ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.SUJETO_A_PROCESO_EN_LIBERTAD &&
                ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.DISCRECIONAL), "Interno inexistente.");
            OnPropertyChanged("IncidenciaNIP");
        }
    }
}
