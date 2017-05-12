using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.EstudioMI;
namespace ControlPenales
{
    partial class EMILiberadoViewModel : ValidationViewModelBase
    {
        private void PopulateEstudiosTraslados() 
        {
            try
            {
                //INFORMACION ACTUAL
                if (emiActual != null)
                    if (emiActual.EMI_SITUACION_JURIDICA != null)
                    {
                        SituacionJuridicaEnabled = EstudioTrasladoEnabled = true;
                        VersionDelito = emiActual.EMI_SITUACION_JURIDICA.VERSION_DELITO_INTERNO;
                        PracticadoEstudios = emiActual.EMI_SITUACION_JURIDICA.PRACT_ESTUDIOS == null ? false : emiActual.EMI_SITUACION_JURIDICA.PRACT_ESTUDIOS == "S" ? true : false;
                        Traslado = emiActual.EMI_SITUACION_JURIDICA.DESEA_TRASLADO == null ? false : emiActual.EMI_SITUACION_JURIDICA.DESEA_TRASLADO == "S" ? true : false;
                        Cuando = emiActual.EMI_SITUACION_JURIDICA.CUANDO_PRACT_ESTUDIOS;
                        Donde = emiActual.EMI_SITUACION_JURIDICA.DONDE_DESEA_TRASLADO;
                        PorqueMotivo = emiActual.EMI_SITUACION_JURIDICA.MOTIVO_DESEA_TRASLADO;
                        MenorPeriodoLibreReingreso = emiActual.EMI_SITUACION_JURIDICA.MENOR_PERIODO_LIBRE_REING;
                        MayorPeriodoLibreReingreso = emiActual.EMI_SITUACION_JURIDICA.MAYOR_PERIODO_LIBRE_REING;
                        return;
                    }
                //INFORMACION HISTORICA
                if (AnteriorEMI != null)
                    if (AnteriorEMI.EMI_SITUACION_JURIDICA != null)
                    {
                        VersionDelito = AnteriorEMI.EMI_SITUACION_JURIDICA.VERSION_DELITO_INTERNO;
                        PracticadoEstudios = AnteriorEMI.EMI_SITUACION_JURIDICA.PRACT_ESTUDIOS == null ? false : AnteriorEMI.EMI_SITUACION_JURIDICA.PRACT_ESTUDIOS == "S" ? true : false;
                        Traslado = AnteriorEMI.EMI_SITUACION_JURIDICA.DESEA_TRASLADO == null ? false : AnteriorEMI.EMI_SITUACION_JURIDICA.DESEA_TRASLADO == "S" ? true : false;
                        Cuando = AnteriorEMI.EMI_SITUACION_JURIDICA.CUANDO_PRACT_ESTUDIOS;
                        Donde = AnteriorEMI.EMI_SITUACION_JURIDICA.DONDE_DESEA_TRASLADO;
                        PorqueMotivo = AnteriorEMI.EMI_SITUACION_JURIDICA.MOTIVO_DESEA_TRASLADO;
                        MenorPeriodoLibreReingreso = AnteriorEMI.EMI_SITUACION_JURIDICA.MENOR_PERIODO_LIBRE_REING;
                        MayorPeriodoLibreReingreso = AnteriorEMI.EMI_SITUACION_JURIDICA.MAYOR_PERIODO_LIBRE_REING;
                        return;
                    }

                //NUEVO REGISTRO
                PracticadoEstudios = false;
                Traslado = false;
                Cuando = Donde = PorqueMotivo = string.Empty;
                MenorPeriodoLibreReingreso = "NINGUNO";
                MayorPeriodoLibreReingreso = "NINGUNO";
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer estudio traslado", ex);
            }
        }

        private void PopulateIngresosAnteriores()
        {
            try
            {
                //INFORMACION ACTUAL
                if (emiActual.EMI_INGRESO_ANTERIOR != null)
                {
                    if (emiActual.EMI_INGRESO_ANTERIOR.Where(w => w.ID_TIPO == 2).Count() > 0)
                    {
                        IngresoAnteriorEnabled = true;
                        LstIngresosAnteriores = new ObservableCollection<EMI_INGRESO_ANTERIOR>(new cEMIIngresosAnteriores().Obtener(emiActual.ID_EMI, emiActual.ID_EMI_CONS, 2));
                        EmptyIngresosAnteriores = LstIngresosAnteriores.Count > 0 ? false : true;
                        return;
                    }
                }

                //INFORMACION HISTORICA
                if (AnteriorEMI != null)
                    if (AnteriorEMI.EMI_INGRESO_ANTERIOR != null)
                    {
                        LstIngresosAnteriores = new ObservableCollection<EMI_INGRESO_ANTERIOR>(AnteriorEMI.EMI_INGRESO_ANTERIOR.Where(w => w.ID_TIPO == 2));
                        EmptyIngresosAnteriores = LstIngresosAnteriores.Count > 0 ? false : true;
                        return;
                    }

                //NUEVO REGISTRO
                LstIngresosAnteriores = new ObservableCollection<EMI_INGRESO_ANTERIOR>();
                EmptyIngresosAnteriores = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer ingreso anterior", ex);
            }
        }

        private void PopulateIngresosAnterioresMenores()
        {
            try
            {
                //INFORMACION ACTUAL
                if (emiActual.EMI_INGRESO_ANTERIOR != null)
                {
                    if (emiActual.EMI_INGRESO_ANTERIOR.Where(w => w.ID_TIPO == 1).Count() > 0)
                    {
                        IngresoAnteriorMenorEnabled = true;
                        LstIngresosAnterioresMenor = new ObservableCollection<EMI_INGRESO_ANTERIOR>(new cEMIIngresosAnteriores().Obtener(emiActual.ID_EMI, emiActual.ID_EMI_CONS, 1));
                        EmptyIngresosAnterioresMenores = LstIngresosAnterioresMenor.Count > 0 ? false : true;
                        return;
                    }
                }

                //INFORMACION HISTORICA
                if (AnteriorEMI != null)
                    if (AnteriorEMI.EMI_INGRESO_ANTERIOR != null)
                    {
                        LstIngresosAnterioresMenor = new ObservableCollection<EMI_INGRESO_ANTERIOR>(AnteriorEMI.EMI_INGRESO_ANTERIOR.Where(w => w.ID_TIPO == 1));
                        EmptyIngresosAnterioresMenores = LstIngresosAnterioresMenor.Count > 0 ? false : true;
                        return;
                    }

                //NUEVO REGISTRO
                LstIngresosAnterioresMenor = new ObservableCollection<EMI_INGRESO_ANTERIOR>();
                EmptyIngresosAnterioresMenores = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer ingreso anterior menores", ex);
            }
        }
    }
}
