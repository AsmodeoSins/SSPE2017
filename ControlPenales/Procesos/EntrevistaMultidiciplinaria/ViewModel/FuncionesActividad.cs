using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Linq;
namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel : ValidationViewModelBase
    {
        private void PopulateActividades()
        {
            //try
            //{
            //    if (emiActual != null)
            //    {
            //        if (LstTipoActividad == null)
            //            LstTipoActividad = new ObservableCollection<EMI_TIPO_ACTIVIDAD>(new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMITipoActividad().Obtener());
            //        if (LstEstatusPrograma == null)
            //            LstEstatusPrograma = new ObservableCollection<EMI_ESTATUS_PROGRAMA>(new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIEstatusPrograma().ObtenerTodos());
            //        if (LstTipoActividad == null)
            //            LstTipoActividad = new ObservableCollection<EMI_TIPO_ACTIVIDAD>(LstTipoActividad.OrderBy(w => w.DESCR));
            //        if (LstAuxiliarActividades == null)
            //            LstAuxiliarActividades = new ObservableCollection<AuxiliarActividades>();

            //        foreach (var item in LstTipoActividad)
            //        {
            //            var objAuxActividades = new AuxiliarActividades();
            //            objAuxActividades.Id = item.ID_EMI_ACTIVIDAD;
            //            objAuxActividades.Actividad = item.DESCR;
            //            objAuxActividades.Actividades = new ObservableCollection<EMI_ACTIVIDAD>(item.EMI_ACTIVIDAD.Where(w => w.ID_EMI == emiActual.ID_EMI && w.ID_EMI_CONS == emiActual.ID_EMI_CONS));
            //            objAuxActividades.IsGridEmpty = objAuxActividades.Actividades.Count > 0 ? false : true;
            //            objAuxActividades.IsGridVisible = !objAuxActividades.IsGridEmpty;
            //            LstAuxiliarActividades.Add(objAuxActividades);
            //        }

            //        return;
            //    }

            //    if (AnteriorEMI != null)
            //    {
            //        if (LstAuxiliarActividades == null)
            //            LstAuxiliarActividades = new ObservableCollection<AuxiliarActividades>();

            //        foreach (var item in LstTipoActividad)
            //        {
            //            var objAuxActividades = new AuxiliarActividades();
            //            objAuxActividades.Id = item.ID_EMI_ACTIVIDAD;
            //            objAuxActividades.Actividad = item.DESCR;
            //            objAuxActividades.Actividades = new ObservableCollection<EMI_ACTIVIDAD>(item.EMI_ACTIVIDAD.Where(w => w.ID_EMI == AnteriorEMI.ID_EMI && w.ID_EMI_CONS == AnteriorEMI.ID_EMI_CONS));
            //            objAuxActividades.IsGridEmpty = objAuxActividades.Actividades.Count > 0 ? false : true;
            //            objAuxActividades.IsGridVisible = !objAuxActividades.IsGridEmpty;
            //            LstAuxiliarActividades.Add(objAuxActividades);
            //        }
            //        return;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer actividades", ex);
            //}
        }
    }
}
