
namespace ControlPenales
{
    partial class ConsultaUnificadaAdminViewModel
    {
        private void setValidationRules()
        {

            base.ClearRules();
            base.AddRule(() => CuClasificacion, () => CuClasificacion != -1, "CLASIFICACION ES REQUERIDA!");
            base.AddRule(() => CuNombre, () => !string.IsNullOrEmpty(CuNombre), "NOMBRE ES REQUERIDO!");
            //base.AddRule(() => CuDocumento, () => CuDocumento != null, "DOCUMENTO ES REQUERIDO!");
            OnPropertyChanged("CuClasificacion");
            OnPropertyChanged("CuNombre");
        }
    }
}