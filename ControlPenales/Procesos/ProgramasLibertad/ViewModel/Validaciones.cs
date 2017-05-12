using ControlPenales;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Servidor;
using ControlPenales.Clases;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class ProgramasLibertadViewModel
    {
        private void ValidacionAgenda() 
        {
            base.ClearRules();
            base.AddRule(() => EstatusAL, () => EstatusAL != -1, "ESTATUS ES REQUERIDO!");
            base.AddRule(() =>  UnidadReceptoraAL , () => UnidadReceptoraAL.Value != -1, "UNIDAD RECEPTORA ES REQUERIDA!");
            base.AddRule(() =>  ProgramaLibertadAL , () => ProgramaLibertadAL.Value != -1, "PROGRAMA ES REQUERIDO!");
            base.AddRule(() => ActividadProgramadaAL, () => ActividadProgramadaAL.Value != -1, "ACTIVIDAD ES REQUERIDA!");
            base.AddRule(() =>  FechaInicioAL  , () => FechaInicioAL.HasValue, "FECHA DE INICIO ES REQUERIDA!");
            base.AddRule(() =>  FechaFinalAL  , () => FechaFinalAL.HasValue, "FECHA FINAL ES REQUERIDO!");
            OnPropertyChanged("EstatusAL");
            OnPropertyChanged("UnidadReceptoraAL");
            OnPropertyChanged("ProgramaLibertadAL");
            OnPropertyChanged("ActividadProgramadaAL");
            OnPropertyChanged("FechaInicioAL");
            OnPropertyChanged("FechaFinalAL");
        }

        private void ValidacionOficioAsignacion() 
        {
            base.ClearRules();
            base.AddRule(() => OAFEcha, () => OAFEcha.HasValue, "FECHA ES REQUERIDA!");
            base.AddRule(() => OACPAnio, () => OACPAnio.HasValue, "AÑO DE LA CAUSA PENAL ES REQUERIDO!");
            base.AddRule(() => OACPFolio, () => OACPFolio.HasValue, "FOLIO DE LA CAUSA PENAL ES REQUERIDO!");
            //base.AddRule(() => OANUC, () => !string.IsNullOrEmpty(OANUC), "NUC ES REQUERIDO!");
            base.AddRule(() => OAJuzgado, () => !string.IsNullOrEmpty(OAJuzgado), "JUZGADO ES REQUERIDO!");
            base.AddRule(() => OADelito, () => !string.IsNullOrEmpty(OADelito), "DELITO ES REQUERIDO!");
            base.AddRule(() => OASustitucionPena, () => !string.IsNullOrEmpty(OASustitucionPena), "SUSTITUCIÓN DE LA PENA ES REQUERIDO!");
            base.AddRule(() => OANoJornadasLetra, () => !string.IsNullOrEmpty(OANoJornadasLetra), "NÚMERO DE JORNADAS ES REQUERIDO!");
            OnPropertyChanged("OAFEcha");
            OnPropertyChanged("OACPAnio");
            OnPropertyChanged("OACPFolio");
            OnPropertyChanged("OANUC");
            OnPropertyChanged("OAJuzgado");
            OnPropertyChanged("OADelito");
            OnPropertyChanged("OASustitucionPena");
            OnPropertyChanged("OANoJornadasLetra");
        }

        private void ValidacionOficioConclusion() 
        {
            base.ClearRules();
            base.AddRule(() => OCFecha, () => OCFecha.HasValue, "FECHA ES REQUERIDA!");
            base.AddRule(() => OCCPAnio, () => OCCPAnio.HasValue, "AÑO DE LA CAUSA PENAL ES REQUERIDO!");
            base.AddRule(() => OCCPFolio, () => OCCPFolio.HasValue, "FOLIO DE LA CAUSA PENAL ES REQUERIDO!");
            base.AddRule(() => OCJuzgado, () => !string.IsNullOrEmpty(OCJuzgado), "JUZGADO ES REQUERIDO!");
            base.AddRule(() => OCDelito, () => !string.IsNullOrEmpty(OCDelito), "DELITO ES REQUERIDO!");
            base.AddRule(() => OCJornadasCumplidas, () => !string.IsNullOrEmpty(OCJornadasCumplidas), "JORNADAS CUMPLIDAS ES REQUERIDO!");
            base.AddRule(() => OCOficioConclusion, () => !string.IsNullOrEmpty(OCOficioConclusion), "OFICIO DE CONCLUSIÓN ES REQUERIDO!");
            base.AddRule(() => OCFechaConclusion, () => OCFechaConclusion.HasValue, "FECHA DE CONCLUSION ES REQUERIDO!");
            OnPropertyChanged("OCFecha");
            OnPropertyChanged("OCCPAnio");
            OnPropertyChanged("OCCPFolio");
            OnPropertyChanged("OCJuzgado");
            OnPropertyChanged("OCDelito");
            OnPropertyChanged("OCJornadasCumplidas");
            OnPropertyChanged("OCOficioConclusion");
            OnPropertyChanged("OCFechaConclusion");
        }

        private void ValidacionOficioBaja() 
        {
            base.ClearRules();
            base.AddRule(() => OBFecha, () => OBFecha.HasValue, "FECHA ES REQUERIDA!");
            base.AddRule(() => OBDias, () => OBDias.HasValue, "DIAS ES REQUERIDA!");
            base.AddRule(() => OBCPAnio, () => OBCPAnio.HasValue, "AÑO DE LA CAUSA PENAL ES REQUERIDO!");
            base.AddRule(() => OBCPFolio, () => OBCPFolio.HasValue, "FOLIO DE LA CAUSA PENAL ES REQUERIDO!");
            base.AddRule(() => OBDiasRegistrados, () => OBDiasRegistrados.HasValue, "DIAS REGISTRADOS ES REQUERIDA!");
            base.AddRule(() => OBPrograma, () => !string.IsNullOrEmpty(OBPrograma), "PROGRAMA ES REQUERIDA!");
            base.AddRule(() => OBMesBaja, () => OBMesBaja.Value != -1 , "MES DE BAJA ES REQUERIDA!");
            base.AddRule(() => OBDiasPendientes, () => OBDiasPendientes.HasValue, "PENDIENTE ES REQUERIDA!");
            base.AddRule(() => OBNumeroBaja, () => OBNumeroBaja != -1, "NUMERO DE BAJA ES REQUERIDO!");
            OnPropertyChanged("OBFecha");
            OnPropertyChanged("OBDias");
            OnPropertyChanged("OBCPAnio");
            OnPropertyChanged("OBCPFolio");
            OnPropertyChanged("OBDiasRegistrados");
            OnPropertyChanged("OBPrograma");
            OnPropertyChanged("OBMesBaja");
            OnPropertyChanged("OBDiasPendientes");
            OnPropertyChanged("OBNumeroBaja");
        }
    }
}
