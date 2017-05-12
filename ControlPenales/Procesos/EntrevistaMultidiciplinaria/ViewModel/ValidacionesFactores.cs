using System;
namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel
    {
        void setValidacionesFactores()
        {
            try
            {
                base.ClearRules();

                base.AddRule(() => VivePadre, () => !string.IsNullOrEmpty(VivePadre), "RECIBE VISITA ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("VivePadre");
                base.AddRule(() => ViveMadre, () =>  !string.IsNullOrEmpty(ViveMadre), "RECIBE VISITA ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("ViveMadre");
                //base.AddRule(() => PadresVivenJuntos, () => VivePadre == "N" && ViveMadre == "N" ? true : !string.IsNullOrEmpty(PadresVivenJuntos), "RECIBE VISITA ES REQUERIDO PARA GRABAR");
                //OnPropertyChanged("PadresVivenJuntos");

                
                base.AddRule(() => RecibeVisitaFamiliar, () => !string.IsNullOrEmpty(RecibeVisitaFamiliar), "RECIBE VISITA ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("RecibeVisitaFamiliar");
                if (!string.IsNullOrEmpty(RecibeVisitaFamiliar))
                    if (RecibeVisitaFamiliar.Equals("S"))
                    {
                        base.AddRule(() => Frecuencia, () => Frecuencia.HasValue ? Frecuencia.Value != -1 : false, "RECIBE VISITA ES REQUERIDO PARA GRABAR");
                    }
                OnPropertyChanged("Frecuencia");
                base.AddRule(() => VisitaIntima, () => !string.IsNullOrEmpty(VisitaIntima), "VISITA INTIMA ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("VisitaIntima");
                base.AddRule(() => ApoyoEconomico, () => !string.IsNullOrEmpty(ApoyoEconomico), "APOYO ECONÓMICO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("ApoyoEconomico");
                if (!string.IsNullOrEmpty(ApoyoEconomico))
                    if (ApoyoEconomico.Equals("S"))
                    {
                        base.AddRule(() => CantidadApoyoEconomico, () => !string.IsNullOrEmpty(CantidadApoyoEconomico), "APOYO ECONÓMICO ES REQUERIDO PARA GRABAR");
                        base.AddRule(() => CantidadFrecuencia, () => CantidadFrecuencia.HasValue ? CantidadFrecuencia.Value != -1 : false, "APOYO ECONÓMICO ES REQUERIDO PARA GRABAR");
                    }
                OnPropertyChanged("CantidadApoyoEconomico");
                OnPropertyChanged("CantidadFrecuencia");
                base.AddRule(() => TotalParejas, () => TotalParejas != null, "TOTAL PAREJAS ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("TotalParejas");
                base.AddRule(() => Hijos, () => Hijos != null, "HIJOS ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("Hijos");
                base.AddRule(() => Registrados, () => Registrados.HasValue ? Registrados <= Hijos : false, "ES MAYOR AL NUMERO DE HIJOS");
                OnPropertyChanged("Registrados");
                base.AddRule(() => CuantosMantieneRelacion, () => CuantosMantieneRelacion.HasValue ? CuantosMantieneRelacion <= Hijos : false, "ES MAYOR AL NUMERO DE HIJOS");
                OnPropertyChanged("CuantosMantieneRelacion");
                base.AddRule(() => CuantosVisitan, () => CuantosVisitan.HasValue ? CuantosVisitan <= Hijos : false, "ES MAYOR AL NUMERO DE HIJOS.");
                OnPropertyChanged("CuantosVisitan");
                base.AddRule(() => CuantasUnion, () => CuantasUnion.HasValue ? CuantasUnion <= TotalParejas : false, "PAREJAS EN UNION NO PUEDE SER MAYOR AL TOTAL DE PAREJAS");
                OnPropertyChanged("CuantasUnion");
                base.AddRule(() => ContactoNombre, () => !string.IsNullOrEmpty(ContactoNombre), "CONTACTO NOMBRE ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("ContactoNombre");
                //base.AddRule(() => ContactoTelefono, () => ContactoTelefono != null, "CONTACTO TELEFONO ES REQUERIDO PARA GRABAR");
                //OnPropertyChanged("ContactoTelefono");
                base.AddRule(() => ContactoTelefono, () => !string.IsNullOrEmpty(TextContactoTelefono), "CONTACTO TELÉFONO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("ContactoTelefono");
                base.AddRule(() => ContactoParentesco, () => ContactoParentesco.HasValue ? ContactoParentesco != -1 : false, "CONTACTO PARENTESCO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("ContactoParentesco");

                base.AddRule(() => Social, () => Social.HasValue ? Social.Value != -1 : false, "SOCIAL ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("Social");
                base.AddRule(() => Cultural, () => Cultural.HasValue ? Cultural.Value != -1 : false, "CULTURAL ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("Cultural");
                base.AddRule(() => Economico, () => Economico.HasValue ? Economico.Value != -1 : false, "ECONÓMICO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("Economico");

                base.AddRule(() => MaltratoEmocional, () => !string.IsNullOrEmpty(MaltratoEmocional), "MALTRATO EMOCIONAL ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("MaltratoEmocional");

                base.AddRule(() => MaltratoFisico, () => !string.IsNullOrEmpty(MaltratoFisico), "MALTRATO FÍSICO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("MaltratoFisico");

                base.AddRule(() => AbusoSexual, () => !string.IsNullOrEmpty(AbusoSexual), "ABUSO SEXUAL ES REQUERIDO PARA GRABAR");

                OnPropertyChanged("AbusoSexual");
                base.AddRule(() => AbandonoFamiliar, () => !string.IsNullOrEmpty(AbandonoFamiliar), "ABANDONO FAMILIAR ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("AbandonoFamiliar");

                if (MaltratoEmocional != null)
                    if (MaltratoEmocional == "S")
                    {
                        base.AddRule(() => EspecifiqueMaltratoEmocional, () => !string.IsNullOrEmpty(EspecifiqueMaltratoEmocional) ? true : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                    }
                OnPropertyChanged("EspecifiqueMaltratoEmocional");

                if (MaltratoFisico != null)
                    if (MaltratoFisico == "S")
                    {
                        base.AddRule(() => EspecifiqueMaltratoFisico, () => !string.IsNullOrEmpty(EspecifiqueMaltratoFisico) ? true : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                    }
                OnPropertyChanged("EspecifiqueMaltratoFisico");

                if (AbusoSexual != null)
                    if (AbusoSexual == "S")
                    {
                        base.AddRule(() => EspecifiqueAbusoSexual, () => !string.IsNullOrEmpty(EspecifiqueAbusoSexual) ? true : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                        if (EdadAbuso != null)
                        {
                            base.AddRule(() => EdadAbuso, () => EdadAbuso <= EdadInterno, "LA EDAD ES MAYOR A LA ACTUAL");

                        }
                        if (EdadInicioContactoSexual != null)
                        {
                            base.AddRule(() => EdadInicioContactoSexual, () => EdadInicioContactoSexual <= EdadInterno, "LA EDAD ES MAYOR A LA ACTUAL");

                        }
                    }
                OnPropertyChanged("EspecifiqueAbusoSexual");
                OnPropertyChanged("EdadAbuso");
                OnPropertyChanged("EdadInicioContactoSexual");

                base.AddRule(() => HuidasHogar, () => HuidasHogar.HasValue ? HuidasHogar.Value != -1 ? true : false : false, "HUIDAS HOGAR ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("HuidasHogar");
                if (AbandonoFamiliar != null)
                    if (AbandonoFamiliar == "S")
                    {
                        base.AddRule(() => EspecifiqueAbandonoFamiliar, () => EspecifiqueAbandonoFamiliar.HasValue ? EspecifiqueAbandonoFamiliar.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                    }
                OnPropertyChanged("EspecifiqueAbandonoFamiliar");
                
                //if (VivePadre != null)
                //    if (VivePadre == "N")
                //    {
                //        base.AddRule(() => FallecioPadre, () => FallecioPadre.HasValue ? FallecioPadre <= new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA) :
                //            false, "LA EDAD ES MAYOR A LA ACTUAL");

                //    }
                //OnPropertyChanged("FallecioPadre");

                //if (ViveMadre != null)
                //    if (ViveMadre == "N")
                //    {
                //        base.AddRule(() => FallecioMadre, () => FallecioMadre.HasValue ? FallecioMadre <= new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA) : false, "LA EDAD ES MAYOR A LA ACTUAL");

                //    }
                //OnPropertyChanged("FallecioMadre");

                //if (PadresVivenJuntos != null)
                //    if (PadresVivenJuntos == "N")
                //    {
                //        base.AddRule(() => EdadInternoSeparacionPadres, () => EdadInternoSeparacionPadres.HasValue ? EdadInternoSeparacionPadres <= new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA) : false, "LA EDAD ES MAYOR A LA ACTUAL");
                //        base.AddRule(() => MotivoSeparacion, () => !string.IsNullOrEmpty(MotivoSeparacion), "ESTE CAMPO ES REQUERIDO PARA GRABAR");

                //    }
                //OnPropertyChanged("EdadInternoSeparacionPadres");
                //OnPropertyChanged("MotivoSeparacion");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en factores", ex);
            }
        }
    }
}
