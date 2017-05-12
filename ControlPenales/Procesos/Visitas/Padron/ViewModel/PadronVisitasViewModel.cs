using Cogent.Biometrics;
using ControlPenales.Clases;
using DPUruNet;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using LinqKit;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WPFPdfViewer;
using ControlPenales.Clases.ReportePasesPorAutorizar;
using ControlPenales.BiometricoServiceReference;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace ControlPenales
{
    partial class PadronVisitasViewModel : FingerPrintScanner
    {
        public PadronVisitasViewModel() { }

        private async Task<bool> GuardarPadronVisita()
        {
            try
            {
                if (HasErrors())
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", string.Format("Error al validar el campo: {0}.", base.Error));
                    return false;
                }
                var hoy = Fechas.GetFechaDateServer;
                var TipoVisitaFamiliar = Parametro.ID_TIPO_VISITA_FAMILIAR;
                var EstatusVisitaAutorizado = Parametro.ID_ESTATUS_VISITA_AUTORIZADO;
                var EstatusVisitaCancelado = Parametro.ID_ESTATUS_VISITA_CANCELADO;
                var EstatusVisitaSuspendido = Parametro.ID_ESTATUS_VISITA_SUSPENDIDO;
                var EstatusVisitaRegistro = Parametro.ID_ESTATUS_VISITA_REGISTRO;
                var EstatusVisitaEnRevision = Parametro.ID_ESTATUS_VISITA_EN_REVISION;
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                {
                    #region PROGRAMACION VISITAS
                    var VisitaAgenda = new List<VISITA_AGENDA>();
                    if (NuevaVisitaAgenda ? ListProgramacionVisita != null ? ListProgramacionVisita.Count > 0 : false : false)
                    {
                        VisitaAgenda = ListProgramacionVisita.Where(w => w.VISITA_AGENDA != null).Select(s => new VISITA_AGENDA
                        {
                            ID_CENTRO = s.ID_CENTRO,
                            ID_ANIO = s.ID_ANIO,
                            ID_IMPUTADO = s.ID_IMPUTADO,
                            ID_INGRESO = s.ID_INGRESO,
                            ID_AREA = s.ID_AREA,
                            ID_DIA = s.ID_DIA,
                            ID_TIPO_VISITA = s.ID_TIPO_VISITA,
                            HORA_FIN = s.HORA_FIN,
                            HORA_INI = s.HORA_INI,
                            ESTATUS = s.ESTATUS ? "0" : "1"
                        }).ToList();
                        if (!new cVisitaAgenda().InsertarLista(VisitaAgenda, SelectImputadoIngreso.ID_CENTRO, SelectImputadoIngreso.ID_ANIO,
                            SelectImputadoIngreso.ID_IMPUTADO, SelectImputadoIngreso.ID_INGRESO))
                        {
                            (new Dialogos()).ConfirmacionDialogo("ERROR", "Ocurrió un error AL GUARDAR LA AGENDA");
                            return false;
                        }
                    }
                    #endregion

                    var persona = new SSP.Servidor.PERSONA();
                    var visitante = new VISITANTE();
                    var VisitaAutorizada = new VISITA_AUTORIZADA();
                    if (!BanderaEditar)
                    {
                        #region [Insercion]
                        if (!string.IsNullOrEmpty(TextNombre) && !string.IsNullOrEmpty(TextPaterno))
                        {
                            var limite = Parametro.LIMITE_VISITA_AUTORIZADA;
                            if (ListVisitantesImputado.Where(wh => wh.OBJETO_VISITA_AUTORIZADA != null ? wh.OBJETO_VISITA_AUTORIZADA.ESTATUS == 0 : wh.OBJETO_PERSONA != null ?
                            (wh.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO.Where(w => w.ID_CENTRO == SelectImputadoIngreso.ID_CENTRO && w.ID_ANIO == SelectImputadoIngreso.ID_ANIO &&
                                w.ID_IMPUTADO == SelectImputadoIngreso.ID_IMPUTADO && w.ID_INGRESO == SelectImputadoIngreso.ID_INGRESO).FirstOrDefault().ID_ESTATUS_VISITA != 2 &&
                                wh.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO.Where(w => w.ID_CENTRO == SelectImputadoIngreso.ID_CENTRO && w.ID_ANIO == SelectImputadoIngreso.ID_ANIO &&
                                w.ID_IMPUTADO == SelectImputadoIngreso.ID_IMPUTADO && w.ID_INGRESO == SelectImputadoIngreso.ID_INGRESO).FirstOrDefault().ID_ESTATUS_VISITA != 3 &&
                                wh.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO.Where(w => w.ID_CENTRO == SelectImputadoIngreso.ID_CENTRO && w.ID_ANIO == SelectImputadoIngreso.ID_ANIO &&
                                w.ID_IMPUTADO == SelectImputadoIngreso.ID_IMPUTADO && w.ID_INGRESO == SelectImputadoIngreso.ID_INGRESO).FirstOrDefault().ID_ESTATUS_VISITA != EstatusVisitaCancelado &&
                                wh.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO.Where(w => w.ID_CENTRO == SelectImputadoIngreso.ID_CENTRO && w.ID_ANIO == SelectImputadoIngreso.ID_ANIO &&
                                w.ID_IMPUTADO == SelectImputadoIngreso.ID_IMPUTADO && w.ID_INGRESO == SelectImputadoIngreso.ID_INGRESO).FirstOrDefault().ID_ESTATUS_VISITA != EstatusVisitaSuspendido) : true).Count() >= limite)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Error", string.Format("No puedes agregar más de " + limite + " registros de visita familiar."));
                                return false;
                            }
                            var respuesta = new cVisitaAutorizada().Insertar(new VISITA_AUTORIZADA
                            {
                                EDAD = TextEdad.HasValue ? TextEdad.Value : FechaNacimiento.HasValue ? Convert.ToInt16(((hoy - FechaNacimiento.Value).TotalDays / 365)) : new Nullable<short>(),
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_PARENTESCO = SelectParentesco,
                                ID_TIPO_VISITA = TipoVisitaFamiliar,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                MATERNO = TextMaterno,
                                NOMBRE = TextNombre,
                                PATERNO = TextPaterno,
                                SEXO = SelectSexo,
                                DOMICILIO_CALLE = TextCalle,
                                DOMICILIO_CODIGO_POSTAL = TextCodigoPostal,
                                DOMICILIO_NUM_EXT = TextNumExt,
                                DOMICILIO_NUM_INT = TextNumInt,
                                ID_COLONIA = SelectColonia,
                                ID_ENTIDAD = SelectEntidad,
                                ID_MUNICIPIO = SelectMunicipio,
                                ID_PAIS = SelectPais,
                                TELEFONO = TextTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""),
                                ID_PERSONA = null,
                                ESTATUS = 0
                            });
                            if (respuesta)
                            {
                                LimpiarCapturaVisita();
                                CapturarVisitanteVisible = CapturarVisitanteEnabled = false;
                                MenuGuardarEnabled = !PInsertar ? PEditar : true;
                                GetDatosIngresoImputadoSeleccionado();
                                base.ClearRules();
                                BanderaEditar = false;
                                OnPropertyChanged();
                                (new Dialogos()).ConfirmacionDialogo("ÉXITO!", "INFORMACIÓN GRABADA EXITOSAMENTE!");
                            }
                            else
                                (new Dialogos()).ConfirmacionDialogo("ERROR", "OCURRIÓ UN ERROR AL GRABAR LA INFORMACIÓN");
                            return respuesta;
                        }
                        else
                        {
                            LimpiarCapturaVisita();
                            CapturarVisitanteVisible = CapturarVisitanteEnabled = false;
                            MenuGuardarEnabled = !PInsertar ? PEditar : true;
                            SelectIngreso = new cIngreso().Obtener(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);
                            base.ClearRules();
                            BanderaEditar = false;
                            OnPropertyChanged();
                            (new Dialogos()).ConfirmacionDialogo("ÉXITO!", "INFORMACIÓN GRABADA EXITOSAMENTE!");
                            return true;
                        }
                        #endregion
                    }
                    else
                    {
                        #region [Actualizacion]
                        if (SelectVisitanteInterno.OBJETO_PERSONA != null)
                        {
                            persona = new SSP.Servidor.PERSONA
                            {
                                MATERNO = TextMaterno,
                                NOMBRE = TextNombre,
                                PATERNO = TextPaterno,
                                SEXO = SelectSexo,
                                DOMICILIO_CALLE = TextCalle,
                                DOMICILIO_CODIGO_POSTAL = TextCodigoPostal,
                                DOMICILIO_NUM_EXT = TextNumExt,
                                DOMICILIO_NUM_INT = TextNumInt,
                                ID_COLONIA = SelectColonia,
                                ID_ENTIDAD = SelectEntidad,
                                ID_MUNICIPIO = SelectMunicipio,
                                ID_PAIS = SelectPais,
                                CURP = TextCurp,
                                RFC = TextRfc,
                                ID_TIPO_DISCAPACIDAD = SelectDiscapacidad,
                                TELEFONO = TextTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""),
                                FEC_NACIMIENTO = SelectVisitanteInterno.OBJETO_PERSONA.FEC_NACIMIENTO,
                                CORIGINAL = SelectVisitanteInterno.OBJETO_PERSONA.CORIGINAL,
                                CORREO_ELECTRONICO = SelectVisitanteInterno.OBJETO_PERSONA.CORREO_ELECTRONICO,
                                ID_ETNIA = SelectVisitanteInterno.OBJETO_PERSONA.ID_ETNIA,
                                ID_PERSONA = SelectVisitanteInterno.OBJETO_PERSONA.ID_PERSONA,
                                ID_TIPO_PERSONA = SelectVisitanteInterno.OBJETO_PERSONA.ID_TIPO_PERSONA,
                                ESTADO_CIVIL = SelectVisitanteInterno.OBJETO_PERSONA.ESTADO_CIVIL,
                                IFE = SelectVisitanteInterno.OBJETO_PERSONA.IFE,
                                LUGAR_NACIMIENTO = SelectVisitanteInterno.OBJETO_PERSONA.LUGAR_NACIMIENTO,
                                NACIONALIDAD = SelectVisitanteInterno.OBJETO_PERSONA.NACIONALIDAD,
                                NORIGINAL = SelectVisitanteInterno.OBJETO_PERSONA.NORIGINAL,
                                SMATERNO = SelectVisitanteInterno.OBJETO_PERSONA.SMATERNO,
                                SPATERNO = SelectVisitanteInterno.OBJETO_PERSONA.SPATERNO,
                                SNOMBRE = SelectVisitanteInterno.OBJETO_PERSONA.SNOMBRE,
                                TELEFONO_MOVIL = SelectVisitanteInterno.OBJETO_PERSONA.TELEFONO_MOVIL,
                            };
                            if (SelectSituacion != SelectVisitanteInterno.OBJETO_PERSONA.VISITANTE.ID_ESTATUS_VISITA)
                            {
                                visitante = new VISITANTE
                                {
                                    ID_PERSONA = SelectVisitanteInterno.OBJETO_PERSONA.VISITANTE.ID_PERSONA,
                                    ID_ESTATUS_VISITA = SelectSituacion,
                                    FEC_ALTA = SelectVisitanteInterno.OBJETO_PERSONA.VISITANTE.FEC_ALTA,
                                    ULTIMA_MODIFICACION = SelectVisitanteInterno.OBJETO_PERSONA.VISITANTE.ULTIMA_MODIFICACION,
                                    ESTATUS_MOTIVO = SelectVisitanteInterno.OBJETO_PERSONA.VISITANTE.ESTATUS_MOTIVO,
                                };
                            }
                        }
                        else if (SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA != null)
                        {
                            if (!new cVisitaAutorizada().Actualizar(new VISITA_AUTORIZADA
                            {
                                EDAD = TextEdad.HasValue ? TextEdad.Value : FechaNacimiento.HasValue ? Convert.ToInt16(((hoy - FechaNacimiento.Value).TotalDays / 365)) : new Nullable<short>(),
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                ID_VISITA = SelectVisitanteInterno.ID_VISITA,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_PARENTESCO = SelectParentesco,
                                ID_TIPO_VISITA = TipoVisitaFamiliar,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                MATERNO = TextMaterno,
                                NOMBRE = TextNombre,
                                PATERNO = TextPaterno,
                                SEXO = SelectSexo,
                                DOMICILIO_CALLE = TextCalle,
                                DOMICILIO_CODIGO_POSTAL = TextCodigoPostal,
                                DOMICILIO_NUM_EXT = TextNumExt,
                                DOMICILIO_NUM_INT = TextNumInt,
                                ID_COLONIA = SelectColonia,
                                ID_ENTIDAD = SelectEntidad,
                                ID_MUNICIPIO = SelectMunicipio,
                                ID_PAIS = SelectPais,
                                ESTATUS = 0,
                                TELEFONO = TextTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")
                            }))
                            {
                                (new Dialogos()).ConfirmacionDialogo("ERROR", "OCURRIÓ UN ERROR AL GRABAR LA INFORMACIÓN");
                                return false;
                            }
                            LimpiarCapturaVisita();
                            CapturarVisitanteVisible = CapturarVisitanteEnabled = false;
                            MenuGuardarEnabled = !PInsertar ? PEditar : true;
                            GetDatosIngresoImputadoSeleccionado();
                            base.ClearRules();
                            BanderaEditar = false;
                            OnPropertyChanged();
                            (new Dialogos()).ConfirmacionDialogo("ÉXITO!", "INFORMACIÓN GRABADA EXITOSAMENTE!");
                            return true;
                        }
                        #endregion
                    }
                    if (!new cPersona().InsertarVisitaAutorizadaTransaccion(VisitaAutorizada, BanderaEditar,
                        VisitaAgenda, SelectVisitanteInterno.OBJETO_PERSONA != null ? persona : null,
                        SelectVisitanteInterno.OBJETO_PERSONA != null ? visitante : null))
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No se pudo guardar la información.");
                        return false;
                    }

                    LimpiarCapturaVisita();
                    CapturarVisitanteVisible = CapturarVisitanteEnabled = false;
                    MenuGuardarEnabled = !PInsertar ? PEditar : true;
                    SelectIngreso = new cIngreso().Obtener(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);
                    base.ClearRules();
                    BanderaEditar = false;
                    OnPropertyChanged();
                    (new Dialogos()).ConfirmacionDialogo("ÉXITO!", "INFORMACIÓN GRABADA EXITOSAMENTE!");
                    return true;
                }
                else
                {
                    if (ListadoInternos == null ? true : !ListadoInternos.Any())
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No puedes crear a un visitante sin relacion con un imputado.");
                        return false;
                    }
                    var persona = new SSP.Servidor.PERSONA();
                    //var personaNIP = new PERSONA_NIP();
                    var VisitantesIngresos = new List<VISITANTE_INGRESO>();
                    var acompanantes = new List<ACOMPANANTE>();
                    var visitante = new VISITANTE();
                    var VisitaAutorizada = new VISITA_AUTORIZADA();
                    var personaFotos = new List<SSP.Servidor.PERSONA_BIOMETRICO>();
                    var personaHuellas = new List<SSP.Servidor.PERSONA_BIOMETRICO>();
                    var PaseIngreso = new VISITANTE_INGRESO_PASE();
                    var PasesIngresos = new List<VISITANTE_INGRESO_PASE>();
                    if (!BanderaEditar)
                    {
                        #region [Insercion]
                        persona = new SSP.Servidor.PERSONA
                        {
                            MATERNO = TextMaterno,
                            NOMBRE = TextNombre,
                            PATERNO = TextPaterno,
                            SEXO = SelectSexo,
                            DOMICILIO_CALLE = TextCalle,
                            DOMICILIO_CODIGO_POSTAL = TextCodigoPostal,
                            DOMICILIO_NUM_EXT = TextNumExt,
                            DOMICILIO_NUM_INT = TextNumInt,
                            ID_COLONIA = SelectColonia,
                            ID_ENTIDAD = SelectEntidad,
                            ID_MUNICIPIO = SelectMunicipio,
                            ID_PAIS = SelectPais,
                            CURP = TextCurp,
                            RFC = TextRfc,
                            ID_TIPO_DISCAPACIDAD = SelectDiscapacidad == -1 ? new Nullable<short>() : SelectDiscapacidad,
                            TELEFONO = TextTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""),
                            FEC_NACIMIENTO = FechaNacimiento,
                            CORREO_ELECTRONICO = TextCorreo,
                            ID_TIPO_PERSONA = 3,

                            ID_PERSONA = BanderaEditar ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.ID_PERSONA : 0 : int.Parse(Fechas.GetFechaDateServer.Year + "" + new cPersona().GetSequence<int>("ID_PERSONA_SEQ")),
                            CORIGINAL = BanderaEditar ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.CORIGINAL : null : null,
                            ID_ETNIA = BanderaEditar ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.ID_ETNIA : new Nullable<short>() : new Nullable<short>(),
                            IFE = BanderaEditar ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.IFE : null : null,
                            LUGAR_NACIMIENTO = BanderaEditar ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.LUGAR_NACIMIENTO : null : null,
                            NACIONALIDAD = BanderaEditar ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.NACIONALIDAD : new Nullable<short>() : new Nullable<short>(),
                            NORIGINAL = BanderaEditar ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.NORIGINAL : new Nullable<int>() : new Nullable<int>(),
                            SMATERNO = BanderaEditar ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.SMATERNO : null : null,
                            SPATERNO = BanderaEditar ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.SMATERNO : null : null,
                            SNOMBRE = BanderaEditar ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.SNOMBRE : null : null,
                            TELEFONO_MOVIL = BanderaEditar ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.TELEFONO_MOVIL : null : null,
                            NOTA_TECNICA = NotaTecnica,
                        };

                        #region Persona NIP
                        //if (BanderaEditar ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.PERSONA_NIP.Count > 0 : true : true)
                        //    personaNIP = new PERSONA_NIP()
                        //    {
                        //        NIP = new cPersona().GetSequence<int>("NIP_SEQ"),
                        //        ID_TIPO_VISITA = TipoVisitaFamiliar,
                        //        ID_PERSONA = persona.ID_PERSONA,
                        //        ID_CENTRO = GlobalVar.gCentro
                        //    };
                        #endregion

                        #region VISITANTE
                        visitante = new VISITANTE
                        {
                            ID_ESTATUS_VISITA = BanderaEditar ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.VISITANTE.ID_ESTATUS_VISITA : EstatusVisitaRegistro : EstatusVisitaRegistro,
                            FEC_ALTA = BanderaEditar ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.VISITANTE.FEC_ALTA : hoy : hoy,
                            ID_PERSONA = persona.ID_PERSONA,
                            //FEC_PLAZO = new Fechas().AgregarDiasHabiles(Fechas.GetFechaDateServer, 10),
                            ULTIMA_MODIFICACION = Fechas.GetFechaDateServer
                        };
                        #endregion

                        #region VISITANTE_INGRESO
                        foreach (var item in ListadoInternos)
                        {
                            VisitantesIngresos.Add(new VISITANTE_INGRESO
                            {
                                EMISION_GAFETE = "N",
                                ID_TIPO_VISITANTE = ListadoInternos.Count > 1
                                ?
                                    SelectVisitanteIngreso != null ?
                                        (item.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && item.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && item.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                        item.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO) ?
                                            SelectTipoVisitante
                                        : item.ID_TIPO_VISITANTE
                                    : item.ID_TIPO_VISITANTE
                                : SelectTipoVisitante,
                                ACCESO_UNICO = ListadoInternos.Count > 1 ?
                                    SelectVisitanteIngreso != null ?
                                        (item.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && item.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && item.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                        item.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO) ?
                                            (SelectAccesoUnico ? "S" : "N")
                                        : item.ACCESO_UNICO
                                    : item.ACCESO_UNICO
                                : (SelectAccesoUnico ? "S" : "N"),
                                ID_ANIO = item.ID_ANIO,
                                ID_CENTRO = item.ID_CENTRO,
                                ID_IMPUTADO = item.ID_IMPUTADO,
                                ID_INGRESO = item.INGRESO.ID_INGRESO,
                                ID_PERSONA = persona.ID_PERSONA,
                                ID_TIPO_REFERENCIA = ListadoInternos.Count > 1 ?
                                    SelectVisitanteIngreso != null ?
                                        (item.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && item.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && item.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                        item.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO) ?
                                            SelectParentesco
                                        : item.ID_TIPO_REFERENCIA
                                    : item.ID_TIPO_REFERENCIA
                                : SelectParentesco,
                                OBSERVACION = ListadoInternos.Count > 1 ?
                                    SelectVisitanteIngreso != null ?
                                        (item.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && item.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && item.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                        item.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO) ?
                                            TextObservacion
                                        : item.OBSERVACION
                                    : item.OBSERVACION
                                : TextObservacion,
                                ID_ESTATUS_VISITA = item.ID_ESTATUS_VISITA,
                                FEC_ALTA = item.FEC_ALTA.HasValue ? item.FEC_ALTA.Value : hoy,
                                FEC_ULTIMA_MOD = hoy,
                                ESTATUS_MOTIVO = item.ESTATUS_MOTIVO,

                                INICIO_REGISTRO = item.INICIO_REGISTRO == null ? FechaInicioRegistro.Value : item.INICIO_REGISTRO,
                            });
                        }
                        #endregion

                        #region ACOMPANANTES
                        if (ListAcompanantes.Count > 0)
                        {
                            foreach (var item in ListAcompanantes)
                            {
                                acompanantes.Add(new ACOMPANANTE()
                                {
                                    ID_VISITANTE = persona.ID_PERSONA,
                                    VIS_ID_ANIO = SelectVisitante.ID_ANIO,
                                    VIS_ID_CENTRO = SelectVisitante.ID_CENTRO,
                                    VIS_ID_IMPUTADO = SelectVisitante.ID_IMPUTADO,
                                    VIS_ID_INGRESO = SelectVisitante.INGRESO.ID_INGRESO,
                                    ID_ACOMPANANTE = item.ID_ACOMPANANTE,
                                    ID_ACOMPANANTE_RELACION = item.ID_ACOMPANANTE_RELACION,
                                    ACO_ID_ANIO = item.ACO_ID_ANIO,
                                    ACO_ID_CENTRO = item.ACO_ID_CENTRO,
                                    ACO_ID_IMPUTADO = item.ACO_ID_IMPUTADO,
                                    ACO_ID_INGRESO = item.ACO_ID_INGRESO,
                                    FEC_REGISTRO = hoy
                                });

                                var acomp = new cPersona().ObtenerPersonaXID(item.ID_ACOMPANANTE);
                                if (acomp.Any())
                                {
                                    if (acomp.FirstOrDefault().VISITANTE.ID_ESTATUS_VISITA == EstatusVisitaEnRevision ||
                                        acomp.FirstOrDefault().VISITANTE.ID_ESTATUS_VISITA == EstatusVisitaRegistro)
                                    {
                                        //if (acomp.FirstOrDefault().VISITANTE.FEC_PLAZO > Fechas.GetFechaDateServer)
                                        //{
                                        var act = new cVisitante().Actualizar(new VISITANTE
                                        {
                                            FEC_ALTA = visitante.FEC_ALTA,
                                            ID_ESTATUS_VISITA = visitante.ID_ESTATUS_VISITA,
                                            ID_PERSONA = visitante.ID_PERSONA,
                                            ULTIMA_MODIFICACION = hoy,
                                            //FEC_PLAZO = new Fechas().AgregarDiasHabiles(Fechas.GetFechaDateServer, 10),
                                            ESTATUS_MOTIVO = visitante.ESTATUS_MOTIVO
                                        });
                                        //}
                                    }
                                }
                            }
                        }

                        #region PASES
                        var VisitanteIngreso = new VISITANTE_INGRESO();
                        var VisitanteIngresoPase = new VISITANTE_INGRESO_PASE();
                        foreach (var item in ListAcompanantes)
                        {
                            var personaIngreso = new cPasesVisitanteIngreso().ObtenerXPersonaEIngreso(SelectVisitante.ID_CENTRO.Value, SelectVisitante.ID_ANIO.Value, SelectVisitante.ID_IMPUTADO.Value, SelectVisitante.INGRESO.ID_INGRESO,
                                item.ID_ACOMPANANTE);
                            var visIng = new cVisitanteIngreso().ObtenerXImputadoYPersona(SelectVisitante.ID_CENTRO.Value, SelectVisitante.ID_ANIO.Value, SelectVisitante.ID_IMPUTADO.Value, SelectVisitante.INGRESO.ID_INGRESO, item.ID_ACOMPANANTE);
                            if (visIng.Any())
                            {
                                VisitantesIngresos.Add(new VISITANTE_INGRESO
                                {
                                    EMISION_GAFETE = "N",
                                    ESTATUS_MOTIVO = visIng.FirstOrDefault().ESTATUS_MOTIVO,
                                    FEC_ALTA = visIng.FirstOrDefault().FEC_ALTA,
                                    FEC_ULTIMA_MOD = Fechas.GetFechaDateServer,
                                    ID_ANIO = visIng.FirstOrDefault().ID_ANIO,
                                    ID_CENTRO = visIng.FirstOrDefault().ID_CENTRO,
                                    ID_ESTATUS_VISITA = Parametro.ID_ESTATUS_VISITA_EN_REVISION,
                                    ID_IMPUTADO = visIng.FirstOrDefault().ID_IMPUTADO,
                                    ID_INGRESO = visIng.FirstOrDefault().ID_INGRESO,
                                    ID_PERSONA = visIng.FirstOrDefault().ID_PERSONA,
                                    ID_TIPO_REFERENCIA = visIng.FirstOrDefault().ID_TIPO_REFERENCIA,
                                    OBSERVACION = visIng.FirstOrDefault().OBSERVACION,
                                    ID_TIPO_VISITANTE = visIng.FirstOrDefault().ID_TIPO_VISITANTE,
                                    ACCESO_UNICO = visIng.FirstOrDefault().ACCESO_UNICO
                                });

                            }
                        }
                        #endregion

                        #endregion

                        #region PASES
                        /*if (ListAcompanantes != null ? ListAcompanantes.Count <= 0 : true)
                            PaseIngreso = new VISITANTE_INGRESO_PASE
                            {
                                ID_ANIO = SelectVisitanteIngreso.ID_ANIO,
                                ID_CENTRO = SelectVisitanteIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectVisitanteIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectVisitanteIngreso.ID_INGRESO,
                                ID_PERSONA = persona.ID_PERSONA,
                                ID_PASE = 1,
                                FECHA_ALTA = hoy,
                                ID_CONSEC = 1
                            };*/
                        #endregion

                        #region VISITA_AUTORIZADA
                        if (SelectVisitante.OBJETO_VISITA_AUTORIZADA != null)
                        {
                            VisitaAutorizada = new VISITA_AUTORIZADA
                            {
                                DOMICILIO_CALLE = SelectVisitante.OBJETO_VISITA_AUTORIZADA.DOMICILIO_CALLE,
                                DOMICILIO_CODIGO_POSTAL = SelectVisitante.OBJETO_VISITA_AUTORIZADA.DOMICILIO_CODIGO_POSTAL,
                                DOMICILIO_NUM_EXT = SelectVisitante.OBJETO_VISITA_AUTORIZADA.DOMICILIO_NUM_EXT,
                                DOMICILIO_NUM_INT = SelectVisitante.OBJETO_VISITA_AUTORIZADA.DOMICILIO_NUM_INT,
                                ID_ANIO = SelectVisitante.ID_ANIO.Value,
                                ID_CENTRO = SelectVisitante.ID_CENTRO.Value,
                                ID_IMPUTADO = SelectVisitante.ID_IMPUTADO.Value,
                                ID_INGRESO = SelectVisitante.INGRESO.ID_INGRESO,
                                ID_PARENTESCO = SelectVisitante.ID_PARENTESCO,
                                ID_TIPO_VISITA = SelectVisitante.OBJETO_VISITA_AUTORIZADA.ID_TIPO_VISITA,
                                EDAD = SelectVisitante.OBJETO_VISITA_AUTORIZADA.EDAD,
                                MATERNO = SelectVisitante.OBJETO_VISITA_AUTORIZADA.MATERNO,
                                NOMBRE = SelectVisitante.OBJETO_VISITA_AUTORIZADA.NOMBRE,
                                PATERNO = SelectVisitante.OBJETO_VISITA_AUTORIZADA.PATERNO,
                                SEXO = SelectVisitante.OBJETO_VISITA_AUTORIZADA.SEXO,
                                ID_COLONIA = SelectVisitante.OBJETO_VISITA_AUTORIZADA.ID_COLONIA,
                                ID_ENTIDAD = SelectVisitante.OBJETO_VISITA_AUTORIZADA.ID_ENTIDAD,
                                ID_MUNICIPIO = SelectVisitante.OBJETO_VISITA_AUTORIZADA.ID_MUNICIPIO,
                                ID_PAIS = SelectVisitante.OBJETO_VISITA_AUTORIZADA.ID_PAIS,
                                TELEFONO = SelectVisitante.OBJETO_VISITA_AUTORIZADA.TELEFONO,
                                ID_VISITA = SelectVisitante.OBJETO_VISITA_AUTORIZADA.ID_VISITA,
                                ESTATUS = 1,
                                ID_PERSONA = persona.ID_PERSONA
                            };
                        }
                        #endregion

                        #region FOTOS
                        if (ImagesToSave == null ? false : ImagesToSave.Count != 1)
                            ImagesToSave = null;
                        if (ImagesToSave != null)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                foreach (var item in ImagesToSave)
                                {
                                    var encoder = new JpegBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                                    encoder.QualityLevel = 100;
                                    var bit = new byte[0];
                                    using (MemoryStream stream = new MemoryStream())
                                    {
                                        encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                                        encoder.Save(stream);
                                        bit = stream.ToArray();
                                        stream.Close();
                                    }
                                    personaFotos.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                                    {
                                        BIOMETRICO = bit,
                                        ID_TIPO_BIOMETRICO = (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO,
                                        ID_FORMATO = (short)enumTipoFormato.FMTO_JPG,
                                        ID_PERSONA = persona.ID_PERSONA
                                    });
                                }
                            }));
                        }
                        #endregion

                        #region HUELLAS
                        if (HuellasCapturadas != null)
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                foreach (var item in HuellasCapturadas)
                                {
                                    personaHuellas.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                                    {
                                        BIOMETRICO = item.BIOMETRICO,
                                        ID_TIPO_BIOMETRICO = (short)item.ID_TIPO_BIOMETRICO,
                                        ID_FORMATO = (short)item.ID_TIPO_FORMATO,
                                        ID_PERSONA = persona.ID_PERSONA
                                    });
                                }
                            }));
                        if (HuellasCapturadas == null)
                            HuellasCapturadas = new List<PlantillaBiometrico>();
                        #endregion

                        #endregion
                    }
                    else
                    {
                        if (SelectVisitante == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validacion!", "Favor de seleccionar un visitante");
                            return false;
                        }
                        #region [Actualizacion]
                        persona = new SSP.Servidor.PERSONA
                        {
                            MATERNO = TextMaterno,
                            NOMBRE = TextNombre,
                            PATERNO = TextPaterno,
                            SEXO = SelectSexo,
                            DOMICILIO_CALLE = TextCalle,
                            DOMICILIO_CODIGO_POSTAL = TextCodigoPostal,
                            DOMICILIO_NUM_EXT = TextNumExt,
                            DOMICILIO_NUM_INT = TextNumInt,
                            ID_COLONIA = SelectColonia,
                            ID_ENTIDAD = SelectEntidad,
                            ID_MUNICIPIO = SelectMunicipio,
                            ID_PAIS = SelectPais,
                            CURP = TextCurp,
                            RFC = TextRfc,
                            ID_TIPO_DISCAPACIDAD = SelectDiscapacidad == -1 ? new Nullable<short>() : SelectDiscapacidad,
                            TELEFONO = TextTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""),
                            FEC_NACIMIENTO = FechaNacimiento,
                            CORREO_ELECTRONICO = TextCorreo,

                            CORIGINAL = SelectVisitante.OBJETO_PERSONA.CORIGINAL,
                            ID_ETNIA = SelectVisitante.OBJETO_PERSONA.ID_ETNIA,
                            ID_PERSONA = SelectVisitante.OBJETO_PERSONA.ID_PERSONA,
                            ID_TIPO_PERSONA = SelectVisitante.OBJETO_PERSONA.ID_TIPO_PERSONA,
                            ESTADO_CIVIL = SelectVisitante.OBJETO_PERSONA.ESTADO_CIVIL,
                            IFE = SelectVisitante.OBJETO_PERSONA.IFE,
                            LUGAR_NACIMIENTO = SelectVisitante.OBJETO_PERSONA.LUGAR_NACIMIENTO,
                            NACIONALIDAD = SelectVisitante.OBJETO_PERSONA.NACIONALIDAD,
                            NORIGINAL = SelectVisitante.OBJETO_PERSONA.NORIGINAL,
                            SMATERNO = SelectVisitante.OBJETO_PERSONA.SMATERNO,
                            SPATERNO = SelectVisitante.OBJETO_PERSONA.SPATERNO,
                            SNOMBRE = SelectVisitante.OBJETO_PERSONA.SNOMBRE,
                            TELEFONO_MOVIL = SelectVisitante.OBJETO_PERSONA.TELEFONO_MOVIL,
                            NOTA_TECNICA = NotaTecnica,
                        };

                        if (SelectVisitante != null)
                        {
                            #region UPDATEs
                            var exist = new cVisitante().Obtener(persona.ID_PERSONA).Any();
                            if (exist)
                            {
                                visitante = new VISITANTE
                                {
                                    ID_PERSONA = persona.ID_PERSONA,
                                    ID_ESTATUS_VISITA = SelectSituacion,
                                    FEC_ALTA = SelectVisitante.OBJETO_PERSONA.VISITANTE.FEC_ALTA,
                                    ULTIMA_MODIFICACION = hoy,
                                    ESTATUS_MOTIVO = SelectVisitante.OBJETO_PERSONA.VISITANTE.ESTATUS_MOTIVO
                                };
                            }
                            else
                            {
                                visitante = new VISITANTE
                                {
                                    ID_PERSONA = persona.ID_PERSONA,
                                    ID_ESTATUS_VISITA = EstatusVisitaRegistro,
                                    FEC_ALTA = hoy,
                                    ULTIMA_MODIFICACION = hoy
                                };
                            }

                            #region visING
                            foreach (var item in ListadoInternos)
                            {
                                var obj = new VISITANTE_INGRESO();
                                obj.EMISION_GAFETE = item.EMISION_GAFETE;
                                obj.ID_ANIO = item.ID_ANIO;
                                obj.ID_CENTRO = item.ID_CENTRO;
                                obj.ID_IMPUTADO = item.ID_IMPUTADO;
                                obj.ID_INGRESO = item.ID_INGRESO;
                                obj.ID_PERSONA = persona.ID_PERSONA;
                                obj.ID_TIPO_REFERENCIA = ListadoInternos.Count > 1 ?
                                        SelectVisitanteIngreso != null ?
                                            (item.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && item.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && item.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                            item.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO) ?
                                                SelectParentesco
                                            : item.ID_TIPO_REFERENCIA
                                        : item.ID_TIPO_REFERENCIA
                                    : SelectParentesco;
                                 obj.OBSERVACION = ListadoInternos.Count > 1 ?
                                        SelectVisitanteIngreso != null ?
                                            (item.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && item.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && item.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                            item.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO) ?
                                                TextObservacion
                                            : item.OBSERVACION
                                        : item.OBSERVACION
                                    : TextObservacion;
                                    obj.ID_ESTATUS_VISITA = item.ID_ESTATUS_VISITA;
                                    obj.FEC_ALTA = item.FEC_ALTA;
                                    obj.FEC_ULTIMA_MOD = hoy;
                                    obj.ESTATUS_MOTIVO = item.ESTATUS_MOTIVO;
                                    obj.ID_TIPO_VISITANTE = ListadoInternos.Count > 1 ?
                                        SelectVisitanteIngreso != null ?
                                            (item.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && item.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && item.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                            item.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO) ?
                                                SelectTipoVisitante
                                            : item.ID_TIPO_VISITANTE
                                        : item.ID_TIPO_VISITANTE
                                    : SelectTipoVisitante;
                                    obj.ACCESO_UNICO = ListadoInternos.Count > 1 ?
                                        SelectVisitanteIngreso != null ?
                                            (item.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && item.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && item.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                            item.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO) ?
                                                (SelectAccesoUnico ? "S" : "N")
                                            : item.ACCESO_UNICO
                                        : item.ACCESO_UNICO
                                    : (SelectAccesoUnico ? "S" : "N");
                                //obj.INICIO_REGISTRO = item.INICIO_REGISTRO.HasValue ? FechaInicioRegistro.Value : new Nullable<DateTime>();
                                    obj.INICIO_REGISTRO = item.INICIO_REGISTRO.HasValue ? item.INICIO_REGISTRO.Value : new Nullable<DateTime>();

                                VisitantesIngresos.Add(obj);
                                #region comentado
                                //VisitantesIngresos.Add(new VISITANTE_INGRESO
                                //{
                                //    EMISION_GAFETE = item.EMISION_GAFETE,
                                //    ID_ANIO = item.ID_ANIO,
                                //    ID_CENTRO = item.ID_CENTRO,
                                //    ID_IMPUTADO = item.ID_IMPUTADO,
                                //    ID_INGRESO = item.ID_INGRESO,
                                //    ID_PERSONA = persona.ID_PERSONA,
                                //    ID_TIPO_REFERENCIA = ListadoInternos.Count > 1 ?
                                //        SelectVisitanteIngreso != null ?
                                //            (item.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && item.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && item.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                //            item.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO) ?
                                //                SelectParentesco
                                //            : item.ID_TIPO_REFERENCIA
                                //        : item.ID_TIPO_REFERENCIA
                                //    : SelectParentesco,
                                //    OBSERVACION = ListadoInternos.Count > 1 ?
                                //        SelectVisitanteIngreso != null ?
                                //            (item.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && item.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && item.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                //            item.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO) ?
                                //                TextObservacion
                                //            : item.OBSERVACION
                                //        : item.OBSERVACION
                                //    : TextObservacion,
                                //    ID_ESTATUS_VISITA = item.ID_ESTATUS_VISITA,
                                //    FEC_ALTA = item.FEC_ALTA,
                                //    FEC_ULTIMA_MOD = hoy,
                                //    ESTATUS_MOTIVO = item.ESTATUS_MOTIVO,
                                //    ID_TIPO_VISITANTE = ListadoInternos.Count > 1 ?
                                //        SelectVisitanteIngreso != null ?
                                //            (item.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && item.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && item.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                //            item.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO) ?
                                //                SelectTipoVisitante
                                //            : item.ID_TIPO_VISITANTE
                                //        : item.ID_TIPO_VISITANTE
                                //    : SelectTipoVisitante,
                                //    ACCESO_UNICO = ListadoInternos.Count > 1 ?
                                //        SelectVisitanteIngreso != null ?
                                //            (item.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && item.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && item.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                //            item.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO) ?
                                //                (SelectAccesoUnico ? "S" : "N")
                                //            : item.ACCESO_UNICO
                                //        : item.ACCESO_UNICO
                                //    : (SelectAccesoUnico ? "S" : "N"),

                                //    INICIO_REGISTRO = item.INICIO_REGISTRO.HasValue ? FechaInicioRegistro.Value : new Nullable<DateTime>(),
                                //});
                                #endregion
                                var visitanteExistente = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<VISITANTE_INGRESO>>(() =>
                                    new cVisitanteIngreso().ObtenerXImputadoYPersona(item.ID_CENTRO, item.ID_ANIO, item.ID_IMPUTADO, item.ID_INGRESO, persona.ID_PERSONA));
                                if (visitanteExistente.Count == 0 ? SelectVisitanteAutorizado != null : false)
                                    VisitaAutorizada = new VISITA_AUTORIZADA
                                    {
                                        ID_VISITA = SelectVisitanteAutorizado.ID_VISITA,
                                        DOMICILIO_CALLE = persona.DOMICILIO_CALLE,
                                        DOMICILIO_CODIGO_POSTAL = persona.DOMICILIO_CODIGO_POSTAL,
                                        DOMICILIO_NUM_EXT = persona.DOMICILIO_NUM_EXT,
                                        DOMICILIO_NUM_INT = persona.DOMICILIO_NUM_INT,
                                        ESTATUS = 1,
                                        ID_ANIO = item.ID_ANIO,
                                        ID_CENTRO = item.ID_CENTRO,
                                        ID_COLONIA = persona.ID_COLONIA,
                                        ID_ENTIDAD = persona.ID_ENTIDAD,
                                        ID_MUNICIPIO = persona.ID_MUNICIPIO,
                                        ID_PAIS = persona.ID_PAIS,
                                        ID_PARENTESCO = SelectParentesco,
                                        ID_TIPO_VISITA = SelectVisitanteAutorizado.ID_TIPO_VISITA,
                                        ID_PERSONA = persona.ID_PERSONA,
                                        ID_IMPUTADO = item.ID_IMPUTADO,
                                        ID_INGRESO = item.ID_INGRESO,
                                        MATERNO = persona.MATERNO,
                                        NOMBRE = persona.NOMBRE,
                                        PATERNO = persona.PATERNO,
                                        SEXO = persona.SEXO,
                                        TELEFONO = persona.TELEFONO,
                                        EDAD = SelectVisitanteAutorizado.EDAD,
                                    };
                            }
                            #endregion

                            #region VISITANTE
                            foreach (var item in VisitantesIngresos)
                            {
                                if (!new cVisitanteIngreso().ObtenerXImputadoYPersona(item.ID_CENTRO, item.ID_ANIO, item.ID_IMPUTADO, item.ID_INGRESO, item.ID_PERSONA).Any())
                                {
                                    visitante = new VISITANTE
                                    {
                                        ID_PERSONA = visitante.ID_PERSONA,
                                        ID_ESTATUS_VISITA = EstatusVisitaRegistro,
                                        FEC_ALTA = visitante.FEC_ALTA,
                                        ESTATUS_MOTIVO = visitante.ESTATUS_MOTIVO,
                                        ULTIMA_MODIFICACION = hoy
                                    };
                                }
                            }
                            if (visitante.ID_ESTATUS_VISITA != EstatusVisitaCancelado && visitante.ID_ESTATUS_VISITA != EstatusVisitaSuspendido)
                            {
                                if (VisitantesIngresos.Where(w => w.ID_ESTATUS_VISITA != EstatusVisitaCancelado && w.ID_ESTATUS_VISITA != EstatusVisitaSuspendido)
                                    .Where(w => w.ID_ESTATUS_VISITA == EstatusVisitaAutorizado).Count() == VisitantesIngresos.Count && visitante.ID_ESTATUS_VISITA != EstatusVisitaAutorizado)
                                {
                                    visitante = new VISITANTE
                                    {
                                        ID_PERSONA = visitante.ID_PERSONA,
                                        ID_ESTATUS_VISITA = EstatusVisitaAutorizado,
                                        FEC_ALTA = visitante.FEC_ALTA,
                                        ESTATUS_MOTIVO = visitante.ESTATUS_MOTIVO,
                                        ULTIMA_MODIFICACION = hoy
                                    };
                                }
                            }
                            #endregion

                            #region ACOMPANANTES
                            if (ListAcompanantes != null ? ListAcompanantes.Count > 0 : false)
                            {
                                acompanantes = new List<ACOMPANANTE>();
                                foreach (var item in ListAcompanantes)
                                {
                                    var newAco = new ACOMPANANTE()
                                    {
                                        ID_VISITANTE = persona.ID_PERSONA,
                                        VIS_ID_ANIO = item.VIS_ID_ANIO,
                                        VIS_ID_CENTRO = item.VIS_ID_CENTRO,
                                        VIS_ID_IMPUTADO = item.VIS_ID_IMPUTADO,
                                        VIS_ID_INGRESO = item.VIS_ID_INGRESO,
                                        ID_ACOMPANANTE = item.ID_ACOMPANANTE,
                                        ID_ACOMPANANTE_RELACION = item.ID_ACOMPANANTE_RELACION,
                                        ACO_ID_ANIO = item.ACO_ID_ANIO,
                                        ACO_ID_CENTRO = item.ACO_ID_CENTRO,
                                        ACO_ID_IMPUTADO = item.ACO_ID_IMPUTADO,
                                        ACO_ID_INGRESO = item.ACO_ID_INGRESO,
                                        FEC_REGISTRO = hoy
                                    };
                                    acompanantes.Add(newAco);

                                    var acomp = new cPersona().ObtenerPersonaXID(item.ID_ACOMPANANTE);
                                    if (acomp.Any() ? (acomp.FirstOrDefault().VISITANTE.ID_ESTATUS_VISITA == EstatusVisitaEnRevision || acomp.FirstOrDefault().VISITANTE.ID_ESTATUS_VISITA == EstatusVisitaRegistro) : false)
                                        await StaticSourcesViewModel.OperacionesAsync<bool>("Cargando...", () =>
                                        {
                                            visitante = new VISITANTE
                                            {
                                                FEC_ALTA = visitante.FEC_ALTA,
                                                ID_ESTATUS_VISITA = visitante.ID_ESTATUS_VISITA,
                                                ID_PERSONA = visitante.ID_PERSONA,
                                                ULTIMA_MODIFICACION = hoy,
                                                ESTATUS_MOTIVO = visitante.ESTATUS_MOTIVO
                                            };
                                            return true;
                                        });



                                    #region PASES
                                    var acomps = new cAcompanante().ObtenerXAcompanantesYVisitante(newAco.VIS_ID_CENTRO.Value, newAco.VIS_ID_ANIO.Value, newAco.VIS_ID_IMPUTADO.Value, newAco.VIS_ID_INGRESO.Value, newAco.ID_VISITANTE,
                                        newAco.ACO_ID_CENTRO.Value, newAco.ACO_ID_ANIO.Value, newAco.ACO_ID_IMPUTADO.Value, newAco.ACO_ID_INGRESO.Value, newAco.ID_ACOMPANANTE).ToList();
                                    if (!acomps.Any())
                                    {
                                        var personaIngreso = new cPasesVisitanteIngreso().ObtenerSiguienteConsecutivo(SelectImputadoIngreso.ID_CENTRO, SelectImputadoIngreso.ID_ANIO,
                                            SelectImputadoIngreso.ID_IMPUTADO, SelectImputadoIngreso.ID_INGRESO, SelectVisitante.OBJETO_PERSONA.ID_PERSONA);
                                        PaseIngreso = new VISITANTE_INGRESO_PASE
                                        {
                                            ID_ANIO = SelectImputadoIngreso.ID_ANIO,
                                            ID_CENTRO = SelectImputadoIngreso.ID_CENTRO,
                                            ID_IMPUTADO = SelectImputadoIngreso.ID_IMPUTADO,
                                            ID_INGRESO = SelectImputadoIngreso.ID_INGRESO,
                                            ID_PERSONA = SelectVisitante.OBJETO_PERSONA.ID_PERSONA,
                                            ID_PASE = 2,
                                            FECHA_ALTA = hoy,
                                            ID_CONSEC = personaIngreso > 0 ? (short)(personaIngreso + 1) : (short)1
                                        };
                                    }
                                    #endregion
                                }
                            }
                            #endregion

                            #endregion
                        }

                        #region Persona NIP
                        //if (BanderaEditar ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.PERSONA_NIP.Where(w => w.ID_TIPO_VISITA == TipoVisitaFamiliar && w.ID_CENTRO == GlobalVar.gCentro).Count() <= 0 : true : true)
                        //    personaNIP = new PERSONA_NIP()
                        //    {
                        //        NIP = new cPersona().GetSequence<int>("NIP_SEQ"),
                        //        ID_TIPO_VISITA = TipoVisitaFamiliar,
                        //        ID_PERSONA = persona.ID_PERSONA,
                        //        ID_CENTRO = GlobalVar.gCentro
                        //    };
                        #endregion

                        #region FOTOS
                        if (ImagesToSave == null ? false : ImagesToSave.Count != 1)
                            ImagesToSave = null;
                        if (ImagesToSave != null)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                foreach (var item in ImagesToSave)
                                {
                                    var encoder = new JpegBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                                    encoder.QualityLevel = 100;
                                    var bit = new byte[0];
                                    using (MemoryStream stream = new MemoryStream())
                                    {
                                        encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                                        encoder.Save(stream);
                                        bit = stream.ToArray();
                                        stream.Close();
                                    }
                                    personaFotos.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                                    {
                                        BIOMETRICO = bit,
                                        ID_TIPO_BIOMETRICO = (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO,
                                        ID_FORMATO = (short)enumTipoFormato.FMTO_JPG,
                                        ID_PERSONA = persona.ID_PERSONA
                                    });
                                }
                            }));
                        }
                        #endregion

                        #region HUELLAS
                        if (HuellasCapturadas != null)
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                foreach (var item in HuellasCapturadas)
                                {
                                    personaHuellas.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                                    {
                                        BIOMETRICO = item.BIOMETRICO,
                                        ID_TIPO_BIOMETRICO = (short)item.ID_TIPO_BIOMETRICO,
                                        ID_FORMATO = (short)item.ID_TIPO_FORMATO,
                                        ID_PERSONA = persona.ID_PERSONA
                                    });
                                }
                            }));
                        if (HuellasCapturadas == null)
                            HuellasCapturadas = new List<PlantillaBiometrico>();
                        #endregion

                        #endregion
                    }
                    if (NuevoListaTonta != null)
                    {
                        var vising = ListadoInternos.First(f => f.ID_CENTRO == NuevoListaTonta.ID_CENTRO && f.ID_ANIO == NuevoListaTonta.ID_ANIO && f.ID_IMPUTADO == NuevoListaTonta.ID_IMPUTADO && f.ID_INGRESO == NuevoListaTonta.ID_INGRESO && f.ID_PERSONA == persona.ID_PERSONA);
                        VisitaAutorizada = new VISITA_AUTORIZADA
                        {
                            ID_PERSONA = vising.ID_PERSONA,
                            ID_CENTRO = vising.ID_CENTRO,
                            ID_ANIO = vising.ID_ANIO,
                            ID_IMPUTADO = vising.ID_IMPUTADO,
                            ID_INGRESO = vising.ID_INGRESO,
                            ID_PARENTESCO = SelectParentesco,
                            ID_TIPO_VISITA = Parametro.ID_TIPO_VISITA_FAMILIAR,
                            DOMICILIO_CALLE = persona.DOMICILIO_CALLE,
                            DOMICILIO_CODIGO_POSTAL = persona.DOMICILIO_CODIGO_POSTAL,
                            DOMICILIO_NUM_EXT = persona.DOMICILIO_NUM_EXT,
                            DOMICILIO_NUM_INT = persona.DOMICILIO_NUM_INT,
                            EDAD = new Fechas().CalculaEdad(persona.FEC_NACIMIENTO),
                            ESTATUS = 1,
                            ID_COLONIA = persona.ID_COLONIA,
                            ID_ENTIDAD = persona.ID_ENTIDAD,
                            ID_MUNICIPIO = persona.ID_MUNICIPIO,
                            ID_PAIS = persona.ID_PAIS,
                            MATERNO = persona.MATERNO,
                            NOMBRE = persona.NOMBRE,
                            PATERNO = persona.PATERNO,
                            SEXO = persona.SEXO,
                            TELEFONO = persona.TELEFONO,
                        };
                    }
                    if (!new cPersona().InsertarVisitaTransaccion(persona, visitante, VisitantesIngresos, acompanantes, VisitaAutorizada, personaFotos, personaHuellas/*personaNIP*/, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO, PaseIngreso, PasesIngresos))
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No se pudo guardar la información.");
                        return false;
                    }
                    LimpiarDatosImputado();
                    CapturarVisitanteVisible = false;
                    CapturarVisitanteEnabled = false;
                    var idPersona = ListadoInternos.Any() ? ListadoInternos.FirstOrDefault().ID_PERSONA : 0;
                    var MayoriaEdad = Parametro.MAYORIA_EDAD;
                    if (ListadoInternos.Count == 1)
                    {
                        MenuGuardarEnabled = !PInsertar ? PEditar : true;
                        SelectVisitanteIngreso = new cVisitanteIngreso().ObtenerXImputadoYPersona(SelectVisitanteIngreso.ID_CENTRO, SelectVisitanteIngreso.ID_ANIO, SelectVisitanteIngreso.ID_IMPUTADO,
                                SelectVisitanteIngreso.ID_INGRESO, persona.ID_PERSONA).FirstOrDefault();
                        SelectImputadoIngreso = SelectVisitanteIngreso.INGRESO;
                        GetDatosIngresoImputadoSeleccionado();
                        ListAcompanantes = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ACOMPANANTE>>(() =>
                            new ObservableCollection<ACOMPANANTE>(new cAcompanante().ObtenerAcompanantesXVisitante(SelectVisitanteIngreso.ID_CENTRO,
                                SelectVisitanteIngreso.ID_ANIO, SelectVisitanteIngreso.ID_IMPUTADO, SelectVisitanteIngreso.ID_INGRESO, SelectVisitanteIngreso.ID_PERSONA)));
                        //* * * * * * * * * * * *//
                        ListBuscarAcompanantes = new ObservableCollection<VISITANTE_INGRESO>(SelectImputadoIngreso.VISITANTE_INGRESO.Where(w =>
                            w.ID_PERSONA != SelectVisitanteIngreso.ID_PERSONA &&
                            !ListAcompanantes.Where(x => x.ID_ACOMPANANTE == w.ID_PERSONA).Any() &&
                            new Fechas().CalculaEdad(w.VISITANTE.PERSONA.FEC_NACIMIENTO.HasValue ? w.VISITANTE.PERSONA.FEC_NACIMIENTO : DateTime.Parse("01/01/1900")) >= MayoriaEdad &&
                            (w.ID_ESTATUS_VISITA == EstatusVisitaEnRevision || w.ID_ESTATUS_VISITA == EstatusVisitaAutorizado)));
                        //* * * * * * * * * * * *//
                        EmptyBuscarAcompanante = ListBuscarAcompanantes.Count == 0;
                    }
                    else
                    {
                        SelectVisitanteIngreso = null;
                        SelectImputadoIngreso = null;
                        ListAcompanantes = new ObservableCollection<ACOMPANANTE>();
                        LimpiarDatosImputado();
                        TextObservacion = string.Empty;
                        AcompananteVisible = Visibility.Collapsed;
                    }
                    SelectVisitante = await StaticSourcesViewModel.CargarDatosAsync<PERSONAVISITAAUXILIAR>(() => ConvertPersonaToAuxiliar(new cPersona().ObtenerPersonaXID(idPersona).FirstOrDefault()));
                    if (TextCodigo == null)
                        TextCodigo = persona.ID_PERSONA;
                    NuevoListaTonta = null;
                    BuscarVisita();
                    SetValidacionesGenerales();
                    #region Comentado
                    //OnPropertyChanged();
                    ///////////////////////////////////////////////////
                    //if (persona != null)
                    //{
                    //    if(TextCodigo == null)
                    //        TextCodigo = persona.ID_PERSONA;
                    //    if(string.IsNullOrEmpty(TextNip))
                    //    {
                    //        var nips = new cPersonaNIP().ObtenerTodos(GlobalVar.gCentro, persona.ID_PERSONA);
                    //        if (nips != null)
                    //        {
                    //            var nip = nips.FirstOrDefault();
                    //            if (nip != null)
                    //            {
                    //                TextNip = nip.NIP.ToString();
                    //            }
                    //        }
                    //    }
                    //}

                    ///////////////////////////////////////////////////
                    #endregion
                    (new Dialogos()).ConfirmacionDialogo("ÉXITO!", "Información grabada exitosamente.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al querer guardar.", ex);
                return false;
            }
        }

        #region [Huellas Digitales]

        private void ShowIdentification(object obj = null)
        {
            ShowPopUp = Visibility.Visible;
            ShowFingerPrint = Visibility.Hidden;
            var Initial442 = new Thread((Init) =>
            {
                try
                {
                    var nRet = 0;

                    CLSFPCaptureDllWrapper.CLS_Initialize();
                    CLSFPCaptureDllWrapper.CLS_SetLanguage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_RESOURCE.ENGLISH);
                    nRet = CLSFPCaptureDllWrapper.CLS_CaptureFP(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_TYPE.IDFLATS);

                    if (nRet == 0)
                    {
                        ScannerMessage = "Procesando...";
                        ShowFingerPrint = Visibility.Visible;
                        ShowLine = Visibility.Visible;
                        ShowOk = Visibility.Hidden;
                        Thread.Sleep(300);
                        HuellasCapturadas = new List<PlantillaBiometrico>();

                        var SaveFingerPrints = new Thread((saver) =>
                        {
                            try
                            {
                                #region [Huellas]
                                for (short i = 1; i <= 10; i++)
                                {
                                    var pBuffer = IntPtr.Zero;
                                    var nBufferLength = 0;
                                    var nNFIQ = 0;

                                    CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.BMP, ref pBuffer, ref nBufferLength);
                                    var bufferBMP = new byte[nBufferLength];
                                    if (pBuffer != IntPtr.Zero)
                                        Marshal.Copy(pBuffer, bufferBMP, 0, nBufferLength);

                                    CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.WSQ, ref pBuffer, ref nBufferLength);
                                    var bufferWSQ = new byte[nBufferLength];
                                    if (pBuffer != IntPtr.Zero)
                                        Marshal.Copy(pBuffer, bufferWSQ, 0, nBufferLength);

                                    CLSFPCaptureDllWrapper.CLS_GetImageNFIQ(((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i), ref nNFIQ, CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN);

                                    Fmd FMD = null;
                                    if (bufferBMP.Length != 0)
                                    {
                                        GuardaHuella = CreateBitmapSourceFromBitmap(new MemoryStream(bufferBMP));
                                        FMD = ExtractFmdfromBmp(new Bitmap(new MemoryStream(bufferBMP)).Clone(new Rectangle(0, 0, 357, 392), System.Drawing.Imaging.PixelFormat.Format8bppIndexed)).Data;
                                    }

                                    Thread.Sleep(200);
                                    switch ((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i)
                                    {
                                        #region [Pulgar Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_THUMB:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Indice Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_INDEX:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Medio Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_MIDDLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Anular Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_RING:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Meñique Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_LITTLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Pulgar Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_THUMB:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Indice Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_INDEX:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Medio Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_MIDDLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Anular Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_RING:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Meñique Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_LITTLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        default:
                                            break;
                                    }
                                }
                                #endregion

                                //if (new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == imputado.ID_ANIO && w.ID_CENTRO == imputado.ID_CENTRO && w.ID_IMPUTADO == imputado.ID_IMPUTADO && w.ID_TIPO_BIOMETRICO >= 11 && w.ID_TIPO_BIOMETRICO <= 20).ToList().Any())
                                //    ScannerMessage = "Huellas Actualizadas Correctamente";
                                //else
                                ScannerMessage = "Huellas Capturadas Correctamente";
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al procesar huellas.", ex);
                            }
                        });

                        SaveFingerPrints.Start();
                        SaveFingerPrints.Join();

                        ShowLine = Visibility.Hidden;
                        Thread.Sleep(1500);
                    }
                    ShowPopUp = Visibility.Hidden;
                    CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                }
                catch
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ShowPopUp = Visibility.Hidden;
                        (new Dialogos()).ConfirmacionDialogo("Error", "Revise que el escanner este bien configurado.");
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    }));
                }
            });

            Initial442.Start();
        }
        private async void OnBuscarPorHuella(string obj = "")
        {
            try
            {
                //kawait Task.Factory.StartNew(() => 
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    //);

                await TaskEx.Delay(400);

                var nRet = -1;
                var bandera = true;
                var requiereGuardarHuellas = Parametro.GuardarHuellaEnBusquedaPadronVisita;
                if (requiereGuardarHuellas)
                    try
                    {
                        nRet = CLSFPCaptureDllWrapper.CLS_Initialize();
                    }
                    catch
                    {
                        bandera = false;
                    }
                else
                    bandera = false;

                var windowBusqueda = new BusquedaHuella();
                windowBusqueda.DataContext = new BusquedaHuellaViewModel(enumTipoPersona.PERSONA_TODOS, nRet == 0, requiereGuardarHuellas);
                windowBusqueda.dgHuella.Columns.Insert(windowBusqueda.dgHuella.Columns.Count, new DataGridTextColumn()
                {
                    Binding = new System.Windows.Data.Binding("Persona")
                    {
                        Converter = new GetTipoPersona()
                    },
                    Header = "TIPO VISITA"
                });
                if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0 : false)
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGURESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                    return;
                }

                windowBusqueda.Owner = PopUpsViewModels.MainWindow;
                windowBusqueda.KeyDown += (s, e) => { if (e.Key == System.Windows.Input.Key.Escape)windowBusqueda.Close(); };
                windowBusqueda.Closed += (s, e) =>
                {
                    HuellasCapturadas = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                    if (bandera == true)
                        CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                    var huella = ((BusquedaHuellaViewModel)windowBusqueda.DataContext);
                    if (!huella.IsSucceed)
                        return;

                    if (huella.ScannerMessage == "HUELLA NO ENCONTRADA")
                    {
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                        return;
                    }

                    if (huella.SelectRegistro != null ? huella.SelectRegistro.Persona == null : null == null)
                        return;

                    SelectVisitante = ConvertPersonaToAuxiliar(huella.SelectRegistro.Persona);
                    FotoHuellaEnabled = BanderaEditar = ContextMenuEnabled = true;
                    GeneralEnabled = ValidarEnabled = true;
                    SetValidacionesGenerales();
                    GetDatosVisitanteSeleccionadoPadron();
                };
                windowBusqueda.ShowDialog();
                AceptarBusquedaHuellaFocus = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }

        #endregion

        #region [Camara]
        private void CapturarFoto(System.Windows.Controls.Image Picture)
        {
            try
            {
                if (Processing)
                    return;
                Processing = true;
                ImagesToSave = ImagesToSave ?? new List<ImageSourceToSave>();
                if (CamaraWeb.ImageControls.Where(w => w.Name == Picture.Name).Any())
                {
                    Picture.Source = CamaraWeb.TomarFoto(Picture);
                    ImagesToSave.Add(new ImageSourceToSave { FrameName = Picture.Name, ImageCaptured = (BitmapSource)Picture.Source });
                    StaticSourcesViewModel.SourceChanged = true;
                    StaticSourcesViewModel.Mensaje("FOTO DE FRENTE", "Foto Capturada", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 1);
                }
                else
                {
                    CamaraWeb.QuitarFoto(Picture);
                    ImagesToSave.Remove(ImagesToSave.Where(wm => wm.FrameName == Picture.Name).SingleOrDefault());
                }
                if (ImagesToSave != null ? ImagesToSave.Count == 1 : false)
                    BotonTomarFotoEnabled = true;
                else
                    BotonTomarFotoEnabled = false;
                Processing = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al tomar una foto.", ex);
            }
        }
        private void AbrirCamara(object obj)
        {
            try
            {
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                TomarFotoLoad(PopUpsViewModels.MainWindow.FotosSenasView);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al estar preparando la camara.", ex);
            }
        }
        private void OpenSetting(string obj)
        {
            CamaraWeb.AdvanceSetting();
        }

        private async void TomarFotoLoad(TomarFotoSenaParticularView Window = null)
        {
            try
            {
                if (!((System.Windows.UIElement)(Window.TomarFotoSenaParticularWindow)).IsVisible) return;
                ImagesToSave = new List<ImageSourceToSave>();
                CamaraWeb = new WebCam(new WindowInteropHelper(Application.Current.Windows[0]).Handle);
                await CamaraWeb.InitializeWebCam(new List<System.Windows.Controls.Image> { Window.ImgSenaParticular });
                if (ImagenPersona.Length != new Imagenes().getImagenPerson().Length)
                    CamaraWeb.AgregarImagenControl(Window.ImgSenaParticular, SelectVisitante.OBJETO_PERSONA == null ? null : new Imagenes().ConvertByteToImageSource(ImagenPersona));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la pantalla para tomar foto.", ex);
            }
        }
        #endregion

        private async void Load_Window(PadronVisitasView Window)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    ListEntidadesAuxiliares = new cEntidad().ObtenerTodos(string.Empty, "S").ToList();
                    ListMunicipiosAuxiliares = new cMunicipio().ObtenerTodos(string.Empty).Where(w => w.ESTATUS == "S").ToList();
                    ListColoniasAuxiliares = new cColonia().ObtenerTodos(string.Empty, new Nullable<short>(), new Nullable<short>()).ToList();
                    GetDatosVisitante();
                    ListLetras = new List<string>(new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" });
                    TextFechaUltimaModificacion = Fechas.GetFechaDateServer.ToString("dd/MM/yyyy");
                    ListTipoVisita = new ObservableCollection<TIPO_VISITA>((new cTipoVisita()).ObtenerTodos().OrderBy(o => o.DESCR));
                    ListAreaVisita = new ObservableCollection<AREA>((new cArea()).ObtenerTodos().OrderBy(o => o.DESCR));
                    Lista_Sources = escaner.Sources();
                    if (Lista_Sources.Count > 0) SelectedSource = Lista_Sources.Where(w => w.Default).SingleOrDefault();
                    HojasMaximo = string.Format("Escaneo permitido: {0} documentos máximo.", escaner.HojasMaximo);
                });

                escaner.EscaneoCompletado += delegate
                {
                    DatePickCapturaDocumento = Fechas.GetFechaDateServer;
                    DocumentoDigitalizado = escaner.ScannedDocument;
                    if (AutoGuardado)
                        if (DocumentoDigitalizado != null)
                            GuardarDocumento();
                    escaner.Dispose();
                };

                ListAreaVisita.Insert(0, new AREA() { ID_AREA = -1, DESCR = "SELECCIONE" });
                SelectAreaVisita = -1;
                ListTipoVisita.Insert(0, new TIPO_VISITA() { ID_TIPO_VISITA = -1, DESCR = "SELECCIONE" });
                SelectTipoVisita = -1;

                #region [Huellas Digitales]
                var myDoubleAnimation = new DoubleAnimation();
                myDoubleAnimation.From = 0;
                myDoubleAnimation.To = 240;
                myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.3));
                myDoubleAnimation.AutoReverse = true;
                myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                Storyboard.SetTargetName(myDoubleAnimation, "Ln");
                Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Canvas.TopProperty));
                var myStoryboard = new Storyboard();
                myStoryboard.Children.Add(myDoubleAnimation);
                if (PopUpsViewModels.MainWindow.HuellasView != null)
                    myStoryboard.Begin(PopUpsViewModels.MainWindow.HuellasView.Ln);
                #endregion

                ConfiguraPermisos();
                LimpiarAsignacion();
                LimpiarPadron();
                LimpiarGeneral();
                SelectHoraEntrada = "0";
                SelectMinutoEntrada = "0";
                SelectHoraSalida = "0";
                SelectMinutoSalida = "0";
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la pantalla.", ex);
            }
        }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                #region LISTA_TONTA
                case "seleccionar_lista_tonta":
                    try
                    {
                        if (!PadronVisitasTab)
                            break;
                        if (SelectParentescoListaTonta <= 0)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar el parentesco.");
                            break;
                        }
                        if (SelectVisitanteAutorizado == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar un visitante.");
                            break;
                        }
                        SelectVisitante.ID_ANIO = SelectVisitanteAutorizado.ID_ANIO;
                        SelectVisitante.ID_CENTRO = SelectVisitanteAutorizado.ID_CENTRO;
                        SelectVisitante.ID_IMPUTADO = SelectVisitanteAutorizado.ID_IMPUTADO;
                        SelectVisitante.INGRESO = new INGRESO();
                        SelectVisitante.INGRESO.ID_INGRESO = SelectVisitanteAutorizado.ID_INGRESO;
                        SelectVisitanteAutorizado.INGRESO.CAMA.CELDA.ID_CELDA = SelectVisitanteAutorizado.INGRESO.CAMA.CELDA.ID_CELDA.ToString().Trim();
                        SelectVisitanteAutorizado.INGRESO.CAMA.CELDA.SECTOR.DESCR = SelectVisitanteAutorizado.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim();
                        SelectVisitanteAutorizado.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR = SelectVisitanteAutorizado.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim();
                        ListadoInternos = ListadoInternos ?? new ObservableCollection<VISITANTE_INGRESO>();
                        var hoy = Fechas.GetFechaDateServer;
                        var EstatusVisitaRegistro = Parametro.ID_ESTATUS_VISITA_REGISTRO;
                        var visIng = new VISITANTE_INGRESO
                        {
                            EMISION_GAFETE = "N",
                            FEC_ALTA = hoy,
                            FEC_ULTIMA_MOD = hoy,
                            ID_ANIO = SelectVisitanteAutorizado.ID_ANIO,
                            ID_CENTRO = SelectVisitanteAutorizado.ID_CENTRO,
                            ID_IMPUTADO = SelectVisitanteAutorizado.ID_IMPUTADO,
                            ID_ESTATUS_VISITA = EstatusVisitaRegistro,
                            ID_INGRESO = SelectVisitanteAutorizado.ID_INGRESO,
                            ID_PERSONA = SelectVisitante.ID_PERSONA.Value,
                            ID_TIPO_REFERENCIA = SelectParentescoListaTonta,
                            ESTATUS_MOTIVO = null,
                            OBSERVACION = null,
                            ESTATUS_VISITA = ListSituacion.Where(w => w.ID_ESTATUS_VISITA == EstatusVisitaRegistro).FirstOrDefault(),
                            TIPO_REFERENCIA = ListTipoRelacion.Where(w => w.ID_TIPO_REFERENCIA == SelectParentescoListaTonta).FirstOrDefault(),
                            INGRESO = SelectVisitanteAutorizado.INGRESO,
                            ACCESO_UNICO = "N",
                            ID_TIPO_VISITANTE = Parametro.ID_TIPO_VISITANTE_ORDINARIO,
                            TIPO_VISITANTE = ListTipoVisitante.Where(w => w.ID_TIPO_VISITANTE == 14).FirstOrDefault()
                        };
                        ListadoInternos.Add(visIng);
                        SelectEstatusRelacion = SelectParentesco = -2;
                        IsDetalleInternosEnable = InsertarNuevoImputadoVisita = SelectParentescoIngresoEnabled = SelectEstatusRelacionEnabled = false;
                        SelectVisitanteIngreso = visIng;
                        SelectImputadoIngreso = visIng.INGRESO;
                        GetDatosIngresoImputadoSeleccionado();
                        IsDetalleInternosEnable = false;
                        TextHeaderDatosInterno = "Datos del Interno Seleccionado";
                        DatosExpedienteVisible = Visibility.Visible;
                        SelectParentescoIngresoEnabled = true;
                        MenuGuardarEnabled = !PInsertar ? PEditar : true;
                        TextObservacion = SelectVisitanteIngreso.OBSERVACION;
                        SelectParentescoAcompanante = -1;
                        ListBuscarAcompanantes = new ObservableCollection<VISITANTE_INGRESO>(SelectImputadoIngreso.VISITANTE_INGRESO.Where(w =>
                            w.ID_PERSONA != SelectVisitanteIngreso.ID_PERSONA &&
                            !ListAcompanantes.Where(x => x.ID_ACOMPANANTE == w.ID_PERSONA).Any() &&
                            new Fechas().CalculaEdad(w.VISITANTE.PERSONA.FEC_NACIMIENTO.HasValue ?
                                w.VISITANTE.PERSONA.FEC_NACIMIENTO : DateTime.Parse("01/01/1900")) >= 18 &&
                            (w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_EN_REVISION || w.ID_ESTATUS_VISITA == EstatusVisitaRegistro)));
                        EmptyBuscarAcompanante = ListBuscarAcompanantes.Count == 0;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.INGRESAR_LISTA_TONTA);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar la visita autorizada.", ex);
                    }
                    break;
                case "cancelar_lista_tonta":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.INGRESAR_LISTA_TONTA);
                    break;
                #endregion

                #region AMPLIAR_DESCRIPCION
                case "cancelar_ampliar_descripcion":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                    break;
                case "guardar_ampliar_descripcion":
                    TextObservacion = TextAmpliarDescripcion;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                    break;
                case "expandir_observacion":
                    TextAmpliarDescripcion = TextObservacion;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                    break;
                #endregion

                #region SELECCIONAR IMPUTADO VISITANTE NUEVO
                case "seleccionar_imputado_visitante_nuevo":
                    try
                    {
                        if (SelectNuevoImputadoVisitante != null ? SelectParentescoNuevoImputado == -1 : false)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar el parentesco.");
                            return;
                        }
                        if (ListadoInternos.Where(w => w.ID_CENTRO == SelectNuevoImputadoVisitante.ID_CENTRO && w.ID_ANIO == SelectNuevoImputadoVisitante.ID_ANIO &&
                                    w.ID_IMPUTADO == SelectNuevoImputadoVisitante.ID_IMPUTADO && w.ID_INGRESO == SelectNuevoImputadoVisitante.INGRESO.ID_INGRESO).Any())
                        {
                            if (await new Dialogos().ConfirmacionDialogoReturn("Validación", "Esta persona ya esta en la lista seleccionada.") == 1)
                                PopUpsViewModels.MainWindow.IngresarImputadoVisitanteView.Focus();
                            return;
                        }
                        SelectNuevoImputadoVisitante.INGRESO.CAMA.CELDA.ID_CELDA = SelectNuevoImputadoVisitante.INGRESO.CAMA.CELDA.ID_CELDA.ToString().Trim();
                        SelectNuevoImputadoVisitante.INGRESO.CAMA.CELDA.SECTOR.DESCR = SelectNuevoImputadoVisitante.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim();
                        SelectNuevoImputadoVisitante.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR = SelectNuevoImputadoVisitante.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim();
                        ListadoInternos.Add(new VISITANTE_INGRESO
                            {
                                EMISION_GAFETE = "N",
                                ID_ANIO = SelectNuevoImputadoVisitante.ID_ANIO.Value,
                                ID_CENTRO = SelectNuevoImputadoVisitante.ID_CENTRO.Value,
                                ID_IMPUTADO = SelectNuevoImputadoVisitante.ID_IMPUTADO.Value,
                                ID_ESTATUS_VISITA = Parametro.ID_ESTATUS_VISITA_REGISTRO,
                                ID_INGRESO = SelectNuevoImputadoVisitante.INGRESO.ID_INGRESO,
                                ID_TIPO_REFERENCIA = SelectParentescoNuevoImputado,
                                ID_PERSONA = SelectVisitante.ID_PERSONA.Value,
                                TIPO_REFERENCIA = ListTipoRelacion.Where(w => w.ID_TIPO_REFERENCIA == SelectParentescoNuevoImputado).FirstOrDefault(),
                                ESTATUS_VISITA = new ESTATUS_VISITA() { ID_ESTATUS_VISITA = Parametro.ID_ESTATUS_VISITA_REGISTRO, DESCR = "REGISTRO" },
                                OBSERVACION = "",
                                INGRESO = SelectNuevoImputadoVisitante.INGRESO,
                                ACCESO_UNICO = "N",
                                ID_TIPO_VISITANTE = Parametro.ID_TIPO_VISITANTE_ORDINARIO,
                                TIPO_VISITANTE = ListTipoVisitante.Where(w => w.ID_TIPO_VISITANTE == 14).FirstOrDefault()
                            });
                        if (SelectNuevoImputadoVisitante.OBJETO_VISITA_AUTORIZADA != null)
                        {
                            ListadoInternosAuxiliar = new ObservableCollection<PERSONAVISITAAUXILIAR>();
                            ListadoInternosAuxiliar.Add(new PERSONAVISITAAUXILIAR
                               {
                                   OBJETO_VISITA_AUTORIZADA = SelectNuevoImputadoVisitante.OBJETO_VISITA_AUTORIZADA
                               });
                        }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.INGRESA_IMPUTADO_VISITANTE);

                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar un nuevo imputado.", ex);
                    }
                    break;
                case "cancelar_imputado_visitante_nuevo":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.INGRESA_IMPUTADO_VISITANTE);
                    await TaskEx.Delay(150);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                #endregion

                #region PADRON VISITA
                case "cargar_padron_visitante":
                    try
                    {
                        if (SelectVisitanteInterno == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Notificacion!", "Debes seleccionar una persona valida.");
                            return;
                        }

                        #region Control de Tiempo
                        FechaInicioRegistro = Fechas.GetFechaDateServer;
                        FechaFinRegistro = null;
                        #endregion

                        var x = SelectVisitante;
                        SelectVisitantePadron = SelectVisitanteInterno;
                        PadronVisitasTab = true;
                        SelectedTab = TabsVisita.PADRON_DE_VISITAS;
                        await TaskEx.Delay(150);
                        AcompananteVisible = Visibility.Collapsed;
                        CrearNuevoVisitanteEnabled = false;
                        FotoVisita = new Imagenes().ConvertByteToImageSource(new Imagenes().getImagenPerson());
                        BanderaEditar = SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.ID_PERSONA != null : false;
                        SetValidacionesGenerales();
                        GetDatosVisitanteSeleccionadoPadron();
                        #region Nota Tecnica
                        if (SelectVisitante != null)
                        {
                            if (SelectVisitante.OBJETO_PERSONA != null)
                            {
                                NotaTecnica = SelectVisitante.OBJETO_PERSONA.NOTA_TECNICA;
                            }
                        }
                        #endregion
                        EntidadEnabled = SelectEntidad > 0;
                        MunicipioEnabled = SelectMunicipio > 0;
                        ColoniaEnabled = SelectColonia > 0;
                        FotoHuellaEnabled = ValidarEnabled = GeneralEnabled = true;
                        MenuGuardarEnabled = !PInsertar ? PEditar : true;
                        DigitalizarDocumentosEnabled = ContextMenuEnabled = SelectVisitante.ID_PERSONA != null;
                        SelectDiscapacitado = (SelectDiscapacidad == null || SelectDiscapacidad == 0) ? "N" : "S";
                        NuevoListaTonta = null;
                        SetValidacionesGenerales();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del visitante.", ex);
                    }
                    break;
                case "insertar_interno_que_visita":
                    InsertarNuevoImputadoVisita = true;
                    IsDetalleInternosEnable = true;
                    TextHeaderDatosInterno = "Seleccion de nuevo interno para el visitante actual";
                    DatosExpedienteVisible = Visibility.Visible;
                    ImagenVisitante = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                    LimpiarDatosImputado();
                    SelectImputadoIngreso = null;
                    ListAcompanantes = new ObservableCollection<ACOMPANANTE>();
                    SetValidacionesGenerales();
                    OnPropertyChanged();
                    break;
                case "borrar_interno_que_visita":

                    break;
                #endregion

                #region MENU
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "ficha_menu":
                    break;
                case "ayuda_menu":
                    ImprimirPasesPorAutorizar();
                    break;
                case "reporte_menu":
                    break;
                case "imprimir_gafete":
                    #region Gafete
                    if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    {
                        if (SelectVisitante == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar a un visitante.");
                            return;
                        }
                        if (SelectVisitante.OBJETO_PERSONA == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes enrolar a la persona para poder imprimir su gafete.");
                            return;
                        }
                        if (SelectImputadoIngreso == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un imputado para imprimir el gafete del visitante.");
                            return;
                        }
                        if (ValidacionesGafete())
                        {
                            FechaFinRegistro = Fechas.GetFechaDateServer;
                            #region GAFETE
                            GafeteView = new GafetesView();
                            GafeteView.rbMenor.IsEnabled = false;
                            GafeteView.rbFamiliar.IsEnabled = false;
                            GafeteView.rbAcompanante.IsEnabled = false;
                            GafeteView.rbDiscapacitado.IsEnabled = false;
                            if (new Fechas().CalculaEdad(SelectVisitante.OBJETO_PERSONA.FEC_NACIMIENTO) < Parametro.MAYORIA_EDAD &&
                                (SelectVisitanteIngreso.VISITANTE.PERSONA.SEXO == "M" ?
                                new Fechas().CalculaEdad(SelectVisitante.OBJETO_PERSONA.FEC_NACIMIENTO) > Parametro.EDAD_MENOR_M :
                                new Fechas().CalculaEdad(SelectVisitante.OBJETO_PERSONA.FEC_NACIMIENTO) > Parametro.EDAD_MENOR_F))
                            {
                                GafeteView.rbMenor.IsEnabled = true;
                                GafeteView.rbMenor.IsChecked = true;
                            }
                            else
                            {
                                if (SelectTipoVisitante == Parametro.ID_TIPO_VISITANTE_INTIMA)
                                {
                                    if (SelectVisitanteIngreso.ACOMPANANTE != null ? SelectVisitanteIngreso.ACOMPANANTE.Count > 0 : false)
                                    {
                                        GafeteView.rbAcompanante.IsEnabled = true;
                                        GafeteView.rbAcompanante.IsChecked = true;
                                    }
                                    GafeteView.rbFamiliar.IsEnabled = true;
                                    GafeteView.rbFamiliar.IsChecked = true;
                                }
                                else if (SelectTipoVisitante == Parametro.ID_TIPO_VISITANTE_ORDINARIO)
                                {
                                    if (SelectVisitanteIngreso.ACOMPANANTE != null ? SelectVisitanteIngreso.ACOMPANANTE.Count > 0 : false)
                                    {
                                        GafeteView.rbAcompanante.IsEnabled = true;
                                        GafeteView.rbAcompanante.IsChecked = true;
                                    }
                                    GafeteView.rbFamiliar.IsEnabled = true;
                                    GafeteView.rbFamiliar.IsChecked = true;
                                }
                                else if (SelectTipoVisitante == Parametro.ID_TIPO_VISITANTE_DISCAPACITADO)
                                {
                                    GafeteView.rbDiscapacitado.IsEnabled = true;
                                    GafeteView.rbDiscapacitado.IsChecked = true;
                                }
                            }
                            #region Checked
                            GafeteView.rbFamiliar.Checked += RB_Checked;
                            GafeteView.rbMenor.Checked += RB_Checked;
                            GafeteView.rbDiscapacitado.Checked += RB_Checked;
                            GafeteView.rbAcompanante.Checked += RB_Checked;
                            #endregion

                            if (GafeteView.rbFamiliar.IsChecked.Value)
                                CrearGafete(GafeteView, GafeteView.rbFamiliar);
                            else if (GafeteView.rbDiscapacitado.IsChecked.Value)
                                CrearGafete(GafeteView, GafeteView.rbDiscapacitado);
                            else if (GafeteView.rbMenor.IsChecked.Value)
                                CrearGafete(GafeteView, GafeteView.rbMenor);
                            else if (GafeteView.rbAcompanante.IsChecked.Value)
                                CrearGafete(GafeteView, GafeteView.rbAcompanante);

                            GafeteView.rbFamiliar.Checked -= RB_Checked;
                            GafeteView.rbMenor.Checked -= RB_Checked;
                            GafeteView.rbDiscapacitado.Checked -= RB_Checked;
                            GafeteView.rbAcompanante.Checked -= RB_Checked;
                            #endregion
                        }
                    }
                    #endregion
                    break;
                case "insertar_menu":
                    break;
                case "limpiar_menu":
                    LimpiarGeneral();
                    break;
                case "guardar_menu":
                    await GuardarPadronVisita();

                    IsDetalleInternosEnable = false;
                    break;
                case "buscar_menu":
                    if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    {
                        AuxIngreso = SelectIngreso;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    }
                    else if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                    else if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                    break;
                #endregion

                #region BUSQUEDA VISITA
                case "cancelar_buscar_visita":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                    break;
                case "crear_nuevo_buscar_visita":
                    var nom = TextNombre;
                    var pat = TextPaterno;
                    var mat = TextMaterno;
                    LimpiarPadron();
                    TextNombre = nom;
                    TextPaterno = pat;
                    TextMaterno = mat;
                    SetValidacionesHojaVisita();
                    ValidarEnabled = GeneralEnabled = ContextMenuEnabled = true;
                    FotoHuellaEnabled = false;
                    OnPropertyChanged();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                    break;
                case "seleccionar_buscar_visita":
                    try
                    {
                        if (SelectVisitante == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar una persona.");
                            ContextMenuEnabled = false;
                        }
                        MenuGuardarEnabled = !PInsertar ? PEditar : true;
                        if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                        {
                            AcompananteVisible = Visibility.Collapsed;
                            CrearNuevoVisitanteEnabled = false;
                            SetValidacionesGenerales();
                            GetDatosVisitanteSeleccionadoPadron();
                            #region Nota Tecnica
                            if (SelectVisitante.OBJETO_PERSONA != null)
                                NotaTecnica = SelectVisitante.OBJETO_PERSONA.NOTA_TECNICA;
                            #endregion
                            FotoHuellaEnabled = true;
                            BanderaEditar = ContextMenuEnabled = SelectVisitante.ID_PERSONA != null;
                        }
                        else if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                        {
                            SetValidacionesHojaVisita();
                            GetDatosVisitanteSeleccionadoAsignacion();
                            FotoHuellaEnabled = false;
                            ContextMenuEnabled = CrearNuevoVisitanteEnabled = true;
                        }
                        else if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                        {
                            if (SelectVisitante.OBJETO_PERSONA.ID_TIPO_DISCAPACIDAD != null && SelectVisitante.OBJETO_PERSONA.ID_TIPO_DISCAPACIDAD > 0)
                            {
                                if (SelectVisitante.OBJETO_PERSONA.TIPO_DISCAPACIDAD.HUELLA == "S")
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de validar la identidad del visitante a traves de la huella digital");
                                    break;
                                }
                            }
                            else
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Favor de validar la identidad del visitante a traves de la huella digital");
                                break;
                            }
                            AcompananteVisible = Visibility.Collapsed;
                            CrearNuevoVisitanteEnabled = false;
                            SetValidacionesGenerales();
                            GetDatosVisitanteSeleccionadoPadron();
                            //TextNip = SelectVisitante.OBJETO_PERSONA.PERSONA_NIP.Where(w => w.ID_CENTRO == SelectVisitante.ID_CENTRO && w.ID_TIPO_VISITA == Parametro.ID_TIPO_VISITA_FAMILIAR).FirstOrDefault().NIP.Value.ToString();
                            FotoHuellaEnabled = true;
                            BanderaEditar = ContextMenuEnabled = SelectVisitante.ID_PERSONA != null;
                        }
                        ValidarEnabled = GeneralEnabled = true;
                        CodigoEnabled = true;
                        OnPropertyChanged();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del visitante.", ex);
                    }
                    break;
                case "buscar_visitante":
                    var codigo = TextCodigo;
                    TextCodigo = new Nullable<long>();
                    BuscarVisita();
                    TextCodigo = codigo;
                    base.ClearRules();
                    OnPropertyChanged();
                    break;
                case "buscar_visita":
                    if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                        CrearNuevoVisitanteEnabled = false;
                    else if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                        CrearNuevoVisitanteEnabled = true;
                    var codig = TextCodigo;
                    TextCodigo = new Nullable<long>();
                    BuscarVisita();
                    TextCodigo = codig;
                    base.ClearRules();
                    OnPropertyChanged();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                    break;
                #endregion

                #region FOTOS
                case "cancelar_tomar_foto_senas":
                    try
                    {
                        if (ImagesToSave != null ? ImagesToSave.Count == 1 : false)
                        {
                            if (!FotoTomada)
                            {
                                ImagesToSave = null;
                            }
                        }
                        else
                        {
                            if (FotoTomada)
                            {
                                ImagesToSave.Add(new ImageSourceToSave { FrameName = "ImgSenaParticular", ImageCaptured = new Imagenes().ConvertByteToBitmap(ImagenPersona) });
                                //BotonTomarFotoEnabled = true;
                            }
                        }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                        if (CamaraWeb != null)
                        {
                            await CamaraWeb.ReleaseVideoDevice();
                            CamaraWeb = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
                    }
                    break;
                case "aceptar_tomar_foto_senas":
                    try
                    {
                        if (ImagenPersona != new Imagenes().getImagenPerson() && (ImagesToSave != null ? ImagesToSave.Count == 1 : false) && ImagesToSave.FirstOrDefault().ImageCaptured != null)
                        {
                            FotoTomada = true;
                            ImagenPersona = new Imagenes().ConvertBitmapToByte(ImagesToSave.FirstOrDefault().ImageCaptured);
                            FotoVisita = new Imagenes().ConvertByteToBitmap(ImagenPersona);
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                            if (CamaraWeb != null)
                            {
                                await CamaraWeb.ReleaseVideoDevice();
                                CamaraWeb = null;
                            }
                            break;
                        }
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "DEBES DE TOMAR UNA FOTO.");
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la foto.", ex);
                    }
                    break;
                case "Open442":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.HUELLAS);
                    this.ShowIdentification();
                    break;
                #endregion

                #region BUSCAR IMPUTADO
                case "nueva_busqueda":
                    NombreD = NombreBuscar = PaternoD = ApellidoPaternoBuscar = MaternoD = ApellidoMaternoBuscar = AnioD = FolioD = string.Empty;
                    FolioBuscar = AnioBuscar = new Nullable<int>();
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>(); //new ObservableCollection<IMPUTADO>();
                    break;
                case "buscar_visible":
                    var ing = SelectIngreso;
                    SelectIngresoAuxiliar = ing;
                    var exp = SelectExpediente;
                    SelectExpedienteAuxiliar = exp;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_salir":
                    SelectIngreso = AuxIngreso;
                    AuxIngreso = null;
                    if (SelectIngreso != null)
                        if (SelectIngreso.IMPUTADO != null)
                            SelectExpediente = SelectIngreso.IMPUTADO;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_seleccionar":
                    try
                    {
                        if (SelectExpediente == null) ContextMenuNuevoVisitanteEnabled = false;
                        if (SelectIngreso == null)
                        {
                            ContextMenuNuevoVisitanteEnabled = false;
                            (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar un ingreso.");
                        }
                        if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no pertenece a este centro.");
                            return;
                        }
                        MenuGuardarEnabled = !PInsertar ? PEditar : true;
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        if (EstatusInactivos.Contains(SelectIngreso.ID_ESTATUS_ADMINISTRATIVO))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "Debes seleccionar un ingreso activo.");
                            return;
                        }
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");
                            return;
                        }
                        ApellidoPaternoBuscar = SelectExpediente.PATERNO;
                        ApellidoMaternoBuscar = SelectExpediente.MATERNO;
                        NombreBuscar = SelectExpediente.NOMBRE;
                        AnioD = SelectExpediente.ID_ANIO.ToString();
                        FolioD = SelectExpediente.ID_IMPUTADO.ToString();
                        AnioBuscar = SelectExpediente.ID_ANIO;
                        FolioBuscar = SelectExpediente.ID_IMPUTADO;
                        ContextMenuNuevoVisitanteEnabled = true;
                        if (!InsertarNuevoImputadoVisita)
                        {
                            SelectImputadoIngreso = SelectIngreso;
                            SetValidacionesGenerales();
                            GetDatosIngresoImputadoSeleccionado();
                        }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                        {
                            if (SelectIngreso.VISITA_AUTORIZADA.Any())
                            {
                                #region LISTA TONTA
                                await TaskEx.Delay(150);
                                //LimpiarDatosImputado();
                                var listaTonta = SelectIngreso.VISITA_AUTORIZADA;
                                if (listaTonta.Any())
                                {
                                    ListVisitantesImputado = new ObservableCollection<PERSONAVISITAAUXILIAR>(listaTonta.Select(s => new PERSONAVISITAAUXILIAR
                                       {
                                           ID_CENTRO = s.ID_CENTRO,
                                           ID_ANIO = s.ID_ANIO,
                                           ID_IMPUTADO = s.ID_IMPUTADO,
                                           ID_VISITA = s.ID_VISITA,
                                           ID_COLONIA = s.ID_COLONIA,
                                           ID_MUNICIPIO = s.ID_MUNICIPIO,
                                           ID_ENTIDAD = s.ID_ENTIDAD,
                                           ID_PAIS = s.ID_PAIS,
                                           SEXO = s.SEXO,
                                           ID_ESTATUS_VISITA = Parametro.ID_ESTATUS_VISITA_REGISTRO,
                                           ESTATUS_VISITA_DESCR = "REGISTRO",
                                           INGRESO_NOMBRE = s.INGRESO.IMPUTADO.NOMBRE,
                                           INGRESO_PATERNO = s.INGRESO.IMPUTADO.PATERNO,
                                           INGRESO_MATERNO = s.INGRESO.IMPUTADO.MATERNO,
                                           ID_PARENTESCO = s.ID_PARENTESCO,
                                           PARENTESCO_DESCR = s.TIPO_REFERENCIA.DESCR,
                                           EDAD = s.EDAD,
                                           TIPO_REFERENCIA = s.TIPO_REFERENCIA,
                                           ESTATUS_VISITA = new ESTATUS_VISITA() { ID_ESTATUS_VISITA = Parametro.ID_ESTATUS_VISITA_REGISTRO, DESCR = "REGISTRO" },
                                           INGRESO = s.INGRESO,
                                           DOMICILIO = s.DOMICILIO_CALLE + " " + (s.DOMICILIO_NUM_EXT.HasValue ? s.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty) + " " + s.DOMICILIO_NUM_INT + " " + (s.DOMICILIO_CODIGO_POSTAL.HasValue ? s.DOMICILIO_CODIGO_POSTAL.Value.ToString() : string.Empty),
                                           TELEFONO = s.TELEFONO,

                                           NOMBRE = s.NOMBRE,
                                           MATERNO = s.MATERNO,
                                           PATERNO = s.PATERNO,
                                           OBJETO_VISITA_AUTORIZADA = s
                                       }).ToList());
                                }
                                if (InsertarNuevoImputadoVisita)
                                {
                                    ListVisitaAutorizada = new ObservableCollection<VISITA_AUTORIZADA>(SelectIngreso.VISITA_AUTORIZADA.Where(w => w.ESTATUS == 0));
                                    EmptyBuscarListaTonta = ListVisitaAutorizada == null ? false : ListVisitaAutorizada.Count > 0 ? false : true;
                                    SelectParentescoListaTonta = -1;
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.INGRESAR_LISTA_TONTA);
                                    break;
                                }
                                if (AcompananteVisible == Visibility.Visible && (SelectVisitante == null ? false : SelectVisitante.OBJETO_PERSONA != null))
                                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.INGRESA_IMPUTADO_VISITANTE);
                                #endregion
                            }
                            else
                            {
                                #region VISING
                                if (await (new Dialogos()).ConfirmarEliminar("Advertencia!", "El imputado no tiene ningun visitante en su lista de visita autorizada, ¿Desea agregarlo? .") == 1)
                                {
                                    if (SelectIngreso.CAMA.ID_CAMA != 0)
                                    {
                                        SelectIngreso.CAMA.CELDA.ID_CELDA = SelectIngreso.CAMA.CELDA.ID_CELDA.ToString().Trim();
                                        SelectIngreso.CAMA.CELDA.SECTOR.DESCR = SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim();
                                        SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR = SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim();
                                    }
                                    var newVisIng = new VISITANTE_INGRESO();
                                    //{
                                    newVisIng.EMISION_GAFETE = "N";
                                    newVisIng.ESTATUS_MOTIVO = string.Empty;
                                    newVisIng.FEC_ALTA = Fechas.GetFechaDateServer;
                                    newVisIng.FEC_ULTIMA_MOD = newVisIng.FEC_ALTA;//Fechas.GetFechaDateServer;
                                    newVisIng.ID_ANIO = SelectIngreso.ID_ANIO;
                                    newVisIng.ID_CENTRO = SelectIngreso.ID_CENTRO;
                                    newVisIng.ID_INGRESO = SelectIngreso.ID_INGRESO;
                                    newVisIng.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                                    newVisIng.ID_ESTATUS_VISITA = Parametro.ID_ESTATUS_VISITA_REGISTRO;
                                    newVisIng.OBSERVACION = string.Empty;
                                    newVisIng.ID_TIPO_REFERENCIA = SelectParentesco;
                                    newVisIng.ID_PERSONA = SelectVisitante.ID_PERSONA.Value;// SelectVisitanteIngreso.ID_PERSONA;
                                    newVisIng.INGRESO = SelectIngreso;
                                    newVisIng.TIPO_REFERENCIA = ListTipoRelacion != null ? ListTipoRelacion.Where(w => w.ID_TIPO_REFERENCIA == SelectParentesco).FirstOrDefault() : null;
                                    newVisIng.ESTATUS_VISITA = ListSituacion != null ? ListSituacion.Where(w => w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_REGISTRO).FirstOrDefault() : null;
                                    newVisIng.VISITANTE = SelectVisitanteIngreso != null ? (SelectVisitanteIngreso.VISITANTE != null ? SelectVisitanteIngreso.VISITANTE : null) : null;
                                    newVisIng.ACCESO_UNICO = SelectAccesoUnico ? "S" : "N";
                                    newVisIng.ID_TIPO_VISITANTE = Parametro.ID_TIPO_VISITANTE_ORDINARIO;
                                    newVisIng.TIPO_VISITANTE = ListTipoVisitante != null ? ListTipoVisitante.Where(w => w.ID_TIPO_VISITANTE == 14).FirstOrDefault() : null;
                                    //};
                                    ListadoInternos.Add(newVisIng);
                                    MenuGuardarEnabled = !PInsertar ? PEditar : true;
                                    SelectVisitanteIngreso = newVisIng;
                                    SituacionEnabled = false;
                                    AccesoUnicoEnabled = SelectVisitanteIngreso.ID_ESTATUS_VISITA != Parametro.ID_ESTATUS_VISITA_AUTORIZADO;
                                    //TextNip = SelectVisitanteIngreso.VISITANTE.PERSONA.PERSONA_NIP != null ?
                                    //    SelectVisitanteIngreso.VISITANTE.PERSONA.PERSONA_NIP.Count > 0 ?
                                    //        SelectVisitanteIngreso.VISITANTE.PERSONA.PERSONA_NIP.FirstOrDefault().NIP.ToString() :
                                    //            string.Empty : string.Empty;
                                    SelectImputadoIngreso = ListadoInternos.FirstOrDefault().INGRESO;
                                    GetDatosIngresoImputadoSeleccionado();
                                    ListBuscarAcompanantes = new ObservableCollection<VISITANTE_INGRESO>();

                                    ListBuscarAcompanantes = new ObservableCollection<VISITANTE_INGRESO>(SelectImputadoIngreso.VISITANTE_INGRESO.Where(w =>
                                        w.ID_PERSONA != SelectVisitanteIngreso.ID_PERSONA &&
                                        !ListAcompanantes.Where(x => x.ID_ACOMPANANTE == w.ID_PERSONA).Any() &&
                                        new Fechas().CalculaEdad(w.VISITANTE.PERSONA.FEC_NACIMIENTO.HasValue ?
                                        w.VISITANTE.PERSONA.FEC_NACIMIENTO :
                                        DateTime.Parse("01/01/1900")) >= 18 &&
                                        (w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_EN_REVISION || w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_AUTORIZADO)));
                                    EmptyBuscarAcompanante = ListBuscarAcompanantes.Count == 0;

                                    //!new cAcompanante().ObtenerXAcompanado(w.ID_PERSONA).Any() &&
                                    SelectEstatusRelacionEnabled = false;
                                    SelectParentescoIngresoEnabled = true;
                                    NuevoListaTonta = new VISITANTE_INGRESO
                                    {
                                        ID_CENTRO = SelectImputadoIngreso.ID_CENTRO,
                                        ID_ANIO = SelectImputadoIngreso.ID_ANIO,
                                        ID_IMPUTADO = SelectImputadoIngreso.ID_IMPUTADO,
                                        ID_INGRESO = SelectImputadoIngreso.ID_INGRESO,
                                        ID_PERSONA = SelectVisitante.ID_PERSONA.Value
                                    };
                                }
                                #endregion
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado.", ex);
                    }
                    break;
                #endregion

                #region ACOMPANANTE
                case "insertar_acompanante":
                    if (SelectAcompanante != null)
                    {
                        var list = new List<ACOMPANANTE>();
                        list.Add(SelectAcompanante);
                        SelectAcompananteAuxiliar = list.FirstOrDefault();
                    }
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.INGRESA_ACOMPANIANTE);
                    break;
                case "seleccionar_buscar_acompanante":
                    BuscarAcompananteVisible = false;
                    AcompananteVisible = Visibility.Visible;
                    break;
                case "salir_buscar_acompanante":
                    BuscarAcompananteVisible = false;
                    AcompananteVisible = Visibility.Visible;
                    break;
                case "borrar_acompanante":
                    if (SelectAcompanante != null)
                        ListAcompanantes.Remove(SelectAcompanante);
                    break;
                #endregion

                #region AGENDA
                case "insertar_agenda_visitante":
                    LimpiarProgramacionVisita();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PROGRAMAR_VISITA);
                    break;
                case "borrar_agenda_visitante":
                    try
                    {
                        if (SelectVisitaProgramada == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar una visita para poder borrarla.");
                            return;
                        }
                        if (SelectVisitaProgramada.VISITA_AGENDA == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No puedes eliminar esta agenda.");
                            return;
                        }
                        var agenda = new cVisitaAgenda().ObtenerTodos(SelectVisitaProgramada.ID_CENTRO, SelectVisitaProgramada.ID_ANIO, SelectVisitaProgramada.ID_IMPUTADO, SelectVisitaProgramada.ID_INGRESO);
                        if (agenda.Any() ? !agenda.Any(a => a.ID_AREA == SelectVisitaProgramada.ID_AREA && a.ID_DIA == SelectVisitaProgramada.ID_DIA && a.ID_TIPO_VISITA == SelectVisitaProgramada.ID_TIPO_VISITA &&
                            a.ESTATUS == (SelectVisitaProgramada.ESTATUS ? "0" : "1") && a.HORA_INI == SelectVisitaProgramada.HORA_INI && a.HORA_FIN == SelectVisitaProgramada.HORA_FIN) : false)
                        {
                            if (!ListProgramacionVisita.Remove(SelectVisitaProgramada))
                            {
                                (new Dialogos()).ConfirmacionDialogo("Exito!", "Ocurrió un error al borrar la programación de visita seleccionada.");
                                return;
                            }
                            base.ClearRules();
                            var ingres = new cIngreso().Obtener(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);
                            SelectIngreso = ingres;
                            SelectImputadoIngreso = ingres;
                            await GetVisitasAgendadas();
                            (new Dialogos()).ConfirmacionDialogo("Exito!", "Borrada satisfactoriamente.");
                            return;
                        }
                        if (await new Dialogos().ConfirmarEliminar("Advertencia", "Esta seguro que desea eliminar este registro?") == 1)
                        {
                            if (new cVisitaAgenda().Actualizar(new VISITA_AGENDA
                            {
                                ESTATUS = "1",
                                ID_DIA = SelectVisitaProgramada.ID_DIA,
                                HORA_FIN = SelectVisitaProgramada.HORA_FIN,
                                HORA_INI = SelectVisitaProgramada.HORA_INI,
                                ID_ANIO = SelectVisitaProgramada.ID_ANIO,
                                ID_AREA = SelectVisitaProgramada.ID_AREA,
                                ID_CENTRO = SelectVisitaProgramada.ID_CENTRO,
                                ID_IMPUTADO = SelectVisitaProgramada.ID_IMPUTADO,
                                ID_INGRESO = SelectVisitaProgramada.ID_INGRESO,
                                ID_TIPO_VISITA = SelectVisitaProgramada.ID_TIPO_VISITA
                            }))
                            {
                                var ingres = new cIngreso().Obtener(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);
                                //clickSwitch("buscar_seleccionar");
                                SelectIngreso = ingres;
                                SelectImputadoIngreso = ingres;
                                await GetVisitasAgendadas();
                                base.ClearRules();
                                (new Dialogos()).ConfirmacionDialogo("Exito!", "Borrada satisfactoriamente.");
                            }
                            else
                            {
                                (new Dialogos()).ConfirmacionDialogo("Exito!", "Ocurrió un error al borrar la programacion de visita seleccionada.");
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al borrar la programación de visita seleccionada.", ex);
                    }
                    break;
                case "guardar_cita":
                    #region VALIDACIONES
                    if (SelectIngreso == null)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar un imputado valido.");
                        return;
                    }
                    if (SelectAreaVisita.HasValue ? SelectAreaVisita.Value <= 0 : true)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar el área a donde se dirigirá.");
                        return;
                    }
                    if (SelectTipoVisita.HasValue ? SelectTipoVisita.Value <= 0 : true)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar el tipo de visita.");
                        return;
                    }
                    if (string.IsNullOrEmpty(SelectDiaVisita) ? true : GetDiaVisitaShort(SelectDiaVisita) <= 0)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar el dia en el que se va asistir.");
                        return;
                    }
                    if (string.IsNullOrEmpty(SelectHoraEntrada))
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar la hora de entrada.");
                        return;
                    }
                    if (string.IsNullOrEmpty(SelectMinutoEntrada))
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar el minuto de la hora de entrada.");
                        return;
                    }
                    if (string.IsNullOrEmpty(SelectHoraSalida))
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar la hora de salida.");
                        return;
                    }
                    if (string.IsNullOrEmpty(SelectMinutoSalida))
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar el minuto de la hora de salida.");
                        return;
                    }
                    if (short.Parse(SelectHoraEntrada) > short.Parse(SelectHoraSalida))
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "La hora de entrada debe ser menor a la hora de salida.");
                        return;
                    }
                    if (SelectHoraEntrada == SelectHoraSalida ? short.Parse(SelectMinutoEntrada) > short.Parse(SelectMinutoSalida) : false)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "La hora de entrada debe ser menor a la hora de salida.");
                        return;
                    }
                    #endregion

                    #region AGREGAR
                    ListProgramacionVisitaAux = ListProgramacionVisitaAux ?? new ObservableCollection<ListaVisitaAgenda>();
                    ListProgramacionVisitaAux.Add(new ListaVisitaAgenda()
                    {
                        HORA_FIN = SelectHoraSalida + SelectMinutoSalida,
                        HORA_INI = SelectHoraEntrada + SelectMinutoEntrada,
                        ID_ANIO = SelectIngreso.ID_ANIO,
                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                        ID_AREA = SelectAreaVisita,
                        ID_DIA = GetDiaVisitaShort(SelectDiaVisita),
                        ID_TIPO_VISITA = SelectTipoVisita.Value,
                        ESTATUS = true,

                        AREA = await StaticSourcesViewModel.CargarDatosAsync<string>(() => new cArea().GetData().Where(w => w.ID_AREA == SelectAreaVisita).FirstOrDefault().DESCR),
                        DIA = SelectDiaVisita,
                        HORA_SALIDA = SelectHoraSalida + ":" + SelectMinutoSalida,
                        HORA_ENTRADA = SelectHoraEntrada + ":" + SelectMinutoEntrada,
                        TIPO_VISITA = await StaticSourcesViewModel.CargarDatosAsync<string>(() => new cTipoVisita().GetData().Where(w => w.ID_TIPO_VISITA == SelectTipoVisita.Value).FirstOrDefault().DESCR),
                        VISITA_AGENDA = new VISITA_AGENDA()
                    });
                    ListProgramacionVisita = new ObservableCollection<ListaVisitaAgenda>(ListProgramacionVisitaAux.Where(w => w.ESTATUS));
                    MenuGuardarEnabled = !PInsertar ? PEditar : true;
                    NuevaVisitaAgenda = true;
                    #endregion

                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PROGRAMAR_VISITA);
                    break;
                case "cancelar_cita":
                    LimpiarProgramacionVisita();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PROGRAMAR_VISITA);
                    break;
                #endregion

                #region DIGITALIZAR DOCUMENTOS
                case "digitalizar_documentos":
                    if (SelectImputadoIngreso != null)
                    {
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarListaTipoDocumentoDigitalizado(); });
                    }
                    else
                        await new Dialogos().ConfirmacionDialogoReturn("Validación", "No ha seleccionado un imputado.");
                    DatePickCapturaDocumento = Fechas.GetFechaDateServer;
                    ObservacionDocumento = string.Empty;
                    break;
                case "Cancelar_digitalizar_documentos":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                    escaner.Hide();
                    DocumentoDigitalizado = null;
                    break;
                case "guardar_documento":
                    GuardarDocumento();
                    break;
                #endregion

                #region Entrega Credencial
                case "aceptar_huella":
                    try
                    {
                        if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                        {
                            #region VISITANTE_INGRESO
                            HuellaWindow.Hide();
                            var autorizado = Parametro.ID_ESTATUS_VISITA_AUTORIZADO;
                            var hoy = Fechas.GetFechaDateServer;
                            if (new cVisitanteIngreso().Actualizar(new VISITANTE_INGRESO
                            {
                                EMISION_GAFETE = "S",
                                FEC_ALTA = SelectVisitanteIngreso.FEC_ALTA,
                                FEC_ULTIMA_MOD = hoy,
                                ID_ANIO = SelectVisitanteIngreso.ID_ANIO,
                                ID_CENTRO = SelectVisitanteIngreso.ID_CENTRO,
                                ID_ESTATUS_VISITA = autorizado,
                                ID_IMPUTADO = SelectVisitanteIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectVisitanteIngreso.ID_INGRESO,
                                ID_PERSONA = SelectVisitanteIngreso.ID_PERSONA,
                                ID_TIPO_REFERENCIA = SelectVisitanteIngreso.ID_TIPO_REFERENCIA,
                                OBSERVACION = SelectVisitanteIngreso.OBSERVACION,
                                ESTATUS_MOTIVO = SelectVisitanteIngreso.ESTATUS_MOTIVO,
                                ACCESO_UNICO = SelectVisitanteIngreso.ACCESO_UNICO,
                                ID_TIPO_VISITANTE = SelectVisitanteIngreso.ID_TIPO_VISITANTE,
                            }))
                            {
                                var menorM = Parametro.EDAD_MENOR_M;
                                var menorF = Parametro.EDAD_MENOR_F;
                                var cancelado = Parametro.ID_ESTATUS_VISITA_CANCELADO;
                                var suspendido = Parametro.ID_ESTATUS_VISITA_SUSPENDIDO;
                                var acompIng = SelectVisitanteIngreso.ACOMPANANTE.Where(w => new Fechas().CalculaEdad(w.VISITANTE_INGRESO1.VISITANTE.PERSONA.FEC_NACIMIENTO) <=
                                        (w.VISITANTE_INGRESO1.VISITANTE.PERSONA.SEXO == "M" ? menorM : menorF) && w.VISITANTE_INGRESO1.ID_ESTATUS_VISITA != cancelado && w.VISITANTE_INGRESO1.ID_ESTATUS_VISITA != suspendido);
                                if (acompIng.Any())
                                    foreach (var visIng1 in acompIng)
                                    {
                                        new cVisitanteIngreso().Actualizar(new VISITANTE_INGRESO
                                        {
                                            EMISION_GAFETE = "S",
                                            FEC_ALTA = visIng1.VISITANTE_INGRESO1.FEC_ALTA,
                                            FEC_ULTIMA_MOD = hoy,
                                            ID_ANIO = visIng1.VISITANTE_INGRESO1.ID_ANIO,
                                            ID_CENTRO = visIng1.VISITANTE_INGRESO1.ID_CENTRO,
                                            ID_ESTATUS_VISITA = autorizado,
                                            ID_IMPUTADO = visIng1.VISITANTE_INGRESO1.ID_IMPUTADO,
                                            ID_INGRESO = visIng1.VISITANTE_INGRESO1.ID_INGRESO,
                                            ID_PERSONA = visIng1.VISITANTE_INGRESO1.ID_PERSONA,
                                            ID_TIPO_REFERENCIA = visIng1.VISITANTE_INGRESO1.ID_TIPO_REFERENCIA,
                                            OBSERVACION = visIng1.VISITANTE_INGRESO1.OBSERVACION,
                                            ESTATUS_MOTIVO = visIng1.VISITANTE_INGRESO1.ESTATUS_MOTIVO,
                                            ACCESO_UNICO = visIng1.VISITANTE_INGRESO1.ACCESO_UNICO,
                                            ID_TIPO_VISITANTE = visIng1.VISITANTE_INGRESO1.ID_TIPO_VISITANTE
                                        });
                                    }
                                HuellaWindow.Close();
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                BuscarVisita();
                                StaticSourcesViewModel.Mensaje("Entrega Credencial", "La credencial fue entregada a la persona.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                            }
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al comparar la huella.", ex);
                    }
                    break;
                case "EntregaPersona":
                    try
                    {
                        if (SelectVisitante == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar una persona.");
                            return;
                        }
                        if (SelectVisitanteIngreso == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar a un imputado.");
                            return;
                        }
                        if (SelectVisitanteIngreso.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_CANCELADO || SelectVisitanteIngreso.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_SUSPENDIDO)
                        {
                            if (await new Dialogos().ConfirmarEliminar("Entrega de Credencial", "Este imputado se encuentra " + SelectVisitanteIngreso.ESTATUS_VISITA.DESCR +
                                ".\nDesea reactivarlo?") == 1)
                            {
                                new cVisitanteIngreso().Actualizar(new VISITANTE_INGRESO
                                {
                                    EMISION_GAFETE = SelectVisitanteIngreso.EMISION_GAFETE,
                                    FEC_ALTA = SelectVisitanteIngreso.FEC_ALTA,
                                    FEC_ULTIMA_MOD = Fechas.GetFechaDateServer,
                                    ID_ANIO = SelectVisitanteIngreso.ID_ANIO,
                                    ID_CENTRO = SelectVisitanteIngreso.ID_CENTRO,
                                    ID_ESTATUS_VISITA = Parametro.ID_ESTATUS_VISITA_AUTORIZADO,
                                    ID_IMPUTADO = SelectVisitanteIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectVisitanteIngreso.ID_INGRESO,
                                    ID_PERSONA = SelectVisitanteIngreso.ID_PERSONA,
                                    ID_TIPO_REFERENCIA = SelectVisitanteIngreso.ID_TIPO_REFERENCIA,
                                    OBSERVACION = SelectVisitanteIngreso.OBSERVACION,
                                    ESTATUS_MOTIVO = SelectVisitanteIngreso.ESTATUS_MOTIVO,
                                    ACCESO_UNICO = SelectVisitanteIngreso.ACCESO_UNICO,
                                    ID_TIPO_VISITANTE = SelectVisitanteIngreso.ID_TIPO_VISITANTE
                                });
                                BuscarVisita();
                            }
                        }
                        else
                        {
                            if (SelectVisitanteIngreso.EMISION_GAFETE == "S")
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Esta persona no tiene una credencial en espera.");
                                return;
                            }
                            if (SelectVisitanteIngreso.ID_ESTATUS_VISITA != Parametro.ID_ESTATUS_VISITA_EN_REVISION)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Esta persona no tiene una credencial en espera.");
                                return;
                            }
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            HuellaWindow = new BuscarPorHuellaYNipView();
                            HuellaWindow.DataContext = this;
                            ConstructorHuella(0);
                            HuellaWindow.Owner = PopUpsViewModels.MainWindow;
                            HuellaWindow.ShowDialog();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar el registro de la entrega de credencial.", ex);
                    }
                    break;
                case "EntregaAcompaniante":
                    try
                    {
                        if (SelectVisitante == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar una persona.");
                            return;
                        }
                        if (SelectAcompanante == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar un acompañante.");
                            return;
                        }
                        if (SelectAcompanante.VISITANTE_INGRESO1.EMISION_GAFETE == "S")
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Este acompañante no tiene una credencial en espera.");
                            return;
                        }
                        if (await new Dialogos().ConfirmarEliminar("Entrega de Credencial", "Esta entregando la credencial a " + SelectVisitante.NOMBRE.Trim() + " " +
                            SelectVisitante.PATERNO.Trim() + " " + SelectVisitante.MATERNO.Trim() + "\nComo representante de " +
                            SelectAcompanante.VISITANTE_INGRESO1.VISITANTE.PERSONA.NOMBRE.Trim() + " " + SelectAcompanante.VISITANTE_INGRESO1.VISITANTE.PERSONA.PATERNO.Trim()
                            + " " + SelectAcompanante.VISITANTE_INGRESO1.VISITANTE.PERSONA.MATERNO.Trim() + "\nDesea continuar?") == 1)
                        {
                            #region VISITANTE_INGRESO
                            var hoy = Fechas.GetFechaDateServer;
                            if (new cVisitanteIngreso().Actualizar(new VISITANTE_INGRESO
                            {
                                EMISION_GAFETE = "S",
                                FEC_ALTA = SelectAcompanante.VISITANTE_INGRESO1.FEC_ALTA,
                                FEC_ULTIMA_MOD = hoy,
                                ID_ANIO = SelectAcompanante.VISITANTE_INGRESO1.ID_ANIO,
                                ID_CENTRO = SelectAcompanante.VISITANTE_INGRESO1.ID_CENTRO,
                                ID_ESTATUS_VISITA = Parametro.ID_ESTATUS_VISITA_AUTORIZADO,
                                ID_IMPUTADO = SelectAcompanante.VISITANTE_INGRESO1.ID_IMPUTADO,
                                ID_INGRESO = SelectAcompanante.VISITANTE_INGRESO1.ID_INGRESO,
                                ID_PERSONA = SelectAcompanante.VISITANTE_INGRESO1.ID_PERSONA,
                                ID_TIPO_REFERENCIA = SelectAcompanante.VISITANTE_INGRESO1.ID_TIPO_REFERENCIA,
                                OBSERVACION = SelectAcompanante.VISITANTE_INGRESO1.OBSERVACION,
                                ESTATUS_MOTIVO = SelectAcompanante.VISITANTE_INGRESO1.ESTATUS_MOTIVO,
                                ACCESO_UNICO = SelectAcompanante.VISITANTE_INGRESO1.ACCESO_UNICO,
                                ID_TIPO_VISITANTE = SelectAcompanante.VISITANTE_INGRESO1.ID_TIPO_VISITANTE
                            }))
                            {
                                new cVisitante().Actualizar(new VISITANTE
                                {
                                    ESTATUS_MOTIVO = SelectAcompanante.VISITANTE_INGRESO1.VISITANTE.ESTATUS_MOTIVO,
                                    FEC_ALTA = SelectAcompanante.VISITANTE_INGRESO1.VISITANTE.FEC_ALTA,
                                    ID_ESTATUS_VISITA = SelectAcompanante.VISITANTE_INGRESO1.VISITANTE.ID_ESTATUS_VISITA,
                                    ID_PERSONA = SelectAcompanante.VISITANTE_INGRESO1.VISITANTE.ID_PERSONA,
                                    ULTIMA_MODIFICACION = hoy,
                                });
                                BuscarVisita();
                                StaticSourcesViewModel.Mensaje("Entrega Credencial", "La credencial de acompañante fue entregada a la persona", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                            }
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar el registro de la entrega de credencial.", ex);
                    }
                    break;
                #endregion

                #region PASES
                case "pase_de_termino":
                    try
                    {
                        if (SelectVisitanteIngreso == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No ha seleccionado un imputado con el que este relacionado este visitante.");
                            return;
                        }
                        if (SelectVisitanteIngreso.VISITA_DOCUMENTO == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El visitante seleccionado no cuenta con ningun documento para verificar su relacion con el imputado.");
                            return;
                        }
                        var param = Parametro.DOCUMENTO_ACCESO_UNICO;
                        var contar = 0;
                        var tipoVisita = 0;
                        var tipoDocto = 0;
                        foreach (var item in param)
                        {
                            tipoVisita = short.Parse(item.Split('-')[0]);
                            tipoDocto = short.Parse(item.Split('-')[1]);
                            if (SelectVisitanteIngreso.VISITA_DOCUMENTO.Any(a => a.DOCUMENTO != null ? a.ID_TIPO_VISITA == tipoVisita ? a.ID_TIPO_DOCUMENTO == tipoDocto : false : false))
                                contar++;
                        }
                        if (contar < param.Count())
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Hacen falta documentos necesarios para poder dar de alta un pase DE TERMINO.");
                            return;
                        }
                        var query = new cPasesVisitanteIngreso().ObtenerXPersonaEIngreso(SelectVisitanteIngreso.ID_CENTRO, SelectVisitanteIngreso.ID_ANIO, SelectVisitanteIngreso.ID_IMPUTADO, SelectVisitanteIngreso.ID_INGRESO, SelectVisitanteIngreso.ID_PERSONA);
                        var ultimoPase = query.Any() ?
                            query.ToList().Any(f => f.ADUANA_INGRESO != null ?
                                f.ADUANA_INGRESO.Any()
                            : false) ?
                                query.ToList().OrderByDescending(o => o.FECHA_ALTA).First(f => f.ADUANA_INGRESO != null ? f.ADUANA_INGRESO.Any() : false)
                            : query.FirstOrDefault()
                        : null;
                        if (ultimoPase != null)
                        {
                            var hoy = Fechas.GetFechaDateServer;
                            if (ultimoPase.FECHA_ALTA.HasValue ?
                                (ultimoPase.FECHA_ALTA.Value.Year == hoy.Year && ultimoPase.FECHA_ALTA.Value.Month == hoy.Month && ultimoPase.FECHA_ALTA.Value.Day == hoy.Day)
                            : false)
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Este visitante ya tiene un pase para hoy y no se le puede generar otro pase.");
                            else if (ultimoPase.ID_PASE == (short)enumTiposPases.DE_TERMINO)
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Este visitante ya utilizo un pase y no se le puede generar un pase DE TERMINO.");
                            else
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No se le puede generar un pase DE TERMINO al visitante seleccionado.");
                            return;
                        }
                        if (await new Dialogos().ConfirmarEliminar("Advertencia!", "Esta seguro que desea generar un pase DE TERMINO para el visitante seleccionado?") == 1)
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                var personaIngreso = new cPasesVisitanteIngreso().ObtenerSiguienteConsecutivo(SelectImputadoIngreso.ID_CENTRO, SelectImputadoIngreso.ID_ANIO, SelectImputadoIngreso.ID_IMPUTADO, SelectImputadoIngreso.ID_INGRESO,
                                    SelectVisitante.OBJETO_PERSONA.ID_PERSONA);
                                if (!new cPasesVisitanteIngreso().Insertar(new VISITANTE_INGRESO_PASE
                                {
                                    ID_ANIO = SelectVisitanteIngreso.ID_ANIO,
                                    ID_CENTRO = SelectVisitanteIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectVisitanteIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectVisitanteIngreso.ID_INGRESO,
                                    ID_PERSONA = SelectVisitanteIngreso.ID_PERSONA,
                                    ID_PASE = (short)enumTiposPases.DE_TERMINO,
                                    FECHA_ALTA = Fechas.GetFechaDateServer,
                                    ID_CONSEC = personaIngreso > 0 ? (short)(personaIngreso + 1) : (short)1
                                }))
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Ocurrió un error al guardar el pase.");
                                    }));
                                    return;
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        SelectVisitante = ConvertPersonaToAuxiliar(new cPersona().Obtener(TextCodigo.Value).FirstOrDefault());
                                        GetDatosVisitanteSeleccionadoPadron();
                                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Pase DE TERMINO registrado con éxito.");
                                    }));
                                    return;
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar el pase.", ex);
                    }
                    break;
                case "pase_extraordinario":
                    try
                    {
                        if (SelectVisitanteIngreso == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No ha seleccionado un imputado con el que este relacionado este visitante.");
                            return;
                        }
                        if (SelectVisitanteIngreso.VISITA_DOCUMENTO == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El visitante seleccionado no cuenta con ningun documento para verificar su relacion con el imputado.");
                            return;
                        }
                        var param = Parametro.DOCUMENTO_ACCESO_UNICO;
                        var contar = 0;
                        var tipoVisita = 0;
                        var tipoDocto = 0;
                        foreach (var item in param)
                        {
                            tipoVisita = short.Parse(item.Split('-')[0]);
                            tipoDocto = short.Parse(item.Split('-')[1]);
                            if (SelectVisitanteIngreso.VISITA_DOCUMENTO.Any(a => a.DOCUMENTO != null ? a.ID_TIPO_VISITA == tipoVisita ? a.ID_TIPO_DOCUMENTO == tipoDocto : false : false))
                                contar++;
                        }
                        if (contar < param.Count())
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Hacen falta documentos necesarios para poder dar de alta un pase EXTRAORDINARIO.");
                            return;
                        }
                        var pases = new cPasesVisitanteIngreso().ObtenerXPersonaEIngreso(SelectVisitanteIngreso.ID_CENTRO, SelectVisitanteIngreso.ID_ANIO, SelectVisitanteIngreso.ID_IMPUTADO, SelectVisitanteIngreso.ID_INGRESO,
                            SelectVisitanteIngreso.ID_PERSONA);
                        var ultimoPase = pases.Any() ? pases.OrderByDescending(o => o.FECHA_ALTA).ToList().First(f => f.ADUANA_INGRESO != null) : null;
                        var MensajeConfirmacion = string.Empty;
                        if (await new Dialogos().ConfirmarEliminar("Advertencia!", (ultimoPase != null ?
                            (ultimoPase.FECHA_ALTA.HasValue ?
                                ("ÚLTIMO PASE: " + ultimoPase.FECHA_ALTA.Value.ToString("dd/MM/yyyy hh:mm tt") + (ultimoPase.ADUANA_INGRESO.Any() ? " [UTILIZADO]" : " [NO UTILIZADO]") + "\n\n")
                            : "ERROR EN EL REGISTRO DEL ÚLTIMO PASE\n\n")
                        : string.Empty) +
                            "Esta seguro que desea generar un pase EXTRAORDINARIO para el visitante seleccionado?") == 1)
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                var personaIngreso = new cPasesVisitanteIngreso().ObtenerSiguienteConsecutivo(SelectImputadoIngreso.ID_CENTRO, SelectImputadoIngreso.ID_ANIO,
                                    SelectImputadoIngreso.ID_IMPUTADO, SelectImputadoIngreso.ID_INGRESO, SelectVisitante.OBJETO_PERSONA.ID_PERSONA);
                                if (!new cPasesVisitanteIngreso().Insertar(new VISITANTE_INGRESO_PASE
                                {
                                    ID_ANIO = SelectVisitanteIngreso.ID_ANIO,
                                    ID_CENTRO = SelectVisitanteIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectVisitanteIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectVisitanteIngreso.ID_INGRESO,
                                    ID_PERSONA = SelectVisitanteIngreso.ID_PERSONA,
                                    ID_PASE = (short)enumTiposPases.EXTRAORDINARIO,
                                    FECHA_ALTA = Fechas.GetFechaDateServer,
                                    ID_CONSEC = personaIngreso > 0 ? (short)(personaIngreso + 1) : (short)1
                                }))
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Ocurrió un error al guardar el pase.");
                                    }));
                                    return;
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        SelectVisitante = ConvertPersonaToAuxiliar(new cPersona().Obtener(TextCodigo.Value).FirstOrDefault());
                                        GetDatosVisitanteSeleccionadoPadron();
                                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Pase EXTRAORDINARIO registrado con éxito.");
                                    }));
                                    return;
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar el pase.", ex);
                    }
                    break;
                case "pase_10_dias":
                    try
                    {
                        if (SelectVisitanteIngreso == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No ha seleccionado un imputado con el que este relacionado este visitante.");
                            return;
                        }
                        if (SelectVisitanteIngreso.VISITA_DOCUMENTO == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El visitante seleccionado no cuenta con ningun documento para verificar su relacion con el imputado.");
                            return;
                        }
                        if (SelectVisitanteIngreso.ID_ESTATUS_VISITA != Parametro.ID_ESTATUS_VISITA_EN_REVISION)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El visitante seleccionado no cuenta con ningun documento para verificar su relacion con el imputado.");
                            return;
                        }
                        if (await new Dialogos().ConfirmarEliminar("Advertencia!", "Esta seguro que desea generar un pase POR 10 DIAS para el visitante seleccionado?") == 1)
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                var personaIngreso = new cPasesVisitanteIngreso().ObtenerSiguienteConsecutivo(SelectImputadoIngreso.ID_CENTRO, SelectImputadoIngreso.ID_ANIO, SelectImputadoIngreso.ID_IMPUTADO, SelectImputadoIngreso.ID_INGRESO,
                                    SelectVisitante.OBJETO_PERSONA.ID_PERSONA);
                                if (!new cPasesVisitanteIngreso().Insertar(new VISITANTE_INGRESO_PASE
                                {
                                    ID_ANIO = SelectVisitanteIngreso.ID_ANIO,
                                    ID_CENTRO = SelectVisitanteIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectVisitanteIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectVisitanteIngreso.ID_INGRESO,
                                    ID_PERSONA = SelectVisitanteIngreso.ID_PERSONA,
                                    ID_PASE = (short)enumTiposPases.POR_10_DIAS,
                                    FECHA_ALTA = Fechas.GetFechaDateServer,
                                    ID_CONSEC = personaIngreso > 0 ? (short)(personaIngreso + 1) : (short)1
                                }))
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Ocurrió un error al guardar el pase.");
                                    }));
                                    return;
                                }
                                else
                                {
                                    SelectVisitante = ConvertPersonaToAuxiliar(new cPersona().Obtener(TextCodigo.Value).FirstOrDefault());
                                    GetDatosVisitanteSeleccionadoPadron();
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Pase POR 10 DIAS registrado con éxito.");
                                    }));
                                    return;
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar el pase.", ex);
                    }
                    break;
                #endregion

                #region OTROS
                case "seleccionar_imputado_busqueda":
                    break;
                case "borrar_visitante_imputado":
                    try
                    {
                        if (SelectVisitanteInterno == null)
                            break;
                        if (SelectVisitanteInterno.OBJETO_PERSONA != null)
                            break;

                        if (await new Dialogos().ConfirmarEliminar("Entrega de Credencial", "Esta seguro que desea " + DeshabilitarVisitaAutorizada.ToLower() + " a esta persona de la lista de visita autorizada?") == 1)
                        {
                            if (new cVisitaAutorizada().Actualizar(new VISITA_AUTORIZADA
                            {
                                DOMICILIO_CALLE = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.DOMICILIO_CALLE,
                                DOMICILIO_CODIGO_POSTAL = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.DOMICILIO_CODIGO_POSTAL,
                                DOMICILIO_NUM_EXT = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.DOMICILIO_NUM_EXT,
                                DOMICILIO_NUM_INT = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.DOMICILIO_NUM_INT,
                                EDAD = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.EDAD,
                                ID_ANIO = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.ID_ANIO,
                                ID_CENTRO = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.ID_CENTRO,
                                ID_COLONIA = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.ID_COLONIA,
                                ID_ENTIDAD = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.ID_ENTIDAD,
                                ID_IMPUTADO = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.ID_IMPUTADO,
                                ID_INGRESO = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.ID_INGRESO,
                                ID_MUNICIPIO = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.ID_MUNICIPIO,
                                ID_PAIS = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.ID_PAIS,
                                ID_PARENTESCO = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.ID_PARENTESCO,
                                ID_TIPO_VISITA = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.ID_TIPO_VISITA,
                                ID_VISITA = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.ID_VISITA,
                                MATERNO = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.MATERNO,
                                NOMBRE = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.NOMBRE,
                                PATERNO = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.PATERNO,
                                SEXO = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.SEXO,
                                TELEFONO = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.TELEFONO,
                                ID_PERSONA = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.ID_PERSONA,
                                ESTATUS = DeshabilitarVisitaAutorizada == "Habilitar" ? 0 : 1,
                            }))
                            {
                                (new Dialogos()).ConfirmacionDialogo("Éxito!", "El visitante ha sido deshabilitado con éxito.");
                                GetDatosIngresoImputadoSeleccionado();
                                break;
                            }
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Hubo un problema al deshabilitar al visitante seleccionado.");
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al insertar un nuevo visitante.", ex);
                    }
                    break;
                case "insertar_nuevo_visitante_imputado":
                    try
                    {
                        if (ListVisitantesImputado.Where(wh => wh.OBJETO_VISITA_AUTORIZADA != null ? wh.OBJETO_VISITA_AUTORIZADA.ESTATUS == 0 : wh.OBJETO_PERSONA != null ?
                            (wh.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO.First(w => w.ID_CENTRO == SelectImputadoIngreso.ID_CENTRO && w.ID_ANIO == SelectImputadoIngreso.ID_ANIO &&
                                w.ID_IMPUTADO == SelectImputadoIngreso.ID_IMPUTADO && w.ID_INGRESO == SelectImputadoIngreso.ID_INGRESO).ID_ESTATUS_VISITA != Parametro.ID_ESTATUS_VISITA_CANCELADO &&
                                wh.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO.First(w => w.ID_CENTRO == SelectImputadoIngreso.ID_CENTRO && w.ID_ANIO == SelectImputadoIngreso.ID_ANIO &&
                                w.ID_IMPUTADO == SelectImputadoIngreso.ID_IMPUTADO && w.ID_INGRESO == SelectImputadoIngreso.ID_INGRESO).ID_ESTATUS_VISITA != Parametro.ID_ESTATUS_VISITA_SUSPENDIDO) : true).Count() < Parametro.LIMITE_VISITA_AUTORIZADA)
                        {
                            LimpiarCapturaVisita();
                            CapturarVisitanteVisible = CapturarVisitanteEnabled = ValidarEnabled = GeneralEnabled = true;
                            MenuGuardarEnabled = !PInsertar ? PEditar : true;
                            SelectSituacion = 11;
                            SituacionEnabled = BanderaEditar = false;
                            CodigoEnabled = true;
                            SelectPais = Parametro.PAIS;//82
                            SelectEntidad = Parametro.ESTADO;//2
                            SetValidacionesHojaVisita();
                            OnPropertyChanged();
                        }
                        else
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No puedes agregar más visitantes, ya se llego al limite permitido.");
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al insertar un nuevo visitante.", ex);
                    }
                    break;
                case "buscar_nip":
                    try
                    {
                        if (string.IsNullOrEmpty(TextNipBusqueda))
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "El campo de busqueda esta vacio.");
                            return;
                        }
                        ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();
                        if (string.IsNullOrEmpty(TextNipBusqueda)) return;
                        HuellaWindow.Hide();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {
                                var auxiliar = new List<ResultadoBusquedaBiometrico>();
                                ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();
                                if (SelectVisitante == null) return;
                                if (SelectVisitanteIngreso == null) return;
                                if (SelectVisitante.ID_PERSONA.HasValue ? SelectVisitante.ID_PERSONA.Value.Equals(int.Parse(TextNipBusqueda)) : false)
                                {
                                    var personabiometrico = SelectVisitante.OBJETO_PERSONA;
                                    var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                                    if (personabiometrico != null ? personabiometrico.PERSONA_BIOMETRICO.Any() : false)
                                        if (personabiometrico.PERSONA_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                                            FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(personabiometrico.PERSONA_BIOMETRICO.First(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).BIOMETRICO);
                                        else
                                            if (personabiometrico.PERSONA_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                                                FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(personabiometrico.PERSONA_BIOMETRICO.First(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).BIOMETRICO);
                                    auxiliar.Add(new ResultadoBusquedaBiometrico
                                    {
                                        AMaterno = string.IsNullOrEmpty(personabiometrico.MATERNO) ? string.Empty : personabiometrico.MATERNO.Trim(),
                                        APaterno = personabiometrico.PATERNO.Trim(),
                                        Expediente = personabiometrico.ID_PERSONA.ToString(),
                                        Foto = FotoBusquedaHuella,
                                        Imputado = null,
                                        NIP = personabiometrico.PERSONA_NIP.Any(a => a.ID_CENTRO == GlobalVar.gCentro) ?
                                            personabiometrico.PERSONA_NIP.First(f => f.ID_CENTRO == GlobalVar.gCentro).NIP.HasValue ?
                                                personabiometrico.PERSONA_NIP.First(f => f.ID_CENTRO == GlobalVar.gCentro).NIP.Value.ToString()
                                            : string.Empty
                                        : string.Empty,
                                        Nombre = personabiometrico.NOMBRE.Trim(),
                                        Persona = personabiometrico,
                                    });
                                }
                                ListResultado = auxiliar.Any() ? auxiliar.ToList() : new List<ResultadoBusquedaBiometrico>();
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar realizar la búsqueda por nip.", ex);
                            }
                        });
                        HuellaWindow.Show();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        break;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al hacer la comparacion por nip visitante.", ex);
                    }
                    break;
                case "borrar_menucontext_interno":
                    break;

                default: // buscar visitantes click enter
                    try
                    {
                        if (obj != null)
                        {
                            TextCodigo = null;
                            //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                            var textbox = obj as TextBox;
                            if (textbox != null)
                            {
                                switch (textbox.Name)
                                {
                                    case "NombreBuscar":
                                        TextNombre = textbox.Text;
                                        break;
                                    case "ApellidoPaternoBuscar":
                                        TextPaterno = textbox.Text;
                                        break;
                                    case "ApellidoMaternoBuscar":
                                        TextMaterno = textbox.Text;
                                        break;
                                    case "FolioBuscar":
                                        var n = 0;
                                        TextCodigo = int.TryParse(textbox.Text, out n) ? n : 0;
                                        break;
                                }
                            }
                            if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                            {
                                BuscarVisitaXIngreso();
                                CrearNuevoVisitanteEnabled = true;
                            }
                            if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                            {
                                BuscarVisita();
                                CrearNuevoVisitanteEnabled = false;
                            }
                            if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                            {
                                BuscarVisita();
                                CrearNuevoVisitanteEnabled = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al hacer la búsqueda de visitantes.", ex);
                    }
                    break;
                #endregion
            }

        }

        private void RB_Checked(object s, RoutedEventArgs e)
        {
            try
            {
                if (s is RadioButton)
                {
                    GafeteView.Hide();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    var rb = (RadioButton)s;
                    CrearGafete(GafeteView, rb);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al hacer la busqueda de visitantes.", ex);
            }
        }

        private async void AcompananteSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "seleccionar_buscar_acompanante":
                    try
                    {
                        if (SelectBuscarAcompanante == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un acompañante.");
                            return;
                        }
                        if (SelectParentescoAcompanante > 0)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar el parentesco.");
                            return;
                        }
                        if (ListAcompanantes.Where(w => w.ID_ACOMPANANTE == SelectBuscarAcompanante.ID_PERSONA).Any())
                            new Dialogos().ConfirmacionDialogo("Advertencia!", "Esta persona ya está seleccionada como acompañante.");
                        else if (new Fechas().CalculaEdad(SelectBuscarAcompanante.VISITANTE.PERSONA.FEC_NACIMIENTO) <= 18)
                            new Dialogos().ConfirmacionDialogo("Advertencia!", "No puedes seleccionar a un acompañante menor de edad.");
                        else
                        {
                            var respuesta = 1;
                            if (SelectBuscarAcompanante.VISITANTE.PERSONA.ID_TIPO_DISCAPACIDAD > 0)
                                respuesta = await new Dialogos().ConfirmarSalida("Advertencia!", "El usuario seleccionado está registrado como persona discapacitada. \nEsta seguro que desea seleccionarlo como acompañante?");
                            switch (respuesta)
                            {
                                case 0: //negativo
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.INGRESA_ACOMPANIANTE);
                                    break;
                                case 1: //afirmativo
                                    var acompanante = new ACOMPANANTE();
                                    acompanante.ID_VISITANTE = 0;
                                    if (SelectVisitante.OBJETO_PERSONA != null)
                                        acompanante.ID_VISITANTE = SelectVisitante.ID_PERSONA.Value;
                                    acompanante.VIS_ID_CENTRO = SelectVisitante.ID_CENTRO;
                                    acompanante.VIS_ID_ANIO = SelectVisitante.ID_ANIO;
                                    acompanante.VIS_ID_IMPUTADO = SelectVisitante.ID_IMPUTADO;
                                    acompanante.VIS_ID_INGRESO = SelectVisitante.INGRESO.ID_INGRESO;
                                    acompanante.ID_ACOMPANANTE = SelectBuscarAcompanante.ID_PERSONA;
                                    acompanante.ACO_ID_ANIO = SelectBuscarAcompanante.ID_ANIO;
                                    acompanante.ACO_ID_CENTRO = SelectBuscarAcompanante.ID_CENTRO;
                                    acompanante.ACO_ID_IMPUTADO = SelectBuscarAcompanante.ID_IMPUTADO;
                                    acompanante.ACO_ID_INGRESO = SelectBuscarAcompanante.ID_INGRESO;
                                    acompanante.FEC_REGISTRO = Fechas.GetFechaDateServer;
                                    acompanante.ID_ACOMPANANTE_RELACION = SelectParentescoAcompanante;
                                    acompanante.VISITANTE_INGRESO = new VISITANTE_INGRESO();
                                    acompanante.VISITANTE_INGRESO.ESTATUS_VISITA = ListSituacion.Where(w => w.ID_ESTATUS_VISITA == SelectBuscarAcompanante.ID_ESTATUS_VISITA).FirstOrDefault();
                                    acompanante.VISITANTE_INGRESO.VISITANTE = new VISITANTE();
                                    acompanante.VISITANTE_INGRESO.VISITANTE.PERSONA = new SSP.Servidor.PERSONA();
                                    acompanante.VISITANTE_INGRESO.VISITANTE.PERSONA.PATERNO = SelectBuscarAcompanante.VISITANTE.PERSONA.PATERNO;
                                    acompanante.VISITANTE_INGRESO.VISITANTE.PERSONA.MATERNO = SelectBuscarAcompanante.VISITANTE.PERSONA.MATERNO;
                                    acompanante.VISITANTE_INGRESO.VISITANTE.PERSONA.NOMBRE = SelectBuscarAcompanante.VISITANTE.PERSONA.NOMBRE;
                                    acompanante.TIPO_REFERENCIA = ListTipoRelacion.Where(w => w.ID_TIPO_REFERENCIA == SelectParentescoAcompanante).FirstOrDefault();
                                    ListAcompanantes.Add(acompanante);
                                    ListBuscarAcompanantes.RemoveAt(ListBuscarAcompanantes.IndexOf(SelectBuscarAcompanante));
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.INGRESA_ACOMPANIANTE);
                                    break;
                                case 2://cancelar
                                    break;

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del acompañante seleccionado.", ex);
                    }
                    break;
                case "cancelar_buscar_acompanante":
                    if (SelectAcompananteAuxiliar != null)
                    {
                        var list = new List<ACOMPANANTE>();
                        list.Add(SelectAcompananteAuxiliar);
                        SelectAcompanante = list.FirstOrDefault();
                    }
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.INGRESA_ACOMPANIANTE);
                    break;
                case "buscar_acompanante":

                    break;
                default:
                    if (obj is TextBox)
                    {
                        var textbox = (TextBox)obj;
                        switch (textbox.Name)
                        {
                            case "NombreAcompananteBuscar":
                                TextNombreAcompanante = textbox.Text;
                                break;
                            case "PaternoAcompananteBuscar":
                                TextPaternoAcompanante = textbox.Text;
                                break;
                            case "MaternoAcompananteBuscar":
                                TextMaternoAcompanante = textbox.Text;
                                break;
                        }
                    }
                    if (string.IsNullOrEmpty(TextNombreAcompanante))
                        TextNombreAcompanante = string.Empty;
                    if (string.IsNullOrEmpty(TextPaternoAcompanante))
                        TextPaternoAcompanante = string.Empty;
                    if (string.IsNullOrEmpty(TextMaternoAcompanante))
                        TextMaternoAcompanante = string.Empty;
                    break;
            }
        }

        private void EnterExpediente(Object obj)
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No tienes permisos para hacer consultas.");
                    return;
                }
                if (obj != null)
                {
                    if (obj.ToString() != "buscar_visible")
                    {
                        //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                        var textbox = obj as TextBox;
                        if (textbox != null)
                        {
                            switch (textbox.Name)
                            {
                                case "NombreBuscar":
                                case "NombreBuscar2":
                                    NombreBuscar = NombreD = textbox.Text;
                                    AnioBuscar = !string.IsNullOrEmpty(AnioD) ? int.Parse(AnioD) : new Nullable<int>();
                                    FolioBuscar = !string.IsNullOrEmpty(FolioD) ? int.Parse(FolioD) : new Nullable<int>();
                                    ApellidoMaternoBuscar = MaternoD;
                                    ApellidoPaternoBuscar = PaternoD;
                                    break;
                                case "ApellidoPaternoBuscar":
                                case "ApellidoPaternoBuscar2":
                                    ApellidoPaternoBuscar = PaternoD = textbox.Text;
                                    AnioBuscar = !string.IsNullOrEmpty(AnioD) ? int.Parse(AnioD) : new Nullable<int>();
                                    FolioBuscar = !string.IsNullOrEmpty(FolioD) ? int.Parse(FolioD) : new Nullable<int>();
                                    ApellidoMaternoBuscar = MaternoD;
                                    NombreBuscar = NombreD;
                                    break;
                                case "ApellidoMaternoBuscar":
                                case "ApellidoMaternoBuscar2":
                                    ApellidoMaternoBuscar = MaternoD = textbox.Text;
                                    AnioBuscar = !string.IsNullOrEmpty(AnioD) ? int.Parse(AnioD) : new Nullable<int>();
                                    FolioBuscar = !string.IsNullOrEmpty(FolioD) ? int.Parse(FolioD) : new Nullable<int>();
                                    ApellidoPaternoBuscar = PaternoD;
                                    NombreBuscar = NombreD;
                                    break;
                                case "FolioBuscar2":
                                case "FolioBuscar":
                                    FolioBuscar = !string.IsNullOrEmpty(textbox.Text) ? int.Parse(textbox.Text) : new Nullable<int>();
                                    AnioBuscar = !string.IsNullOrEmpty(AnioD) ? int.Parse(AnioD) : new Nullable<int>();
                                    ApellidoMaternoBuscar = MaternoD;
                                    NombreBuscar = NombreD;
                                    ApellidoPaternoBuscar = PaternoD;
                                    break;
                                case "AnioBuscar":
                                case "AnioBuscar2":
                                    AnioBuscar = !string.IsNullOrEmpty(textbox.Text) ? int.Parse(textbox.Text) : new Nullable<int>();
                                    FolioBuscar = !string.IsNullOrEmpty(FolioD) ? int.Parse(FolioD) : new Nullable<int>();
                                    ApellidoMaternoBuscar = MaternoD;
                                    NombreBuscar = NombreD;
                                    ApellidoPaternoBuscar = PaternoD;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        NombreBuscar = NombreD;
                        ApellidoPaternoBuscar = PaternoD;
                        ApellidoMaternoBuscar = MaternoD;
                    }
                    var ing = SelectIngreso;
                    SelectIngresoAuxiliar = ing;
                    var exp = SelectExpediente;
                    SelectExpedienteAuxiliar = exp;
                    BuscarImputado(NombreD, PaternoD, MaternoD);
                    if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    {
                        SelectParentescoNuevoImputado = -1;
                        SelectEstatusRelacionEnabled = false;
                        SelectParentescoIngresoEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda de imputados.", ex);
            }
        }

        private async void EnterBuscar(Object obj)
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No tienes permisos para hacer consultas.");
                    return;
                }
                if (obj != null)
                {
                    if (obj.ToString() != "buscar_visible")
                    {
                        //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                        var textbox = obj as TextBox;
                        if (textbox != null)
                        {
                            switch (textbox.Name)
                            {
                                case "NombreBuscar":
                                    NombreD = NombreBuscar = textbox.Text;
                                    AnioD = AnioBuscar.HasValue ? AnioBuscar.ToString() : string.Empty;
                                    FolioD = FolioBuscar.HasValue ? FolioBuscar.ToString() : string.Empty;
                                    MaternoD = ApellidoMaternoBuscar;
                                    PaternoD = ApellidoPaternoBuscar;
                                    break;
                                case "ApellidoPaternoBuscar":
                                    PaternoD = ApellidoPaternoBuscar = textbox.Text;
                                    AnioD = AnioBuscar.HasValue ? AnioBuscar.ToString() : string.Empty;
                                    FolioD = FolioBuscar.HasValue ? FolioBuscar.ToString() : string.Empty;
                                    MaternoD = ApellidoMaternoBuscar;
                                    NombreD = NombreBuscar;
                                    break;
                                case "ApellidoMaternoBuscar":
                                    MaternoD = ApellidoMaternoBuscar = textbox.Text;
                                    AnioD = AnioBuscar.HasValue ? AnioBuscar.ToString() : string.Empty;
                                    FolioD = FolioBuscar.HasValue ? FolioBuscar.ToString() : string.Empty;
                                    PaternoD = ApellidoPaternoBuscar;
                                    NombreD = NombreBuscar;
                                    break;
                                case "FolioBuscar":
                                    FolioD = textbox.Text;
                                    FolioBuscar = int.Parse(textbox.Text);
                                    AnioD = AnioBuscar.HasValue ? AnioBuscar.ToString() : string.Empty;
                                    MaternoD = ApellidoMaternoBuscar;
                                    NombreD = NombreBuscar;
                                    PaternoD = ApellidoPaternoBuscar;
                                    break;
                                case "AnioBuscar":
                                    AnioD = textbox.Text;
                                    AnioBuscar = int.Parse(textbox.Text);
                                    FolioD = FolioBuscar.HasValue ? FolioBuscar.ToString() : string.Empty;
                                    MaternoD = ApellidoMaternoBuscar;
                                    NombreD = NombreBuscar;
                                    PaternoD = ApellidoPaternoBuscar;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        NombreBuscar = NombreD;
                        ApellidoPaternoBuscar = PaternoD;
                        ApellidoMaternoBuscar = MaternoD;
                    }
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count == 1)
                    {
                        ImagenIngreso = new Imagenes().getImagenPerson();
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        var ingreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        //if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).Any(a => !EstatusInactivos.Contains(a.ID_ESTATUS_ADMINISTRATIVO)))
                        if (EstatusInactivos.Contains(ingreso.ID_ESTATUS_ADMINISTRATIVO))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                            return;
                        }
                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).First(w => !EstatusInactivos.Contains(w.ID_ESTATUS_ADMINISTRATIVO)).ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no pertenece a este centro.");
                            return;
                        }
                        SelectImputadoIngreso = ListExpediente[0].INGRESO.First(w => !EstatusInactivos.Contains(w.ID_ESTATUS_ADMINISTRATIVO));
                    }
                    else if (ListExpediente.Count == 0 && (!string.IsNullOrEmpty(NombreBuscar) || !string.IsNullOrEmpty(ApellidoPaternoBuscar) || !string.IsNullOrEmpty(ApellidoMaternoBuscar)))
                        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun imputado con esos datos.");
                    EmptyExpedienteVisible = !(ListExpediente.Count > 0);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda de imputados.", ex);
            }
        }

        private async void BuscarImputado(string nombre, string paterno, string materno)
        {
            try
            {
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count > 0)
                {
                    SelectExpediente = ListExpediente.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO).FirstOrDefault();
                }
                if (ListExpediente.Count == 1)
                {
                    var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                    if (EstatusInactivos.Any(a => a.HasValue ? a.Value == ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO : false))
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                        }));
                        return;
                    }
                    var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                    if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().TRASLADO_DETALLE.Any(w => (w.ID_ESTATUS != "CA" ? w.TRASLADO.ORIGEN_TIPO != "F" : false) && w.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ANIO.ToString()
                            + "/" + ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningun cambio de información.");
                        return;
                    }
                    if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no pertenece a este centro.");
                        return;
                    }
                    if (TabsVisita.PADRON_DE_VISITAS != SelectedTab)
                    {
                        SelectImputadoIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        SelectEstatusRelacionEnabled = false;
                        SelectParentescoIngresoEnabled = true;
                        GetDatosIngresoImputadoSeleccionado();
                        ContextMenuNuevoVisitanteEnabled = ProgramacionVisitasMenuEnabled = true;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        return;
                    }
                }
                else if (ListExpediente.Count == 0)
                {
                    if (!string.IsNullOrEmpty(NombreD) || !string.IsNullOrEmpty(PaternoD) || !string.IsNullOrEmpty(MaternoD))
                        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun imputado con esos datos.");
                }

                if (ListExpediente.Count > 0)//Empty row
                    EmptyExpedienteVisible = false;
                else
                    EmptyExpedienteVisible = true;
                ContextMenuNuevoVisitanteEnabled = false;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al hacer la búsqueda.", ex);
            }
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();

                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;

                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al querer guardar.", ex);
                return new List<IMPUTADO>();
            }
        }

        private void ClickEnter(Object obj)
        {
            if (!PConsultar)
            {
                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No tienes permisos para hacer consultas.");
                return;
            }
            base.ClearRules();
            if (obj != null)
            {
                //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                var textbox = obj as TextBox;
                if (textbox != null)
                {
                    switch (textbox.Name)
                    {
                        case "NombreBuscar":
                            TextNombre = textbox.Text;
                            break;
                        case "ApellidoPaternoBuscar":
                            TextPaterno = textbox.Text;
                            break;
                        case "ApellidoMaternoBuscar":
                            TextMaterno = textbox.Text;
                            break;
                        case "CodigoBuscar":
                            var n = 0;
                            TextCodigo = int.TryParse(textbox.Text, out n) ? n : 0;
                            break;
                    }
                }

            }
            if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
            {
                BuscarVisitaXIngreso();
                CrearNuevoVisitanteEnabled = true;
            }
            if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
            {
                BuscarVisita();
                CrearNuevoVisitanteEnabled = IsDetalleInternosEnable = false;
            }
            if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
            {
                BuscarVisita();
                CrearNuevoVisitanteEnabled = false;
            }
        }

        private async void BuscarVisita()
        {
            try
            {
                long? codigo = null;
                if (TextCodigo != null)
                    codigo = TextCodigo;
                if (string.IsNullOrEmpty(TextNombre))
                    TextNombre = string.Empty;
                if (string.IsNullOrEmpty(TextPaterno))
                    TextPaterno = string.Empty;
                if (string.IsNullOrEmpty(TextMaterno))
                    TextMaterno = string.Empty;
                var lista = new List<PERSONAVISITAAUXILIAR>();

                if (TextCodigo.HasValue)
                {
                    #region PERSONA
                    var persona = await StaticSourcesViewModel.CargarDatosAsync<SSP.Servidor.PERSONA>(() => new cPersona().Obtener(TextCodigo.Value).FirstOrDefault());
                    if (persona != null)
                    {
                        try
                        {
                            #region Validaciones
                            if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                            {
                                if (persona.ID_TIPO_DISCAPACIDAD != null ? persona.TIPO_DISCAPACIDAD.HUELLA != "S" : false)
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Validación", "Favor de autentificar al visitante con la huella digital.");
                                    return;
                                }
                                else
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Validación", "Favor de autentificar al visitante con la huella digital.");
                                    return;
                                }
                            }
                            #endregion

                            if (PopUpsViewModels.VisibleBuscarVisitaExistente == Visibility.Visible)
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                            SelectVisitante = ConvertPersonaToAuxiliar(persona);
                            GeneralEnabled = ValidarEnabled = BanderaEditar = ContextMenuEnabled = FotoHuellaEnabled = true;
                            CodigoEnabled = true;
                            SelectEstatusRelacionEnabled = false;
                            SelectParentescoIngresoEnabled = true;
                            SetValidacionesGenerales();
                            GetDatosVisitanteSeleccionadoPadron();
                            #region Nota Tecnica
                            NotaTecnica = persona.NOTA_TECNICA;
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda de visitantes.", ex);
                        }
                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Error", "No se encontro ninguna persona con ese numero de visitante.");
                        TextCodigo = new Nullable<long>();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                    }
                    #endregion
                }
                else
                {

                    #region PERSONAS
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        var personas = new cPersona().ObtenerTodos(TextNombre, TextPaterno, TextMaterno, codigo.HasValue ? (int)codigo.Value : new Nullable<int>(), 1);
                        lista.AddRange(personas.ToList().Select(s => ConvertPersonaToAuxiliar(s)));
                    });
                    #endregion

                    ListVisitantes = new ObservableCollection<PERSONAVISITAAUXILIAR>(lista);
                    if (ListVisitantes.Count > 0)
                    {
                        EmptyBuscarRelacionInternoVisible = false;
                        SeleccionarVisitaExistente = true;
                    }
                    else
                    {
                        EmptyBuscarRelacionInternoVisible = true;
                        SeleccionarVisitaExistente = false;
                    }
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda de visitantes.", ex);
            }
        }

        private async void BuscarVisitaXIngreso()
        {
            try
            {
                long? codigo = null;
                if (TextCodigo != null)
                    codigo = TextCodigo;
                if (string.IsNullOrEmpty(TextNombre))
                    TextNombre = string.Empty;
                if (string.IsNullOrEmpty(TextPaterno))
                    TextPaterno = string.Empty;
                if (string.IsNullOrEmpty(TextMaterno))
                    TextMaterno = string.Empty;
                var lista = new List<PERSONAVISITAAUXILIAR>();
                ListVisitantes = new ObservableCollection<PERSONAVISITAAUXILIAR>();

                #region PERSONAS
                var personas = await StaticSourcesViewModel.CargarDatosAsync<IQueryable<SSP.Servidor.PERSONA>>(() =>
                    new cPersona().ObtenerPersonasVisitantes(TextNombre, TextPaterno, TextMaterno, codigo));
                lista.AddRange(personas.Select(s => ConvertPersonaToAuxiliar(s)));
                #endregion

                ListVisitantes = new ObservableCollection<PERSONAVISITAAUXILIAR>(lista);
                if (ListVisitantes.Count > 0)
                {
                    EmptyBuscarRelacionInternoVisible = false;
                    SeleccionarVisitaExistente = true;
                }
                else
                {
                    EmptyBuscarRelacionInternoVisible = true;
                    SeleccionarVisitaExistente = false;
                }
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda de visitantes.", ex);
            }
        }

        private void CrearGafete(GafetesView view, RadioButton rb)
        {
            try
            {
                #region GAFETE
                List<Gafete> gafetes = new List<Gafete>();
                var gaf = new Gafete();
                gaf.ImagenVisitante = SelectVisitante.OBJETO_PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                gaf.NombreVisitante = string.Format("{0} {1} {2}",
                    !string.IsNullOrEmpty(SelectVisitante.NOMBRE) ? SelectVisitante.NOMBRE.Trim() : string.Empty,
                    !string.IsNullOrEmpty(SelectVisitante.PATERNO) ? SelectVisitante.PATERNO.Trim() : string.Empty,
                    !string.IsNullOrEmpty(SelectVisitante.MATERNO) ? SelectVisitante.MATERNO.Trim() : string.Empty);
                gaf.NumeroVisita = SelectVisitante.ID_PERSONA.ToString();
                gaf.Fecha = Fechas.GetFechaDateServer.ToString("dd DE MMMM DE yyyy").ToUpper();
                gaf.Usuario = GlobalVar.gUsr;
                gaf.TipoVisita = SelectVisitanteIngreso.TIPO_VISITANTE.DESCR.Trim();
                gaf.Parentesco = SelectVisitanteIngreso.TIPO_REFERENCIA.DESCR.Trim();
                gaf.Observacion = SelectVisitanteIngreso.OBSERVACION == null ? string.Empty : SelectVisitanteIngreso.OBSERVACION.Trim();
                var ingreso = SelectImputadoIngreso;
                gaf.NombreInterno = string.Format("{0} {1} {2}",
                    !string.IsNullOrEmpty(ingreso.IMPUTADO.NOMBRE) ? ingreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
                    !string.IsNullOrEmpty(ingreso.IMPUTADO.PATERNO) ? ingreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                    !string.IsNullOrEmpty(ingreso.IMPUTADO.MATERNO) ? ingreso.IMPUTADO.MATERNO.Trim() : string.Empty);
                gaf.Centro = ingreso.CENTRO.DESCR.Trim();
                gaf.NombreDirectorCentro = string.IsNullOrEmpty(ingreso.CENTRO.DIRECTOR) ? string.Empty : ingreso.CENTRO.DIRECTOR.Trim();
                gaf.ImagenImputado = ingreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                        ingreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                            ingreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                                ingreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                                    new Imagenes().getImagenPerson();
                gaf.Ciudad = ingreso.CENTRO.MUNICIPIO.MUNICIPIO1.Trim();
                if (rb.Name == "rbAcompanante")
                {
                    if (SelectVisitanteIngreso.ACOMPANANTE.Count > 0)
                    {
                        var visitantes = SelectVisitanteIngreso.ACOMPANANTE.ToList();
                        for (int i = 0; i < visitantes.Count; i++)
                        {
                            if (i == 0)
                            {
                                if (visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                {
                                    gaf.ImagenMenor1 = visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                }
                                gaf.NombreMenor1 = string.Format("{0} {1} {2}",
                                    !string.IsNullOrEmpty(visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.NOMBRE) ? visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.NOMBRE.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.PATERNO) ? visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.PATERNO.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.MATERNO) ? visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.MATERNO.Trim() : string.Empty);
                            }
                            if (i == 1)
                            {
                                if (visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                {
                                    gaf.ImagenMenor2 = visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                }
                                gaf.NombreMenor2 = string.Format("{0} {1} {2}",
                                    !string.IsNullOrEmpty(visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.NOMBRE) ? visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.NOMBRE.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.PATERNO) ? visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.PATERNO.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.MATERNO) ? visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.MATERNO.Trim() : string.Empty);
                            }
                            if (i == 2)
                            {
                                if (visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                {
                                    gaf.ImagenMenor3 = visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                }
                                gaf.NombreMenor3 = string.Format("{0} {1} {2}",
                                    !string.IsNullOrEmpty(visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.NOMBRE) ? visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.NOMBRE.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.PATERNO) ? visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.PATERNO.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.MATERNO) ? visitantes[i].VISITANTE_INGRESO1.VISITANTE.PERSONA.MATERNO.Trim() : string.Empty);
                            }
                        }
                        view.GafetesReport.LocalReport.ReportPath = "Reportes/GafeteAcompanante.rdlc";
                    }
                }
                if (rb.Name == "rbMenor")
                {
                    if (SelectVisitanteIngreso.ACOMPANANTE1.Count > 0)
                    {
                        if (new Fechas().CalculaEdad(SelectVisitanteIngreso.VISITANTE.PERSONA.FEC_NACIMIENTO) < Parametro.MAYORIA_EDAD &&
                            new Fechas().CalculaEdad(SelectVisitanteIngreso.VISITANTE.PERSONA.FEC_NACIMIENTO) > Parametro.EDAD_MENOR_M)
                        {
                            var acompa = SelectVisitanteIngreso.ACOMPANANTE1.Where(w => w.ACO_ID_ANIO == ingreso.ID_ANIO && w.ACO_ID_CENTRO == ingreso.ID_CENTRO &&
                                w.ACO_ID_IMPUTADO == ingreso.ID_IMPUTADO && w.ACO_ID_INGRESO == ingreso.ID_INGRESO).FirstOrDefault().VISITANTE_INGRESO.VISITANTE.PERSONA;
                            gaf.NombreAcompanante = acompa.PATERNO.Trim() + " " + acompa.MATERNO.Trim() + " " + acompa.NOMBRE.Trim();
                            view.GafetesReport.LocalReport.ReportPath = "Reportes/GafeteMenor.rdlc";
                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Credencial no disponible");
                            return;
                        }
                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El menor no tiene registrado ningun acompañante.");
                        return;
                    }
                }
                if (rb.Name == "rbFamiliar")
                {
                    if (SelectVisitante.OBJETO_PERSONA.TIPO_DISCAPACIDAD != null ? SelectVisitante.OBJETO_PERSONA.TIPO_DISCAPACIDAD.HUELLA == "S" : false)
                        view.GafetesReport.LocalReport.ReportPath = "Reportes/GafeteDiscapacitado.rdlc";
                    else
                        view.GafetesReport.LocalReport.ReportPath = "Reportes/Gafete.rdlc";
                }
                if (rb.Name == "rbDiscapacitado")
                {
                    gaf.Observacion = SelectVisitante.OBJETO_PERSONA.TIPO_DISCAPACIDAD.DESCR;
                    view.GafetesReport.LocalReport.ReportPath = "Reportes/GafeteDiscapacitado.rdlc";
                }

                view.GafetesReport.ShowExportButton = false;
                gaf.LogoBackground = new Imagenes().getLogoBackground();
                gafetes.Add(gaf);
                view.GafetesReport.LocalReport.DataSources.Clear();
                Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds.Name = "DataSet1";
                rds.Value = gafetes;
                view.GafetesReport.LocalReport.DataSources.Add(rds);
                view.GafetesReport.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                view.GafetesReport.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                view.GafetesReport.ZoomPercent = 100;
                view.GafetesReport.RefreshReport();

                view.Owner = PopUpsViewModels.MainWindow;
                view.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                view.ShowDialog();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar el gafete del visitante.", ex);
            }
        }

        private bool ValidacionesGafete()
        {
            try
            {
                #region VALIDACIONES
                if (SelectVisitante.OBJETO_PERSONA == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Error en base de datos.");
                    return false;
                }
                if (SelectVisitante.OBJETO_PERSONA.VISITANTE == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar a una persona ya credencializada.");
                    return false;
                }
                if (SelectVisitanteIngreso == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Error en la relacion del visitante con el imputado.");
                    return false;
                }
                if (SelectVisitanteIngreso.VISITANTE == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes guardar la relacion del imputado y el visitante antes de imprimir el gafete.");
                    return false;
                }
                if (SelectVisitanteIngreso.TIPO_VISITANTE == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar un tipo de visitante con el imputado seleccionado.");
                    return false;
                }
                if (SelectVisitanteIngreso.TIPO_REFERENCIA == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar un tipo de relacion con el imputado seleccionado.");
                    return false;
                }
                if (!SelectVisitante.OBJETO_PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El visitante no tiene foto registrada.");
                    return false;
                }
                if (SelectImputadoIngreso.INGRESO_BIOMETRICO.Count <= 0)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El imputado no tiene fotos registradas.");
                    return false;
                }
                if (new Fechas().CalculaEdad(SelectVisitanteIngreso.VISITANTE.PERSONA.FEC_NACIMIENTO) <= Parametro.EDAD_MENOR_M)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El visitante tiene " + new Fechas().CalculaEdad(SelectVisitanteIngreso.VISITANTE.PERSONA.FEC_NACIMIENTO)
                        + " años y no puede tener gafete.");
                    return false;
                }
                if (!SelectImputadoIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El visitante no tiene foto de frente registrada.");
                    return false;
                }

                if (SelectVisitante.OBJETO_PERSONA.ID_TIPO_DISCAPACIDAD != null || SelectVisitante.OBJETO_PERSONA.ID_TIPO_DISCAPACIDAD != 0)
                {
                    if (SelectVisitante.OBJETO_PERSONA.TIPO_DISCAPACIDAD.HUELLA == "S" ?
                        !SelectVisitante.OBJETO_PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO >= (short)enumTipoBiometrico.PULGAR_DERECHO && w.ID_TIPO_BIOMETRICO <= (short)enumTipoBiometrico.MEDIO_IZQUIERDO).Any()
                    : false)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El visitante no ha capturado sus huellas.");
                        return false;
                    }
                }
                else
                    if (!SelectVisitante.OBJETO_PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO >= (short)enumTipoBiometrico.PULGAR_DERECHO && w.ID_TIPO_BIOMETRICO <= (short)enumTipoBiometrico.MEDIO_IZQUIERDO).Any())
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El visitante no ha capturado sus huellas.");
                        return false;
                    }
                var tipoVisita = (short)0;
                if (SelectVisitanteIngreso.ID_TIPO_VISITANTE == Parametro.ID_TIPO_VISITANTE_ORDINARIO || SelectVisitanteIngreso.ID_TIPO_VISITANTE == Parametro.ID_TIPO_VISITANTE_DISCAPACITADO)
                {
                    if (SelectVisitanteIngreso.VISITA_DOCUMENTO.Count > 0)
                    {
                        var doctosFamiliares = Parametro.DOCTOS_NECESARIOS_FAMILIAR;
                        tipoVisita = short.Parse(doctosFamiliares.FirstOrDefault().Split('-')[0]);
                        var tipoDocto = (short)0;
                        foreach (var itm in doctosFamiliares)
                        {
                            tipoDocto = short.Parse(itm.Split('-')[1]);
                            if (!SelectVisitanteIngreso.VISITA_DOCUMENTO.Where(w => w.ID_TIPO_VISITA == tipoVisita && w.ID_TIPO_DOCUMENTO == tipoDocto).Any())
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El visitante no cuenta con los documentos necesarios para imprimir el gafete.");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El visitante no cuenta con los documentos necesarios para imprimir el gafete.");
                        return false;
                    }
                }
                if (SelectVisitanteIngreso.ID_TIPO_VISITANTE == Parametro.ID_TIPO_VISITANTE_INTIMA)
                {
                    if (SelectVisitanteIngreso.VISITA_DOCUMENTO.Count > 0)
                    {
                        var doctosIntima = Parametro.DOCTOS_NECESARIOS_INTIMA;
                        var tipoDocto = (short)0;
                        tipoVisita = short.Parse(doctosIntima.FirstOrDefault().Split('-')[0]);
                        foreach (var itm in doctosIntima)
                        {
                            tipoDocto = short.Parse(itm.Split('-')[1]);
                            if (!SelectVisitanteIngreso.VISITA_DOCUMENTO.Where(w => w.ID_TIPO_VISITA == tipoVisita && w.ID_TIPO_DOCUMENTO == tipoDocto).Any())
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El visitante no cuenta con los documentos necesarios para ser VISITA INTIMA.");
                                return false;
                            }
                        }
                        doctosIntima = Parametro.DOCTOS_FAMILIAR_INTIMA;
                        tipoVisita = short.Parse(doctosIntima.FirstOrDefault().Split('-')[0]);
                        foreach (var itm in doctosIntima)
                        {
                            tipoDocto = short.Parse(itm.Split('-')[1]);
                            if (!SelectVisitanteIngreso.VISITA_DOCUMENTO.Where(w => w.ID_TIPO_VISITA == tipoVisita && w.ID_TIPO_DOCUMENTO == tipoDocto).Any())
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El visitante no cuenta con los documentos necesarios para imprimir el gafete.");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El visitante no cuenta con los documentos necesarios para ser VISITA INTIMA.");
                        return false;
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar los datos del visitante.", ex);
            }
            return true;
        }

        private void LlenarCamposVisitanteInterno(PERSONAVISITAAUXILIAR value)
        {
            TextCalle = TextNombre = TextPaterno = TextMaterno = TextCurp = TextRfc = TextCorreo = /*TextNip =*/ TextNumInt = string.Empty;
            TextCodigo = TextNumExt = TextCodigoPostal = new Nullable<int>();
            TextTelefono = null;
            FotoVisita = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
            FechaNacimiento = new Nullable<DateTime>();
            TextEdad = null;
            SelectAccesoUnico = false;
            SelectSexo = "S";
            SelectPais = Parametro.PAIS;//82;
            ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
            SelectEntidad = 2;
            ListMunicipio = new ObservableCollection<MUNICIPIO>();
            ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
            SelectMunicipio = SelectTipoVisitante = SelectSituacion = -1;
            ListColonia = new ObservableCollection<COLONIA>();
            ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
            SelectColonia = -1;
            SelectParentesco = -1;
            CodigoEnabled = true;
            ValidarEnabled = GeneralEnabled = EntidadEnabled = MunicipioEnabled = ColoniaEnabled = false;
            TextFechaUltimaModificacion = TextFechaAlta = Fechas.GetFechaDateServer.ToString("dd/MM/yyyy");
            TextNombre = value.NOMBRE;
            TextPaterno = value.PATERNO;
            TextMaterno = value.MATERNO;
            TextCurp = value.CURP;
            TextRfc = value.RFC;
            TextTelefono = value.OBJETO_PERSONA != null ? value.OBJETO_PERSONA.TELEFONO.ToString() : string.Empty;
            TextEdad = value.EDAD;
            TextCorreo = value.OBJETO_PERSONA != null ? value.OBJETO_PERSONA.CORREO_ELECTRONICO : string.Empty;
            TextCalle = value.OBJETO_PERSONA != null ? value.OBJETO_PERSONA.DOMICILIO_CALLE : value.OBJETO_VISITA_AUTORIZADA != null ? value.OBJETO_VISITA_AUTORIZADA.DOMICILIO_CALLE : string.Empty;
            TextNumExt = value.OBJETO_PERSONA != null ? value.OBJETO_PERSONA.DOMICILIO_NUM_EXT : value.OBJETO_VISITA_AUTORIZADA != null ? value.OBJETO_VISITA_AUTORIZADA.DOMICILIO_NUM_EXT : new Nullable<int>();
            TextNumInt = value.OBJETO_PERSONA != null ? value.OBJETO_PERSONA.DOMICILIO_NUM_INT : value.OBJETO_VISITA_AUTORIZADA != null ? value.OBJETO_VISITA_AUTORIZADA.DOMICILIO_NUM_INT : string.Empty;
            TextCodigoPostal = value.OBJETO_PERSONA != null ? value.OBJETO_PERSONA.DOMICILIO_CODIGO_POSTAL : value.OBJETO_VISITA_AUTORIZADA != null ? value.OBJETO_VISITA_AUTORIZADA.DOMICILIO_CODIGO_POSTAL : new Nullable<int>();
            FechaNacimiento = value.OBJETO_PERSONA != null ? value.OBJETO_PERSONA.FEC_NACIMIENTO : new Nullable<DateTime>();
            SelectSexo = value.OBJETO_PERSONA != null ? value.OBJETO_PERSONA.SEXO : value.OBJETO_VISITA_AUTORIZADA != null ? value.OBJETO_VISITA_AUTORIZADA.SEXO : string.Empty;
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                SelectPais = value.OBJETO_PERSONA != null ? value.OBJETO_PERSONA.ID_PAIS : value.OBJETO_VISITA_AUTORIZADA != null ? value.OBJETO_VISITA_AUTORIZADA.ID_PAIS : new Nullable<short>();
                ListEntidad = new ObservableCollection<ENTIDAD>(ListEntidadesAuxiliares.Where(w => w.ID_PAIS_NAC == SelectPais).OrderBy(o => o.DESCR));
                ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                SelectEntidad = value.OBJETO_PERSONA != null ? value.OBJETO_PERSONA.ID_ENTIDAD : value.OBJETO_VISITA_AUTORIZADA != null ? value.OBJETO_VISITA_AUTORIZADA.ID_ENTIDAD : new Nullable<short>();
                ListMunicipio = new ObservableCollection<MUNICIPIO>(ListMunicipiosAuxiliares.Where(w => w.ID_ENTIDAD == SelectEntidad).OrderBy(o => o.MUNICIPIO1));
                ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                SelectMunicipio = value.OBJETO_PERSONA != null ? value.OBJETO_PERSONA.ID_MUNICIPIO : value.OBJETO_VISITA_AUTORIZADA != null ? value.OBJETO_VISITA_AUTORIZADA.ID_MUNICIPIO : new Nullable<short>();
                ListColonia = new ObservableCollection<COLONIA>(ListColoniasAuxiliares.Where(w => w.ID_ENTIDAD == SelectEntidad && w.ID_MUNICIPIO == SelectMunicipio).OrderBy(o => o.DESCR));
                ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                SelectColonia = value.OBJETO_PERSONA != null ? value.OBJETO_PERSONA.ID_COLONIA : value.OBJETO_VISITA_AUTORIZADA != null ? value.OBJETO_VISITA_AUTORIZADA.ID_COLONIA : new Nullable<short>();
            }));
            SelectParentesco = value.OBJETO_VISITA_AUTORIZADA != null ? value.OBJETO_VISITA_AUTORIZADA.ID_PARENTESCO.Value : (short)-1;
            CapturarVisitanteVisible = CapturarVisitanteEnabled = ValidarEnabled = GeneralEnabled = true;
            SetValidacionesHojaVisita();
            OnPropertyChanged();
        }

        public PERSONAVISITAAUXILIAR ConvertPersonaToAuxiliar(SSP.Servidor.PERSONA persona)
        {
            try
            {
                var auxiliar = new PERSONAVISITAAUXILIAR();

                if (persona != null)
                {
                    auxiliar.ID_PERSONA = persona.ID_PERSONA;
                    auxiliar.ID_COLONIA = persona.ID_COLONIA;
                    auxiliar.ID_MUNICIPIO = persona.ID_MUNICIPIO;
                    auxiliar.ID_ENTIDAD = persona.ID_ENTIDAD;
                    auxiliar.ID_PAIS = persona.ID_PAIS;
                    auxiliar.SEXO = persona.SEXO;
                    auxiliar.RFC = persona.RFC;
                    auxiliar.CURP = persona.CURP;
                    auxiliar.TELEFONO = persona.TELEFONO;
                    auxiliar.FECHA_NACIMIENTO = persona.FEC_NACIMIENTO;
                    auxiliar.EDAD = persona.FEC_NACIMIENTO.HasValue ? (short)((Fechas.GetFechaDateServer - persona.FEC_NACIMIENTO.Value).TotalDays / 365) : new Nullable<short>();
                    auxiliar.DOMICILIO = persona.DOMICILIO_CALLE + " " + (persona.DOMICILIO_NUM_EXT.HasValue ? persona.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty) + " " + persona.DOMICILIO_NUM_INT + " " + (persona.DOMICILIO_CODIGO_POSTAL.HasValue ? persona.DOMICILIO_CODIGO_POSTAL.Value.ToString() : string.Empty);
                    auxiliar.NOMBRE = persona.NOMBRE;
                    auxiliar.MATERNO = persona.MATERNO;
                    auxiliar.PATERNO = persona.PATERNO;
                    auxiliar.OBJETO_PERSONA = persona;
                    if (persona.VISITANTE != null)
                    {
                        auxiliar.ID_ESTATUS_VISITA = persona.VISITANTE.ID_ESTATUS_VISITA;
                        if (SelectVisitanteIngreso != null)
                        {
                            auxiliar.ESTATUS_VISITA = persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).ESTATUS_VISITA : null;
                            auxiliar.ESTATUS_VISITA_DESCR = persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).ESTATUS_VISITA.DESCR : null;
                            auxiliar.INGRESO = persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).INGRESO : null;
                            auxiliar.PARENTESCO_DESCR = persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).TIPO_REFERENCIA.DESCR : "SIN INFORMACIÓN";
                            auxiliar.ID_PARENTESCO = persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).TIPO_REFERENCIA.ID_TIPO_REFERENCIA : (short)9999;
                            auxiliar.TIPO_REFERENCIA = persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).TIPO_REFERENCIA : new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = 9999, DESCR = "SIN INFORMACIÓN" };
                            auxiliar.INGRESO_NOMBRE = persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).INGRESO.IMPUTADO.NOMBRE : string.Empty;
                            auxiliar.INGRESO_PATERNO = persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).INGRESO.IMPUTADO.PATERNO : string.Empty;
                            auxiliar.INGRESO_MATERNO = persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).INGRESO.IMPUTADO.MATERNO : string.Empty;
                            auxiliar.ID_CENTRO = persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).ID_CENTRO : (short)0;
                            auxiliar.ID_ANIO = persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).INGRESO.ID_ANIO : new Nullable<short>();
                            auxiliar.ID_IMPUTADO = persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).INGRESO.ID_IMPUTADO : new Nullable<int>();
                            auxiliar.DE_TERMINO = persona.VISITANTE.VISITANTE_INGRESO.Any() ?
                                persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO && f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO &&
                                f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).VISITANTE_INGRESO_PASE.Any(a => a.ID_PASE == (short)enumTiposPases.DE_TERMINO ? a.AUTORIZADO == "S" ? a.ADUANA_INGRESO != null ? a.ADUANA_INGRESO.Any() : false : false : false)
                            : false;
                        }
                        else
                        {
                            auxiliar.ESTATUS_VISITA = SelectVisitanteIngreso != null ? persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO && f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO &&
                                f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).ESTATUS_VISITA : null : null;
                            auxiliar.ESTATUS_VISITA_DESCR = SelectVisitanteIngreso != null ? persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO && f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO &&
                                f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).ESTATUS_VISITA.DESCR : null : null;
                            auxiliar.INGRESO = SelectVisitanteIngreso != null ? persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO && f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO &&
                                f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).INGRESO : null : null;
                            auxiliar.PARENTESCO_DESCR = SelectVisitanteIngreso != null ? persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO && f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO &&
                                f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).TIPO_REFERENCIA.DESCR : "SIN INFORMACIÓN" : "SIN INFORMACIÓN";
                            auxiliar.ID_PARENTESCO = SelectVisitanteIngreso != null ? persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO && f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO &&
                                f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).TIPO_REFERENCIA.ID_TIPO_REFERENCIA : (short)9999 : (short)9999;
                            auxiliar.TIPO_REFERENCIA = SelectVisitanteIngreso != null ? persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO && f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO &&
                                f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).TIPO_REFERENCIA : new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = 9999, DESCR = "SIN INFORMACIÓN" } : new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = 9999, DESCR = "SIN INFORMACIÓN" };
                            auxiliar.INGRESO_NOMBRE = SelectVisitanteIngreso != null ? persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO && f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO &&
                                f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).INGRESO.IMPUTADO.NOMBRE : string.Empty : string.Empty;
                            auxiliar.INGRESO_PATERNO = SelectVisitanteIngreso != null ? persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO && f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO &&
                                f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).INGRESO.IMPUTADO.PATERNO : string.Empty : string.Empty;
                            auxiliar.INGRESO_MATERNO = SelectVisitanteIngreso != null ? persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO && f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO &&
                                f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).INGRESO.IMPUTADO.MATERNO : string.Empty : string.Empty;
                            auxiliar.ID_CENTRO = SelectVisitanteIngreso != null ? persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO && f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO &&
                                f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).ID_CENTRO : (short)0 : (short)0;
                            auxiliar.ID_ANIO = SelectVisitanteIngreso != null ? persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO && f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO &&
                                f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).INGRESO.ID_ANIO : new Nullable<short>() : new Nullable<short>();
                            auxiliar.ID_IMPUTADO = SelectVisitanteIngreso != null ? persona.VISITANTE.VISITANTE_INGRESO.Any() ? persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO && f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO &&
                                f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).INGRESO.ID_IMPUTADO : new Nullable<int>() : new Nullable<int>();
                            auxiliar.DE_TERMINO = SelectVisitanteIngreso != null ? persona.VISITANTE.VISITANTE_INGRESO.Any() ?
                                persona.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO && f.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && f.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO &&
                                f.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO).VISITANTE_INGRESO_PASE.Any(a => a.ID_PASE == (short)enumTiposPases.DE_TERMINO ? a.AUTORIZADO == "S" ? a.ADUANA_INGRESO != null ? a.ADUANA_INGRESO.Any() : false : false : false)
                            : false : false;
                        }
                    }
                }
                return auxiliar;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar los datos de un visitante.", ex);
                return new PERSONAVISITAAUXILIAR();
            }
        }

        #region Limpiar

        private void LimpiarPadron()
        {
            base.ClearRules();

            TextCallePadron = TextNombrePadron = TextPaternoPadron = TextMaternoPadron = TextCurpPadron = TextRfcPadron = string.Empty;
            TextCorreoPadron = TextNipPadron = TextNumIntPadron = TextTelefonoPadron = null;
            TextCodigoPadron = TextNumExtPadron = TextCodigoPostalPadron = null;
            SelectVisitantePadron = null;
            SelectImputadoIngresoPadron = null;
            SelectExpedientePadron = null;
            SelectIngresoPadron = null;
            FechaNacimientoPadron = Fechas.GetFechaDateServer;
            SelectAccesoUnicoPadron = false;
            SelectSexoPadron = "S";
            SelectPaisPadron = Parametro.PAIS;//82;
            SelectEntidadPadron = Parametro.ESTADO;//2;
            SelectMunicipioPadron = SelectTipoVisitantePadron = SelectSituacionPadron = -1;
            SelectColoniaPadron = -1;
            SelectParentescoPadron = -1;
            GeneralEnabledPadron = ValidarEnabledPadron = EntidadEnabledPadron = MunicipioEnabledPadron = ColoniaEnabledPadron = false;
            CodigoEnabledPadron = false;
            ListAcompanantesPadron = new ObservableCollection<ACOMPANANTE>();
            _ListadoInternosPadron = new ObservableCollection<VISITANTE_INGRESO>();
            ListExpedientePadron = new ObservableCollection<IMPUTADO>();
            listVisitantesPadron = new ObservableCollection<PERSONAVISITAAUXILIAR>();
            MenuGuardarEnabled = PInsertar;
        }

        private void LimpiarAsignacion()
        {
            base.ClearRules();
            TextCalleAsignacion = TextNombreAsignacion = TextPaternoAsignacion = TextMaternoAsignacion = TextCurpAsignacion = TextRfcAsignacion =
                TextCorreoAsignacion = TextNipAsignacion = TextNumIntAsignacion = TextTelefonoAsignacion = string.Empty;
            TextCodigoAsignacion = TextNumExtAsignacion = TextCodigoPostalAsignacion = new Nullable<int>();
            SelectVisitanteAsignacion = null;
            SelectImputadoIngresoAsignacion = null;
            SelectExpedienteAsignacion = null;
            SelectIngresoAsignacion = null;
            FechaNacimientoAsignacion = Fechas.GetFechaDateServer;
            SelectSexoAsignacion = "S";
            SelectPaisAsignacion = Parametro.PAIS;//82;
            SelectEntidadAsignacion = Parametro.ESTADO;//2;
            SelectMunicipioAsignacion = SelectTipoVisitanteAsignacion = SelectSituacionAsignacion = SelectParentescoAsignacion = -1;
            SelectColoniaAsignacion = -1;
            GeneralEnabledAsignacion = ValidarEnabledAsignacion = EntidadEnabledAsignacion = MunicipioEnabledAsignacion = ColoniaEnabledAsignacion =
                SelectAccesoUnicoAsignacion = ProgramacionVisitasMenuEnabled = CodigoEnabledAsignacion = false;
            ListAcompanantesAsignacion = new ObservableCollection<ACOMPANANTE>();
            _ListadoInternosAsignacion = new ObservableCollection<VISITANTE_INGRESO>();
            ListExpedienteAsignacion = new ObservableCollection<IMPUTADO>();
            ListVisitantesAsignacion = new ObservableCollection<PERSONAVISITAAUXILIAR>();
            ListProgramacionVisitaAux = ListProgramacionVisita = new ObservableCollection<ListaVisitaAgenda>();
            SelectVisitaProgramada = null;
            NuevaVisitaAgenda = false;
            TextVisitaDeTermino = string.Empty;
        }

        private void LimpiarDatosImputado()
        {
            NombreD = PaternoD = MaternoD = AnioD = FolioD = NoControlD = IngresosD = UbicacionD = TipoSeguridadD = FecIngresoD =
            ClasificacionJuridicaD = EstatusD = string.Empty;
            ImagenIngreso = new Imagenes().getImagenPerson();
        }

        private void LimpiarGeneral()
        {
            base.ClearRules();
            TextCalle = TextNombre = TextPaterno = TextMaterno = TextCurp = TextRfc = TextCorreo = /*TextNip =*/ TextNumInt = NombreD =
                PaternoD = MaternoD = AnioD = FolioD = NoControlD = IngresosD = UbicacionD = TipoSeguridadD = FecIngresoD =
                    NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = ClasificacionJuridicaD = EstatusD =
                        TextObservacion = TextTelefono = NotaTecnica = string.Empty;
            FolioBuscar = AnioBuscar = new Nullable<int>();
            TextCodigo = TextNumExt = TextCodigoPostal = null;
            ImagenVisitante = FotoVisita = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
            ImagenIngreso = ImagenPersona = new Imagenes().getImagenPerson();
            SelectVisitante = SelectVisitanteInterno = null;
            SelectVisitanteIngreso = null;
            SelectImputadoIngreso = null;
            SelectExpediente = null;
            SelectIngreso = null;
            TextVisitaDeTermino = string.Empty;
            FechaNacimiento = Fechas.GetFechaDateServer;
            TextFechaUltimaModificacion = TextFechaAlta = Fechas.GetFechaDateServer.ToString("dd/MM/yyyy");
            TextEdad = new Nullable<short>();
            SelectParentescoIngresoEnabled = FotoHuellaEnabled = SelectAccesoUnico = false;
            SelectSexo = "S";
            SelectPais = Parametro.PAIS; //82;
            ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
            SelectEntidad = Parametro.ESTADO;//2;
            ListMunicipio = new ObservableCollection<MUNICIPIO>();
            ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
            SelectMunicipio = -1;
            ListColonia = new ObservableCollection<COLONIA>();
            ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
            SelectColonia = -1;
            SelectTipoVisitante = SelectSituacion = SelectEstatusRelacion = -1;
            SelectParentesco = -1;
            GeneralEnabled = ValidarEnabled = EntidadEnabled = MunicipioEnabled = ColoniaEnabled = CapturarVisitanteEnabled = CapturarVisitanteVisible =
                ContextMenuEnabled = ProgramacionVisitasMenuEnabled = CodigoEnabled = false;
            MenuGuardarEnabled = PInsertar;
            ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
            ListAcompanantes = new ObservableCollection<ACOMPANANTE>();
            ListadoInternos = ListBuscarAcompanantes = new ObservableCollection<VISITANTE_INGRESO>();
            EmptyBuscarAcompanante = ListBuscarAcompanantes.Count == 0;
            ListVisitantes = ListVisitantesImputado = new ObservableCollection<PERSONAVISITAAUXILIAR>();
            TextHeaderDatosInterno = "Seleccion de nuevo interno para el visitante actual";
            AcompananteVisible = DatosExpedienteVisible = Visibility.Collapsed;
            ListProgramacionVisitaAux = ListProgramacionVisita = new ObservableCollection<ListaVisitaAgenda>();
            if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
            {
                NuevoListaTonta = null;
                PaseExtraordinarioEnabled = false;
                PaseDiasEnabled = false;
                PaseTerminoEnabled = false;
            }
            OnPropertyChanged();
        }

        private void LimpiarCapturaVisita()
        {
            TextCalle = TextNombre = TextPaterno = TextMaterno = TextCurp = TextRfc = TextCorreo = TextNumInt = TextTelefono = NotaTecnica = string.Empty;
            TextCodigo = TextNumExt = TextCodigoPostal = null;
            ImagenVisitante = FotoVisita = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
            SelectVisitanteInterno = null;
            FechaNacimiento = DateTime.Parse("01/01/2001");
            TextEdad = null;
            SelectAccesoUnico = false;
            SelectSexo = "S";
            SelectPais = Parametro.PAIS; //82;
            ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
            SelectEntidad = Parametro.ESTADO; //2;
            ListMunicipio = new ObservableCollection<MUNICIPIO>();
            ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
            SelectMunicipio = SelectTipoVisitante = SelectSituacion = -1;
            ListColonia = new ObservableCollection<COLONIA>();
            ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
            SelectColonia = -1;
            SelectParentesco = -1;
            CodigoEnabled = false;
            ValidarEnabled = GeneralEnabled = EntidadEnabled = MunicipioEnabled = ColoniaEnabled = false;
            TextFechaUltimaModificacion = TextFechaAlta = Fechas.GetFechaDateServer.ToString("dd/MM/yyyy");
        }

        #endregion

        #region LlenarCombos

        private void GetDatosVisitante()
        {
            ListPais = ListPais ?? new ObservableCollection<PAIS_NACIONALIDAD>((new cPaises()).ObtenerTodos().OrderBy(o => o.PAIS));
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                ListPais.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE" });
                SelectPais = Parametro.PAIS;//82;
                SelectSexo = "S";
            }));

            ListTipoVisitante = ListTipoVisitante ?? new ObservableCollection<TIPO_VISITANTE>((new cTipoVisitante()).ObtenerTodos().OrderBy(o => o.DESCR));
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                ListTipoVisitante.Insert(0, new TIPO_VISITANTE() { ID_TIPO_VISITANTE = -1, DESCR = "SELECCIONE" });
                SelectTipoVisitante = -1;
                SelectAccesoUnico = false;
            }));

            ListTipoRelacion = ListTipoRelacion ?? new ObservableCollection<TIPO_REFERENCIA>((new cTipoReferencia()).ObtenerTodos().OrderBy(o => o.DESCR));
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                ListTipoRelacion.Insert(0, new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = -1, DESCR = "SELECCIONE" });
                SelectParentesco = -1;
            }));

            ListSituacion = ListSituacion ?? new ObservableCollection<ESTATUS_VISITA>((new cEstatusVisita()).ObtenerTodos().OrderBy(o => o.DESCR));
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                ListSituacion.Insert(0, new ESTATUS_VISITA() { ID_ESTATUS_VISITA = -1, DESCR = "SELECCIONE" });
                SelectSituacion = SelectSituacionPadron = SelectEstatusRelacion = -1;
            }));

            ListDiscapacidades = ListDiscapacidades ?? new ObservableCollection<TIPO_DISCAPACIDAD>((new cTipoDiscapacidad()).ObtenerTodos().OrderBy(o => o.DESCR));
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                ListDiscapacidades.Insert(0, new TIPO_DISCAPACIDAD() { ID_TIPO_DISCAPACIDAD = -1, DESCR = "SELECCIONE" });
                SelectDiscapacidad = -1;
            }));

            ListTipoPase = ListTipoPase ?? new ObservableCollection<PASE_CATALOGO>((new cPases()).ObtenerTodos().OrderBy(o => o.ID_PASE));
        }

        #endregion

        #region GetDatos

        private void GetDatosVisitanteSeleccionadoInternoAsignacion()
        {
            try
            {
                if (SelectVisitanteInterno != null)
                {
                    if (SelectVisitanteInterno.OBJETO_PERSONA != null)
                    {
                        #region PERSONA
                        TextCodigo = SelectVisitanteInterno.ID_PERSONA;
                        TextNombre = string.IsNullOrEmpty(SelectVisitanteInterno.NOMBRE) ? string.Empty : SelectVisitanteInterno.NOMBRE.Trim();
                        TextPaterno = string.IsNullOrEmpty(SelectVisitanteInterno.PATERNO) ? string.Empty : SelectVisitanteInterno.PATERNO.Trim();
                        TextMaterno = string.IsNullOrEmpty(SelectVisitanteInterno.MATERNO) ? string.Empty : SelectVisitanteInterno.MATERNO.Trim();
                        SelectSexo = SelectVisitanteInterno.SEXO;
                        FechaNacimiento = SelectVisitanteInterno.OBJETO_PERSONA.FEC_NACIMIENTO;
                        TextCurp = SelectVisitanteInterno.CURP;
                        TextRfc = SelectVisitanteInterno.RFC;
                        TextTelefono = SelectVisitanteInterno.OBJETO_PERSONA.TELEFONO != null ? SelectVisitanteInterno.OBJETO_PERSONA.TELEFONO.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "") : string.Empty;
                        TextEdad = SelectVisitanteInterno.FECHA_NACIMIENTO.HasValue ? Convert.ToInt16(((Fechas.GetFechaDateServer - SelectVisitanteInterno.FECHA_NACIMIENTO.Value).TotalDays / 365)) : SelectVisitanteInterno.EDAD.HasValue ? SelectVisitanteInterno.EDAD.Value : new Nullable<short>();
                        TextCorreo = SelectVisitanteInterno.OBJETO_PERSONA.CORREO_ELECTRONICO;
                        SelectSituacion = SelectVisitanteInterno.ID_ESTATUS_VISITA;
                        SelectAccesoUnico = false;
                        TextFechaUltimaModificacion = SelectVisitanteInterno.OBJETO_PERSONA.VISITANTE.ULTIMA_MODIFICACION.ToString();
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            SelectPais = SelectVisitanteInterno.ID_PAIS;
                            ListEntidad = new ObservableCollection<ENTIDAD>(ListEntidadesAuxiliares.Where(w => w.ID_PAIS_NAC == SelectVisitanteInterno.ID_PAIS).OrderBy(o => o.DESCR));
                            ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                            SelectEntidad = SelectVisitanteInterno.ID_ENTIDAD;
                            ListMunicipio = new ObservableCollection<MUNICIPIO>(ListMunicipiosAuxiliares.Where(w => w.ID_ENTIDAD == SelectVisitanteInterno.ID_ENTIDAD).OrderBy(o => o.MUNICIPIO1));
                            ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                            SelectMunicipio = SelectVisitanteInterno.ID_MUNICIPIO;
                            ListColonia = new ObservableCollection<COLONIA>(ListColoniasAuxiliares.Where(w => w.ID_ENTIDAD == SelectVisitanteInterno.ID_ENTIDAD && w.ID_MUNICIPIO == SelectVisitanteInterno.ID_MUNICIPIO).OrderBy(o => o.DESCR));
                            ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                            SelectColonia = SelectVisitanteInterno.ID_COLONIA;
                        }));
                        SelectParentesco = SelectVisitanteInterno.TIPO_REFERENCIA.ID_TIPO_REFERENCIA;
                        SelectSexo = string.IsNullOrEmpty(SelectVisitanteInterno.OBJETO_PERSONA.SEXO) ? "S" : SelectVisitanteInterno.OBJETO_PERSONA.SEXO;
                        TextCalle = SelectVisitanteInterno.OBJETO_PERSONA.DOMICILIO_CALLE;
                        TextNumExt = SelectVisitanteInterno.OBJETO_PERSONA.DOMICILIO_NUM_EXT;
                        TextNumInt = SelectVisitanteInterno.OBJETO_PERSONA.DOMICILIO_NUM_INT;
                        TextCodigoPostal = SelectVisitanteInterno.OBJETO_PERSONA.DOMICILIO_CODIGO_POSTAL;
                        TextFechaAlta = SelectVisitanteInterno.OBJETO_PERSONA.VISITANTE.FEC_ALTA.HasValue ? SelectVisitanteInterno.OBJETO_PERSONA.VISITANTE.FEC_ALTA.Value.ToString("dd/MM/yyyy") : string.Empty;
                        SelectDiscapacidad = SelectVisitanteInterno.OBJETO_PERSONA.ID_TIPO_DISCAPACIDAD;
                        SelectDiscapacitado = SelectVisitanteInterno.OBJETO_PERSONA.ID_TIPO_DISCAPACIDAD > 0 ? "S" : "N";
                        ImagenVisitante = SelectVisitanteInterno.OBJETO_PERSONA.PERSONA_BIOMETRICO.Any() ? new Imagenes().ConvertByteToBitmap(SelectVisitanteInterno.OBJETO_PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO) : new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                        if (SelectVisitanteInterno.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO.Any())
                        {
                            foreach (var item in SelectVisitanteInterno.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO)
                            {
                                if (item.INGRESO.CAMA != null ? item.INGRESO.CAMA.ID_CAMA != 0 : false)
                                {
                                    item.INGRESO.CAMA.CELDA.ID_CELDA = item.INGRESO.CAMA.CELDA.ID_CELDA.ToString().Trim();
                                    item.INGRESO.CAMA.CELDA.SECTOR.DESCR = item.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim();
                                    item.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR = item.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim();
                                }
                            }
                        }
                        var estatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        ListadoInternos = new ObservableCollection<VISITANTE_INGRESO>(SelectVisitanteInterno.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO.Where(w => w.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro &&
                            !estatusInactivos.Contains(w.INGRESO.ID_ESTATUS_ADMINISTRATIVO)));
                        #endregion
                    }
                    else if (SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA != null)
                    {
                        #region VISITA_AUTORIZADA
                        TextCodigo = null;
                        TextNombre = string.IsNullOrEmpty(SelectVisitanteInterno.NOMBRE) ? string.Empty : SelectVisitanteInterno.NOMBRE.Trim();
                        TextPaterno = string.IsNullOrEmpty(SelectVisitanteInterno.PATERNO) ? string.Empty : SelectVisitanteInterno.PATERNO.Trim();
                        TextMaterno = string.IsNullOrEmpty(SelectVisitanteInterno.MATERNO) ? string.Empty : SelectVisitanteInterno.MATERNO.Trim();
                        SelectTipoVisitante = Parametro.ID_TIPO_VISITANTE_ORDINARIO;
                        SelectSexo = string.IsNullOrEmpty(SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.SEXO) ? "S" : SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.SEXO;
                        FechaNacimiento = Fechas.GetFechaDateServer.AddYears(SelectVisitanteInterno.EDAD.HasValue ? -SelectVisitanteInterno.EDAD.Value : 0);
                        TextCurp = SelectVisitanteInterno.CURP;
                        TextRfc = SelectVisitanteInterno.RFC;
                        TextTelefono = SelectVisitanteInterno.TELEFONO;
                        TextCorreo = string.Empty;
                        TextEdad = SelectVisitanteInterno.EDAD;
                        SelectSituacion = Parametro.ID_ESTATUS_VISITA_REGISTRO;
                        SelectParentesco = SelectVisitanteInterno.TIPO_REFERENCIA.ID_TIPO_REFERENCIA;
                        TextFechaUltimaModificacion = Fechas.GetFechaDateServer.ToString();
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            SelectPais = SelectVisitanteInterno.ID_PAIS;
                            ListEntidad = new ObservableCollection<ENTIDAD>(ListEntidadesAuxiliares.Where(w => w.ID_PAIS_NAC == SelectVisitanteInterno.ID_PAIS).OrderBy(o => o.DESCR));
                            ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                            SelectEntidad = SelectVisitanteInterno.ID_ENTIDAD;
                            ListMunicipio = new ObservableCollection<MUNICIPIO>(ListMunicipiosAuxiliares.Where(w => w.ID_ENTIDAD == SelectVisitanteInterno.ID_ENTIDAD).OrderBy(o => o.MUNICIPIO1));
                            ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                            SelectMunicipio = SelectVisitanteInterno.ID_MUNICIPIO;
                            ListColonia = new ObservableCollection<COLONIA>(ListColoniasAuxiliares.Where(w => w.ID_ENTIDAD == SelectVisitanteInterno.ID_ENTIDAD && w.ID_MUNICIPIO == SelectVisitanteInterno.ID_MUNICIPIO).OrderBy(o => o.DESCR));
                            ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                            SelectColonia = SelectVisitanteInterno.ID_COLONIA;
                        }));
                        TextCalle = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.DOMICILIO_CALLE;
                        TextNumExt = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.DOMICILIO_NUM_EXT;
                        TextNumInt = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.DOMICILIO_NUM_INT;
                        TextCodigoPostal = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.DOMICILIO_CODIGO_POSTAL;
                        SelectDiscapacidad = -1;
                        TextFechaAlta = Fechas.GetFechaDateServer.ToString("dd/MM/yyyy");
                        ImagenVisitante = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                        if (SelectVisitanteInterno.INGRESO != null ? SelectVisitanteInterno.INGRESO.CAMA != null ? SelectVisitanteInterno.INGRESO.CAMA.ID_CAMA != 0 : false : false)
                        {
                            SelectVisitanteInterno.INGRESO.CAMA.CELDA.ID_CELDA = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.INGRESO.CAMA.CELDA.ID_CELDA.ToString().Trim();
                            SelectVisitanteInterno.INGRESO.CAMA.CELDA.SECTOR.DESCR = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim();
                            SelectVisitanteInterno.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR = SelectVisitanteInterno.OBJETO_VISITA_AUTORIZADA.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim();
                        }
                        #endregion
                        ListadoInternos = new ObservableCollection<VISITANTE_INGRESO>();
                        ListadoInternos.Add(new VISITANTE_INGRESO()
                        {
                            INGRESO = SelectVisitanteInterno.INGRESO,
                            ID_CENTRO = (short)SelectVisitanteInterno.ID_CENTRO,
                            ID_ANIO = (short)SelectVisitanteInterno.ID_ANIO,
                            ID_IMPUTADO = (int)SelectVisitanteInterno.ID_IMPUTADO,
                            ID_ESTATUS_VISITA = SelectVisitanteInterno.ID_ESTATUS_VISITA,
                            TIPO_REFERENCIA = SelectVisitanteInterno.TIPO_REFERENCIA,
                            ESTATUS_VISITA = SelectVisitanteInterno.ESTATUS_VISITA,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del visitante.", ex);
            }
        }

        private void GetDatosVisitanteSeleccionadoAsignacion()
        {
            try
            {
                if (SelectVisitante != null)
                {
                    if (SelectVisitante.OBJETO_PERSONA != null)
                    {
                        #region PERSONA
                        TextCodigo = SelectVisitante.ID_PERSONA;
                        TextNombre = string.IsNullOrEmpty(SelectVisitante.NOMBRE) ? string.Empty : SelectVisitante.NOMBRE.Trim();
                        TextPaterno = string.IsNullOrEmpty(SelectVisitante.PATERNO) ? string.Empty : SelectVisitante.PATERNO.Trim();
                        TextMaterno = string.IsNullOrEmpty(SelectVisitante.MATERNO) ? string.Empty : SelectVisitante.MATERNO.Trim();
                        SelectSexo = SelectVisitante.SEXO;
                        FechaNacimiento = SelectVisitante.OBJETO_PERSONA.FEC_NACIMIENTO;
                        TextCurp = SelectVisitante.CURP;
                        TextRfc = SelectVisitante.RFC;
                        TextTelefono = SelectVisitanteInterno.OBJETO_PERSONA.TELEFONO.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                        TextCorreo = SelectVisitante.OBJETO_PERSONA.CORREO_ELECTRONICO;
                        SelectSituacion = SelectVisitante.OBJETO_PERSONA.VISITANTE.ID_ESTATUS_VISITA;
                        SelectAccesoUnico = false;
                        TextFechaUltimaModificacion = SelectVisitante.OBJETO_PERSONA.VISITANTE.ULTIMA_MODIFICACION.ToString();
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            SelectPais = SelectVisitante.ID_PAIS;
                            ListEntidad = new ObservableCollection<ENTIDAD>(ListEntidadesAuxiliares.Where(w => w.ID_PAIS_NAC == SelectVisitante.ID_PAIS).OrderBy(o => o.DESCR));
                            ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                            SelectEntidad = SelectVisitante.ID_ENTIDAD;
                            ListMunicipio = new ObservableCollection<MUNICIPIO>(ListMunicipiosAuxiliares.Where(w => w.ID_ENTIDAD == SelectVisitante.ID_ENTIDAD).OrderBy(o => o.MUNICIPIO1));
                            ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                            SelectMunicipio = SelectVisitante.ID_MUNICIPIO;
                            ListColonia = new ObservableCollection<COLONIA>(ListColoniasAuxiliares.Where(w => w.ID_ENTIDAD == SelectVisitante.ID_ENTIDAD && w.ID_MUNICIPIO == SelectVisitante.ID_MUNICIPIO).OrderBy(o => o.DESCR));
                            ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                            SelectColonia = SelectVisitante.ID_COLONIA;
                        }));
                        SelectSituacion = SelectVisitante.OBJETO_PERSONA.VISITANTE.ID_ESTATUS_VISITA;
                        TextCalle = SelectVisitante.OBJETO_PERSONA.DOMICILIO_CALLE;
                        TextNumExt = SelectVisitante.OBJETO_PERSONA.DOMICILIO_NUM_EXT;
                        TextNumInt = SelectVisitante.OBJETO_PERSONA.DOMICILIO_NUM_INT;
                        TextCodigoPostal = SelectVisitante.OBJETO_PERSONA.DOMICILIO_CODIGO_POSTAL;
                        SelectDiscapacidad = SelectVisitante.OBJETO_PERSONA.ID_TIPO_DISCAPACIDAD;
                        if (SelectVisitante.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO.Any())
                        {
                            foreach (var item in SelectVisitante.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO)
                            {
                                if (item.INGRESO.CAMA.ID_CAMA != 0)
                                {
                                    item.INGRESO.CAMA.CELDA.ID_CELDA = item.INGRESO.CAMA.CELDA.ID_CELDA.ToString().Trim();
                                    item.INGRESO.CAMA.CELDA.SECTOR.DESCR = item.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim();
                                    item.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR = item.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim();
                                }
                            }
                        }
                        var estatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        ListadoInternos = new ObservableCollection<VISITANTE_INGRESO>(SelectVisitante.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO.Where(w => w.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro &&
                            !estatusInactivos.Contains(w.INGRESO.ID_ESTATUS_ADMINISTRATIVO)));
                        #endregion

                        SituacionEnabled = false;
                    }
                    else if (SelectVisitante.OBJETO_VISITA_AUTORIZADA != null)
                    {
                        #region VISITA_AUTORIZADA

                        TextCodigo = null;
                        TextNombre = string.IsNullOrEmpty(SelectVisitante.NOMBRE) ? string.Empty : SelectVisitante.NOMBRE.Trim();
                        TextPaterno = string.IsNullOrEmpty(SelectVisitante.PATERNO) ? string.Empty : SelectVisitante.PATERNO.Trim();
                        TextMaterno = string.IsNullOrEmpty(SelectVisitante.MATERNO) ? string.Empty : SelectVisitante.MATERNO.Trim();
                        SelectTipoVisitante = -1;
                        SelectSexo = "S";
                        FechaNacimiento = Fechas.GetFechaDateServer;
                        TextCurp = SelectVisitante.CURP;
                        TextRfc = SelectVisitante.RFC;
                        TextTelefono = SelectVisitante.TELEFONO;
                        TextCorreo = string.Empty;
                        TextEdad = SelectVisitante.EDAD;
                        SelectSituacion = 11;
                        TextFechaUltimaModificacion = Fechas.GetFechaDateServer.ToString();
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            SelectPais = SelectVisitante.ID_PAIS;
                            ListEntidad = new ObservableCollection<ENTIDAD>(ListEntidadesAuxiliares.Where(w => w.ID_PAIS_NAC == SelectVisitante.ID_PAIS).OrderBy(o => o.DESCR));
                            ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                            SelectEntidad = SelectVisitante.ID_ENTIDAD;
                            ListMunicipio = new ObservableCollection<MUNICIPIO>(ListMunicipiosAuxiliares.Where(w => w.ID_ENTIDAD == SelectVisitante.ID_ENTIDAD).OrderBy(o => o.MUNICIPIO1));
                            ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                            SelectMunicipio = SelectVisitante.ID_MUNICIPIO;
                            ListColonia = new ObservableCollection<COLONIA>(ListColoniasAuxiliares.Where(w => w.ID_ENTIDAD == SelectVisitante.ID_ENTIDAD && w.ID_MUNICIPIO == SelectVisitante.ID_MUNICIPIO).OrderBy(o => o.DESCR));
                            ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                            SelectColonia = SelectVisitante.ID_COLONIA;
                        }));
                        TextCalle = SelectVisitante.OBJETO_VISITA_AUTORIZADA.DOMICILIO_CALLE;
                        TextNumExt = SelectVisitante.OBJETO_VISITA_AUTORIZADA.DOMICILIO_NUM_EXT;
                        TextNumInt = SelectVisitante.OBJETO_VISITA_AUTORIZADA.DOMICILIO_NUM_INT;
                        TextCodigoPostal = SelectVisitante.OBJETO_VISITA_AUTORIZADA.DOMICILIO_CODIGO_POSTAL;
                        SelectDiscapacidad = -1;
                        if (SelectVisitante.INGRESO != null ? SelectVisitante.INGRESO.CAMA != null ? SelectVisitante.INGRESO.CAMA.ID_CAMA != 0 : false : false)
                        {
                            SelectVisitante.INGRESO.CAMA.CELDA.ID_CELDA = SelectVisitante.OBJETO_VISITA_AUTORIZADA.INGRESO.CAMA.CELDA.ID_CELDA.ToString().Trim();
                            SelectVisitante.INGRESO.CAMA.CELDA.SECTOR.DESCR = SelectVisitante.OBJETO_VISITA_AUTORIZADA.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim();
                            SelectVisitante.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR = SelectVisitante.OBJETO_VISITA_AUTORIZADA.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim();
                        }
                        #endregion

                        ListadoInternos = new ObservableCollection<VISITANTE_INGRESO>();
                        ListadoInternos.Add(new VISITANTE_INGRESO()
                        {
                            INGRESO = SelectVisitante.INGRESO,
                            ID_CENTRO = (short)SelectVisitante.ID_CENTRO,
                            ID_ANIO = (short)SelectVisitante.ID_ANIO,
                            ID_IMPUTADO = (int)SelectVisitante.ID_IMPUTADO,
                            ID_ESTATUS_VISITA = SelectVisitante.ID_ESTATUS_VISITA,
                            TIPO_REFERENCIA = SelectVisitante.TIPO_REFERENCIA,
                            ESTATUS_VISITA = SelectVisitante.ESTATUS_VISITA,
                        });
                        SituacionEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del visitante.", ex);
            }
        }

        private void GetDatosVisitanteSeleccionadoPadron()
        {
            try
            {
                if (SelectVisitante != null)
                {
                    if (SelectVisitante.OBJETO_PERSONA != null)
                    {
                        #region PERSONA
                        TextCodigo = SelectVisitante.ID_PERSONA;
                        TextNombre = string.IsNullOrEmpty(SelectVisitante.NOMBRE) ? string.Empty : SelectVisitante.NOMBRE.Trim();
                        TextPaterno = string.IsNullOrEmpty(SelectVisitante.PATERNO) ? string.Empty : SelectVisitante.PATERNO.Trim();
                        TextMaterno = string.IsNullOrEmpty(SelectVisitante.MATERNO) ? string.Empty : SelectVisitante.MATERNO.Trim();
                        SelectSexo = SelectVisitante.SEXO;
                        FechaNacimiento = SelectVisitante.OBJETO_PERSONA.FEC_NACIMIENTO;
                        TextCurp = string.IsNullOrEmpty(SelectVisitante.OBJETO_PERSONA.CURP) ? string.Empty : SelectVisitante.OBJETO_PERSONA.CURP.Trim();
                        TextRfc = string.IsNullOrEmpty(SelectVisitante.OBJETO_PERSONA.RFC) ? string.Empty : SelectVisitante.OBJETO_PERSONA.RFC.Trim();
                        SelectSituacion = SelectVisitante.ID_ESTATUS_VISITA;
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            SelectPais = SelectVisitante.ID_PAIS;
                            ListEntidad = new ObservableCollection<ENTIDAD>(ListEntidadesAuxiliares.Where(w => w.ID_PAIS_NAC == SelectVisitante.ID_PAIS).OrderBy(o => o.DESCR));
                            ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                            SelectEntidad = SelectVisitante.ID_ENTIDAD;
                            ListMunicipio = new ObservableCollection<MUNICIPIO>(ListMunicipiosAuxiliares.Where(w => w.ID_ENTIDAD == SelectVisitante.ID_ENTIDAD).OrderBy(o => o.MUNICIPIO1));
                            ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                            SelectMunicipio = SelectVisitante.ID_MUNICIPIO;
                            ListColonia = new ObservableCollection<COLONIA>(ListColoniasAuxiliares.Where(w => w.ID_ENTIDAD == SelectVisitante.ID_ENTIDAD && w.ID_MUNICIPIO == SelectVisitante.ID_MUNICIPIO).OrderBy(o => o.DESCR));
                            ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                            SelectColonia = SelectVisitante.ID_COLONIA;
                        }));
                        TextCalle = SelectVisitante.OBJETO_PERSONA.DOMICILIO_CALLE;
                        TextNumExt = SelectVisitante.OBJETO_PERSONA.DOMICILIO_NUM_EXT;
                        TextNumInt = SelectVisitante.OBJETO_PERSONA.DOMICILIO_NUM_INT;
                        TextCodigoPostal = SelectVisitante.OBJETO_PERSONA.DOMICILIO_CODIGO_POSTAL;
                        TextTelefono = string.IsNullOrEmpty(SelectVisitante.OBJETO_PERSONA.TELEFONO) ? string.Empty : SelectVisitante.OBJETO_PERSONA.TELEFONO.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                        TextCorreo = SelectVisitante.OBJETO_PERSONA.CORREO_ELECTRONICO;
                        SelectParentesco = SelectVisitante.ID_PARENTESCO.HasValue ? SelectVisitante.ID_PARENTESCO.Value : (short)-1;

                        #region Foto
                        if (SelectVisitante.OBJETO_PERSONA.PERSONA_BIOMETRICO.Any())
                        {
                            var img = SelectVisitante.OBJETO_PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault();
                            if (img != null)
                            {
                                FotoVisita = new Imagenes().ConvertByteToBitmap(img.BIOMETRICO);
                            }
                            else
                                FotoVisita = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                        }
                        else
                            FotoVisita = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                        #endregion

                        ImagesToSave = null;
                        var pass = new cPasesVisitanteIngreso().ObtenerXPersona(SelectVisitante.OBJETO_PERSONA.ID_PERSONA);
                        SelectTipoPase = pass.Any() ? pass.OrderBy(o => o.FECHA_ALTA).FirstOrDefault().ID_PASE.Value : (short)-1;
                        PaseEnabled = pass.Any() ? pass.OrderBy(o => o.FECHA_ALTA).FirstOrDefault().ID_PASE.Value == 1 ? false : true : false;
                        #endregion

                        if (SelectVisitante.OBJETO_PERSONA.VISITANTE != null)
                        {
                            TextFechaUltimaModificacion = SelectVisitante.OBJETO_PERSONA.VISITANTE.ULTIMA_MODIFICACION.ToString();
                            if (SelectVisitante.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO.Any())
                            {
                                foreach (var item in SelectVisitante.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO)
                                {
                                    if (item.INGRESO != null)
                                        if (item.INGRESO.CAMA != null)
                                            if (item.INGRESO.CAMA != null ? item.INGRESO.CAMA.ID_CAMA != 0 : false)
                                            {
                                                item.INGRESO.CAMA.CELDA.ID_CELDA = item.INGRESO.CAMA.CELDA.ID_CELDA.ToString().Trim();
                                                item.INGRESO.CAMA.CELDA.SECTOR.DESCR = item.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim();
                                                item.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR = item.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim();
                                            }
                                }
                            }
                            var estatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                            ListadoInternos = new ObservableCollection<VISITANTE_INGRESO>(SelectVisitante.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO.Where(w => w.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro &&
                                !estatusInactivos.Contains(w.INGRESO.ID_ESTATUS_ADMINISTRATIVO)));
                            if (ListadoInternos != null)
                                if (ListadoInternos.Count == 1)
                                {
                                    MenuGuardarEnabled = !PInsertar ? PEditar : true;
                                    SelectVisitanteIngreso = ListadoInternos.FirstOrDefault();
                                    TextObservacion = SelectVisitanteIngreso.OBSERVACION;
                                    SelectAccesoUnico = SelectVisitanteIngreso.ACCESO_UNICO == "S";
                                    SelectTipoVisitante = SelectVisitanteIngreso.ID_TIPO_VISITANTE.HasValue ? SelectVisitanteIngreso.ID_TIPO_VISITANTE.Value : (short)-1;
                                    SituacionEnabled = false;
                                    AccesoUnicoEnabled = SelectVisitanteIngreso.ID_ESTATUS_VISITA != Parametro.ID_ESTATUS_VISITA_AUTORIZADO;
                                    SelectImputadoIngreso = ListadoInternos.FirstOrDefault().INGRESO;
                                    GetDatosIngresoImputadoSeleccionado();
                                    ListBuscarAcompanantes = new ObservableCollection<VISITANTE_INGRESO>();
                                    ListBuscarAcompanantes = new ObservableCollection<VISITANTE_INGRESO>(SelectImputadoIngreso.VISITANTE_INGRESO.Where(w =>
                                        w.ID_PERSONA != SelectVisitanteIngreso.ID_PERSONA &&
                                        !ListAcompanantes.Where(x => x.ID_ACOMPANANTE == w.ID_PERSONA).Any() &&
                                        new Fechas().CalculaEdad(w.VISITANTE.PERSONA.FEC_NACIMIENTO.HasValue ?
                                        w.VISITANTE.PERSONA.FEC_NACIMIENTO :
                                        DateTime.Parse("01/01/1900")) >= 18 &&
                                        (w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_EN_REVISION || w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_AUTORIZADO)));
                                    EmptyBuscarAcompanante = ListBuscarAcompanantes.Count == 0;
                                    SelectEstatusRelacionEnabled = false;
                                    SelectParentescoIngresoEnabled = true;
                                }
                                else
                                    SelectEstatusRelacionEnabled = SelectParentescoIngresoEnabled = false;
                            else
                                SelectEstatusRelacionEnabled = SelectParentescoIngresoEnabled = false;

                            SelectDiscapacitado = SelectVisitante.OBJETO_PERSONA.ID_TIPO_DISCAPACIDAD.HasValue ?
                                SelectVisitante.OBJETO_PERSONA.ID_TIPO_DISCAPACIDAD.Value <= 0 ? "N" : "S" : "N";
                            SelectDiscapacidad = SelectVisitante.OBJETO_PERSONA.ID_TIPO_DISCAPACIDAD.HasValue ?
                                SelectVisitante.OBJETO_PERSONA.ID_TIPO_DISCAPACIDAD.Value : (short)0;
                        }
                    }
                    else if (SelectVisitante.OBJETO_VISITA_AUTORIZADA != null)
                    {
                        #region VISITA_AUTORIZADA
                        TextCodigo = null;
                        TextNombre = string.IsNullOrEmpty(SelectVisitante.NOMBRE) ? string.Empty : SelectVisitante.NOMBRE.Trim();
                        TextPaterno = string.IsNullOrEmpty(SelectVisitante.PATERNO) ? string.Empty : SelectVisitante.PATERNO.Trim();
                        TextMaterno = string.IsNullOrEmpty(SelectVisitante.MATERNO) ? string.Empty : SelectVisitante.MATERNO.Trim();
                        SelectTipoVisitante = Parametro.ID_TIPO_VISITANTE_ORDINARIO;
                        SelectSexo = SelectVisitante.SEXO;
                        TextRfc = SelectVisitante.RFC;
                        TextCurp = SelectVisitante.CURP;
                        FechaNacimiento = Fechas.GetFechaDateServer.AddYears(SelectVisitante.EDAD.HasValue ? -SelectVisitante.EDAD.Value : 0); // SelectVisitante.OBJETO_PERSONA.FEC_NACIMIENTO;
                        TextTelefono = SelectVisitante.TELEFONO;
                        TextCorreo = string.Empty;
                        TextEdad = SelectVisitante.EDAD;
                        SelectAccesoUnico = false;
                        SelectSituacion = Parametro.ID_ESTATUS_VISITA_REGISTRO;
                        TextFechaUltimaModificacion = Fechas.GetFechaDateServer.ToString();
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            SelectPais = SelectVisitante.ID_PAIS;
                            ListEntidad = new ObservableCollection<ENTIDAD>(ListEntidadesAuxiliares.Where(w => w.ID_PAIS_NAC == SelectVisitante.ID_PAIS).OrderBy(o => o.DESCR));
                            ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                            SelectEntidad = SelectVisitante.ID_ENTIDAD;
                            ListMunicipio = new ObservableCollection<MUNICIPIO>(ListMunicipiosAuxiliares.Where(w => w.ID_ENTIDAD == SelectVisitante.ID_ENTIDAD).OrderBy(o => o.MUNICIPIO1));
                            ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                            SelectMunicipio = SelectVisitante.ID_MUNICIPIO;
                            ListColonia = new ObservableCollection<COLONIA>(ListColoniasAuxiliares.Where(w => w.ID_ENTIDAD == SelectVisitante.ID_ENTIDAD && w.ID_MUNICIPIO == SelectVisitante.ID_MUNICIPIO).OrderBy(o => o.DESCR));
                            ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                            SelectColonia = SelectVisitante.ID_COLONIA;
                        }));
                        TextCalle = SelectVisitante.OBJETO_VISITA_AUTORIZADA.DOMICILIO_CALLE;
                        TextNumExt = SelectVisitante.OBJETO_VISITA_AUTORIZADA.DOMICILIO_NUM_EXT;
                        TextNumInt = SelectVisitante.OBJETO_VISITA_AUTORIZADA.DOMICILIO_NUM_INT;
                        TextCodigoPostal = SelectVisitante.OBJETO_VISITA_AUTORIZADA.DOMICILIO_CODIGO_POSTAL;
                        SelectDiscapacitado = "N";
                        SelectDiscapacidad = 0;
                        SelectParentesco = SelectVisitante.ID_PARENTESCO.HasValue ? SelectVisitante.ID_PARENTESCO.Value : (short)-1;
                        SelectEstatusRelacion = SelectVisitante.ID_ESTATUS_VISITA;
                        TextObservacion = string.Empty;
                        SelectTipoPase = -1;
                        if (SelectVisitante.INGRESO != null ? SelectVisitante.INGRESO.CAMA != null ? SelectVisitante.INGRESO.CAMA.ID_CAMA != 0 : false : false)
                        {
                            SelectVisitante.INGRESO.CAMA.CELDA.ID_CELDA = SelectVisitante.OBJETO_VISITA_AUTORIZADA.INGRESO.CAMA.CELDA.ID_CELDA.ToString().Trim();
                            SelectVisitante.INGRESO.CAMA.CELDA.SECTOR.DESCR = SelectVisitante.OBJETO_VISITA_AUTORIZADA.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim();
                            SelectVisitante.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR = SelectVisitante.OBJETO_VISITA_AUTORIZADA.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim();
                        }
                        #endregion

                        ListadoInternos = new ObservableCollection<VISITANTE_INGRESO>();
                        ListadoInternos.Add(new VISITANTE_INGRESO()
                        {
                            INGRESO = SelectVisitante.INGRESO,
                            ID_CENTRO = (short)SelectVisitante.ID_CENTRO,
                            ID_ANIO = (short)SelectVisitante.ID_ANIO,
                            ID_IMPUTADO = (int)SelectVisitante.ID_IMPUTADO,
                            ID_ESTATUS_VISITA = SelectVisitante.ID_ESTATUS_VISITA,
                            TIPO_REFERENCIA = SelectVisitante.TIPO_REFERENCIA,
                            ESTATUS_VISITA = SelectVisitante.ESTATUS_VISITA,
                            ID_INGRESO = SelectVisitante.INGRESO.ID_INGRESO,
                            ID_TIPO_REFERENCIA = SelectVisitante.ID_PARENTESCO
                        });
                        SelectEstatusRelacionEnabled = false;
                        SelectParentescoIngresoEnabled = AccesoUnicoEnabled = true;
                        SelectVisitanteIngreso = ListadoInternos.FirstOrDefault();
                        SelectImputadoIngreso = SelectVisitanteIngreso.INGRESO;
                        GetDatosIngresoImputadoSeleccionado();
                        //* * * * * * * * * * * *//
                        ListBuscarAcompanantes = new ObservableCollection<VISITANTE_INGRESO>(SelectImputadoIngreso.VISITANTE_INGRESO.Where(w =>
                            w.ID_PERSONA != SelectVisitanteIngreso.ID_PERSONA &&
                            !ListAcompanantes.Where(x => x.ID_ACOMPANANTE == w.ID_PERSONA).Any() &&
                            new Fechas().CalculaEdad(w.VISITANTE.PERSONA.FEC_NACIMIENTO.HasValue ?
                                w.VISITANTE.PERSONA.FEC_NACIMIENTO : DateTime.Parse("01/01/1900")) >= 18 &&
                            (w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_EN_REVISION || w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_AUTORIZADO)));
                        //* * * * * * * * * * * *//
                        EmptyBuscarAcompanante = ListBuscarAcompanantes.Count == 0;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del visitante.", ex);
            }
        }

        private async void GetDatosIngresoImputadoSeleccionado()
        {
            try
            {
                if (SelectImputadoIngreso != null)
                {
                    AnioD = SelectImputadoIngreso.ID_ANIO.ToString();
                    FolioD = SelectImputadoIngreso.ID_IMPUTADO.ToString();
                    NombreD = string.IsNullOrEmpty(SelectImputadoIngreso.IMPUTADO.NOMBRE) ? string.Empty : SelectImputadoIngreso.IMPUTADO.NOMBRE.Trim();
                    PaternoD = string.IsNullOrEmpty(SelectImputadoIngreso.IMPUTADO.PATERNO) ? string.Empty : SelectImputadoIngreso.IMPUTADO.PATERNO.Trim();
                    MaternoD = string.IsNullOrEmpty(SelectImputadoIngreso.IMPUTADO.MATERNO) ? string.Empty : SelectImputadoIngreso.IMPUTADO.MATERNO.Trim();
                    IngresosD = SelectImputadoIngreso.IMPUTADO.INGRESO.Count.ToString();
                    UbicacionD = SelectImputadoIngreso.CAMA != null ? SelectImputadoIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + SelectImputadoIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() + "" +
                        SelectImputadoIngreso.CAMA.CELDA.ID_CELDA.ToString().Trim() + "-" + SelectImputadoIngreso.CAMA.ID_CAMA : string.Empty;
                    TipoSeguridadD = SelectImputadoIngreso.TIPO_SEGURIDAD.DESCR;
                    FecIngresoD = SelectImputadoIngreso.FEC_INGRESO_CERESO == null ? null : SelectImputadoIngreso.FEC_INGRESO_CERESO.ToString();
                    ClasificacionJuridicaD = SelectImputadoIngreso.CLASIFICACION_JURIDICA.DESCR;
                    EstatusD = SelectImputadoIngreso.ESTATUS_ADMINISTRATIVO.DESCR;
                    ImagenIngreso = SelectImputadoIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                        SelectImputadoIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                            SelectImputadoIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                                SelectImputadoIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                                    new Imagenes().getImagenPerson();
                    var pases = new cVisitanteIngresoPase().ObtenerTodos().Where(w => w.AUTORIZADO == "S" ? w.ID_PASE == (short)enumTiposPases.DE_TERMINO ? (w.ID_CENTRO == SelectImputadoIngreso.ID_CENTRO && w.ID_ANIO == SelectImputadoIngreso.ID_ANIO &&
                        w.ID_IMPUTADO == SelectImputadoIngreso.ID_IMPUTADO && w.ID_INGRESO == SelectImputadoIngreso.ID_INGRESO) : false : false);
                    var _pase = pases.Any() ? pases.ToList().Any(a => a.ADUANA_INGRESO != null ? a.ADUANA_INGRESO.Any() : false) ? pases.ToList().FirstOrDefault(a => a.ADUANA_INGRESO != null ? a.ADUANA_INGRESO.Any() : false) : null : null;
                    TextVisitaDeTermino = _pase != null ?
                        _pase.ADUANA_INGRESO.First(w => w.ID_CENTRO == SelectImputadoIngreso.ID_CENTRO && w.ID_ANIO == SelectImputadoIngreso.ID_ANIO && w.ID_IMPUTADO == SelectImputadoIngreso.ID_IMPUTADO &&
                        w.ID_INGRESO == SelectImputadoIngreso.ID_INGRESO).ENTRADA_FEC.HasValue ?
                            _pase.ADUANA_INGRESO.First(w => w.ID_CENTRO == SelectImputadoIngreso.ID_CENTRO && w.ID_ANIO == SelectImputadoIngreso.ID_ANIO && w.ID_IMPUTADO == SelectImputadoIngreso.ID_IMPUTADO &&
                            w.ID_INGRESO == SelectImputadoIngreso.ID_INGRESO).ENTRADA_FEC.Value.ToString("dd \\de MMMM, yyyy a la\\s HH:mm").ToUpper()
                            : "NO SE " + (_pase.VISITANTE_INGRESO.INGRESO.CAUSA_PENAL.Any() ? "UTILIZÓ" : "HA UTILIZADO") + " EL PASE PARA LA VISITA DE TERMINO"
                    : "NO SE " + (SelectImputadoIngreso.CAUSA_PENAL.Any() ? "UTILIZÓ" : "HA UTILIZADO") + " EL PASE PARA LA VISITA DE TERMINO";
                    NuevaVisitaAgenda = false;
                    if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    {

                        #region VISITA_AUTORIZADA
                        var autorizadas = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<VISITA_AUTORIZADA>>(() =>
                            new cVisitaAutorizada().ObtenerXIngresoYNoCredencializados(SelectImputadoIngreso.ID_CENTRO, SelectImputadoIngreso.ID_ANIO, SelectImputadoIngreso.ID_IMPUTADO, SelectImputadoIngreso.ID_INGRESO));
                        var lista = autorizadas.Select(s => new PERSONAVISITAAUXILIAR
                        {
                            ID_CENTRO = s.ID_CENTRO,
                            ID_ANIO = s.ID_ANIO,
                            ID_IMPUTADO = s.ID_IMPUTADO,
                            INGRESO = s.INGRESO,
                            ID_VISITA = s.ID_VISITA,
                            ID_COLONIA = s.ID_COLONIA,
                            ID_MUNICIPIO = s.ID_MUNICIPIO,
                            ID_ENTIDAD = s.ID_ENTIDAD,
                            ID_PAIS = s.ID_PAIS,
                            SEXO = s.SEXO,
                            ID_ESTATUS_VISITA = (short)(s.ESTATUS == 0 ? Parametro.ID_ESTATUS_VISITA_REGISTRO : Parametro.ID_ESTATUS_VISITA_SUSPENDIDO),
                            ESTATUS_VISITA_DESCR = s.ESTATUS == 0 ? "REGISTRO" : "SUSPENDIDO",
                            INGRESO_NOMBRE = s.INGRESO.IMPUTADO.NOMBRE,
                            INGRESO_PATERNO = s.INGRESO.IMPUTADO.PATERNO,
                            INGRESO_MATERNO = s.INGRESO.IMPUTADO.MATERNO,
                            ID_PARENTESCO = s.ID_PARENTESCO,
                            PARENTESCO_DESCR = s.TIPO_REFERENCIA.DESCR,
                            EDAD = s.EDAD,
                            TIPO_REFERENCIA = s.TIPO_REFERENCIA,
                            ESTATUS_VISITA = new ESTATUS_VISITA() { ID_ESTATUS_VISITA = (short)(s.ESTATUS == 0 ? Parametro.ID_ESTATUS_VISITA_REGISTRO : Parametro.ID_ESTATUS_VISITA_SUSPENDIDO), DESCR = s.ESTATUS == 0 ? "REGISTRO" : "SUSPENDIDO" },
                            TELEFONO = s.TELEFONO,
                            DOMICILIO = s.DOMICILIO_CALLE + " " + (s.DOMICILIO_NUM_EXT.HasValue ? s.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty) + " " + s.DOMICILIO_NUM_INT + " " + (s.DOMICILIO_CODIGO_POSTAL.HasValue ? s.DOMICILIO_CODIGO_POSTAL.Value.ToString() : string.Empty),
                            NOMBRE = s.NOMBRE,
                            MATERNO = s.MATERNO,
                            PATERNO = s.PATERNO,
                            OBJETO_VISITA_AUTORIZADA = s
                        }).ToList();
                        #endregion

                        #region PERSONAS
                        var personas = await StaticSourcesViewModel.CargarDatosAsync<IQueryable<SSP.Servidor.PERSONA>>(() =>
                            new cPersona().ObtenerPersonasVisitantesXIngreso(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO));
                        lista.AddRange(personas.ToList().Select(s => new PERSONAVISITAAUXILIAR()
                        {
                            ID_PERSONA = s.ID_PERSONA,
                            ID_COLONIA = s.ID_COLONIA,
                            ID_MUNICIPIO = s.ID_MUNICIPIO,
                            ID_ENTIDAD = s.ID_ENTIDAD,
                            ID_PAIS = s.ID_PAIS,
                            SEXO = s.SEXO,
                            RFC = s.RFC,
                            CURP = s.CURP,
                            TELEFONO = s.TELEFONO,
                            FECHA_NACIMIENTO = s.FEC_NACIMIENTO,
                            EDAD = s.FEC_NACIMIENTO.HasValue ? (short)((Fechas.GetFechaDateServer - s.FEC_NACIMIENTO.Value).TotalDays / 365) : new Nullable<short>(),
                            DOMICILIO = s.DOMICILIO_CALLE + " " + (s.DOMICILIO_NUM_EXT.HasValue ? s.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty) + " " + s.DOMICILIO_NUM_INT + " " + (s.DOMICILIO_CODIGO_POSTAL.HasValue ? s.DOMICILIO_CODIGO_POSTAL.Value.ToString() : string.Empty),
                            NOMBRE = s.NOMBRE,
                            MATERNO = s.MATERNO,
                            PATERNO = s.PATERNO,
                            OBJETO_PERSONA = s,
                            ID_ESTATUS_VISITA = s.VISITANTE != null ? s.VISITANTE.ID_ESTATUS_VISITA : new Nullable<short>(),
                            DE_TERMINO = s.VISITANTE != null ?
                                (s.VISITANTE.VISITANTE_INGRESO.Any() ?
                                    s.VISITANTE.VISITANTE_INGRESO.Any(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO) ?
                                        s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_ANIO == SelectIngreso.ID_ANIO && f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).VISITANTE_INGRESO_PASE.Any(a => a.ID_PASE == (short)enumTiposPases.DE_TERMINO ?
                                            a.AUTORIZADO == "S" ?
                                                a.ADUANA_INGRESO != null ?
                                                    a.ADUANA_INGRESO.Any()
                                                : false
                                            : false
                                        : false)
                                    : false
                                : false)
                            : false,
                            ESTATUS_VISITA = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.Any(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO) ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).ESTATUS_VISITA : null : null) : null,
                            ESTATUS_VISITA_DESCR = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.Any(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO) ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).ESTATUS_VISITA.DESCR : null : null) : null,
                            INGRESO = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.Any(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO) ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).INGRESO : null : null) : null,
                            PARENTESCO_DESCR = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.Any(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO) ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).TIPO_REFERENCIA.DESCR : "SIN INFORMACION" : "SIN INFORMACION") : null,
                            ID_PARENTESCO = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.Any(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO) ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).TIPO_REFERENCIA.ID_TIPO_REFERENCIA : (short)9999 : (short)9999) : new Nullable<short>(),
                            TIPO_REFERENCIA = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.Any(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO) ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).TIPO_REFERENCIA :
                                new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = 9999, DESCR = "SIN INFORMACION" } : new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = 9999, DESCR = "SIN INFORMACION" }) : null,
                            INGRESO_NOMBRE = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.Any(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO) ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).INGRESO.IMPUTADO.NOMBRE : string.Empty : string.Empty) : null,
                            INGRESO_PATERNO = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.Any(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO) ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).INGRESO.IMPUTADO.PATERNO : string.Empty : string.Empty) : null,
                            INGRESO_MATERNO = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.Any(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO) ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).INGRESO.IMPUTADO.MATERNO : string.Empty : string.Empty) : null,
                            ID_CENTRO = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.Any(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO) ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).ID_CENTRO : (short)0 : (short)0) : new Nullable<short>(),
                            ID_ANIO = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.Any(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO) ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).INGRESO.ID_ANIO : new Nullable<short>() : new Nullable<short>()) : new Nullable<short>(),
                            ID_IMPUTADO = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.Any(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO) ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO
                                && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).INGRESO.ID_IMPUTADO : new Nullable<int>() : new Nullable<int>()) : new Nullable<int>(),
                            ///TODO: VISITANTE DE TERMINO
                        }));
                        #endregion

                        ListVisitantesImputado = new ObservableCollection<PERSONAVISITAAUXILIAR>(lista);
                        if (ListVisitantesImputado.Count > 0)
                        {
                            EmptyBuscarRelacionInternoVisible = false;
                            SeleccionarVisitaExistente = true;
                        }
                        else
                        {
                            EmptyBuscarRelacionInternoVisible = true;
                            SeleccionarVisitaExistente = false;
                        }
                        await GetVisitasAgendadas();
                    }
                    else if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    {
                        if (SelectVisitanteIngreso != null)
                        {
                            if (SelectVisitante.OBJETO_PERSONA != null)
                            {
                                var personaNip = SelectVisitante.OBJETO_PERSONA.PERSONA_NIP.Where(w => w.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && w.ID_TIPO_VISITA == Parametro.ID_TIPO_VISITA_FAMILIAR);
                                SelectDiscapacitado = SelectVisitante.OBJETO_PERSONA.ID_TIPO_DISCAPACIDAD.HasValue ? SelectVisitante.OBJETO_PERSONA.ID_TIPO_DISCAPACIDAD.Value > 0 ? "S" : "N" : "N";
                                SelectDiscapacidad = SelectVisitante.OBJETO_PERSONA.ID_TIPO_DISCAPACIDAD.HasValue ? SelectVisitante.OBJETO_PERSONA.ID_TIPO_DISCAPACIDAD.Value : (short)-1;
                            }
                            else
                                SelectDiscapacitado = (SelectDiscapacidad == null || SelectDiscapacidad == 0) ? "N" : "S";
                            SelectParentesco = SelectVisitanteIngreso.ID_TIPO_REFERENCIA.HasValue ? SelectVisitanteIngreso.ID_TIPO_REFERENCIA.Value : (short)-1;
                            SelectEstatusRelacion = SelectVisitanteIngreso.ID_ESTATUS_VISITA.HasValue ? SelectVisitanteIngreso.ID_ESTATUS_VISITA.Value : (short)-1;
                            SelectAccesoUnico = SelectVisitanteIngreso.ACCESO_UNICO == "S";
                            TextObservacion = SelectVisitanteIngreso.OBSERVACION;
                            SelectTipoVisitante = SelectVisitanteIngreso.ID_TIPO_VISITANTE.HasValue ? SelectVisitanteIngreso.ID_TIPO_VISITANTE.Value : (short)-1;
                            ListAcompanantes = new ObservableCollection<ACOMPANANTE>(SelectVisitanteIngreso.ACOMPANANTE1);
                            AcompananteVisible = ListAcompanantes.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                            var hoy = Fechas.GetFechaDateServer;
                            var x = new cVisitanteIngresoPase().ObtenerTodos().Where(a =>
                                (a.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && a.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && a.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO && a.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO) ?
                                    a.ID_PASE == (short)enumTiposPases.DE_TERMINO ?
                                        a.ADUANA_INGRESO.Any()
                                    : false
                                : false);
                            PaseTerminoEnabled = SelectVisitanteIngreso.ID_PERSONA > 0 ?
                                !SelectVisitanteIngreso.INGRESO.CAUSA_PENAL.Any()
                                &&
                                !SelectVisitanteIngreso.INGRESO.ADUANA_INGRESO.Any(a => a.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && a.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && a.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                    a.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO && a.ADUANA.ID_PERSONA == SelectVisitanteIngreso.ID_PERSONA)
                                &&
                                !new cVisitanteIngresoPase().ObtenerTodos().Any(a => (a.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && a.ID_ANIO == SelectVisitanteIngreso.ID_ANIO && a.ID_IMPUTADO == SelectVisitanteIngreso.ID_IMPUTADO &&
                                a.ID_INGRESO == SelectVisitanteIngreso.ID_INGRESO) ?
                                    a.ID_PASE == (short)enumTiposPases.DE_TERMINO ?
                                        a.ADUANA_INGRESO.Any()
                                    : false
                                : false)
                            : false;
                            PaseDiasEnabled = false;
                            var contar = 0;
                            var tipoVisita = 0;
                            var tipoDocto = 0;
                            var VisitaAutorizada = Parametro.ID_ESTATUS_VISITA_AUTORIZADO;
                            PaseExtraordinarioEnabled = false;
                            if (SelectVisitanteIngreso.ID_TIPO_VISITANTE == Parametro.ID_TIPO_VISITANTE_ORDINARIO || SelectVisitanteIngreso.ID_TIPO_VISITANTE == Parametro.ID_TIPO_VISITANTE_DISCAPACITADO)
                            {
                                var DoctosFamiliares = Parametro.DOCTOS_NECESARIOS_FAMILIAR;
                                foreach (var item in DoctosFamiliares)
                                {
                                    tipoVisita = short.Parse(item.Split('-')[0]);
                                    tipoDocto = short.Parse(item.Split('-')[1]);
                                    if (SelectVisitanteIngreso.VISITA_DOCUMENTO.Any(a => a.DOCUMENTO != null ? a.ID_TIPO_VISITA == tipoVisita ? a.ID_TIPO_DOCUMENTO == tipoDocto : false : false))
                                        contar++;
                                }
                                PaseDiasEnabled = contar == DoctosFamiliares.Count() ?
                                    SelectVisitanteIngreso.ID_ESTATUS_VISITA.HasValue ?
                                        SelectVisitanteIngreso.ID_ESTATUS_VISITA.Value == Parametro.ID_ESTATUS_VISITA_EN_REVISION ?
                                            SelectVisitanteIngreso.VISITANTE_INGRESO_PASE.Any(a => (a.ID_PASE == (short)enumTiposPases.INICIAL_UNICO || a.ID_PASE == (short)enumTiposPases.POR_10_DIAS) ?
                                                a.ADUANA_INGRESO != null ?
                                                    !a.ADUANA_INGRESO.Any()
                                                : false
                                            : false)
                                        : false
                                    : false
                                : false;
                                if ((!SelectVisitanteIngreso.VISITANTE_INGRESO_PASE.Any(a => a.ID_PASE == (short)enumTiposPases.EXTRAORDINARIO &&
                                    (a.FECHA_ALTA.HasValue ? a.FECHA_ALTA.Value.Year == hoy.Year && a.FECHA_ALTA.Value.Month == hoy.Month && a.FECHA_ALTA.Value.Day == hoy.Day : false))) && contar == DoctosFamiliares.Count())
                                    PaseExtraordinarioEnabled = true;
                            }
                            else if (SelectVisitanteIngreso.ID_TIPO_VISITANTE == Parametro.ID_TIPO_VISITANTE_FORANEO)
                            {
                                var foran = Parametro.DOCUMENTOS_FORANEOS;
                                foreach (var item in foran)
                                {
                                    tipoVisita = short.Parse(item.Split('-')[0]);
                                    tipoDocto = short.Parse(item.Split('-')[1]);
                                    if (SelectVisitanteIngreso.VISITA_DOCUMENTO.Any(a => a.DOCUMENTO != null ? a.ID_TIPO_VISITA == tipoVisita ? a.ID_TIPO_DOCUMENTO == tipoDocto : false : false))
                                        contar++;
                                }
                                if ((!SelectVisitanteIngreso.VISITANTE_INGRESO_PASE.Any(a => a.ID_PASE == (short)enumTiposPases.EXTRAORDINARIO &&
                                    (a.FECHA_ALTA.HasValue ? a.FECHA_ALTA.Value.Year == hoy.Year && a.FECHA_ALTA.Value.Month == hoy.Month && a.FECHA_ALTA.Value.Day == hoy.Day : false))) && contar == foran.Count())
                                    PaseExtraordinarioEnabled = true;
                            }
                            else if (SelectVisitanteIngreso.ID_TIPO_VISITANTE == Parametro.ID_TIPO_VISITANTE_INTIMA)
                            {
                                var foran = Parametro.DOCTOS_NECESARIOS_INTIMA;
                                foreach (var item in foran)
                                {
                                    tipoVisita = short.Parse(item.Split('-')[0]);
                                    tipoDocto = short.Parse(item.Split('-')[1]);
                                    if (SelectVisitanteIngreso.VISITA_DOCUMENTO.Any(a => a.DOCUMENTO != null ? a.ID_TIPO_VISITA == tipoVisita ? a.ID_TIPO_DOCUMENTO == tipoDocto : false : false))
                                        contar++;
                                }
                                var DoctosFamiliares = Parametro.DOCTOS_NECESARIOS_FAMILIAR;
                                PaseDiasEnabled = contar == DoctosFamiliares.Count() ?
                                    SelectVisitanteIngreso.ID_ESTATUS_VISITA.HasValue ?
                                        SelectVisitanteIngreso.ID_ESTATUS_VISITA.Value == Parametro.ID_ESTATUS_VISITA_EN_REVISION ?
                                            SelectVisitanteIngreso.VISITANTE_INGRESO_PASE.Any(a => (a.ID_PASE == (short)enumTiposPases.INICIAL_UNICO || a.ID_PASE == (short)enumTiposPases.POR_10_DIAS) ?
                                                a.ADUANA_INGRESO != null ?
                                                    !a.ADUANA_INGRESO.Any()
                                                : false
                                            : false)
                                        : false
                                    : false
                                : false;
                                if (contar == foran.Count())
                                    PaseExtraordinarioEnabled = true;
                            }
                            else
                            {

                            }
                        }
                    }
                    else if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                        if (SelectVisitanteIngreso != null)
                        {
                            SelectParentesco = SelectVisitanteIngreso.ID_TIPO_REFERENCIA.HasValue ? SelectVisitanteIngreso.ID_TIPO_REFERENCIA.Value : (short)-1;
                            SelectEstatusRelacion = SelectVisitanteIngreso.ID_ESTATUS_VISITA.HasValue ? SelectVisitanteIngreso.ID_ESTATUS_VISITA.Value : (short)-1;
                            ListAcompanantes = new ObservableCollection<ACOMPANANTE>(SelectVisitanteIngreso.ACOMPANANTE.Where(w => new Fechas().CalculaEdad(w.VISITANTE_INGRESO1.VISITANTE.PERSONA.FEC_NACIMIENTO) < Parametro.MAYORIA_EDAD && new Fechas().CalculaEdad(w.VISITANTE_INGRESO1.VISITANTE.PERSONA.FEC_NACIMIENTO) > (w.VISITANTE_INGRESO1.VISITANTE.PERSONA.SEXO == "M" ? Parametro.EDAD_MENOR_M : Parametro.EDAD_MENOR_F)));
                            AcompananteVisible = ListAcompanantes.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                        }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del visitante.", ex);
            }
        }

        private async Task GetVisitasAgendadas()
        {
            try
            {
                ListProgramacionVisitaAux = new ObservableCollection<ListaVisitaAgenda>();
                var VisitaEdificio = new cVisitaEdificio().ObtenerTodosActivos(SelectImputadoIngreso.ID_CENTRO, new Nullable<int>());
                if (VisitaEdificio.Count() > 0)
                {
                    var visitaEncontrada = VisitaEdificio.Where(w => (w.CELDA_INICIO == SelectImputadoIngreso.ID_UB_CELDA || w.CELDA_FINAL == SelectImputadoIngreso.ID_UB_CELDA) &&
                        w.ID_SECTOR == SelectImputadoIngreso.ID_UB_SECTOR && w.ID_EDIFICIO == SelectImputadoIngreso.ID_UB_EDIFICIO);
                    if (!visitaEncontrada.Any())
                    {
                        var celdas = new cCelda().ObtenerPorSector(SelectImputadoIngreso.ID_UB_SECTOR, SelectImputadoIngreso.ID_UB_EDIFICIO, SelectImputadoIngreso.ID_UB_CENTRO.Value)
                            .OrderBy(o => o.ID_CELDA);
                        foreach (var itemVisita in VisitaEdificio.Where(w => w.ID_SECTOR == SelectImputadoIngreso.ID_UB_SECTOR &&
                            w.ID_EDIFICIO == SelectImputadoIngreso.ID_UB_EDIFICIO))
                        {
                            if (celdas.ToList().IndexOf(celdas.Where(w => w.ID_CELDA == SelectImputadoIngreso.ID_UB_CELDA).FirstOrDefault()) >=
                                celdas.ToList().IndexOf(celdas.Where(w => w.ID_CELDA == itemVisita.CELDA_INICIO).FirstOrDefault()) &&
                                celdas.ToList().IndexOf(celdas.Where(w => w.ID_CELDA == SelectImputadoIngreso.ID_UB_CELDA).FirstOrDefault()) <=
                                celdas.ToList().IndexOf(celdas.Where(w => w.ID_CELDA == itemVisita.CELDA_FINAL).FirstOrDefault()))
                            {
                                ListProgramacionVisitaAux.Add(new ListaVisitaAgenda
                                {
                                    HORA_FIN = itemVisita.HORA_FIN,
                                    HORA_INI = itemVisita.HORA_INI,
                                    ID_ANIO = SelectImputadoIngreso.ID_ANIO,
                                    ID_CENTRO = SelectImputadoIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectImputadoIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectImputadoIngreso.ID_INGRESO,
                                    ID_AREA = itemVisita.ID_AREA.HasValue ? itemVisita.ID_AREA.Value : new Nullable<short>(),
                                    ID_DIA = itemVisita.DIA.HasValue ? itemVisita.DIA.Value : (short)-1,
                                    ID_TIPO_VISITA = itemVisita.ID_TIPO_VISITA.HasValue ? itemVisita.ID_TIPO_VISITA.Value : (short)-1,
                                    ESTATUS = itemVisita.ESTATUS == "0",

                                    AREA = itemVisita.ID_AREA.HasValue ? itemVisita.AREA.DESCR : string.Empty,
                                    DIA = itemVisita.DIA.HasValue ? Enum.GetName(typeof(DayOfWeek), itemVisita.DIA.Value) : string.Empty,
                                    HORA_SALIDA = itemVisita.HORA_FIN.Insert(2, ":"),
                                    HORA_ENTRADA = itemVisita.HORA_INI.Insert(2, ":"),
                                    TIPO_VISITA = itemVisita.ID_TIPO_VISITA.HasValue ? itemVisita.TIPO_VISITA.DESCR : string.Empty,
                                    VISITA_EDIFICIO = itemVisita
                                });
                            }
                        }
                    }
                    else
                    {
                        ListProgramacionVisitaAux.Add(new ListaVisitaAgenda
                        {
                            HORA_FIN = visitaEncontrada.FirstOrDefault().HORA_FIN,
                            HORA_INI = visitaEncontrada.FirstOrDefault().HORA_INI,
                            ID_ANIO = SelectImputadoIngreso.ID_ANIO,
                            ID_CENTRO = SelectImputadoIngreso.ID_CENTRO,
                            ID_IMPUTADO = SelectImputadoIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectImputadoIngreso.ID_INGRESO,
                            ID_AREA = visitaEncontrada.FirstOrDefault().ID_AREA.HasValue ? visitaEncontrada.FirstOrDefault().ID_AREA.Value : new Nullable<short>(),
                            ID_DIA = visitaEncontrada.FirstOrDefault().DIA.HasValue ? visitaEncontrada.FirstOrDefault().DIA.Value : (short)-1,
                            ID_TIPO_VISITA = visitaEncontrada.FirstOrDefault().ID_TIPO_VISITA.HasValue ? visitaEncontrada.FirstOrDefault().ID_TIPO_VISITA.Value : (short)-1,
                            ESTATUS = visitaEncontrada.FirstOrDefault().ESTATUS == "0",

                            AREA = visitaEncontrada.FirstOrDefault().ID_AREA.HasValue ? visitaEncontrada.FirstOrDefault().AREA.DESCR : string.Empty,
                            DIA = visitaEncontrada.FirstOrDefault().VISITA_DIA.DESCR,
                            HORA_SALIDA = visitaEncontrada.FirstOrDefault().HORA_FIN.Insert(2, ":"),
                            HORA_ENTRADA = visitaEncontrada.FirstOrDefault().HORA_INI.Insert(2, ":"),
                            TIPO_VISITA = visitaEncontrada.FirstOrDefault().ID_TIPO_VISITA.HasValue ? visitaEncontrada.FirstOrDefault().TIPO_VISITA.DESCR : string.Empty,
                            VISITA_EDIFICIO = visitaEncontrada.FirstOrDefault()
                        });
                    }
                }
                foreach (var item in SelectImputadoIngreso.VISITA_AGENDA.Where(w => w.ESTATUS == "0"))
                {
                    ListProgramacionVisitaAux.Add(new ListaVisitaAgenda
                    {
                        HORA_FIN = item.HORA_FIN,
                        HORA_INI = item.HORA_INI,
                        ID_ANIO = item.ID_ANIO,
                        ID_CENTRO = item.ID_CENTRO,
                        ID_IMPUTADO = item.ID_IMPUTADO,
                        ID_INGRESO = item.ID_INGRESO,
                        ID_AREA = item.ID_AREA.HasValue ? item.ID_AREA.Value : (short)-1,
                        ID_DIA = item.ID_DIA,
                        ID_TIPO_VISITA = item.ID_TIPO_VISITA,
                        ESTATUS = item.ESTATUS == "0",
                        AREA = item.ID_AREA.HasValue ? item.AREA.DESCR : string.Empty,
                        DIA = item.VISITA_DIA.DESCR,
                        HORA_SALIDA = item.HORA_FIN.Insert(2, ":"),
                        HORA_ENTRADA = item.HORA_INI.Insert(2, ":"),
                        TIPO_VISITA = item.TIPO_VISITA.DESCR,
                        VISITA_AGENDA = item
                    });
                }
                foreach (var item in new cVisitaApellido().ObtenerTodosActivos(GlobalVar.gCentro, new Nullable<int>()))
                {
                    var letra = SelectImputadoIngreso.IMPUTADO.PATERNO[0].ToString();
                    if (((item.LETRA_INICIAL == letra || item.LETRA_FINAL == letra) ||
                        (ListLetras.IndexOf(item.LETRA_INICIAL) < ListLetras.IndexOf(letra) && (ListLetras.IndexOf(item.LETRA_FINAL) > ListLetras.IndexOf(letra)))))
                    {
                        ListProgramacionVisitaAux.Add(new ListaVisitaAgenda
                        {
                            HORA_FIN = item.HORA_FIN,
                            HORA_INI = item.HORA_INI,
                            ID_ANIO = SelectImputadoIngreso.ID_ANIO,
                            ID_CENTRO = SelectImputadoIngreso.ID_CENTRO,
                            ID_IMPUTADO = SelectImputadoIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectImputadoIngreso.ID_INGRESO,
                            ID_AREA = item.ID_AREA,
                            ID_DIA = item.ID_DIA.HasValue ? item.ID_DIA.Value : (short)-1,
                            ID_TIPO_VISITA = item.ID_TIPO_VISITA.HasValue ? item.ID_TIPO_VISITA.Value : (short)-1,
                            ESTATUS = item.ESTATUS == "0",
                            AREA = item.ID_AREA.HasValue ? item.AREA.DESCR : string.Empty,
                            DIA = item.ID_DIA.HasValue ? item.VISITA_DIA.DESCR : string.Empty,
                            HORA_SALIDA = item.HORA_FIN.Insert(2, ":"),
                            HORA_ENTRADA = item.HORA_INI.Insert(2, ":"),
                            TIPO_VISITA = item.ID_TIPO_VISITA.HasValue ? item.TIPO_VISITA.DESCR : string.Empty,
                            VISITA_APELLIDO = item
                        });
                    }
                }
                ListProgramacionVisita = new ObservableCollection<ListaVisitaAgenda>(ListProgramacionVisitaAux.Where(w => w.ESTATUS));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del visitante.", ex);
            }
        }

        private void SetDiaVisita(string dia)
        {
            SelectDiaVisita = dia;
        }

        private short GetDiaVisitaShort(string dia)
        {
            switch (dia)
            {
                case "DOMINGO":
                    return 1;
                case "LUNES":
                    return 2;
                case "MARTES":
                    return 3;
                case "MIÉRCOLES":
                    return 4;
                case "JUEVES":
                    return 5;
                case "VIERNES":
                    return 6;
                case "SÁBADO":
                    return 7;
                default:
                    return 0;
            }
        }

        private void SeleccionarImputado(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    {
                        if (!(obj is DataGrid))
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un imputado.");
                            return;
                        }
                        if (((DataGrid)obj).SelectedItem == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un imputado.");
                            return;
                        }
                        SituacionEnabled = false;
                        SelectVisitanteIngreso = ((VISITANTE_INGRESO)((DataGrid)obj).SelectedItem);
                        #region Obtenemos el ingreso
                        if (SelectVisitanteIngreso != null ? SelectVisitante != null : false)
                        {
                            SelectVisitante.ESTATUS_VISITA = SelectVisitanteIngreso.ESTATUS_VISITA;
                            SelectVisitante.ESTATUS_VISITA_DESCR = SelectVisitanteIngreso.ESTATUS_VISITA.DESCR;


                            SelectVisitante.INGRESO = SelectVisitanteIngreso.INGRESO;
                            SelectVisitante.PARENTESCO_DESCR = SelectVisitanteIngreso.TIPO_REFERENCIA.DESCR;
                            SelectVisitante.ID_PARENTESCO = SelectVisitanteIngreso.TIPO_REFERENCIA.ID_TIPO_REFERENCIA;
                            SelectVisitante.TIPO_REFERENCIA = SelectVisitanteIngreso.TIPO_REFERENCIA != null ? SelectVisitanteIngreso.TIPO_REFERENCIA : new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = 9999, DESCR = "SIN INFORMACIÓN" };
                            SelectVisitante.INGRESO_NOMBRE = SelectVisitanteIngreso.INGRESO.IMPUTADO.NOMBRE;
                            SelectVisitante.INGRESO_PATERNO = SelectVisitanteIngreso.INGRESO.IMPUTADO.PATERNO;
                            SelectVisitante.INGRESO_MATERNO = SelectVisitanteIngreso.INGRESO.IMPUTADO.MATERNO;
                            SelectVisitante.ID_CENTRO = SelectVisitanteIngreso.ID_CENTRO;
                            SelectVisitante.ID_ANIO = SelectVisitanteIngreso.INGRESO.ID_ANIO;
                            SelectVisitante.ID_IMPUTADO = SelectVisitanteIngreso.INGRESO.ID_IMPUTADO;
                        }
                        #endregion

                        var vi = new cVisitanteIngreso().ObtenerVisitanteIngreso(SelectVisitanteIngreso.ID_CENTRO, SelectVisitanteIngreso.ID_ANIO, SelectVisitanteIngreso.ID_IMPUTADO, SelectVisitanteIngreso.ID_INGRESO,
                            TextCodigo != null ? TextCodigo.Value : 0).FirstOrDefault();
                        if (vi != null)
                            SelectVisitanteIngreso = vi;
                        SelectImputadoIngreso = SelectVisitanteIngreso.INGRESO;
                        GetDatosIngresoImputadoSeleccionado();
                        IsDetalleInternosEnable = false;
                        TextHeaderDatosInterno = "Datos del Interno Seleccionado";
                        DatosExpedienteVisible = Visibility.Visible;
                        SelectParentescoIngresoEnabled = true;
                        MenuGuardarEnabled = !PInsertar ? PEditar : true;
                        TextObservacion = SelectVisitanteIngreso.OBSERVACION;
                        SelectParentescoAcompanante = -1;
                        if (SelectDiscapacidad == null || SelectDiscapacidad == 0)
                            SelectDiscapacitado = "N";
                        else
                            SelectDiscapacitado = "S";
                        ListBuscarAcompanantes = new ObservableCollection<VISITANTE_INGRESO>(SelectImputadoIngreso.VISITANTE_INGRESO.Where(w =>
                            w.ID_PERSONA != SelectVisitanteIngreso.ID_PERSONA &&
                            !ListAcompanantes.Where(x => x.ID_ACOMPANANTE == w.ID_PERSONA).Any() &&
                            new Fechas().CalculaEdad(w.VISITANTE.PERSONA.FEC_NACIMIENTO.HasValue ?
                                w.VISITANTE.PERSONA.FEC_NACIMIENTO : DateTime.Parse("01/01/1900")) >= 18 &&
                            (w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_EN_REVISION || w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_AUTORIZADO)));
                        EmptyBuscarAcompanante = ListBuscarAcompanantes.Count == 0;
                    }
                    else if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    {
                        if (obj is DataGrid)
                        {
                            if (((DataGrid)obj).SelectedItem is PERSONAVISITAAUXILIAR)
                            {
                                SelectVisitanteInterno = ((PERSONAVISITAAUXILIAR)((DataGrid)obj).SelectedItem);
                                if (SelectVisitanteInterno != null)
                                {
                                    SituacionEnabled = false;
                                    SetValidacionesHojaVisita();
                                    CapturarVisitanteVisible = CapturarVisitanteEnabled = GeneralEnabled = ValidarEnabled = BanderaEditar =
                                        ProgramacionVisitasMenuEnabled = true;
                                    MenuGuardarEnabled = !PInsertar ? PEditar : true;
                                    GetDatosVisitanteSeleccionadoInternoAsignacion();
                                    CodigoEnabled = true;
                                }
                            }
                        }
                        else
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un imputado.");
                    }
                    else if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    {
                        if (!(obj is DataGrid))
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un imputado.");
                            return;
                        }
                        if (((DataGrid)obj).SelectedItem == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un imputado.");
                            return;
                        }
                        SituacionEnabled = false;
                        SelectVisitanteIngreso = ((VISITANTE_INGRESO)((DataGrid)obj).SelectedItem);
                        SelectImputadoIngreso = SelectVisitanteIngreso.INGRESO;
                        GetDatosIngresoImputadoSeleccionado();
                        IsDetalleInternosEnable = false;
                        TextHeaderDatosInterno = "Datos del Interno Seleccionado";
                        DatosExpedienteVisible = Visibility.Visible;
                        MenuGuardarEnabled = !PInsertar ? PEditar : true;
                        TextObservacion = SelectVisitanteIngreso.OBSERVACION;
                        SelectParentescoAcompanante = -1;
                        SelectTipoVisitante = SelectVisitanteIngreso.ID_TIPO_VISITANTE;
                        ListBuscarAcompanantes = new ObservableCollection<VISITANTE_INGRESO>(SelectImputadoIngreso.VISITANTE_INGRESO.Where(w =>
                            w.ID_PERSONA != SelectVisitanteIngreso.ID_PERSONA &&
                            !ListAcompanantes.Where(x => x.ID_ACOMPANANTE == w.ID_PERSONA).Any() &&
                            new Fechas().CalculaEdad(w.VISITANTE.PERSONA.FEC_NACIMIENTO.HasValue ? w.VISITANTE.PERSONA.FEC_NACIMIENTO : DateTime.Parse("01/01/1900")) >= 18 &&
                            (w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_EN_REVISION || w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_AUTORIZADO)));
                        EmptyBuscarAcompanante = ListBuscarAcompanantes.Count == 0;
                    }
                    OnPropertyChanged();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del visitante.", ex);
            }
        }

        private void SeleccionarAcompaniante(object obj)
        {
            if (obj is ACOMPANANTE)
                if (((ACOMPANANTE)obj).VISITANTE_INGRESO1.VISITANTE.PERSONA.PERSONA_BIOMETRICO != null)
                    if (((ACOMPANANTE)obj).VISITANTE_INGRESO1.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                        ImagenAcompanante = ((ACOMPANANTE)obj).VISITANTE_INGRESO1.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
        }

        #endregion

        private bool HasErrors()
        {
            return base.HasErrors;
        }

        private async void Scan(PdfViewer obj)
        {
            try
            {
                await Task.Factory.StartNew(async () =>
                {
                    await escaner.Scann(Duplex, SelectedSource, obj);
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al escanear el documento.", ex);
            }
        }

        private void GuardarDocumento()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    escaner.Hide();

                    if (SelectedTipoDocumento == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Elija El Tipo De Documento A Digitalizar");
                        return;
                    }
                    if (DocumentoDigitalizado == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Digitalice Un Documento Para Guardar");
                        return;
                    }
                    if (DocumentoDigitalizado.Length <= 0)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Digitalice Un Documento Para Guardar");
                        return;
                    }
                    if (SelectVisitante.ID_PERSONA <= 0)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "No Esta Autorizado Para Ser Visitante");
                        return;
                    }
                    var VisitanteIntimo = Parametro.ID_TIPO_VISITANTE_INTIMA;
                    var hoy = Fechas.GetFechaDateServer;
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            var Documentos = new cVisitaDocumento().GetData().Where(w => w.ID_CENTRO == SelectVisitante.ID_CENTRO &&
                                w.ID_ANIO == SelectVisitante.ID_ANIO.Value && w.ID_IMPUTADO == SelectVisitante.ID_IMPUTADO &&
                                w.ID_INGRESO == SelectVisitante.INGRESO.ID_INGRESO && w.ID_PERSONA == SelectVisitante.OBJETO_PERSONA.ID_PERSONA &&
                                w.ID_TIPO_DOCUMENTO == SelectedTipoDocumento.ID_TIPO_DOCUMENTO && w.ID_TIPO_VISITA == SelectedTipoDocumento.ID_TIPO_VISITA).FirstOrDefault();

                            if (Documentos == null)
                                if (new cVisitaDocumento().Insertar(new VISITA_DOCUMENTO
                                {
                                    ID_PERSONA = SelectVisitante.OBJETO_PERSONA.ID_PERSONA,
                                    CAPTURA_FEC = DatePickCapturaDocumento,
                                    DOCUMENTO = DocumentoDigitalizado,
                                    ID_ANIO = SelectVisitante.ID_ANIO.Value,
                                    ID_CENTRO = SelectVisitante.ID_CENTRO.Value,
                                    ID_IMPUTADO = SelectVisitante.ID_IMPUTADO.Value,
                                    ID_INGRESO = SelectVisitante.INGRESO.ID_INGRESO,
                                    ID_TIPO_DOCUMENTO = SelectedTipoDocumento.ID_TIPO_DOCUMENTO,
                                    ID_TIPO_VISITA = SelectedTipoDocumento.ID_TIPO_VISITA,
                                    OBSERV = ObservacionDocumento
                                }))
                                {
                                    SelectedTipoDocumento.DIGITALIZADO = SelectAccesoUnico ? true : SelectedTipoDocumento.DIGITALIZADO;
                                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                    {
                                        StaticSourcesViewModel.Mensaje("Digitalización", "Documento Grabado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                    }));
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                    {
                                        StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un error al Grabar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                    }));
                                }
                            else
                                if (new cVisitaDocumento().Actualizar(new VISITA_DOCUMENTO
                                {
                                    CAPTURA_FEC = DatePickCapturaDocumento,
                                    DOCUMENTO = DocumentoDigitalizado,
                                    ID_ANIO = SelectVisitante.ID_ANIO.Value,
                                    ID_CENTRO = SelectVisitante.ID_CENTRO.Value,
                                    ID_IMPUTADO = SelectVisitante.ID_IMPUTADO.Value,
                                    ID_INGRESO = SelectVisitante.INGRESO.ID_INGRESO,
                                    ID_TIPO_DOCUMENTO = SelectedTipoDocumento.ID_TIPO_DOCUMENTO,
                                    ID_TIPO_VISITA = SelectedTipoDocumento.ID_TIPO_VISITA,
                                    ID_PERSONA = SelectVisitante.OBJETO_PERSONA.ID_PERSONA,
                                    OBSERV = ObservacionDocumento
                                }))
                                {
                                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                    {
                                        StaticSourcesViewModel.Mensaje("Digitalización", "Documento Actualizado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                    }));
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                    {
                                        StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un error al Actualizar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                    }));
                                }
                            CargarListaTipoDocumentoDigitalizado();
                        });
                    //}
                    DocumentoDigitalizado = null;
                    if (ValidarDocumentacionCompleta())
                    {
                        var ingres = SelectVisitante.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO.Where(w =>
                                w.INGRESO.ID_INGRESO == SelectImputadoIngreso.ID_INGRESO && w.ID_IMPUTADO == SelectImputadoIngreso.ID_IMPUTADO &&
                                w.ID_ANIO == SelectImputadoIngreso.ID_ANIO && w.ID_CENTRO == SelectImputadoIngreso.ID_CENTRO);
                        if (!ingres.Any())
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Error!", "Al ligar documentos con el visitante.");
                            return;
                        }
                        if (ingres.FirstOrDefault().ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_REGISTRO || (SelectTipoVisitante == VisitanteIntimo && ingres.FirstOrDefault().ID_TIPO_VISITANTE != VisitanteIntimo))
                        {
                            #region VISITANTE_INGRESO
                            //short? estatus = 0;
                            //if (SelectAccesoUnico)
                                var estatus = SelectAccesoUnico ? Parametro.ID_ESTATUS_VISITA_AUTORIZADO : Parametro.ID_ESTATUS_VISITA_EN_REVISION;
                            var visitIngr = new VISITANTE_INGRESO
                            {
                                EMISION_GAFETE = "N",
                                FEC_ALTA = ingres.FirstOrDefault().FEC_ALTA,
                                FEC_ULTIMA_MOD = hoy,
                                ID_ANIO = ingres.FirstOrDefault().ID_ANIO,
                                ID_CENTRO = ingres.FirstOrDefault().ID_CENTRO,
                                ID_ESTATUS_VISITA = estatus,
                                ID_IMPUTADO = ingres.FirstOrDefault().ID_IMPUTADO,
                                ID_INGRESO = ingres.FirstOrDefault().ID_INGRESO,
                                ID_PERSONA = SelectVisitante.OBJETO_PERSONA.ID_PERSONA,
                                ID_TIPO_REFERENCIA = ingres.FirstOrDefault().ID_TIPO_REFERENCIA,
                                OBSERVACION = ingres.FirstOrDefault().OBSERVACION,
                                ESTATUS_MOTIVO = ingres.FirstOrDefault().ESTATUS_MOTIVO,
                                ACCESO_UNICO = ingres.FirstOrDefault().ACCESO_UNICO,
                                ID_TIPO_VISITANTE = ingres.FirstOrDefault().ID_TIPO_VISITANTE,
                            };
                            if (new cVisitanteIngreso().Actualizar(visitIngr))
                            {
                                #region VISITANTE
                                if (SelectTipoVisitante == VisitanteIntimo && visitIngr.ID_TIPO_VISITANTE != VisitanteIntimo)
                                {
                                    new cVisitante().Actualizar(new VISITANTE
                                    {
                                        ID_ESTATUS_VISITA = SelectVisitante.OBJETO_PERSONA.VISITANTE.ID_ESTATUS_VISITA,
                                        FEC_ALTA = SelectVisitante.OBJETO_PERSONA.VISITANTE.FEC_ALTA,
                                        ID_PERSONA = SelectVisitante.OBJETO_PERSONA.ID_PERSONA,
                                        ULTIMA_MODIFICACION = hoy,
                                        ESTATUS_MOTIVO = SelectVisitante.OBJETO_PERSONA.VISITANTE.ESTATUS_MOTIVO
                                    });
                                }
                                #endregion

                                #region PASES
                                var personaIngreso = new cPasesVisitanteIngreso().ObtenerSiguienteConsecutivo(SelectImputadoIngreso.ID_CENTRO, SelectImputadoIngreso.ID_ANIO,
                                    SelectImputadoIngreso.ID_IMPUTADO, SelectImputadoIngreso.ID_INGRESO, SelectVisitante.OBJETO_PERSONA.ID_PERSONA);
                                var pase = new cPasesVisitanteIngreso().Insertar(new VISITANTE_INGRESO_PASE
                                {
                                    ID_ANIO = SelectImputadoIngreso.ID_ANIO,
                                    ID_CENTRO = SelectImputadoIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectImputadoIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectImputadoIngreso.ID_INGRESO,
                                    ID_PERSONA = SelectVisitante.OBJETO_PERSONA.ID_PERSONA,
                                    ID_PASE = 1,
                                    FECHA_ALTA = Fechas.GetFechaDateServer,
                                    ID_CONSEC = personaIngreso > 0 ? (short)(personaIngreso + 1) : (short)1
                                });
                                #endregion

                                SelectVisitante = ConvertPersonaToAuxiliar(new cPersona().ObtenerPersonaXID(SelectVisitante.OBJETO_PERSONA.ID_PERSONA).FirstOrDefault());
                                BanderaEditar = SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.ID_PERSONA != null : false;
                                SetValidacionesGenerales();
                                GetDatosVisitanteSeleccionadoPadron();
                                FotoHuellaEnabled = ValidarEnabled = GeneralEnabled = true;
                                MenuGuardarEnabled = !PInsertar ? PEditar : true;
                                DigitalizarDocumentosEnabled = ContextMenuEnabled = SelectVisitante.ID_PERSONA != null;
                                SetValidacionesGenerales();
                            }
                            #endregion

                        }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                        Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Expediente creado con éxito, su pase esta en estatus de " + (SelectAccesoUnico ? "\"AUTORIZADO\"" : "\"REVISION\""));
                        }));
                    }
                    else
                        if (AutoGuardado)
                            escaner.Show();
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al digitalizar documento.", ex);
            }
        }

        private async void AbrirDocumento(PdfViewer obj)
        {
            try
            {
                if (SelectedTipoDocumento == null)
                {
                    escaner.Hide();
                    await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Elija El Tipo De Documento A Digitalizar");
                    return;
                }
                if (SelectVisitante == null)
                {
                    escaner.Hide();
                    await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "No Esta Autorizado Para Ser Visitante");
                    return;
                }
                if (SelectVisitante != null ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.ID_PERSONA <= 0 : false : false)
                {
                    escaner.Hide();
                    await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "No Esta Autorizado Para Ser Visitante");
                    return;
                }
                escaner.Show();
                var Documentos = new cVisitaDocumento().GetData().Where(w => w.ID_CENTRO == SelectVisitante.ID_CENTRO &&
                    w.ID_ANIO == SelectVisitante.ID_ANIO.Value && w.ID_IMPUTADO == SelectVisitante.ID_IMPUTADO &&
                    w.ID_INGRESO == SelectVisitante.INGRESO.ID_INGRESO && w.ID_PERSONA == SelectVisitante.OBJETO_PERSONA.ID_PERSONA &&
                    w.ID_TIPO_DOCUMENTO == SelectedTipoDocumento.ID_TIPO_DOCUMENTO && w.ID_TIPO_VISITA == SelectedTipoDocumento.ID_TIPO_VISITA).FirstOrDefault();
                if (Documentos == null)
                {
                    StaticSourcesViewModel.Mensaje("Digitalización", "Documento No Digitalizado", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                    obj.Visibility = Visibility.Collapsed;
                    DocumentoDigitalizado = null;
                    return;
                }
                if (Documentos.DOCUMENTO == null)
                {
                    StaticSourcesViewModel.Mensaje("Digitalización", "Documento No Digitalizado", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                    obj.Visibility = Visibility.Collapsed;
                    DocumentoDigitalizado = null;
                    return;
                }
                DocumentoDigitalizado = Documentos.DOCUMENTO;
                ObservacionDocumento = Documentos.OBSERV;
                DatePickCapturaDocumento = Documentos.CAPTURA_FEC;
                if (DocumentoDigitalizado == null)
                    return;
                await Task.Factory.StartNew(() =>
                {
                    var fileNamepdf = Path.GetTempPath() + Path.GetRandomFileName().Split('.')[0] + ".pdf";
                    File.WriteAllBytes(fileNamepdf, DocumentoDigitalizado);
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        obj.LoadFile(fileNamepdf);
                        obj.Visibility = Visibility.Visible;
                    }));
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al abrir documento.", ex);
            }
        }

        void CargarListaTipoDocumentoDigitalizado()
        {
            try
            {
                if (SelectVisitante == null)
                {
                    StaticSourcesViewModel.Mensaje("Validación", "Favor de seleccionar un visitante", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                    return;
                }

                var ListDocumentos = new List<VISITA_DOCUMENTO>();
                var ListTipoDocumentoAux = new ObservableCollection<TipoDocumento>(new cTipoDocumento().GetData().Select(s => new TipoDocumento
                    {
                        DESCR = s.DESCR,
                        DIGITALIZADO = false,
                        ID_TIPO_DOCUMENTO = s.ID_TIPO_DOCUMENTO,
                        ID_TIPO_VISITA = s.ID_TIPO_VISITA
                    }).OrderBy(o => o.DESCR).ToList());
                ListDocumentos = new cVisitaDocumento().GetData().Where(w => w.ID_CENTRO == SelectVisitante.ID_CENTRO &&
                w.ID_ANIO == SelectVisitante.ID_ANIO.Value && w.ID_IMPUTADO == SelectVisitante.ID_IMPUTADO &&
                w.ID_INGRESO == SelectVisitante.INGRESO.ID_INGRESO &&
                w.ID_PERSONA == SelectVisitante.OBJETO_PERSONA.ID_PERSONA).ToList();
                foreach (var item in ListDocumentos)
                {
                    ListTipoDocumentoAux.Where(w => w.ID_TIPO_DOCUMENTO == item.ID_TIPO_DOCUMENTO &&
                        w.ID_TIPO_VISITA == item.ID_TIPO_VISITA).FirstOrDefault().DIGITALIZADO = true;
                }
                ListTipoDocumento = new ObservableCollection<TipoDocumento>(ListTipoDocumentoAux.Where(w =>
                    (SelectTipoVisitante == Parametro.ID_TIPO_VISITANTE_INTIMA ? (w.ID_TIPO_VISITA == Parametro.ID_TIPO_VISITA_INTIMA || w.ID_TIPO_VISITA == Parametro.ID_TIPO_VISITA_FAMILIAR) :
                    (w.ID_TIPO_VISITA == Parametro.ID_TIPO_VISITA_FAMILIAR))).OrderBy(o => o.ID_TIPO_VISITA).ThenBy(t => t.ID_TIPO_DOCUMENTO));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los tipos de documento.", ex);
            }
        }

        private void TipoPaseOpen(Object obj)
        {
            TipoPaseAbre = true;
        }

        private bool ValidaDocumentosCompletos()
        {
            try
            {
                var lista = new ObservableCollection<TipoDocumento>(ListTipoDocumento);
                var parametros = Parametro.DOCUMENTOS_NO_NECESARIOS;
                if (parametros != null)
                {
                    var tipo_documento = 0;
                    var tipo_visita = 0;
                    var obj = new List<TipoDocumento>().AsEnumerable();
                    foreach (var p in parametros)
                    {
                        tipo_documento = short.Parse(p.Split('-')[1]);
                        tipo_visita = short.Parse(p.Split('-')[0]);
                        obj = lista.Where(w => w.ID_TIPO_DOCUMENTO == tipo_documento && w.ID_TIPO_VISITA == tipo_visita);
                        if (obj != null)
                        {
                            var o = obj.FirstOrDefault();
                            lista.Remove(o);
                        }
                    }
                }
                if (lista.Where(w => w.DIGITALIZADO == false).Count() > 0)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar los documentos.", ex);
                return false;
            }
        }

        private void ImprimirPasesPorAutorizar()
        {
            try
            {
                var lista = new cVisitanteIngresoPase().ObtenerTodos();
                if (lista != null)
                {
                    var centro = new cCentro().Obtener(GlobalVar.gCentro).SingleOrDefault();
                    //DATOS DEL REPORTE
                    var reporte = new List<cReporte>();
                    reporte.Add(new cReporte()
                    {
                        Logo1 = Parametro.REPORTE_LOGO1,
                        Logo2 = Parametro.REPORTE_LOGO2,
                        Encabezado1 = Parametro.ENCABEZADO1,
                        Encabezado2 = Parametro.ENCABEZADO2,
                        Encabezado3 = centro.DESCR.Trim().ToUpper(),
                        Encabezado4 = "PASES POR AUTORIZAR"
                    });
                    var header = new cCabeceraReporte();
                    var listReporte = new List<cPasesPorAutorizar>();
                    foreach (var p in lista)
                    {
                        listReporte.Add(
                            new cPasesPorAutorizar()
                            {
                                Vigencia = p.ID_PASE == 2 ? "10 Dias" : p.ID_PASE == 3 ? "1 Dia" : string.Empty,
                                NombreInterno = p.VISITANTE_INGRESO.INGRESO.IMPUTADO.NOMBRE.Trim() + " " + p.VISITANTE_INGRESO.INGRESO.IMPUTADO.PATERNO.Trim() + " " + p.VISITANTE_INGRESO.INGRESO.IMPUTADO.MATERNO.Trim(),
                                NombreVisitante = p.VISITANTE_INGRESO.VISITANTE.PERSONA.NOMBRE.Trim() + " " + p.VISITANTE_INGRESO.VISITANTE.PERSONA.PATERNO.Trim() + " " + p.VISITANTE_INGRESO.VISITANTE.PERSONA.MATERNO.Trim(),
                                Parentesco = p.VISITANTE_INGRESO.TIPO_REFERENCIA.DESCR.Trim(),
                                Autorizo = string.Empty,
                                DiaAlta = p.FECHA_ALTA.Value.ToString("dd/MM/yyyy"),
                                DiaIngreso = string.Empty
                            });
                    }
                    var view = new ReportesView();
                    view.Owner = PopUpsViewModels.MainWindow;
                    view.Show();
                    view.Report.LocalReport.ReportPath = "Reportes/rPasesPorAutorizar.rdlc";
                    view.Report.LocalReport.DataSources.Clear();
                    var ds1 = new List<cCabeceraReporte>();
                    ds1.Add(header);
                    Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds1.Name = "DataSet1";
                    rds1.Value = ds1;
                    view.Report.LocalReport.DataSources.Add(rds1);
                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet2";
                    rds2.Value = listReporte;
                    view.Report.LocalReport.DataSources.Add(rds2);
                    Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds3.Name = "DataSet3";
                    rds3.Value = reporte;
                    view.Report.LocalReport.DataSources.Add(rds3);
                    view.Report.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte para imprimir.", ex);
            }
        }

        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.PADRON_VISITAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        PInsertar = true;
                    if (p.EDITAR == 1)
                        PEditar = true;
                    if (p.CONSULTAR == 1)
                        PConsultar = true;
                    if (p.IMPRIMIR == 1)
                        PImprimir = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }

        #region Documentos
        private bool ValidarDocumentacionCompleta()
        {
            try
            {
                var vd = new cVisitaDocumento().ObtenerTodos(SelectVisitanteIngreso.ID_CENTRO, SelectVisitanteIngreso.ID_ANIO, SelectVisitanteIngreso.ID_IMPUTADO, SelectVisitanteIngreso.ID_INGRESO, SelectVisitanteIngreso.ID_PERSONA);
                bool visitaIntima = false;
                string[] Documentos = null;
                if (SelectTipoVisitante == VisitanteOrdinario || SelectTipoVisitante == VisitanteDiscapacidad)
                    Documentos = Parametro.DOCTOS_NECESARIOS_FAMILIAR;
                else
                    if (SelectTipoVisitante == VisitanteIntima)
                    {
                        visitaIntima = true;
                        Documentos = Parametro.DOCTOS_NECESARIOS_INTIMA;
                    }
                if (Documentos != null)
                {
                    var tipoVisita = short.Parse(Documentos.FirstOrDefault().Split('-')[0]);
                    var tipoDocto = (short)0;
                    foreach (var itm in Documentos)
                    {
                        tipoDocto = short.Parse(itm.Split('-')[1]);
                        if (!vd.Where(w => w.ID_TIPO_VISITA == tipoVisita && w.ID_TIPO_DOCUMENTO == tipoDocto).Any())
                        {
                            return false;
                        }
                    }
                }
                if (visitaIntima)
                {
                    string[] DocumentosIntima = null;
                    DocumentosIntima = Parametro.DOCTOS_FAMILIAR_INTIMA;
                    if (DocumentosIntima != null)
                    {
                        var tipoVisitaI = short.Parse(DocumentosIntima.FirstOrDefault().Split('-')[0]);
                        var tipoDoctoI = (short)0;
                        foreach (var itm in DocumentosIntima)
                        {
                            tipoDoctoI = short.Parse(itm.Split('-')[1]);
                            if (!SelectVisitanteIngreso.VISITA_DOCUMENTO.Where(w => w.ID_TIPO_VISITA == tipoVisitaI && w.ID_TIPO_DOCUMENTO == tipoDoctoI).Any())
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar si los documentos", ex);
                return false;
            }
        }
        #endregion

        #region Programacion Visitas
        private void LimpiarProgramacionVisita()
        {
            HoraLowerVal = new DateTime(0001, 01, 01, 07, 0, 0);
            HoraUpperVal = new DateTime(0001, 01, 01, 19, 0, 0);
            PVDomingo = PVLunes = PVMartes = PVMiercoles = PVJueves = PVViernes = PVSabado = false;
            SelectAreaVisita = -1;
            SelectDiaVisita = string.Empty;
            SelectTipoVisita = -1;
            SelectHoraEntrada = "07";
            SelectMinutoEntrada = "00";
            SelectHoraSalida = "19";
            SelectMinutoSalida = "00";
        }

        private void ProgramarVisitaLoad(ProgramarVisitaView Window)
        {
            try
            {
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la programacion de visitas.", ex);
            }
        }
        #endregion

        #region CONFIRMAR POR HUELLA
        private void LimpiarCampos()
        {
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                ScannerMessage = "Capture Huella";
                ColorMessage = new SolidColorBrush(Colors.Green);
                AceptarBusquedaHuellaFocus = true;
            }));
            _SelectRegistro = null;
            PropertyImage = null;
        }

        private void OnLoad(BuscarPorHuellaYNipView Window)
        {
            try
            {
                BuscarPor = enumTipoPersona.PERSONA_TODOS;
                ListResultado = null;
                PropertyImage = null;
                FotoRegistro = new Imagenes().getImagenPerson();
                TextNipBusqueda = string.Empty;
                var myDoubleAnimation = new DoubleAnimation();
                myDoubleAnimation.From = 0;
                myDoubleAnimation.To = 185;
                myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.3));
                myDoubleAnimation.AutoReverse = true;
                myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                Storyboard.SetTargetName(myDoubleAnimation, "Ln");
                Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Canvas.TopProperty));
                var myStoryboard = new Storyboard();
                myStoryboard.Children.Add(myDoubleAnimation);
                myStoryboard.Begin(Window.Ln);
                Window.Closed += (s, e) =>
                {
                    try
                    {
                        if (OnProgress == null) return;
                        if (!_IsSucceed) SelectRegistro = null;
                        OnProgress.Abort();
                        CancelCaptureAndCloseReader(OnCaptured);
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar busqueda", ex);
                    }
                };
                if (CurrentReader != null)
                {
                    CurrentReader.Dispose();
                    CurrentReader = null;
                }
                CurrentReader = Readers[0];
                if (CurrentReader == null) return;
                if (!OpenReader()) Window.Close();
                if (!StartCaptureAsync(OnCaptured)) Window.Close();
                OnProgress = new Thread(() => InvokeDelegate(Window));
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ScannerMessage = "Capture Huella";
                    ColorMessage = new SolidColorBrush(Colors.Green);
                }));
                GuardandoHuellas = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos de la busqueda por huella.", ex);
            }
        }

        private async void Aceptar(Window Window)
        {
            try
            {
                if (ScannerMessage.Contains("Procesando...")) return;
                CancelKeepSearching = true;
                isKeepSearching = true;
                await WaitForFingerPrints();
                _IsSucceed = true;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    try
                    {
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
                    }
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
            }
        }

        public async override void OnCaptured(DPUruNet.CaptureResult captureResult)
        {
            try
            {
                if (ScannerMessage.Contains("Procesando...")) return;
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    TextNipBusqueda = string.Empty;
                    PropertyImage = new BitmapImage();
                    ShowLoading = Visibility.Visible;
                    ShowCapturar = Visibility.Collapsed;
                    ShowLine = Visibility.Visible;
                    ScannerMessage = "Procesando...";
                    ColorMessage = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
                }));
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    base.OnCaptured(captureResult);
                }));
                ListResultado = null;
                switch (BuscarPor)
                {
                    case enumTipoPersona.IMPUTADO:
                        await CompareImputado();
                        break;
                    case enumTipoPersona.PERSONA_TODOS:
                    case enumTipoPersona.PERSONA_VISITA:
                    case enumTipoPersona.PERSONA_ABOGADO:
                    case enumTipoPersona.PERSONA_EMPLEADO:
                    case enumTipoPersona.PERSONA_EXTERNA:
                        await ComparePersona();
                        break;
                    default:
                        break;
                }
                GuardandoHuellas = true;
                ShowLoading = Visibility.Collapsed;
                ShowCapturar = Conectado ? Visibility.Visible : Visibility.Collapsed;
                ShowLine = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda por huella.", ex);
            }
        }

        private async void Capture(string obj)
        {
            try
            {
                ShowLoading = Visibility.Visible;
                ShowLine = Visibility.Visible;
                var nRet = -1;
                try
                {
                    CLSFPCaptureDllWrapper.CLS_SetLanguage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_RESOURCE.ENGLISH);
                    nRet = CLSFPCaptureDllWrapper.CLS_CaptureFP(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_TYPE.IDFLATS);
                    ShowCapturar = Visibility.Collapsed;
                    #region [Huellas]
                    if (nRet == 0)
                    {
                        ScannerMessage = "Procesando...";
                        ShowLine = Visibility.Visible;
                        ListResultado = null;
                        HuellasCapturadas = new List<PlantillaBiometrico>();

                        for (short i = 1; i <= 10; i++)
                        {
                            var pBuffer = IntPtr.Zero;
                            var nBufferLength = 0;
                            var nNFIQ = 0;
                            ListResultado = null;
                            GuardandoHuellas = false;
                            CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.BMP, ref pBuffer, ref nBufferLength);
                            var bufferBMP = new byte[nBufferLength];
                            if (pBuffer != IntPtr.Zero) Marshal.Copy(pBuffer, bufferBMP, 0, nBufferLength);
                            CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.WSQ, ref pBuffer, ref nBufferLength);
                            var bufferWSQ = new byte[nBufferLength];
                            if (pBuffer != IntPtr.Zero) Marshal.Copy(pBuffer, bufferWSQ, 0, nBufferLength);
                            CLSFPCaptureDllWrapper.CLS_GetImageNFIQ(((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i), ref nNFIQ, CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN);
                            Fmd FMD = null;
                            if (bufferBMP.Length != 0)
                            {
                                PropertyImage = CreateBitmapSourceFromBitmap(new MemoryStream(bufferBMP));
                                FMD = ExtractFmdfromBmp(new Bitmap(new MemoryStream(bufferBMP)).Clone(new Rectangle(0, 0, 357, 392), System.Drawing.Imaging.PixelFormat.Format8bppIndexed)).Data;
                            }
                            ShowContinuar = Visibility.Collapsed;
                            await TaskEx.Delay(1);
                            switch ((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i)
                            {
                                #region [Pulgar Derecho]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_THUMB:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, (short)enumTipoBiometrico.PULGAR_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, (short)enumTipoBiometrico.PULGAR_DERECHO);
                                    break;
                                #endregion
                                #region [Indice Derecho]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_INDEX:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.INDICE_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.INDICE_DERECHO);
                                    break;
                                #endregion
                                #region [Medio Derecho]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_MIDDLE:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MEDIO_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MEDIO_DERECHO);
                                    break;
                                #endregion
                                #region [Anular Derecho]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_RING:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.ANULAR_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.ANULAR_DERECHO);
                                    break;
                                #endregion
                                #region [Meñique Derecho]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_LITTLE:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MENIQUE_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MENIQUE_DERECHO);
                                    break;
                                #endregion
                                #region [Pulgar Izquierdo]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_THUMB:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.PULGAR_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, (short)enumTipoBiometrico.PULGAR_DERECHO);
                                    break;
                                #endregion
                                #region [Indice Izquierdo]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_INDEX:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.INDICE_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.INDICE_IZQUIERDO);
                                    break;
                                #endregion
                                #region [Medio Izquierdo]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_MIDDLE:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MEDIO_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MEDIO_IZQUIERDO);
                                    break;
                                #endregion
                                #region [Anular Izquierdo]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_RING:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.ANULAR_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.ANULAR_IZQUIERDO);
                                    break;
                                #endregion
                                #region [Meñique Izquierdo]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_LITTLE:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MENIQUE_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MENIQUE_IZQUIERDO);
                                    isKeepSearching = true;
                                    break;
                                #endregion
                                default:
                                    break;
                            }

                            ShowContinuar = Visibility.Visible;
                            ShowCapturar = Visibility.Collapsed;
                            if (!CancelKeepSearching) await KeepSearch();
                            else
                                if (!_GuardarHuellas) break;
                        }

                        GuardandoHuellas = true;
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            ScannerMessage = "Vuelve a capturar las huellas";
                            ColorMessage = new SolidColorBrush(Colors.DarkOrange);
                        }));
                    }
                    #endregion
                }
                catch
                {
                    CLSFPCaptureDllWrapper.CLS_Terminate();
                }

                if (nRet == 0)
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = "Busqueda Terminada";
                        ColorMessage = new SolidColorBrush(Colors.Green);
                        AceptarBusquedaHuellaFocus = true;
                    }));
                ShowLine = Visibility.Collapsed;
                ShowLoading = Visibility.Collapsed;
                ShowContinuar = Visibility.Collapsed;
                await TaskEx.Delay(1500);
                ShowCapturar = Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda por huella.", ex);
            }

        }

        private async Task WaitForFingerPrints()
        {
            await Task.Factory.StartNew(() =>
            {
                while (!GuardandoHuellas) ;
            });
        }

        private Task<bool> CompareImputado(byte[] Huella = null, enumTipoBiometrico? Finger = null)
        {
            try
            {
                var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null ?? Huella;
                var verifyFinger = Finger ?? (DD_Dedo.HasValue ? DD_Dedo.Value : enumTipoBiometrico.INDICE_DERECHO);
                if (bytesHuella == null)
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = Finger == null ? "Vuelve a capturar las huellas" : ScannerMessage = "Siguiente Huella";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.DarkOrange);
                        ShowLine = Visibility.Collapsed;
                    }));
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ScannerMessage = "Procesando...";
                    ColorMessage = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
                    AceptarBusquedaHuellaFocus = false;
                }));
                var Service = new BiometricoServiceClient();
                if (Service == null)
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = "Error en el servicio de comparacion";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.Red);
                        ShowLine = Visibility.Collapsed;
                    }));
                var list = SelectExpediente.IMPUTADO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)verifyFinger && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.CALIDAD > 0 && w.BIOMETRICO != null).AsEnumerable()
                    .Select(s => new
                    {
                        IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                        FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data
                    }).ToList();
                var doIdentify = Comparison.Identify(Importer.ImportFmd(bytesHuella, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, list.Where(w => w.FMD != null).Select(s => s.FMD), (0x7fffffff / 100000), 10);
                var identify = true;
                identify = doIdentify.ResultCode == Constants.ResultCode.DP_SUCCESS ? doIdentify.Indexes.Count() > 0 : false;
                if (identify)
                {
                    if (HuellaCompromiso)
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            HuellaWindow.Close();
                            CartaView.Close();
                            HuellaCompromiso = false;
                            clickSwitch("acepto_compromiso");
                        }));
                    }
                    else
                    {
                        ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>(); var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                        if (SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().INGRESO_BIOMETRICO.Any())
                            if (SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO &&
                                w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().INGRESO_BIOMETRICO.Where(w =>
                                    w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                            else
                                if (SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO &&
                                    w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                    FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().INGRESO_BIOMETRICO.Where(w =>
                                        w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                        ListResultado.Add(new ResultadoBusquedaBiometrico
                        {
                            AMaterno = string.IsNullOrEmpty(SelectExpediente.MATERNO) ? string.Empty : SelectExpediente.MATERNO.Trim(),
                            APaterno = SelectExpediente.PATERNO.Trim(),
                            Nombre = SelectExpediente.NOMBRE.Trim(),
                            Expediente = SelectExpediente.ID_ANIO + " / " + SelectExpediente.ID_IMPUTADO,
                            Foto = FotoBusquedaHuella,
                            Imputado = SelectExpediente,
                            NIP = SelectExpediente.NIP
                        });
                        ListResultado = new List<ResultadoBusquedaBiometrico>(ListResultado);
                        SelectRegistro = ListResultado.FirstOrDefault();
                        ShowContinuar = Visibility.Collapsed;
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            if (!CancelKeepSearching)
                            {
                                ScannerMessage = "Huella empatada";
                                AceptarBusquedaHuellaFocus = true;
                                ColorMessage = new SolidColorBrush(Colors.Green);
                            }
                        }));
                        if (Finger != null) Service.Close();
                        return TaskEx.FromResult(false);
                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        if (!CancelKeepSearching)
                        {
                            ScannerMessage = "Huella no concuerda";
                            ColorMessage = new SolidColorBrush(Colors.Red);
                            AceptarBusquedaHuellaFocus = true;
                        }
                    }));
                    _IsSucceed = false;
                    if (!CancelKeepSearching) _SelectRegistro = null;
                    PropertyImage = null;
                }
                Service.Close();
                FingerPrintData = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda por huella.", ex);
            }
            return TaskEx.FromResult(true);
        }

        private Task<bool> ComparePersona(byte[] Huella = null, enumTipoBiometrico? Finger = null)
        {
            try
            {
                var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null ?? Huella;
                var verifyFinger = Finger ?? (DD_Dedo.HasValue ? DD_Dedo.Value : enumTipoBiometrico.INDICE_DERECHO);
                if (bytesHuella == null)
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = Finger == null ? "Vuelve a capturar las huellas" : "Siguiente Huella";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.DarkOrange);
                        ShowLine = Visibility.Collapsed;
                    }));
                }
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ScannerMessage = "Procesando...";
                    ColorMessage = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
                    AceptarBusquedaHuellaFocus = false;
                }));
                var Service = new BiometricoServiceClient();
                if (Service == null)
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = "Error en el servicio de comparacion";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.Red);
                        ShowLine = Visibility.Collapsed;
                    }));
                var list = SelectVisitanteIngreso.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)verifyFinger && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.BIOMETRICO != null).AsEnumerable()
                    .Select(s => new
                    {
                        IMPUTADO = new cHuellasPersona { ID_PERSONA = s.ID_PERSONA },
                        FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data
                    }).ToList();
                var doIdentify = Comparison.Identify(Importer.ImportFmd(bytesHuella, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, list.Where(w => w.FMD != null).Select(s => s.FMD), (0x7fffffff / 100000), 10);
                var identify = true;
                identify = doIdentify.ResultCode == Constants.ResultCode.DP_SUCCESS ? doIdentify.Indexes.Count() > 0 : false;
                if (identify)
                {
                    ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();
                    var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                    if (SelectVisitanteIngreso.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Any(w => (w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO || w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO) &&
                        w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                        FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(SelectVisitanteIngreso.VISITANTE.PERSONA.PERSONA_BIOMETRICO.First(w => (w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO || w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO) &&
                            w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).BIOMETRICO);
                    ListResultado.Add(new ResultadoBusquedaBiometrico
                    {
                        AMaterno = string.IsNullOrEmpty(SelectVisitanteIngreso.VISITANTE.PERSONA.MATERNO) ? string.Empty : SelectVisitanteIngreso.VISITANTE.PERSONA.MATERNO.Trim(),
                        APaterno = SelectVisitanteIngreso.VISITANTE.PERSONA.PATERNO.Trim(),
                        Nombre = SelectVisitanteIngreso.VISITANTE.PERSONA.NOMBRE.Trim(),
                        Expediente = SelectVisitanteIngreso.ID_PERSONA.ToString(),
                        Foto = FotoBusquedaHuella,
                        Persona = SelectVisitanteIngreso.VISITANTE.PERSONA,
                    });
                    ListResultado = new List<ResultadoBusquedaBiometrico>(ListResultado);
                    SelectRegistro = ListResultado.FirstOrDefault();
                    ShowContinuar = Visibility.Collapsed;
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        if (!CancelKeepSearching)
                        {
                            ScannerMessage = "Huella concuerda";
                            AceptarBusquedaHuellaFocus = true;
                            ColorMessage = new SolidColorBrush(Colors.Green);
                        }
                    }));
                    if (Finger != null) Service.Close();
                    return TaskEx.FromResult(false);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        if (!CancelKeepSearching)
                        {
                            ScannerMessage = "Huella no concuerda";
                            ColorMessage = new SolidColorBrush(Colors.Red);
                            AceptarBusquedaHuellaFocus = true;
                        }
                    }));
                    _IsSucceed = false;
                    if (!CancelKeepSearching) _SelectRegistro = null;
                    PropertyImage = null;
                }
                Service.Close();
                FingerPrintData = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda por huella.", ex);
            }
            return TaskEx.FromResult(true);
        }

        private async Task KeepSearch()
        {
            await Task.Factory.StartNew(() =>
            {
                while (!isKeepSearching) ;
            });
            isKeepSearching = false;
        }

        private void ConstructorHuella(enumTipoPersona tipobusqueda, bool? set442 = null, bool GuardarHuellas = false)
        {
            try
            {
                BuscarPor = tipobusqueda;
                Conectado = set442.HasValue ? set442.Value : false;
                ShowCapturar = set442.HasValue ? set442.Value ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
                _GuardarHuellas = GuardarHuellas;
                switch (tipobusqueda)
                {
                    case enumTipoPersona.IMPUTADO:
                        CabeceraBusqueda = "Datos del Imputado";
                        CabeceraFoto = "Foto Imputado";
                        break;
                    case enumTipoPersona.PERSONA_VISITA:
                    case enumTipoPersona.PERSONA_EXTERNA:
                        CabeceraBusqueda = "Datos de la Persona";
                        CabeceraFoto = "Foto Persona";
                        break;
                    case enumTipoPersona.PERSONA_ABOGADO:
                        CabeceraBusqueda = "Datos del Abogado";
                        CabeceraFoto = "Foto Abogado";
                        break;
                    case enumTipoPersona.PERSONA_EMPLEADO:
                        CabeceraBusqueda = "Datos del Empleado";
                        CabeceraFoto = "Foto Empleado";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la busqueda por huella.", ex);
            }
        }

        private async void OnBuscarPorHuellaConfirma(string obj = "")
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }

                await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));
                await TaskEx.Delay(400);
                var nRet = -1;
                var bandera = true;
                var requiereGuardarHuellas = Parametro.GuardarHuellaEnBusquedaPadronVisita;
                if (requiereGuardarHuellas)
                    try
                    {
                        nRet = CLSFPCaptureDllWrapper.CLS_Initialize();
                    }
                    catch
                    {
                        bandera = false;
                    }
                else
                    bandera = false;

                var windowBusqueda = new BusquedaHuella();
                windowBusqueda.DataContext = this;
                ConstructorHuella(enumTipoPersona.PERSONA_TODOS, nRet == 0, requiereGuardarHuellas);
                windowBusqueda.dgHuella.Columns.Insert(windowBusqueda.dgHuella.Columns.Count, new DataGridTextColumn()
                {
                    Binding = new System.Windows.Data.Binding("Imputado")
                    {
                        Converter = new GetTipoPersona()
                    },
                    Header = "IMPUTADO"
                });
                if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0 : false)
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGURESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                    return;
                }
                windowBusqueda.Owner = PopUpsViewModels.MainWindow;
                windowBusqueda.KeyDown += (s, e) => { if (e.Key == System.Windows.Input.Key.Escape)windowBusqueda.Close(); };
                windowBusqueda.Closed += (s, e) =>
                {
                    HuellasCapturadas = ((NotaMedicaViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                    if (bandera == true) CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    if (!IsSucceed) return;
                    if (SelectRegistro != null ? SelectRegistro.Persona == null : null == null) return;
                };
                windowBusqueda.ShowDialog();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }
        #endregion

        #region Solicitud de Visita Familiar
        private void ImprimirSolicitudVisitaFamiliar()
        {
            try
            {
                CultureInfo cultura = new CultureInfo("es-MX");
                var hoy = Fechas.GetFechaDateServer;
                var diccionario = new Dictionary<string, string>();
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                diccionario.Add("<<centro>>", centro.DESCR.Trim());
                diccionario.Add("<<ciudad>>", centro.MUNICIPIO.MUNICIPIO1.Trim());
                diccionario.Add("<<dia>>", hoy.Day.ToString());
                diccionario.Add("<<mes>>", cultura.DateTimeFormat.GetMonthName(hoy.Month));
                diccionario.Add("<<anio>>", hoy.Year.ToString());



            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al imprimir la solicitud de la visita familiar.", ex);
            }
        }

        #endregion

        #region Cambio SelectedItem de Busqueda de Expediente
        private async void OnModelChangedSwitch(object parametro)
        {
            if (parametro != null)
            {
                switch (parametro.ToString())
                {
                    case "cambio_expediente":
                        if (SelectExpediente != null && (SelectExpediente.INGRESO == null || SelectExpediente.INGRESO.Count == 0))
                        {
                            IMPUTADO valor = new IMPUTADO();
                            if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                                valor = SelectExpedienteAsignacion;
                            else
                                valor = SelectExpedientePadron;

                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                if (_SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                                    SelectExpedienteAsignacion = new cImputado().Obtener(valor.ID_IMPUTADO, valor.ID_ANIO, valor.ID_CENTRO).First();
                                else
                                    SelectExpedientePadron = new cImputado().Obtener(valor.ID_IMPUTADO, valor.ID_ANIO, valor.ID_CENTRO).First();
                                RaisePropertyChanged("SelectExpediente");
                            });
                            if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                                valor = SelectExpedienteAsignacion;
                            else
                                valor = SelectExpedientePadron;
                            //MUESTRA LOS INGRESOS
                            if (valor.INGRESO != null && valor.INGRESO.Count > 0)
                            {
                                EmptyIngresoVisible = false;
                                SelectIngreso = valor.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                            }
                            else
                                EmptyIngresoVisible = true;

                            //OBTENEMOS FOTO DE FRENTE
                            if (SelectIngreso != null)
                            {
                                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                else
                                    ImagenImputado = new Imagenes().getImagenPerson();
                            }
                            else
                                ImagenImputado = new Imagenes().getImagenPerson();
                        }
                        break;
                }
            }
        }
        #endregion

    }
    public class PERSONAVISITAAUXILIAR
    {
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public string NOMBRE { get; set; }
        public string DOMICILIO { get; set; }
        public string TELEFONO { get; set; }
        public string ESTATUS_VISITA_DESCR { get; set; }
        public string PARENTESCO_DESCR { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string INGRESO_NOMBRE { get; set; }
        public string INGRESO_PATERNO { get; set; }
        public string INGRESO_MATERNO { get; set; }
        public string SEXO { get; set; }
        public short? EDAD { get; set; }
        public int? ID_PERSONA { get; set; }
        public int? ID_IMPUTADO { get; set; }
        public short? ID_ANIO { get; set; }
        public short? ID_CENTRO { get; set; }
        public short ID_VISITA { get; set; }
        public short? ID_PARENTESCO { get; set; }
        public short? ID_ESTATUS_VISITA { get; set; }
        public int? ID_COLONIA { get; set; }
        public short? ID_MUNICIPIO { get; set; }
        public short? ID_ENTIDAD { get; set; }
        public short? ID_PAIS { get; set; }
        public DateTime? FECHA_NACIMIENTO { get; set; }
        public bool DE_TERMINO { get; set; }
        public SSP.Servidor.PERSONA OBJETO_PERSONA { get; set; }
        public VISITA_AUTORIZADA OBJETO_VISITA_AUTORIZADA { get; set; }
        public ESTATUS_VISITA ESTATUS_VISITA { get; set; }
        public TIPO_REFERENCIA TIPO_REFERENCIA { get; set; }
        public INGRESO INGRESO { get; set; }
    }
    class ACOMPANANTE_AUXILIAR
    {
        public int? ID_PERSONA { get; set; }
        public int? ID_IMPUTADO { get; set; }
        public short? ID_ANIO { get; set; }
        public short ID_CENTRO { get; set; }
        public SSP.Servidor.PERSONA PERSONA { get; set; }
        public ESTATUS_VISITA ESTATUS_VISITA { get; set; }
        public TIPO_REFERENCIA TIPO_REFERENCIA { get; set; }
    }
    class ListaVisitaAgenda
    {
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public short ID_TIPO_VISITA { get; set; }
        public short ID_DIA { get; set; }
        public bool ESTATUS { get; set; }
        public string HORA_INI { get; set; }
        public string HORA_FIN { get; set; }
        public Nullable<short> ID_AREA { get; set; }
        public string TIPO_VISITA { get; set; }
        public string DIA { get; set; }
        public string HORA_ENTRADA { get; set; }
        public string HORA_SALIDA { get; set; }
        public string AREA { get; set; }
        public VISITA_EDIFICIO VISITA_EDIFICIO { get; set; }
        public VISITA_AGENDA VISITA_AGENDA { get; set; }
        public VISITA_APELLIDO VISITA_APELLIDO { get; set; }
    }
}
