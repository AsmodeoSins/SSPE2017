using System;
namespace ControlPenales
{
    partial class EMILiberadoViewModel
    {
        void setValidacionesEntrevistaMuiltidiciplinaria()
        {
            try
            {
                base.ClearRules();
                //base.AddRule(() => ApellidoPaterno, () => !string.IsNullOrEmpty(ApellidoPaterno), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => ApellidoMaterno, () => !string.IsNullOrEmpty(ApellidoMaterno), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Nombre, () => !string.IsNullOrEmpty(Nombre), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => EstadoCivil, () => !string.IsNullOrEmpty(EstadoCivil), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Sexo, () => !string.IsNullOrEmpty(Sexo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => FechaNacimiento, () => FechaNacimiento != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Edad, () => !string.IsNullOrEmpty(Edad), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Religion, () => !string.IsNullOrEmpty(Religion), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Etnia, () => !string.IsNullOrEmpty(Etnia), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Apodo, () => !string.IsNullOrEmpty(Apodo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => LNMunicipio, () => !string.IsNullOrEmpty(LNMunicipio), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => LNEstado, () => !string.IsNullOrEmpty(LNEstado), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => LNPais, () => !string.IsNullOrEmpty(LNPais), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Nacionalidad, () => !string.IsNullOrEmpty(Nacionalidad), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => FechaLlegada, () => FechaLlegada != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Años, () => !string.IsNullOrEmpty(Años.ToString()), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Meses, () => !string.IsNullOrEmpty(Meses.ToString()), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Dias, () => !string.IsNullOrEmpty(Dias), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => DPais, () => !string.IsNullOrEmpty(DPais), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => DEstado, () => !string.IsNullOrEmpty(DEstado), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => DMunicipio, () => !string.IsNullOrEmpty(DMunicipio), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Colonia, () => !string.IsNullOrEmpty(Colonia), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Calle, () => !string.IsNullOrEmpty(Calle), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => NumeroExterior, () => NumeroExterior != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => NumeroInterior, () => NumeroInterior != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => CodigoPostal, () => CodigoPostal != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en entrevista multidiciplinaria", ex);
            }
        }
    }
}
