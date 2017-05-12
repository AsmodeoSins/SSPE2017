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
    partial class EntrevistaMultidiciplinariaViewModel : ValidationViewModelBase
    {
        private void PopulateGrupoFamiliarPV()
        {
            try
            {
                //if (LstOcupacion == null)
                //    LstOcupacion = new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos());
                //if (LstEstadoCivil == null)
                //    LstEstadoCivil = new ObservableCollection<ESTADO_CIVIL>(new cEstadoCivil().ObtenerTodos());


                LstPV = new ObservableCollection<Clases.GrupoFamiliarPV>();
                //if (emiActual != null)
                if (emiActual != null)
                {
                    //if (emiActual.INGRESO != null)
                    if (SelectIngreso != null)
                    {
                        //if (emiActual.INGRESO.VISITA_AUTORIZADA != null)
                        if (SelectIngreso.VISITA_AUTORIZADA != null)
                        {
                            if (LstGrupoFamiliar != null)
                                //foreach (var p in emiActual.INGRESO.VISITA_AUTORIZADA)
                                foreach (var p in SelectIngreso.VISITA_AUTORIZADA)
                                {
                                    if (LstGrupoFamiliar.Where(w => w.ID_VISITA == p.ID_VISITA && w.ID_INGRESO == p.ID_INGRESO &&
                                        w.ID_IMPUTADO == p.ID_IMPUTADO && w.ID_CENTRO == p.ID_CENTRO && w.ID_ANIO == p.ID_ANIO).Count() == 0)
                                    {
                                        LstPV.Add(
                                            new Clases.GrupoFamiliarPV()
                                            {
                                                Seleccionado = false,
                                                Nombre = p.NOMBRE,
                                                Paterno = p.PATERNO,
                                                Materno = p.MATERNO,
                                                IdReferencia = p.ID_PARENTESCO,
                                                TipoReferencia = p.TIPO_REFERENCIA,
                                                IdGrupo = 1,
                                                ViveConEl = false,
                                                ID_ANIO = p.ID_ANIO,
                                                ID_CENTRO = p.ID_CENTRO,
                                                ID_IMPUTADO = p.ID_IMPUTADO,
                                                ID_INGRESO = p.ID_INGRESO,
                                                ID_VISITA = p.ID_VISITA,
                                                Edad = p.EDAD,
                                                IdOcupacion = -1,
                                                IdEstadoCivil = -1,
                                                Domicilio = string.Empty,
                                                FNacimiento = p.PERSONA != null ? p.PERSONA.FEC_NACIMIENTO : null
                                            });
                                    }
                                }
                            //else
                            //    StaticSourcesViewModel.Mensaje("", "No existen registros", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA);
                        }

                    }
                }
                if (LstPV.Count > 0)
                    EmptyPadronVisita = false;
                else
                    EmptyPadronVisita = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer grupo familiar PV", ex);
            }
        }

        private void AgregarGrupoFamiliarPV()
        {
            try
            {
                if (emiActual != null)
                {
                    if (LstPV != null)
                    {
                        int edad = 0;
                        foreach (var pv in LstPV)
                        {
                            if (LstGrupoFamiliar == null)
                                LstGrupoFamiliar = new ObservableCollection<EMI_GRUPO_FAMILIAR>();
                            if (pv.Seleccionado)
                            {
                                edad = new Fechas().CalculaEdad(Fechas.GetFechaDateServer);
                                var egf = new EMI_GRUPO_FAMILIAR();
                                egf.NOMBRE = pv.Nombre;
                                egf.PATERNO = pv.Paterno;
                                egf.MATERNO = pv.Materno;
                                egf.EDAD = pv.Edad;
                                egf.DOMICILIO = pv.Domicilio;
                                egf.VIVE_C_EL = pv.ViveConEl == true ? "S" : "N";
                                egf.ID_OCUPACION = pv.IdOcupacion!= -1 ? pv.IdOcupacion : null;
                                egf.OCUPACION = pv.IdOcupacion != -1 ?  pv.Ocupacion : null;
                                egf.ID_RELACION = pv.IdReferencia != - 1 ? pv.IdReferencia : null;
                                egf.TIPO_REFERENCIA = pv.IdReferencia != -1 ?  pv.TipoReferencia : null;
                                egf.ID_ESTADO_CIVIL = pv.IdEstadoCivil != -1 ? pv.IdEstadoCivil  : null;
                                egf.ESTADO_CIVIL = pv.IdEstadoCivil != -1 ? pv.EstadoCivil : null;
                                egf.GRUPO = pv.IdGrupo;
                                egf.NACIMIENTO_FEC = pv.FNacimiento;
                                egf.ID_ANIO = pv.ID_ANIO;
                                egf.ID_CENTRO = pv.ID_CENTRO;
                                egf.ID_IMPUTADO = (int)pv.ID_IMPUTADO;
                                egf.ID_INGRESO = pv.ID_INGRESO;
                                egf.ID_VISITA = (short)pv.ID_VISITA;
                                egf.ID_EMI = emiActual.ID_EMI;
                                egf.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                                egf.ID_CONS = LstGrupoFamiliar.LastOrDefault() != null ? (short)(LstGrupoFamiliar.LastOrDefault().ID_CONS + 1) : (short)1;

                                LstGrupoFamiliar.Add(egf);
                            }
                        }
                    }
                }
                else
                    AvisoImputadoVacio();
                if (LstGrupoFamiliar != null)
                {
                    if (LstGrupoFamiliar.Count > 0)
                        IsGrupoFamiliarEmpty = false;
                    else
                        IsGrupoFamiliarEmpty = true;
                }
                else
                    IsGrupoFamiliarEmpty = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar grupo familiar PV", ex);
            }
        }

        #region [LOAD]
        private void DatosGrupoFamiliarLoad(DatosGrupoFamiliar Window = null)
        {
            try
            {
                if (TabFactoresSocioFamiliaresSelected)
                {
                    if (TabGrupoFamiliarSelected)//LA OPCION ESTA SELECCIONADA
                    {
                        AntecedenteGrupoFamiliarEnabled = true;
                        base.ClearRules();
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos grupo familiar", ex);
            }
        }

        private void AntecedenteFamiliarLoad(AntecedentesGrupoFamilliar Window = null)
        {
            try
            {
                if (TabFactoresSocioFamiliaresSelected)
                {
                    if (TabGrupoFamiliarAntecedenteSelected)
                    {
                        ConductasParasocialesEnabled = UsoDrogaEnabled = true;
                        base.ClearRules();
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar antecedente familiar", ex);
            }
        }

        private void UsoDrogaLoad(UsoDrogas Window = null)
        {
            try
            {
                if (TabConductasParasocialesSelected)
                {
                    if (TabUsoDrogaSelected)
                    {
                        HPSEnabled = true;
                        base.ClearRules();
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar uso droga", ex);
            }
        }
        #endregion
        private void DatosGrupoFamiliarUnload(DatosGrupoFamiliar Window = null)
        {
            try
            {
                GuardarDatosGrupoFamiliar();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de datos grupo familiar", ex);
            }
        }

        private void AntecedenteFamiliarUnload(AntecedentesGrupoFamilliar Window = null)
        {
            try
            {
                GuardarAntecedentesGrupoFamiliar();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de antecedente familiar", ex);
            }
        }

        private void UsoDrogaUnload(UsoDrogas Window = null)
        {
            try
            {
                GuardarUsoDroga();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de uso droga", ex);
            }
        }
    }
}
