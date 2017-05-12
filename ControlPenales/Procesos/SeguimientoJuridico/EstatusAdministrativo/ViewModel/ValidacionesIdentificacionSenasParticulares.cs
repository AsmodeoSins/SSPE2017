
namespace ControlPenales
{
    partial class EstatusAdministrativoViewModel
    {
        void setValidacionesIdentificacionSeniasParticulares()
        {
            #region DatosGenerales
            //base.AddRule(() => SelectSexo, () => !string.IsNullOrEmpty(SelectSexo), "TIPO DE INGRESO ES REQUERIDO!");
            //base.AddRule(() => SelectTipoIngreso, () => SelectTipoIngreso >= 0, "TIPO DE INGRESO ES REQUERIDO!");
            //base.AddRule(() => SelectTipoIngreso, () => SelectTipoIngreso >= 0, "TIPO DE INGRESO ES REQUERIDO!");
            //base.AddRule(() => SelectTipoIngreso, () => SelectTipoIngreso >= 0, "TIPO DE INGRESO ES REQUERIDO!");
            //base.AddRule(() => SelectTipoIngreso, () => SelectTipoIngreso >= 0, "TIPO DE INGRESO ES REQUERIDO!");
            //base.AddRule(() => SelectTipoIngreso, () => SelectTipoIngreso >= 0, "TIPO DE INGRESO ES REQUERIDO!");
            //base.AddRule(() => SelectTipoIngreso, () => SelectTipoIngreso >= 0, "TIPO DE INGRESO ES REQUERIDO!");
            //base.AddRule(() => SelectTipoIngreso, () => SelectTipoIngreso >= 0, "TIPO DE INGRESO ES REQUERIDO!");
            //base.AddRule(() => SelectTipoIngreso, () => SelectTipoIngreso >= 0, "TIPO DE INGRESO ES REQUERIDO!");
            //base.AddRule(() => SelectTipoIngreso, () => SelectTipoIngreso >= 0, "TIPO DE INGRESO ES REQUERIDO!");
            base.ClearRules();
            base.AddRule(() => TextCantidad, () => !string.IsNullOrWhiteSpace(TextCantidad) && int.Parse(TextCantidad) > 0, "CANTIDAD ES REQUERIDA");
            base.AddRule(()=> IsPresentaSelected,()=>IsPresentaSelected,"PRESENTA ES REQUERIDA!");
            base.AddRule(()=> IsTipoSelected,()=>IsTipoSelected, "TIPO ES REQUERIDO");
            base.AddRule(() => IsImagenTatuajeCapturada, () => IsImagenTatuajeCapturada, "FOTO ES REQUERIDA");
            RaisePropertyChanged("TextCantidad");
            RaisePropertyChanged("IsPresentaSelected");
            RaisePropertyChanged("IsTipoSelected");
            RaisePropertyChanged("IsImagenTatuajeCapturada");
            //if (SelectPresentaIngresar == false && SelectPresentaIntramuros == false)
            //{
            //    (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar si es al ingresar o fue intramuros.");
            //    return false;
            //}
            //if (SelectAnatomiaTopografica == null)
            //{
            //    (new Dialogos()).ConfirmacionDialogo("Error", "En la region seleccionada.");
            //    return false;
            //}
            //if (SelectTipoSenia <= 0)
            //{
            //    (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar el tipo de seña.");
            //    return false;
            //}
            //if (SelectTipoSenia == 2 && (SelectTatuaje == null || SelectTatuaje.ID_TATUAJE <= 0))
            //{
            //    (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar el tipo de tatuaje.");
            //    return false;
            //}
            ///*else if (SelectTipoSenia == 2 && (SelectClasificacionTatuaje == null || SelectClasificacionTatuaje.ID_TATUAJE_CLA == ""))
            //    return false;*/
            //if (string.IsNullOrEmpty(CodigoSenia))
            //{
            //    (new Dialogos()).ConfirmacionDialogo("Error", "En el código.");
            //    return false;
            //}
            //if (string.IsNullOrEmpty(TextSignificado))
            //{
            //    (new Dialogos()).ConfirmacionDialogo("Error", "En el significado.");
            //    return false;
            //}
            //if (string.IsNullOrEmpty(TextCantidad))
            //{
            //    (new Dialogos()).ConfirmacionDialogo("Error", "Debes ingresar la cantidad.");
            //    FocusCantidad = true;
            //    return false;
            //}
            //if (ImagenTatuaje == null)
            //{
            //    (new Dialogos()).ConfirmacionDialogo("Error", "Debes tomar la foto de la seña.");
            //    return false;
            //}
            #endregion
        }
    }
}
