using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlPenales
{
    partial class CreacionListasExamenPViewModel
    {
        private Microsoft.Reporting.WinForms.ReportDataSource EncabezadoReportesFueroComun(INGRESO _ingreso)
        {
            try
            {
                string NombreImputado = _ingreso != null ? _ingreso.IMPUTADO != null ?
                    string.Format("NOMBRE DEL INTERNO: {0} {1} {2}", !string.IsNullOrEmpty(_ingreso.IMPUTADO.NOMBRE) ? _ingreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
                                  !string.IsNullOrEmpty(_ingreso.IMPUTADO.PATERNO) ? _ingreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                                  !string.IsNullOrEmpty(_ingreso.IMPUTADO.MATERNO) ? _ingreso.IMPUTADO.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty;

                var ds1 = new List<cEncabezado>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                cEncabezado Encabezado = new cEncabezado();
                Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                Encabezado.TituloDos = Parametro.ENCABEZADO2;
                Encabezado.ImagenIzquierda = Parametro.LOGO_ESTADO;
                Encabezado.ImagenFondo = Parametro.REPORTE_LOGO2;
                Encabezado.NoImputado = NombreImputado;
                Encabezado.NombreReporte = Parametro.ENCABEZADO_FUERO_COMUN;
                Encabezado.ImagenFondo = Parametro.REPORTE_LOGO_ISO;
                Encabezado.PieUno = Parametro.DESCR_ISO_1;
                Encabezado.PieDos = Parametro.DESCR_ISO_2;
                Encabezado.PieTres = Parametro.DESCR_ISO_3;
                ds1.Add(Encabezado);
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                return rds1;
            }

            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource EncabezadoReportesFueroFederal()
        {
            try
            {
                var ds1 = new List<cReporte>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                cReporte Encabezado = new cReporte();
                Encabezado.Encabezado1 = Parametro.ENCABEZADO_FUERO_FEDERAL_1;
                Encabezado.Encabezado2 = Parametro.ENCABEZADO_FUERO_FEDERAL_2;
                Encabezado.Encabezado3 = Parametro.ENCABEZADO_FUERO_FEDERAL_3;
                Encabezado.Encabezado4 = Parametro.ENCABEZADO_FUERO_FEDERAL_4;
                Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO_FUERO_FEDERAL_2;
                Encabezado.ImagenMedio = Parametro.REPORTE_LOGO_FUERO_FEDERAL_1;
                Encabezado.ImagenDerecha = Parametro.REPORTE_LOGO_FUERO_FEDERAL_3;
                ds1.Add(Encabezado);
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                return rds1;
            }

            catch (Exception exc)
            {
                throw;
            }
        }


        #region comun
        #region MEDICO
        private void MedicoComun(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun(_ing));
                View.Report.LocalReport.DataSources.Add(DatosEstudioMedicoFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioMedicoFueroComun.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioMedicoFueroComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                if (_ingreso == null)
                    new Microsoft.Reporting.WinForms.ReportDataSource();

                var _Datos = new List<cRealizacionEstudios>();
                var _DatosEstudioMedicoFrueroComun = new cPersonalidadEstudioMedicoComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                CamposBase.EdadInterno = _ingreso.IMPUTADO != null ? new Fechas().CalculaEdad(_ingreso.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty;
                CamposBase.SexoInterno = _ingreso.IMPUTADO != null ? _ingreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO" : string.Empty;
                if (_DatosEstudioMedicoFrueroComun != null)
                {
                    CamposBase.AntecedentesHeredoFamiliares = _DatosEstudioMedicoFrueroComun.P2_HEREDO_FAMILIARES;
                    CamposBase.AntecedentesPersonalesNoPatologicos = _DatosEstudioMedicoFrueroComun.P3_ANTPER_NOPATO;
                    CamposBase.AntecedentesConsumoToxicosEstadoActual = _DatosEstudioMedicoFrueroComun.P31_CONSUMO_TOXICO;
                    CamposBase.DescrTatuajesCicatricesRecAntiguasMalformaciones = _DatosEstudioMedicoFrueroComun.P32_TATUAJES_CICATRICES;
                    CamposBase.AntecedentesPatologicos = _DatosEstudioMedicoFrueroComun.P4_PATOLOGICOS;
                    CamposBase.PadecimientoActual = _DatosEstudioMedicoFrueroComun.P5_PADECIMIENTOS;
                    CamposBase.TensionArterial = _DatosEstudioMedicoFrueroComun.SIGNOS_TA;
                    CamposBase.Teperatura = _DatosEstudioMedicoFrueroComun.SIGNOS_TEMPERATURA;
                    CamposBase.Pulso = _DatosEstudioMedicoFrueroComun.SIGNOS_PULSO;
                    CamposBase.Respiracion = _DatosEstudioMedicoFrueroComun.SIGNOS_RESPIRACION;
                    CamposBase.Peso = _DatosEstudioMedicoFrueroComun.SIGNOS_PESO;
                    CamposBase.Estatura = _DatosEstudioMedicoFrueroComun.SIGNOS_ESTATURA;
                    CamposBase.Abdomen = _DatosEstudioMedicoFrueroComun.COORDINADOR;
                    CamposBase.Actitud = _DatosEstudioMedicoFrueroComun.ELABORO;
                    CamposBase.ImpresionDiagnostica = _DatosEstudioMedicoFrueroComun.P7_IMPRESION_DIAGNOSTICA;
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", _DatosEstudioMedicoFrueroComun.P8_DICTAMEN_MEDICO == (short)eDiagnosticoDictamen.FAVORABLE ? "X" : string.Empty,
                        _DatosEstudioMedicoFrueroComun.P8_DICTAMEN_MEDICO == (short)eDiagnosticoDictamen.DESFAVORABLE ? "X" : string.Empty);//estructura provisional del dictamen
                    CamposBase.FechaRealizacionEstudio = _DatosEstudioMedicoFrueroComun.ESTUDIO_FEC.HasValue ? _DatosEstudioMedicoFrueroComun.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }

            catch (Exception exc)
            {

                throw;
            }
        }
        #endregion
        #region PSIQUIATRICO COMUN
        private void PsiquiatricoComun(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun(_ing));
                View.Report.LocalReport.DataSources.Add(DatosEstudioPsiquiatricoFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioPsiquiatricoFueroComun.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioPsiquiatricoFueroComun(INGRESO _ing, short idEstudio)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var _DatosEstudioPsiquatricoComun = new cPersonalidadEstudioPsiquiatricoComun().GetData(x => x.ID_ESTUDIO == idEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                if (_DatosEstudioPsiquatricoComun != null)
                {
                    CamposBase.AspectoFisico = _DatosEstudioPsiquatricoComun.A1_ASPECTO_FISICO;
                    CamposBase.ConductaMotora = _DatosEstudioPsiquatricoComun.B1_CONDUCTA_MOTORA;
                    CamposBase.Habla = _DatosEstudioPsiquatricoComun.C1_HABLA;
                    CamposBase.Actitud = _DatosEstudioPsiquatricoComun.D1_ACTITUD;
                    CamposBase.EstadoAnimo = _DatosEstudioPsiquatricoComun.A2_ESTADO_ANIMO;
                    CamposBase.ExpresAfectiva = _DatosEstudioPsiquatricoComun.B2_EXPRESION_AFECTIVA;
                    CamposBase.Adecuacion = _DatosEstudioPsiquatricoComun.C2_ADECUACION;
                    CamposBase.Alucinaciones = _DatosEstudioPsiquatricoComun.A3_ALUCINACIONES;
                    CamposBase.Ilusiones = _DatosEstudioPsiquatricoComun.B3_ILUSIONES;
                    CamposBase.Despersonalizacion = _DatosEstudioPsiquatricoComun.C3_DESPERSONALIZACION;
                    CamposBase.Desrealizacion = _DatosEstudioPsiquatricoComun.D3_DESREALIZACION;
                    CamposBase.CursoPensamiento = _DatosEstudioPsiquatricoComun.A4_CURSO;
                    CamposBase.ContinuidadPensamiento = _DatosEstudioPsiquatricoComun.B4_CONTINUIDAD;
                    CamposBase.ContenidoPensamiento = _DatosEstudioPsiquatricoComun.C4_CONTENIDO;
                    CamposBase.PensamientoAbstracto = _DatosEstudioPsiquatricoComun.D4_ABASTRACTO;
                    CamposBase.Concentracion = _DatosEstudioPsiquatricoComun.E4_CONCENTRACION;
                    CamposBase.Orientacion = _DatosEstudioPsiquatricoComun.P5_ORIENTACION;
                    CamposBase.Memoria = _DatosEstudioPsiquatricoComun.P6_MEMORIA;
                    CamposBase.BajaToleranciaFrust = _DatosEstudioPsiquatricoComun.A7_BAJA_TOLERANCIA;
                    CamposBase.ExpresionDesadapt = _DatosEstudioPsiquatricoComun.B7_EXPRESION;
                    CamposBase.Adecuada = _DatosEstudioPsiquatricoComun.C7_ADECUADA;
                    CamposBase.CapacJuicio = _DatosEstudioPsiquatricoComun.P8_CAPACIDAD_JUICIO;
                    CamposBase.Introspeccion = _DatosEstudioPsiquatricoComun.P9_INTROSPECCION;
                    CamposBase.Fiabilidad = _DatosEstudioPsiquatricoComun.P10_FIANILIDAD;
                    CamposBase.ImpresionDiagnostica = _DatosEstudioPsiquatricoComun.P11_IMPRESION;
                    CamposBase.Abdomen = _DatosEstudioPsiquatricoComun.COORDINADOR;
                    CamposBase.AliasInterno = _DatosEstudioPsiquatricoComun.MEDICO_PSIQUIATRA;
                    CamposBase.FechaRealizacionEstudio = _DatosEstudioPsiquatricoComun.ESTUDIO_FEC.HasValue ? _DatosEstudioPsiquatricoComun.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Dictamen = string.Format("FAVORABLE ( {0} ) \t DESFAVORABLE ( {1} )", _DatosEstudioPsiquatricoComun.P12_DICTAMEN_PSIQUIATRICO == (short)eDiagnosticoDictamen.FAVORABLE ? "X" : string.Empty,
                        _DatosEstudioPsiquatricoComun.P12_DICTAMEN_PSIQUIATRICO == (short)eDiagnosticoDictamen.DESFAVORABLE ? "X" : string.Empty);
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }

            catch (Exception exc)
            {
                throw;
            }
        }

        #endregion
        #region PSICOLOGICO COMUN
        private void PsicologicoComun(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun(_ing));
                View.Report.LocalReport.DataSources.Add(DatosEstudioPsicologicoFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosProgUno(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosProgDos(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosProgTres(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosProgCuatro(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioPsicologicoFueroComun.rdlc";
                View.Report.RefreshReport();

                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                string fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);

                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);

                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;
                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;

                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioPsicologicoFueroComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var _DatosPsicologicoComun = new cPersonalidadEstudioPsicologicoComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                if (_DatosPsicologicoComun != null)
                {
                    CamposBase.CondicionesGralesInterno = _DatosPsicologicoComun.P1_CONDICIONES_GRALES;
                    CamposBase.ExamenMental = _DatosPsicologicoComun.P2_EXAMEN_MENTAL;
                    CamposBase.DescripcionPrincRasgosIngresoRelComDelito = _DatosPsicologicoComun.P3_PRINCIPALES_RASGOS;
                    CamposBase.LauretaBenderTexto = string.Format("( {0} ) TEST GUESTALTICO VISOMOTOR DE LAURETTA BENDER", _DatosPsicologicoComun.P4_TEST_GUALTICO == (short)eSINO.SI ? "X" : string.Empty);
                    CamposBase.MatricesRavenTexto = string.Format("( {0} ) TEST DE MATRICES PROGRESIVAS DE RAVEN", _DatosPsicologicoComun.P4_TEST_MATRICES == (short)eSINO.SI ? "X" : string.Empty);
                    CamposBase.HTPTexto = string.Format("( {0} ) TEST (HTP) CASA, ARBOL, PERSONA", _DatosPsicologicoComun.P4_TEST_HTP == (short)eSINO.SI ? "X" : string.Empty);
                    CamposBase.MinnessotaTexto = string.Format("( {0} ) INVENTARIO MULTIFÁSICO DE LA PERSONALIDAD MINESOTA (MMPI 1 o 2).", _DatosPsicologicoComun.P4_INVENTARIO_MULTIFASICO == (short)eSINO.SI ? "X" : string.Empty);
                    CamposBase.OtroTestTexto = string.Format("( {0} ) OTRA (S) {1}", _DatosPsicologicoComun.P4_OTRAS == (short)eSINO.SI ? "X" : string.Empty, _DatosPsicologicoComun.P4_OTRA_MENCIONAR);
                    CamposBase.NivelIntelectualTextro = string.Format(
                        " ( {0} ) SUPERIOR \n ( {1} ) SUPERIOR AL TÉRMINO MEDIO \n ( {2} ) MEDIO \n ( {3} ) INFERIOR AL TÉRMINO MEDIO \n ( {4} ) INFERIOR \n ( {5} ) DEFICIENTE",
                        _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.SUPERIOR ? "X" : string.Empty, _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.SUPERIOR_TERMINO_MEDIO ? "X" : string.Empty,
                        _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.MEDIO ? "X" : string.Empty, _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.INFERIOR_TERMINO_MEDIO ? "X" : string.Empty,
                        _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.INFERIOR ? "X" : string.Empty, _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.DEFICIENTE ? "X" : string.Empty);
                    CamposBase.DatosDisfuncionNeuroTexto = string.Format(" ( {0} ) NO PRESENTA \n ( {1} ) SE SOSPECHA \n ( {2} ) CON DATOS CLÍNICOS EVIDENTES",
                        _DatosPsicologicoComun.P52_DISFUNCION_NEUROLOGICA == (short)eDisfuncionNeurologica.NO_PRESENTA ? "X" : string.Empty, _DatosPsicologicoComun.P52_DISFUNCION_NEUROLOGICA == (short)eDisfuncionNeurologica.SE_SOSPECHA ? "X" : string.Empty
                        , _DatosPsicologicoComun.P52_DISFUNCION_NEUROLOGICA == (short)eDisfuncionNeurologica.DATOS_CLINICOS_EVIDENTES ? "X" : string.Empty);
                    CamposBase.IntegracionDinamica = _DatosPsicologicoComun.P6_INTEGRACION;
                    CamposBase.RasgosPersonalidadRelComisionDelitoLogradoModificarInternamiento = _DatosPsicologicoComun.P8_RASGOS_PERSONALIDAD;
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE ", _DatosPsicologicoComun.P9_DICTAMEN_REINSERCION == (short)eDiagnosticoDictamen.FAVORABLE ? "X" : string.Empty, _DatosPsicologicoComun.P9_DICTAMEN_REINSERCION == (short)eDiagnosticoDictamen.DESFAVORABLE ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = _DatosPsicologicoComun.P10_MOTIVACION_DICTAMEN;
                    CamposBase.CasoNegativoSenialeProgramasARemitir = _DatosPsicologicoComun.P11_CASO_NEGATIVO;
                    CamposBase.RequiereTratExtraMurosTexto = string.Format("SI ( {0} ) NO ( {1} )", _DatosPsicologicoComun.P12_REQUIERE_TRATAMIENTO == "S" ? "X" : string.Empty, _DatosPsicologicoComun.P12_REQUIERE_TRATAMIENTO == "N" ? "X" : string.Empty);
                    CamposBase.CualExtramuros = _DatosPsicologicoComun.P12_CUAL;
                    CamposBase.FechaRealizacionEstudio = _DatosPsicologicoComun.ESTUDIO_FEC.HasValue ? _DatosPsicologicoComun.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Abdomen = _DatosPsicologicoComun.COORDINADOR;
                    CamposBase.Actitud = _DatosPsicologicoComun.ELABORO;
                };

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgUno(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cProgramasFueroComun>();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _Padre = new cPersonalidadEstudioPsicologicoComun().GetData(x => x.ID_ESTUDIO == _IdEstudio).FirstOrDefault();
                if (_Padre != null)
                {
                    var _DatosCapacitacion = new cProgramaRealizacionEstudios().GetData(x => x.ID_ESTUDIO == _Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    cProgramasFueroComun CamposBase = new cProgramasFueroComun();
                    if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                    {
                        short _con = 1;
                        foreach (var item in _DatosCapacitacion)
                        {
                            if (item.ID_TIPO_PROGRAMA == (short)eGrupos.PROGRAMAS_DESHABITUAMIENTO)
                            {
                                CamposBase = new cProgramasFueroComun();
                                CamposBase.Duraci = item.DURACION;
                                CamposBase.Observaciones = item.OBSERVACION;
                                CamposBase._Consecutivo = _con;
                                CamposBase.Programa = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty;
                                _Datos.Add(CamposBase);
                                _con++;
                            };
                        };
                    }
                    else
                    {
                        CamposBase = new cProgramasFueroComun();
                        CamposBase.Duraci = string.Empty;
                        CamposBase.Observaciones = string.Empty;
                        CamposBase._Consecutivo = 1;
                        CamposBase.Programa = string.Empty;
                        _Datos.Add(CamposBase);
                    }
                }

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgDos(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Padre = new cPersonalidadEstudioPsicologicoComun().GetData(x => x.ID_ESTUDIO == _IdEstudio).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _Datos = new List<cProgramasFueroComun>();

                if (_Padre != null)
                {
                    var _DatosCapacitacion = new cProgramaRealizacionEstudios().GetData(x => x.ID_ESTUDIO == _Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    cProgramasFueroComun CamposBase = new cProgramasFueroComun();
                    if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                    {
                        short _con = 1;
                        foreach (var item in _DatosCapacitacion)
                        {
                            if (item.ID_TIPO_PROGRAMA == (short)eGrupos.PROGRAMAS_MODIFIC_CONDUCTA)
                            {
                                CamposBase = new cProgramasFueroComun();
                                CamposBase.Duraci = item.CONCLUYO == "S" ? "SI" : "NO";
                                CamposBase.Observaciones = item.OBSERVACION;
                                CamposBase._Consecutivo = _con;
                                CamposBase.Programa = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty;
                                _Datos.Add(CamposBase);
                                _con++;
                            };
                        };
                    }
                    else
                    {
                        CamposBase = new cProgramasFueroComun();
                        CamposBase.Duraci = string.Empty;
                        CamposBase.Observaciones = string.Empty;
                        CamposBase._Consecutivo = 1;
                        CamposBase.Programa = string.Empty;
                        _Datos.Add(CamposBase);
                    }
                }

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet4";
                return _respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgTres(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Padre = new cPersonalidadEstudioPsicologicoComun().GetData(x => x.ID_ESTUDIO == _IdEstudio).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _Datos = new List<cProgramasFueroComun>();

                cProgramasFueroComun CamposBase = new cProgramasFueroComun();
                if (_Padre != null)
                {
                    var _DatosCapacitacion = new cProgramaRealizacionEstudios().GetData(x => x.ID_ESTUDIO == _Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);

                    if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                    {
                        short _con = 1;
                        foreach (var item in _DatosCapacitacion)
                        {
                            if (item.ID_TIPO_PROGRAMA == (short)eGrupos.COMPLEMENTARIO)
                            {
                                CamposBase = new cProgramasFueroComun()
                                {
                                    Duraci = item.CONCLUYO == "S" ? "SI" : "NO",
                                    Observaciones = item.OBSERVACION,
                                    _Consecutivo = _con,
                                    Programa = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty,
                                };

                                _Datos.Add(CamposBase);
                                _con++;
                            };
                        };
                    }
                    else
                    {
                        CamposBase = new cProgramasFueroComun()
                        {
                            Duraci = string.Empty,
                            Observaciones = string.Empty,
                            _Consecutivo = 1,
                            Programa = string.Empty
                        };

                        _Datos.Add(CamposBase);
                    }
                }

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet5";
                return _respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgCuatro(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cProgramasFueroComun>();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _Padre = new cPersonalidadEstudioPsicologicoComun().GetData(x => x.ID_ESTUDIO == _IdEstudio).FirstOrDefault();
                if (_Padre != null)
                {
                    var _DatosCapacitacion = new cProgramaRealizacionEstudios().GetData(x => x.ID_ESTUDIO == _Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    cProgramasFueroComun CamposBase = new cProgramasFueroComun();
                    if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                    {
                        short _con = 1;
                        foreach (var item in _DatosCapacitacion)
                        {
                            if (item.ID_TIPO_PROGRAMA != (short)eGrupos.COMPLEMENTARIO && item.ID_TIPO_PROGRAMA != (short)eGrupos.PROGRAMAS_DESHABITUAMIENTO && item.ID_TIPO_PROGRAMA != (short)eGrupos.PROGRAMAS_MODIFIC_CONDUCTA)
                            {
                                CamposBase = new cProgramasFueroComun()
                                {
                                    Duraci = item.CONCLUYO == "S" ? "SI" : "NO",
                                    Observaciones = item.OBSERVACION,
                                    _Consecutivo = _con,
                                    Programa = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty
                                };

                                _Datos.Add(CamposBase);
                                _con++;
                            };
                        };
                    }
                    else
                    {
                        CamposBase = new cProgramasFueroComun()
                        {
                            Duraci = string.Empty,
                            Observaciones = string.Empty,
                            _Consecutivo = 1,
                            Programa = string.Empty
                        };
                        _Datos.Add(CamposBase);
                    }
                };

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet6";
                return _respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region CRIMINOLOGICO COMUN
        private void CriminologicoComun(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun(_ing));
                View.Report.LocalReport.DataSources.Add(DatosEstudioCriminodiagnosticoFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rCriminoDiagnosticoFC.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioCriminodiagnosticoFueroComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var _DatosCriminodiagnosticoComun = new cPersonalidadEstudioCriminodiagnosticoComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO && x.ID_INGRESO == _ingreso.ID_INGRESO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                if (_DatosCriminodiagnosticoComun != null)
                {
                    CamposBase.VersionDelitoSegunInterno = _DatosCriminodiagnosticoComun.P1_VERSION_DELITO;
                    CamposBase.MomentoCometerDelitoEncontrabaInfluenciaDrogaTexto = string.Format(" ( {0} ) NO \n ( {1} ) SI", _DatosCriminodiagnosticoComun.P1_DROGADO == "N" ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P1_DROGADO == "S" ? "X" : string.Empty);
                    CamposBase.DescripcionDrogasTexto = string.Format(" ( {0} ) ALCOHOL \n ( {1} ) DROGAS ILEGALES \n ( {2} ) OTRA ", _DatosCriminodiagnosticoComun.P1_ALCOHOL == (short)eSINO.SI ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P1_DROGRA_ILEGAL == (short)eSINO.SI ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P1_OTRA == (short)eSINO.SI ? "X" : string.Empty);
                    CamposBase.Criminogenesis = _DatosCriminodiagnosticoComun.P2_CRIMINOGENESIS;
                    CamposBase.AntecedentesEvolucionConductasParaAntiSociales = _DatosCriminodiagnosticoComun.P3_CONDUCTA_ANTISOCIAL;
                    CamposBase.ClasificCriminologTexto = string.Format(" ( {0} ) PRIMO DELINCUENTE \n ( {1} ) REINCIDENTE ESPECIFICO \n ( {2} ) REINCIDENTE GENÉRICO \n ( {3} ) HABITUAL \n ( {4} ) PROFESIONAL ", _DatosCriminodiagnosticoComun.P4_CLASIFICACION_CRIMINOLOGICA == (short)eCapacidadCriminal.PRIMO_DELINCUENTE ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P4_CLASIFICACION_CRIMINOLOGICA == (short)eCapacidadCriminal.REINCIDENTE_ESPECIFICO ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P4_CLASIFICACION_CRIMINOLOGICA == (short)eCapacidadCriminal.REINCIDENTE_GENERICO ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P4_CLASIFICACION_CRIMINOLOGICA == (short)eCapacidadCriminal.HABITUAL ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P4_CLASIFICACION_CRIMINOLOGICA == (short)eCapacidadCriminal.PROFESIONAL ? "X" : string.Empty);
                    CamposBase.IntimidacionPenaImpuestaTexto = string.Format("SI ( {0} ) NO ( {1} )", _DatosCriminodiagnosticoComun.P5_INTIMIDACION == "S" ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P5_INTIMIDACION == "N" ? "X" : string.Empty);
                    CamposBase.PorqueIntimidacion = _DatosCriminodiagnosticoComun.P5_PORQUE;
                    CamposBase.CapacidadCriminalActualTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) MEDIA BAJA ( {2} ) BAJA ( {3} )", _DatosCriminodiagnosticoComun.P6_CAPACIDAD_CRIMINAL == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6_CAPACIDAD_CRIMINAL == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P6_CAPACIDAD_CRIMINAL == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6_CAPACIDAD_CRIMINAL == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.EgocentrismoTexto = string.Format("ALTO ( {0} ) MEDIO ( {1} ) MEDIO BAJO ( {2} ) BAJO ( {3} )", _DatosCriminodiagnosticoComun.P6A_EGOCENTRICO == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6A_EGOCENTRICO == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P6A_EGOCENTRICO == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6A_EGOCENTRICO == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.LabilidadAfectivaTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) MEDIO BAJA ( {2} ) BAJA ( {3} )", _DatosCriminodiagnosticoComun.P6B_LIABILIDAD_EFECTIVA == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6B_LIABILIDAD_EFECTIVA == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P6B_LIABILIDAD_EFECTIVA == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6B_LIABILIDAD_EFECTIVA == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.AgresividadTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) MEDIA BAJA ( {2} ) BAJA ( {3} )", _DatosCriminodiagnosticoComun.P6C_AGRESIVIDAD == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6C_AGRESIVIDAD == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P6C_AGRESIVIDAD == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6C_AGRESIVIDAD == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.IndiferenciaAfectTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) MEDIA BAJA ( {2} ) BAJA ( {3} )", _DatosCriminodiagnosticoComun.P6D_INDIFERENCIA_AFECTIVA == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6D_INDIFERENCIA_AFECTIVA == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P6D_INDIFERENCIA_AFECTIVA == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6D_INDIFERENCIA_AFECTIVA == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.AdaptabSocialTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) BAJA ( {2} ) ", _DatosCriminodiagnosticoComun.P7_ADAPTACION_SOCIAL == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P7_ADAPTACION_SOCIAL == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P7_ADAPTACION_SOCIAL == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.IndicePeligrosidadCriminActualTexto = string.Format("MÁXIMA ( {0} ) MEDIA-MÁXIMA ( {1} ) MEDIA ( {2} ) MEDIA-MÍNIMA ( {3} ) MÍNIMA ( {4} )", _DatosCriminodiagnosticoComun.P8_INDICE_PELIGROSIDAD == (short)ePeligrosidad.MAXIMA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P8_INDICE_PELIGROSIDAD == (short)ePeligrosidad.MEDIA_MAXIMA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P8_INDICE_PELIGROSIDAD == (short)ePeligrosidad.MEDIA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P8_INDICE_PELIGROSIDAD == (short)ePeligrosidad.MEDIA_MINIMA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P8_INDICE_PELIGROSIDAD == (short)ePeligrosidad.MINIMA ? "X" : string.Empty);
                    CamposBase.PronosticoReincidenciaTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) MEDIA BAJA ( {2} ) BAJA ( {3} )", _DatosCriminodiagnosticoComun.P9_PRONOSTICO_REINCIDENCIA == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P9_PRONOSTICO_REINCIDENCIA == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P9_PRONOSTICO_REINCIDENCIA == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P9_PRONOSTICO_REINCIDENCIA == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE ", _DatosCriminodiagnosticoComun.P10_DICTAMEN_REINSERCION == (short)eDiagnosticoDictamen.FAVORABLE ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P10_DICTAMEN_REINSERCION == (short)eDiagnosticoDictamen.DESFAVORABLE ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = _DatosCriminodiagnosticoComun.P10_MOTIVACION_DICTAMEN;
                    CamposBase.CasoNegativoSenialeProgramasARemitir = _DatosCriminodiagnosticoComun.P11_PROGRAMAS_REMITIRSE;
                    CamposBase.RequiereTratExtraMurosTexto = string.Format("Si ( {0} ) NO ( {1} )", _DatosCriminodiagnosticoComun.P12_TRATAMIENTO_EXTRAMUROS == "S" ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P12_TRATAMIENTO_EXTRAMUROS == "N" ? "X" : string.Empty);
                    CamposBase.CualExtramuros = _DatosCriminodiagnosticoComun.P12_CUAL;
                    CamposBase.FechaRealizacionEstudio = _DatosCriminodiagnosticoComun.ESTUDIO_FEC.HasValue ? _DatosCriminodiagnosticoComun.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Actitud = _DatosCriminodiagnosticoComun.ELABORO;
                    CamposBase.Abdomen = _DatosCriminodiagnosticoComun.COORDINADOR;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }
            catch (Exception exc)
            {

                throw;
            }
        }

        #endregion

        #region SOCIOFAMILIAR COMUN
        private void SocioFamiliarComun(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun(_ing));
                View.Report.LocalReport.DataSources.Add(DatosEstudioSocioFamiliarFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosVisitas(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosProgramasFortalecimientoComun(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosProgramasReligiososComun(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioSocioFamFC.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioSocioFamiliarFueroComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var _DatosEstudioSocioEconomicoComun = new cPersonalidadEstudioSocioFamiliarComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                CamposBase.NombreInterno = string.Format("{0} {1} {2} ", _ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(_ingreso.IMPUTADO.NOMBRE) ? _ingreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                    _ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(_ingreso.IMPUTADO.PATERNO) ? _ingreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                    _ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(_ingreso.IMPUTADO.MATERNO) ? _ingreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);
                var municipio = new cMunicipio().Obtener(_ingreso.IMPUTADO.NACIMIENTO_ESTADO.Value, _ingreso.IMPUTADO.NACIMIENTO_MUNICIPIO.Value).FirstOrDefault();
                CamposBase.LugarFecNacInterno = string.Format("{0} {1}", _ingreso.IMPUTADO.NACIMIENTO_MUNICIPIO.HasValue ? !string.IsNullOrEmpty(municipio.MUNICIPIO1) ? municipio.MUNICIPIO1.Trim() : string.Empty : string.Empty,
                    _ingreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? _ingreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty);
                //CamposBase.EstadoCivilInterno = _ingreso.IMPUTADO != null ? _ingreso.IMPUTADO.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(_ingreso.IMPUTADO.ESTADO_CIVIL.DESCR) ? _ingreso.IMPUTADO.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                CamposBase.EstadoCivilInterno = _ingreso != null ? _ingreso.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(_ingreso.ESTADO_CIVIL.DESCR) ? _ingreso.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                //CamposBase.DomicilioInterno = string.Format("{0} {1}", _ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(_ingreso.IMPUTADO.DOMICILIO_CALLE) ? _ingreso.IMPUTADO.DOMICILIO_CALLE.Trim() : string.Empty : string.Empty,
                //    _ingreso.IMPUTADO != null ? _ingreso.IMPUTADO.DOMICILIO_NUM_EXT.HasValue ? _ingreso.IMPUTADO.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty : string.Empty);
                CamposBase.DomicilioInterno = string.Format("{0} {1}", _ingreso != null ? !string.IsNullOrEmpty(_ingreso.DOMICILIO_CALLE) ? _ingreso.DOMICILIO_CALLE.Trim() : string.Empty : string.Empty,
                    _ingreso != null ? _ingreso.DOMICILIO_NUM_EXT.HasValue ? _ingreso.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty : string.Empty);
                //CamposBase.TelefonoInterno = _ingreso.IMPUTADO != null ? _ingreso.IMPUTADO.TELEFONO.HasValue ? _ingreso.IMPUTADO.TELEFONO.Value.ToString() : string.Empty : string.Empty;
                CamposBase.TelefonoInterno = _ingreso != null ? _ingreso.TELEFONO.HasValue ? _ingreso.TELEFONO.Value.ToString() : string.Empty : string.Empty;

                if (_DatosEstudioSocioEconomicoComun != null)
                {
                    CamposBase.FamiliaPrimaria = _DatosEstudioSocioEconomicoComun.P21_FAMILIA_PRIMARIA;
                    CamposBase.FamiliaSecundaria = _DatosEstudioSocioEconomicoComun.P22_FAMILIA_SECUNDARIA;
                    CamposBase.AdultoMayorProgramaEspecial = string.Format(" ( {0} ) SI \n ( {1} ) NO", _DatosEstudioSocioEconomicoComun.P3_TERCERA_EDAD == "S" ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P3_TERCERA_EDAD == "N" ? "X" : string.Empty);
                    CamposBase.RecibeVisText = string.Format(" ( {0} ) NO \n ( {1} ) SI", _DatosEstudioSocioEconomicoComun.P4_RECIBE_VISITA == "N" ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P4_RECIBE_VISITA == "S" ? "X" : string.Empty);
                    CamposBase.QuienesVisitaG = string.Format(" PADRE ( {0} ) MADRE ( {1} ) ESPOSA(O)/CONCUBINA(O) ( {2} ) HERMANOS ( {3} ) HIJOS ( {4} ) OTROS FAMILIARES ( {5} ) \n ",
                        _DatosEstudioSocioEconomicoComun.P4_PADRE == 1 ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P4_MADRE == 1 ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P4_ESPOSOA == 1 ? "X" : string.Empty,
                        _DatosEstudioSocioEconomicoComun.P4_HERMANOS == 1 ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P4_HIJOS == 1 ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P4_OTROS == 1 ? "X" : string.Empty);
                    CamposBase.TextoGenerico1 = string.Format("ESPECIFICAR QUIEN:  {0}", _DatosEstudioSocioEconomicoComun.P4_OTROS_EPECIFICAR);
                    CamposBase.FrecuenciaV = _DatosEstudioSocioEconomicoComun.P4_FRECUENCIA;
                    CamposBase.RazonNoRecibeVisitasTexto = _DatosEstudioSocioEconomicoComun.P4_MOTIVO_NO_VISITA;
                    CamposBase.MantieneComunicTelefonicaTexto = string.Format(" ( {0} ) NO, \t ESPECIFICAR POR QUE: {1} \n \n ( {2} ) SI, ESPECIFICAR QUIEN: \t {3}", _DatosEstudioSocioEconomicoComun.P5_COMUNICACION_TELEFONICA == "S" ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P5_COMUNICACION_TELEFONICA == "S" ? _DatosEstudioSocioEconomicoComun.P5_NO_POR_QUE : string.Empty,
                        _DatosEstudioSocioEconomicoComun.P5_COMUNICACION_TELEFONICA == "N" ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P5_COMUNICACION_TELEFONICA == "N" ? _DatosEstudioSocioEconomicoComun.P5_NO_POR_QUE : string.Empty);
                    CamposBase.CuentaApoyoFamiliaAlgunaPersona = _DatosEstudioSocioEconomicoComun.P6_APOYO_EXTERIOR;
                    CamposBase.PlanesSerExternado = _DatosEstudioSocioEconomicoComun.P7_PLANES_INTERNO;
                    CamposBase.QuienViviraSerExternado = _DatosEstudioSocioEconomicoComun.P7_VIVIRA;
                    CamposBase.CuentaOfertaTrabajoTexto = string.Format(" ( {0} ) SI, \t ESPECIFICAR: {1} \n\n ( {2} ) NO", _DatosEstudioSocioEconomicoComun.P8_OFERTA_TRABAJO == "S" ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P8_OFERTA_ESPECIFICAR, _DatosEstudioSocioEconomicoComun.P8_OFERTA_TRABAJO == "N" ? "X" : string.Empty);
                    CamposBase.CuentaAvalMoralTexto = string.Format(" ( {0} ) SI, \t ESPECIFICAR: {1} \n\n ( {2} ) NO", _DatosEstudioSocioEconomicoComun.P9_AVAL_MORAL == "S" ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P9_AVAL_ESPECIFICAR, _DatosEstudioSocioEconomicoComun.P9_AVAL_MORAL == "N" ? "X" : string.Empty);
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", _DatosEstudioSocioEconomicoComun.P10_DICTAMEN == (short)eSINO.SI ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P10_DICTAMEN == (short)eSINO.NO ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = _DatosEstudioSocioEconomicoComun.P11_MOTIVACION_DICTAMEN;
                    CamposBase.Abdomen = _DatosEstudioSocioEconomicoComun.ELABORO;
                    CamposBase.Actitud = _DatosEstudioSocioEconomicoComun.COORDINADOR;
                    CamposBase.FechaRealizacionEstudio = _DatosEstudioSocioEconomicoComun.ESTUDIO_FEC.HasValue ? _DatosEstudioSocioEconomicoComun.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }

            catch (Exception exc)
            {
                throw;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosVisitas(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleVisitas = new List<cPadronVisitantesRealizacionEstudios>();

                var Padre = new cPersonalidadEstudioSocioFamiliarComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cComunicacionComun().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    if (Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleVisitas.Add(new cPadronVisitantesRealizacionEstudios
                            {
                                Frecuencia = !string.IsNullOrEmpty(item.FRECUENCIA) ? item.FRECUENCIA.Trim() : string.Empty,
                                NombreTelefono = !string.IsNullOrEmpty(item.NOMBRE) ? item.NOMBRE.Trim() : string.Concat(string.Empty, " / ", !string.IsNullOrEmpty(item.TELEFONO) ? item.TELEFONO.Trim() : string.Empty),
                                Parentesco = item.ID_TIPO_REFERENCIA.HasValue ? !string.IsNullOrEmpty(item.TIPO_REFERENCIA.DESCR) ? item.TIPO_REFERENCIA.DESCR.Trim() : string.Empty : string.Empty
                            });
                        };
                    };
                }

                else
                {
                    var _DatoVacio = new cPadronVisitantesRealizacionEstudios()
                    {
                        Frecuencia = string.Empty,
                        NombreTelefono = string.Empty,
                        Parentesco = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor

                    _DetalleVisitas.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleVisitas;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }

            catch (System.Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgramasFortalecimientoComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleVisitas = new List<cProgramasSocioEconomicoComunReporte>();
                var Padre = new cPersonalidadEstudioSocioFamiliarComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cGruposSocioEconomicoComun().GetData(x => x.ID_INGRESO == Padre.ID_INGRESO && x.ID_IMPUTADO == Padre.ID_IMPUTADO && x.ID_CENTRO == Padre.ID_CENTRO && x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_ANIO == Padre.ID_ANIO);
                    if (Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            if (item.ID_TIPO_PROGRAMA == 9)
                            {
                                _DetalleVisitas.Add(new cProgramasSocioEconomicoComunReporte
                                {
                                    Generico = item.TIPO_PROGRAMA != null ? !string.IsNullOrEmpty(item.TIPO_PROGRAMA.NOMBRE) ? item.TIPO_PROGRAMA.NOMBRE.Trim() : string.Empty : string.Empty,
                                    Generico2 = item.CONGREGACION,
                                    Generico3 = item.PERIODO,
                                    Generico4 = item.OBSERVACIONES
                                });
                            };
                        };
                    }

                    else
                    {
                        var _DatoVacio = new cProgramasSocioEconomicoComunReporte()
                        {
                            Generico = string.Empty,
                            Generico2 = string.Empty,
                            Generico3 = string.Empty,
                            Generico4 = string.Empty
                        };//se crea un registro sin nada, el report data source espera un valor
                        _DetalleVisitas.Add(_DatoVacio);
                    }
                }

                _respuesta.Value = _DetalleVisitas;
                _respuesta.Name = "DataSet4";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }


        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgramasReligiososComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleVisitas = new List<cProgramasSocioEconomicoComunReporte>();
                var Padre = new cPersonalidadEstudioSocioFamiliarComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cGruposSocioEconomicoComun().GetData(x => x.ID_INGRESO == Padre.ID_INGRESO && x.ID_IMPUTADO == Padre.ID_IMPUTADO && x.ID_CENTRO == Padre.ID_CENTRO && x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_ANIO == Padre.ID_ANIO); //new cGruposSocioEconomicoComun().GetData(x => x.ID_INGRESO == Padre.ID_INGRESO && x.ID_IMPUTADO == Padre.ID_IMPUTADO && x.ID_CENTRO == Padre.ID_CENTRO && x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_ANIO == Padre.ID_ANIO && x.CONGREGACION != string.Empty);
                    if (Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            if (item.ID_TIPO_PROGRAMA == 11)
                            {
                                _DetalleVisitas.Add(new cProgramasSocioEconomicoComunReporte
                                {
                                    Generico = item.TIPO_PROGRAMA != null ? !string.IsNullOrEmpty(item.TIPO_PROGRAMA.NOMBRE) ? item.TIPO_PROGRAMA.NOMBRE.Trim() : string.Empty : string.Empty,
                                    Generico2 = item.CONGREGACION,
                                    Generico3 = item.PERIODO,
                                    Generico4 = item.OBSERVACIONES
                                });
                            }
                            else
                                continue;
                        };
                    }

                    else
                    {
                        var _DatoVacio = new cProgramasSocioEconomicoComunReporte()
                        {
                            Generico = string.Empty,
                            Generico2 = string.Empty,
                            Generico3 = string.Empty,
                            Generico4 = string.Empty
                        };//se crea un registro sin nada, el report data source espera un valor
                        _DetalleVisitas.Add(_DatoVacio);
                    }
                }

                _respuesta.Value = _DetalleVisitas;
                _respuesta.Name = "DataSet5";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }


        #endregion
        #region EDUCATIVO COMUN
        private void EducativoComun(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun(_ing));
                View.Report.LocalReport.DataSources.Add(DatosEstudioEducativoCulturalDeportivoFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosEscolaridadAnterior(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosActividadesEscolares(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosActividadesCulturales(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosActividadesDeportivas(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEStudiodioEducCultDepFC.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioEducativoCulturalDeportivoFueroComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var dato = new cPersonalidadEstudioEducativoCultDepComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();

                if (dato != null)
                {
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", dato.P3_DICTAMEN == (short)eDiagnosticoDictamen.FAVORABLE ? "X" : string.Empty, dato.P3_DICTAMEN == (short)eDiagnosticoDictamen.DESFAVORABLE ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = dato.P4_MOTIVACION_DICTAMEN;
                    CamposBase.FechaRealizacionEstudio = dato.ESTUDIO_FEC.HasValue ? dato.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Abdomen = dato.COORDINADOR;
                    CamposBase.Actitud = dato.ELABORO;
                }

                else
                {
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", string.Empty, string.Empty);
                    CamposBase.MotivacionDictamen = string.Empty;
                    CamposBase.FechaRealizacionEstudio = string.Empty;
                    CamposBase.Abdomen = string.Empty;
                    CamposBase.Actitud = string.Empty;
                }


                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEscolaridadAnterior(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleEscolaridad = new List<cEscolaridadAnterior>();
                var Padre = new cPersonalidadEstudioEducativoCultDepComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cEscolaridadesAnterioresIngreso().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleEscolaridad.Add(new cEscolaridadAnterior
                            {
                                DescripcionConcluida = item.CONCLUIDA == "S" ? "SI" : "NO",
                                NivelEducativo = !string.IsNullOrEmpty(item.EDUCACION_GRADO.DESCR) ? item.EDUCACION_GRADO.DESCR.Trim() : string.Empty,
                                ObservacionesEscolaridadAnterior = !string.IsNullOrEmpty(item.OBSERVACION) ? item.OBSERVACION.Trim() : string.Empty
                            });
                        };
                    }
                }
                else
                {
                    var _DatoVacio = new cEscolaridadAnterior()
                    {
                        DescripcionConcluida = string.Empty,
                        NivelEducativo = string.Empty,
                        ObservacionesEscolaridadAnterior = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor

                    _DetalleEscolaridad.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleEscolaridad;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActividadesEscolares(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleEscolaridad = new List<cActividadesEducativas>();
                var Padre = new cPersonalidadEstudioEducativoCultDepComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cEscolaridadesAnterioresIngreso().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO && !string.IsNullOrEmpty(x.RENDIMIENTO) && !string.IsNullOrEmpty(x.INTERES));
                    if (Datos != null && Datos.Any())
                    {
                        var _Act = new cActividad().GetData(x => x.ID_ACTIVIDAD == 2).FirstOrDefault();
                        foreach (var item in Datos)
                        {
                            _DetalleEscolaridad.Add(new cActividadesEducativas
                            {
                                Concluida = item.CONCLUIDA == "S" ? "SI" : "NO",
                                Nivel = _Act != null ? _Act.DESCR : string.Empty, //item.PFC_VII_EDUCATIVO != null ? item.PFC_VII_EDUCATIVO.PFC_VII_ACTIVIDAD.Any() ? item.PFC_VII_EDUCATIVO.PFC_VII_ACTIVIDAD.FirstOrDefault().ACTIVIDAD : string.Empty : string.Empty,//  item.EDUCACION_GRADO != null ? !string.IsNullOrEmpty(item.EDUCACION_GRADO.DESCR) ? item.EDUCACION_GRADO.DESCR.Trim() : string.Empty : string.Empty,
                                Observaciones = !string.IsNullOrEmpty(item.OBSERVACION) ? item.OBSERVACION.Trim() : string.Empty,
                                Interes = item.INTERES,
                                RendimientoEscolar = item.RENDIMIENTO
                            });
                        };
                    };
                }

                else
                {
                    var _DatoVacio = new cActividadesEducativas()
                    {
                        Concluida = string.Empty,
                        Interes = string.Empty,
                        Nivel = string.Empty,
                        Observaciones = string.Empty,
                        RendimientoEscolar = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor

                    _DetalleEscolaridad.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleEscolaridad;
                _respuesta.Name = "DataSet4";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActividadesCulturales(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleEscolaridad = new List<cActividadesCulturales>();
                var Padre = new cPersonalidadEstudioEducativoCultDepComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cActividadesEstudioEducativoFueroComun().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleEscolaridad.Add(new cActividadesCulturales
                            {
                                Actividad = item.ACTIVIDAD,
                                Duracion = item.DURACION,
                                Observaciones = item.OBSERVACION,
                                Programa = item.PFC_VII_PROGRAMA != null ? !string.IsNullOrEmpty(item.PFC_VII_PROGRAMA.DESCR) ? item.PFC_VII_PROGRAMA.DESCR.Trim() : string.Empty : string.Empty
                            });
                        };
                    }
                }

                else
                {
                    var _DatoVacio = new cActividadesCulturales()
                    {
                        Actividad = string.Empty,
                        Duracion = string.Empty,
                        Observaciones = string.Empty,
                        Programa = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor

                    _DetalleEscolaridad.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleEscolaridad;
                _respuesta.Name = "DataSet5";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActividadesDeportivas(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleEscolaridad = new List<cActividadesCulturales>();
                var Padre = new cPersonalidadEstudioEducativoCultDepComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cActividadesEstudioEducativoFueroComun().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleEscolaridad.Add(new cActividadesCulturales
                            {
                                Actividad = string.Empty,//item.ACTIVIDAD,
                                Duracion = string.Empty,//item.DURACION,
                                Observaciones = string.Empty,//item.OBSERVACION,
                                Programa = string.Empty//item.PFC_VII_PROGRAMA != null ? !string.IsNullOrEmpty(item.PFC_VII_PROGRAMA.DESCR) ? item.PFC_VII_PROGRAMA.DESCR.Trim() : string.Empty : string.Empty
                            });
                        };
                    };
                }

                else
                {
                    var _DatoVacio = new cActividadesCulturales()
                    {
                        Actividad = string.Empty,
                        Duracion = string.Empty,
                        Observaciones = string.Empty,
                        Programa = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor

                    _DetalleEscolaridad.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleEscolaridad;
                _respuesta.Name = "DataSet6";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        #endregion
        #region CAPACITACION COMUN
        private void CapacitacionComun(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun(_ing));
                View.Report.LocalReport.DataSources.Add(DatosEstudioCapacitacionTrabajoPenitenciarioFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosCapacitacionLaboral(_ing, _IdEstudio));//Capacitacion laboral
                View.Report.LocalReport.DataSources.Add(DatosActivNoGratificadas(_ing, _IdEstudio));//Actividades no gratificadas
                View.Report.LocalReport.DataSources.Add(DatosActivGratificadas(_ing, _IdEstudio));//Actividades gratificadas
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioCapTrabPenitFC.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioCapacitacionTrabajoPenitenciarioFueroComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var _DatosTrabajo = new cEstudioCapacitTrabajoPenitenciarioComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                if (_DatosTrabajo != null)
                {
                    CamposBase.OficioActivDesempenadaAntesReclucion = _DatosTrabajo.P1_TRABAJO_ANTES;
                    CamposBase.ResponsabilidadTexto = string.Format("BUENA ( {0} ) \t REGULAR ( {1} ) \t MALA ( {2} ) ", _DatosTrabajo.P3_RESPONSABILIDAD == "B" ? "X" : string.Empty, _DatosTrabajo.P3_RESPONSABILIDAD == "R" ? "X" : string.Empty, _DatosTrabajo.P3_RESPONSABILIDAD == "M" ? "X" : string.Empty);
                    CamposBase.CalidadTrabajoTexto = string.Format("BUENA ( {0} ) \t REGULAR ( {1} ) \t MALA ( {2} ) ", _DatosTrabajo.P3_CALIDAD == "B" ? "X" : string.Empty, _DatosTrabajo.P3_CALIDAD == "R" ? "X" : string.Empty, _DatosTrabajo.P3_CALIDAD == "M" ? "X" : string.Empty);
                    CamposBase.PerseveranciaTexto = string.Format("BUENA ( {0} ) \t REGULAR ( {1} ) \t MALA ( {2} ) ", _DatosTrabajo.P3_PERSEVERANCIA == "B" ? "X" : string.Empty, _DatosTrabajo.P3_PERSEVERANCIA == "R" ? "X" : string.Empty, _DatosTrabajo.P3_PERSEVERANCIA == "M" ? "X" : string.Empty);
                    CamposBase.CuentaFondoAhorroTexto = string.Format(" SI ( {0} ) \n NO ( {1} )", _DatosTrabajo.P4_FONDO_AHORRO == "S" ? "X" : string.Empty, _DatosTrabajo.P4_FONDO_AHORRO == "N" ? "X" : string.Empty);
                    CamposBase.DiasOtrosCentros = _DatosTrabajo.P5_DIAS_OTROS_CENTROS.HasValue ? _DatosTrabajo.P5_DIAS_OTROS_CENTROS.Value.ToString() : string.Empty;
                    CamposBase.DiasCentroActual = _DatosTrabajo.P5_DIAS_CENTRO_ACTUAL.HasValue ? _DatosTrabajo.P5_DIAS_CENTRO_ACTUAL.Value.ToString() : string.Empty;
                    CamposBase.DiasTotalLaborados = _DatosTrabajo.P5_DIAS_LABORADOS.HasValue ? _DatosTrabajo.P5_DIAS_LABORADOS.Value.ToString() : string.Empty;
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", _DatosTrabajo.P6_DICTAMEN == "F" ? "X" : string.Empty, _DatosTrabajo.P6_DICTAMEN == "D" ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = _DatosTrabajo.P7_MOTIVACION;
                    CamposBase.FechaRealizacionEstudio = _DatosTrabajo.ESTUDIO_FEC.HasValue ? _DatosTrabajo.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Abdomen = _DatosTrabajo.COORDINADOR;
                    CamposBase.Actitud = _DatosTrabajo.ELABORO;
                    CamposBase.TextoGenerico11 = _DatosTrabajo.P5_PERIODO_LABORAL.HasValue ? Fechas.fechaLetra(_DatosTrabajo.P5_PERIODO_LABORAL.Value, false).ToUpper() : string.Empty;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCapacitacionLaboral(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cCapacitacionLaboral>();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var Padre = new cEstudioCapacitTrabajoPenitenciarioComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var _DatosCapacitacion = new cActrividadesEstudioCapacitacionTrabajoFueroComun().GetData(x => x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    cCapacitacionLaboral CamposBase = new cCapacitacionLaboral();
                    if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                        foreach (var item in _DatosCapacitacion)
                        {
                            if (item.PFC_VIII_CAPACITACION != null)
                            {
                                if (item.PFC_VIII_CAPACITACION.TIPO == "L")//Capacitacion Laboral
                                {
                                    CamposBase = new cCapacitacionLaboral()
                                    {
                                        Concluyo = item.CONCLUYO == "S" ? "SI" : "NO",
                                        Observaciones = item.OBSERVACION,
                                        Periodo = item.PERIODO,
                                        Taller = item.PFC_VIII_CAPACITACION != null ? !string.IsNullOrEmpty(item.PFC_VIII_CAPACITACION.DESCR) ? item.PFC_VIII_CAPACITACION.DESCR.Trim() : string.Empty : string.Empty
                                    };

                                    _Datos.Add(CamposBase);
                                }
                                else
                                    continue;
                            }
                            else
                                continue;
                        };
                }

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActivNoGratificadas(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cCapacitacionLaboral>();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var Padre = new cEstudioCapacitTrabajoPenitenciarioComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var _DatosCapacitacion = new cActrividadesEstudioCapacitacionTrabajoFueroComun().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    cCapacitacionLaboral CamposBase = new cCapacitacionLaboral();
                    if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                        foreach (var item in _DatosCapacitacion)
                        {
                            if (item.PFC_VIII_CAPACITACION != null)
                            {
                                if (item.PFC_VIII_CAPACITACION.TIPO == "N")//Activ. no gratificadas
                                {
                                    CamposBase = new cCapacitacionLaboral()
                                    {
                                        Concluyo = item.CONCLUYO,
                                        Observaciones = item.OBSERVACION,
                                        Periodo = item.PERIODO,
                                        Taller = item.PFC_VIII_CAPACITACION != null ? !string.IsNullOrEmpty(item.PFC_VIII_CAPACITACION.DESCR) ? item.PFC_VIII_CAPACITACION.DESCR.Trim() : string.Empty : string.Empty
                                    };

                                    _Datos.Add(CamposBase);
                                }
                                else
                                    continue;
                            }
                            else
                                continue;
                        };
                }

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet4";
                return _respuesta;

            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActivGratificadas(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cCapacitacionLaboral>();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cCapacitacionLaboral CamposBase = new cCapacitacionLaboral();
                var Padre = new cEstudioCapacitTrabajoPenitenciarioComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var _DatosCapacitacion = new cActrividadesEstudioCapacitacionTrabajoFueroComun().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                        foreach (var item in _DatosCapacitacion)
                        {
                            if (item.PFC_VIII_CAPACITACION != null)
                            {
                                if (item.PFC_VIII_CAPACITACION.TIPO == "G")//Activ. gratificadas
                                {
                                    CamposBase = new cCapacitacionLaboral()
                                    {
                                        Concluyo = item.CONCLUYO,
                                        Observaciones = item.OBSERVACION,
                                        Periodo = item.PERIODO,
                                        Taller = item.PFC_VIII_CAPACITACION != null ? !string.IsNullOrEmpty(item.PFC_VIII_CAPACITACION.DESCR) ? item.PFC_VIII_CAPACITACION.DESCR.Trim() : string.Empty : string.Empty
                                    };

                                    _Datos.Add(CamposBase);
                                }
                                else
                                    continue;
                            }
                            else
                                continue;
                        };
                }

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet5";
                return _respuesta;

            }
            catch (Exception exc)
            {
                throw;
            }
        }

        #endregion
        #region SEGURIDAD COMUN
        private void SeguridadComun(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun(_ing));
                View.Report.LocalReport.DataSources.Add(DatosInformeAreaSeguridadCustodiaFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(SancionesInformeAreaSeguridadCustodiaFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/RInformeAreaSeguridadCustodiaFC.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };


                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosInformeAreaSeguridadCustodiaFueroComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                var datosReporte = new cInformeAreaSeguridadCustodiaComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (datosReporte != null)
                {
                    CamposBase.ConductaObservadaCentroTexto = string.Format(" ( {0} ) BUENA \t ( {1} ) REGULAR \t ( {2} ) MALA ", datosReporte.P1_CONDUCTA_CENTRO == "B" ? "X" : string.Empty, datosReporte.P1_CONDUCTA_CENTRO == "R" ? "X" : string.Empty, datosReporte.P1_CONDUCTA_CENTRO == "M" ? "X" : string.Empty);
                    CamposBase.ConductaAutoridadTexto = string.Format(" ( {0} ) BUENA \t ( {1} ) REGULAR \t ( {2} ) MALA ", datosReporte.P2_CONDUCTA_AUTORIDAD == "B" ? "X" : string.Empty, datosReporte.P2_CONDUCTA_AUTORIDAD == "R" ? "X" : string.Empty, datosReporte.P2_CONDUCTA_AUTORIDAD == "M" ? "X" : string.Empty);
                    CamposBase.ConductaGralTexto = string.Format(" ( {0} ) REBELDE ( {1} ) AGRESIVO ( {2} ) DISCIPLINADO ( {3} ) SE ADAPTA SIN CONFLICTOS ", datosReporte.P3_CONDUCTA_GENERAL == 1 ? "X" : string.Empty, datosReporte.P3_CONDUCTA_GENERAL == 2 ? "X" : string.Empty, datosReporte.P3_CONDUCTA_GENERAL == 3 ? "X" : string.Empty, datosReporte.P3_CONDUCTA_GENERAL == 4 ? "X" : string.Empty);
                    CamposBase.RelacionCompanerosTexto = string.Format(" ( {0} ) AISLAMIENTO ( {1} ) AGRESIVIDAD ( {2} ) CAMARADERIA ( {3} ) DOMINANTE ( {4} ) INDIFERENTE ", datosReporte.P4_RELACION_COMPANEROS == 1 ? "X" : string.Empty, datosReporte.P4_RELACION_COMPANEROS == 2 ? "X" : string.Empty, datosReporte.P4_RELACION_COMPANEROS == 3 ? "X" : string.Empty, datosReporte.P4_RELACION_COMPANEROS == 4 ? "X" : string.Empty, datosReporte.P4_RELACION_COMPANEROS == 5 ? "X" : string.Empty);
                    CamposBase.RegistraCorrectivosDiscTexto = string.Format(" SI ( {0} ) NO ( {1} )", datosReporte.P5_CORRECTIVOS == "S" ? "X" : string.Empty, datosReporte.P5_CORRECTIVOS == "N" ? "X" : string.Empty);
                    CamposBase.OpinionConductaGralInterno = string.Format(" ( {0} ) BUENA ( {1} ) REGULAR ( {2} ) MALA ", datosReporte.P6_OPINION_CONDUCTA == "B" ? "X" : string.Empty, datosReporte.P6_OPINION_CONDUCTA == "R" ? "X" : string.Empty, datosReporte.P6_OPINION_CONDUCTA == "M" ? "X" : string.Empty);
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", datosReporte.P7_DICTAMEN == "F" ? "X" : string.Empty, datosReporte.P7_DICTAMEN == "D" ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = datosReporte.P8_MOTIVACION;
                    CamposBase.FechaRealizacionEstudio = datosReporte.ESTUDIO_FEC.HasValue ? datosReporte.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Abdomen = datosReporte.COMANDANTE;
                    CamposBase.Actitud = datosReporte.ELABORO;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }
            catch (System.Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource SancionesInformeAreaSeguridadCustodiaFueroComun(INGRESO _ingreso, short _IdEstudio)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                var _Datos = new List<cCorrectivosDisc>();
                cCorrectivosDisc CamposBase = new cCorrectivosDisc();
                var Padre = new cInformeAreaSeguridadCustodiaComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var datosReporte = new cCorrectivosSeguridadFueroComun().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    if (datosReporte != null && datosReporte.Any())
                        foreach (var item in datosReporte)
                        {
                            CamposBase = new cCorrectivosDisc()
                            {
                                Fecha = item.CORRECTIVO_FEC.HasValue ? item.CORRECTIVO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty,
                                Motivo = item.MOTIVO,
                                Sancion = item.SANCION
                            };

                            _Datos.Add(CamposBase);
                        };
                };

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet3";
            }

            catch (System.Exception ex)
            {
                throw ex;
            }

            return _respuesta;
        }
        #endregion
        #endregion
        #region federal
        #region ACTA FEDERAL
        private void ActaFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                View.Report.LocalReport.DataSources.Add(DatosAreasTecnicasActaInterd(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosActaConsejoTecnicoInteridsciplinarioFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rActaConsejoTecInterdFF.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }


        private Microsoft.Reporting.WinForms.ReportDataSource DatosAreasTecnicasActaInterd(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cActaDeterminoRealizacionEstudiosPersonalidad>();
                var _DatosCapacitacion = new cActaDeterminoFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO && x.ID_ANIO == _ing.ID_ANIO && x.ID_CENTRO == _ing.ID_CENTRO);
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cActaDeterminoRealizacionEstudiosPersonalidad CamposBase = new cActaDeterminoRealizacionEstudiosPersonalidad();
                if (_DatosCapacitacion != null)
                    if (_DatosCapacitacion.Any())
                        foreach (var item in _DatosCapacitacion)
                        {
                            CamposBase = new cActaDeterminoRealizacionEstudiosPersonalidad()
                            {
                                Nombre = item.NOMBRE,
                                NombreArea = item.AREA_TECNICA != null ? !string.IsNullOrEmpty(item.AREA_TECNICA.DESCR) ? item.AREA_TECNICA.DESCR.Trim() : string.Empty : string.Empty,
                                Opinion = !string.IsNullOrEmpty(item.OPINION) ? item.OPINION == "F" ? "FAVORABLE" : "DESFAVORABLE" : string.Empty
                            };

                            _Datos.Add(CamposBase);
                        };

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;

            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActaConsejoTecnicoInteridsciplinarioFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                string _Del = string.Empty;
                string PartirDe = string.Empty;
                var CausaPenal = new cCausaPenal().GetData(x => x.ID_IMPUTADO == _ing.ID_IMPUTADO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.ACTIVO).FirstOrDefault();
                if (CausaPenal != null)
                {
                    var _delitos = new cCausaPenalDelito().Obtener(Convert.ToInt32(_ing.ID_CENTRO), Convert.ToInt32(_ing.ID_ANIO), Convert.ToInt32(_ing.ID_IMPUTADO), Convert.ToInt32(_ing.ID_INGRESO));
                    if (_delitos.Any())
                        foreach (var item in _delitos)
                        {
                            if (_delitos.Count > 1)
                                _Del += string.Format("{0} y ", item.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item.MODALIDAD_DELITO.DESCR) ? item.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty);
                            else
                                _Del += string.Format("{0} ", item.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item.MODALIDAD_DELITO.DESCR) ? item.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty);
                        };

                    var _Sentencia = new cSentencia().GetData(c => c.ID_INGRESO == _ing.ID_INGRESO && c.ID_IMPUTADO == _ing.ID_IMPUTADO && c.ID_CAUSA_PENAL == CausaPenal.ID_CAUSA_PENAL).FirstOrDefault();
                    if (_Sentencia != null)
                    {
                        var InicioComp = new cSentencia().GetData(x => x.ID_SENTENCIA == _Sentencia.ID_SENTENCIA && x.ID_IMPUTADO == _ing.ID_IMPUTADO && x.ID_INGRESO == _ing.ID_INGRESO).FirstOrDefault();
                        if (InicioComp != null)
                            PartirDe = InicioComp.FEC_INICIO_COMPURGACION.HasValue ? InicioComp.FEC_INICIO_COMPURGACION.Value.ToString("dd/MM/yyyy") : string.Empty;
                    };
                };

                var _Datos = new List<cRealizacionEstudios>();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                CamposBase.NombreInterno = string.Format("{0} {1} {2}", _ing.IMPUTADO != null ? !string.IsNullOrEmpty(_ing.IMPUTADO.NOMBRE) ? _ing.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                    _ing.IMPUTADO != null ? !string.IsNullOrEmpty(_ing.IMPUTADO.PATERNO) ? _ing.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                    _ing.IMPUTADO != null ? !string.IsNullOrEmpty(_ing.IMPUTADO.MATERNO) ? _ing.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);

                CamposBase.DelitoInterno = _Del;
                CamposBase.SentenciaInterno = CalcularSentencia().ToUpper();
                CamposBase.APartirDe = PartirDe;

                var _DatosActHecha = new cActaConsejoTecnicoFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                if (_DatosActHecha != null)
                {
                    CamposBase.ExpInterno = _DatosActHecha.EXPEDIENTE;
                    CamposBase.EnSesionFecha = _DatosActHecha.SESION_FEC.HasValue ? _DatosActHecha.SESION_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;

                    var _Edo = new cEntidad().GetData(x => x.ID_ENTIDAD == Parametro.ESTADO).FirstOrDefault();
                    if (_Edo != null)
                        CamposBase.Estado = !string.IsNullOrEmpty(_Edo.DESCR) ? _Edo.DESCR.Trim() : string.Empty;

                    CENTRO _DetalleCentro = new cCentro().GetData(x => x.ID_CENTRO == _DatosActHecha.ID_CENTRO).FirstOrDefault();
                    CamposBase.NombbreCentro = _DetalleCentro != null ? !string.IsNullOrEmpty(_DetalleCentro.DESCR) ? _DetalleCentro.DESCR.Trim() : string.Empty : string.Empty;
                    CamposBase.ActuacionTexto = _DatosActHecha.APROBADO_APLAZADO == "S" ? "APROBADO" : "APLAZADO";
                    CamposBase.VotosTexto = _DatosActHecha.APROBADO_POR == "M" ? "MAYORIA" : "UNANIMIDAD";
                    CamposBase.LugarDesc = _DatosActHecha.LUGAR;
                    CamposBase.TramiteTexto = !string.IsNullOrEmpty(_DatosActHecha.TRAMITE) ? _DatosActHecha.TRAMITE.Trim() : string.Empty;
                    CamposBase.DirectorCRS = !string.IsNullOrEmpty(_DatosActHecha.DIRECTOR) ? _DatosActHecha.DIRECTOR.Trim() : string.Empty;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        #endregion

        #region MEDICO FEDERAL
        private void MedicoFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                View.Report.LocalReport.DataSources.Add(DatosCuerpoMedicoFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosToxicosMedicoFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioMedicoFF.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;
                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoMedicoFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                if (_ing != null && _ing.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioMedicoFederal = new cEstudioMedicoFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioMedicoFederal != null)
                    {
                        CamposBase.NombreInterno = !string.IsNullOrEmpty(_DatosEstudioMedicoFederal.NOMBRE) ? _DatosEstudioMedicoFederal.NOMBRE.Trim() : string.Empty;
                        CamposBase.AliasInterno = !string.IsNullOrEmpty(_DatosEstudioMedicoFederal.ALIAS) ? _DatosEstudioMedicoFederal.ALIAS.Trim() : string.Empty;
                        CamposBase.EdadInterno = _DatosEstudioMedicoFederal.EDAD.HasValue ? _DatosEstudioMedicoFederal.EDAD.Value.ToString() : string.Empty;
                        if (!string.IsNullOrEmpty(_DatosEstudioMedicoFederal.EDO_CIVIL))
                        {
                            short _EstInterno = short.Parse(_DatosEstudioMedicoFederal.EDO_CIVIL);
                            var _EdoCivil = new cEstadoCivil().GetData(x => x.ID_ESTADO_CIVIL == _EstInterno).FirstOrDefault();
                            if (_EdoCivil != null)
                                CamposBase.EstadoCivilInterno = !string.IsNullOrEmpty(_EdoCivil.DESCR) ? _EdoCivil.DESCR.Trim() : string.Empty;
                        };

                        CamposBase.OriginarioDeInterno = _DatosEstudioMedicoFederal.ORIGINARIO_DE;
                        CamposBase.OcupacionAnteriorInterno = _DatosEstudioMedicoFederal.OCUPACION_ANT.HasValue ? !string.IsNullOrEmpty(_DatosEstudioMedicoFederal.OCUPACION.DESCR) ? _DatosEstudioMedicoFederal.OCUPACION.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.OcupacionActualInterno = _DatosEstudioMedicoFederal.OCUPACION_ACT.HasValue ? !string.IsNullOrEmpty(_DatosEstudioMedicoFederal.OCUPACION1.DESCR) ? _DatosEstudioMedicoFederal.OCUPACION1.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.DelitoInterno = _DatosEstudioMedicoFederal.DELITO;
                        CamposBase.SentenciaInterno = _DatosEstudioMedicoFederal.SENTENCIA;
                        CamposBase.AntecedentesHeredoFamFederal = _DatosEstudioMedicoFederal.ANTE_HEREDO_FAM;
                        CamposBase.AntecedentesPersonalesNoPatFederal = _DatosEstudioMedicoFederal.ANTE_PERSONAL_NO_PATOLOGICOS;
                        CamposBase.AntecedentesPatoFederal = _DatosEstudioMedicoFederal.ANTE_PATOLOGICOS;
                        CamposBase.PadecimientoActual = _DatosEstudioMedicoFederal.PADECIMIENTO_ACTUAL;
                        CamposBase.InterrogAparatosSistFederal = _DatosEstudioMedicoFederal.INTERROGATORIO_APARATOS;
                        CamposBase.ExploracionFisicaCabezCuello = _DatosEstudioMedicoFederal.EXP_FIS_CABEZA_CUELLO;
                        CamposBase.Torax = _DatosEstudioMedicoFederal.EXP_FIS_TORAX;
                        CamposBase.Abdomen = _DatosEstudioMedicoFederal.EXP_FIS_ABDOMEN;
                        CamposBase.OrganosGenit = _DatosEstudioMedicoFederal.EXP_FIS_GENITALES;
                        CamposBase.Extremidades = _DatosEstudioMedicoFederal.EXP_FIS_EXTREMIDADES;
                        CamposBase.TensionArterial = _DatosEstudioMedicoFederal.TA;
                        CamposBase.Teperatura = _DatosEstudioMedicoFederal.TEMPERATURA;
                        CamposBase.Pulso = _DatosEstudioMedicoFederal.PULSO;
                        CamposBase.Respiracion = _DatosEstudioMedicoFederal.RESPIRACION;
                        CamposBase.Estatura = _DatosEstudioMedicoFederal.ESTATURA;
                        CamposBase.DescripcionTatuajesCicatrRecAntiguasMalformacionesFederal = _DatosEstudioMedicoFederal.TATUAJES;
                        CamposBase.Diagnostico = _DatosEstudioMedicoFederal.DIAGNOSTICO;
                        CamposBase.TerpeuticaImpl = _DatosEstudioMedicoFederal.RESULTADOS_OBTENIDOS;
                        CamposBase.Conclusion = _DatosEstudioMedicoFederal.CONCLUSION;
                        CamposBase.DirectorCRS = _DatosEstudioMedicoFederal.DIRECTOR_CENTRO;
                        CamposBase.LugarDesc = _DatosEstudioMedicoFederal.LUGAR;
                        CamposBase.MatricesRavenTexto = _DatosEstudioMedicoFederal.MEDICO;
                        CamposBase.TextoGenerico1 = string.Format("( {0} ) ", _DatosEstudioMedicoFederal.ASIST_FARMACODEPENDENCIA == "S" ? "X" : string.Empty);
                        CamposBase.TextoGenerico2 = string.Format("( {0} ) ", _DatosEstudioMedicoFederal.ASIST_AA == "S" ? "X" : string.Empty);
                        CamposBase.TextoGenerico3 = string.Format("( {0} ) ", _DatosEstudioMedicoFederal.ASIST_OTROS == "S" ? "X" : string.Empty);
                        CamposBase.TextoGenerico4 = _DatosEstudioMedicoFederal.ASIST_OTROS_ESPECIF;
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosToxicosMedicoFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleToxicos = new List<cAntecedentesConsumoToxicosMedicoFederal>();
                var Padre = new cEstudioMedicoFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cSustanciaToxicaFueroFederal().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleToxicos.Add(new cAntecedentesConsumoToxicosMedicoFederal
                            {
                                Cantidad = item.CANTIDAD.ToString(),
                                EdadInicio = item.EDAD_INICIO.HasValue ? item.EDAD_INICIO.Value.ToString() : string.Empty,
                                Periodicidad = item.PERIODICIDAD,
                                Tipo = item.DROGA != null ? !string.IsNullOrEmpty(item.DROGA.DESCR) ? item.DROGA.DESCR.Trim() : string.Empty : string.Empty
                            });
                        };
                    }
                    else
                    {
                        _DetalleToxicos.Add(new cAntecedentesConsumoToxicosMedicoFederal
                        {
                            Cantidad = string.Empty,
                            EdadInicio = string.Empty,
                            Periodicidad = string.Empty,
                            Tipo = string.Empty
                        });
                    }
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        #endregion

        #region PSICOLOGICO FEDERAL

        private void PsicologicoFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                View.Report.LocalReport.DataSources.Add(DatosCuerpoPsicologicoFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioPsicologicoFF.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoPsicologicoFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                if (_ing != null && _ing.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioMedicoFederal = new cEstudioPsicologicoFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioMedicoFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioMedicoFederal.NOMBRE;
                        CamposBase.AliasInterno = _DatosEstudioMedicoFederal.SOBRENOMBRE;
                        CamposBase.EdadInterno = _DatosEstudioMedicoFederal.EDAD.HasValue ? _DatosEstudioMedicoFederal.EDAD.ToString() : string.Empty;
                        CamposBase.DelitoInterno = _DatosEstudioMedicoFederal.DELITO;
                        CamposBase.ActitudTomadaAntesEntrevista = string.Concat("ACTITUD TOMADA ANTE LA ENTREVISTA: ", _DatosEstudioMedicoFederal.ACTITUD);
                        CamposBase.ExamenMentalFF = string.Concat("EXAMEN MENTAL: ", _DatosEstudioMedicoFederal.EXAMEN_MENTAL);
                        CamposBase.PruebasAplicadas = string.Concat("PRUEBAS APLICADAS: ", _DatosEstudioMedicoFederal.PRUEBAS_APLICADAS);
                        CamposBase.NivelInt = _DatosEstudioMedicoFederal.NIVEL_INTELECTUAL;
                        CamposBase.CI = _DatosEstudioMedicoFederal.CI;
                        CamposBase.IndiceLesionOrganica = _DatosEstudioMedicoFederal.INDICE_LESION_ORGANICA;
                        CamposBase.DinamicaPersonalidadIngreso = _DatosEstudioMedicoFederal.DINAM_PERSON_INGRESO;
                        CamposBase.DinamicaPersonalidadActual = _DatosEstudioMedicoFederal.DINAM_PERSON_ACTUAL;
                        CamposBase.ResultadosTratamientoProp = _DatosEstudioMedicoFederal.RESULT_TRATAMIENTO;
                        CamposBase.RequiereTratExtraMurosTexto = string.Format("REQUERIMIENTOS DE CONTINUACIÓN DE TRATAMIENTO: SI ({0})  NO ({1})", _DatosEstudioMedicoFederal.REQ_CONT_TRATAMIENTO == "S" ? "X" : string.Empty, _DatosEstudioMedicoFederal.REQ_CONT_TRATAMIENTO == "N" ? "X" : string.Empty);
                        CamposBase.Abdomen = _DatosEstudioMedicoFederal.INTERNO;
                        CamposBase.Actitud = _DatosEstudioMedicoFederal.EXTERNO;
                        CamposBase.EspecifiqueExtraM = string.Format("ESPECIFIQUE: {0}", _DatosEstudioMedicoFederal.ESPECIFIQUE);
                        CamposBase.DirectorCRS = _DatosEstudioMedicoFederal.DIRECTOR_DENTRO;
                        CamposBase.LugarDesc = _DatosEstudioMedicoFederal.LUGAR;
                        CamposBase.Pronostico = string.Format("PRONÓSTICO DE REINTEGRACIÓN SOCIAL: {0}", _DatosEstudioMedicoFederal.PRONOSTICO_REINTEGRACION);
                        CamposBase.OpinionSobreOtorgamientoBeneficio = string.Format("OPINIÓN SOBRE EL OTORGAMIENTO DEL BENEFICIO: {0}", _DatosEstudioMedicoFederal.OPINION);
                        CamposBase.MatricesRavenTexto = _DatosEstudioMedicoFederal.PSICOLOGO;
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }

        #endregion
        #region TRABAJO SOCIAL FEDERAL
        private void SocioFamiliarFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                View.Report.LocalReport.DataSources.Add(DatosCuerpoTSFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosGrupoFamiliarPrimarioFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosGrupoFamiliarSecundarioFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioTrabajoSocFF.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }


        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoTSFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                if (_ing != null && _ing.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioTSFederal = new cEstudioTrabajoSocialFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioTSFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioTSFederal.NOMBRE;
                        CamposBase.DialectoInterno = _DatosEstudioTSFederal.DIALECTO.HasValue ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.DIALECTO1.DESCR) ? _DatosEstudioTSFederal.DIALECTO1.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.LugarFecNacInterno = _DatosEstudioTSFederal.LUGAR_NAC;
                        CamposBase.EscolaridadAnteriorIngreso = _DatosEstudioTSFederal.ESCOLARIDAD_CENTRO.HasValue ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.ESCOLARIDAD1.DESCR) ? _DatosEstudioTSFederal.ESCOLARIDAD1.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.EscolaridadAct = _DatosEstudioTSFederal.ESCOLARIDAD_ACTUAL.HasValue ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.ESCOLARIDAD.DESCR) ? _DatosEstudioTSFederal.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.OcupacionAnteriorInterno = _DatosEstudioTSFederal.TRABAJO_DESEMP_ANTES;
                        CamposBase.DomicilioInterno = _DatosEstudioTSFederal.DOMICILIO;
                        CamposBase.CaractZona = string.Format("URBANA ( {0} )  SUB - URBANA ( {1} )  RURAL ( {2} )  CRIMINOGENA (Existencia de bandas o pandillas, sobrepoblación, prostíbulos, cantinas, billares, etc.) ( {3} )",
                            _DatosEstudioTSFederal.ECO_FP_ZONA == "U" ? "X" : string.Empty, _DatosEstudioTSFederal.ECO_FP_ZONA == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.ECO_FP_ZONA == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.ECO_FP_ZONA == "N" ? "X" : string.Empty);
                        CamposBase.ResponsManutencionHogar = string.Format("RESPONSABLE(S) DE LA MANUTENCIÓN DEL HOGAR: {0}", _DatosEstudioTSFederal.ECO_FP_RESPONSABLE);
                        CamposBase.TotalIngresosMensuales = _DatosEstudioTSFederal.ECO_FP_TOTAL_INGRESOS_MEN.HasValue ? _DatosEstudioTSFederal.ECO_FP_TOTAL_INGRESOS_MEN.Value.ToString("c") : string.Empty;
                        CamposBase.TotalEgresosMensuales = _DatosEstudioTSFederal.ECO_FP_TOTAL_EGRESOS_MEN.HasValue ? _DatosEstudioTSFederal.ECO_FP_TOTAL_EGRESOS_MEN.Value.ToString("c") : string.Empty;
                        CamposBase.ActualmenteInternoCooperaEcon = _DatosEstudioTSFederal.ECO_FP_COOPERA_ACTUALMENTE;
                        CamposBase.TieneFondoAhorro = _DatosEstudioTSFederal.ECO_FP_FONDOS_AHORRO;
                        CamposBase.GrupoFamPrimarioTexto = string.Format(" GRUPO FAMILIAR: FUNCIONAL ( {0} ) DISFUNCIONAL ( {1} )", _DatosEstudioTSFederal.CARACT_FP_GRUPO == "F" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FP_GRUPO == "D" ? "X" : string.Empty);
                        CamposBase.RelacionesInterfamiliaresTexto = string.Format("RELACIONES INTERFAMILIARES: ADECUADAS ( {0} ) INADECUADAS ( {1} )", _DatosEstudioTSFederal.CARACT_FP_RELAC_INTERFAM == "A" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FP_RELAC_INTERFAM == "I" ? "X" : string.Empty);
                        CamposBase.HuboViolenciaIntrafamTexto = string.Format("{0}", _DatosEstudioTSFederal.CARACT_FP_VIOLENCIA_FAM == "S" ? "SI" : "NO");
                        CamposBase.EspecificarViolenciaIntrafam = _DatosEstudioTSFederal.CARACT_FP_VIOLENCIA_FAM_ESPEFI;
                        CamposBase.NivelSocioEconomicoCultPrimario = string.Format("NIVEL SOCIO-ECONÓMICO Y CULTURAL: ALTO ( {0} ) MEDIO ( {1} ) BAJO ( {2} )", _DatosEstudioTSFederal.CARACT_FP_NIVEL_SOCIO_CULTURAL == "A" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FP_NIVEL_SOCIO_CULTURAL == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FP_NIVEL_SOCIO_CULTURAL == "B" ? "X" : string.Empty);
                        CamposBase.AlgunIntegTieneAntecedPenales = string.Format("ALGÚN INTEGRANTE DE LA FAMILIA TIENE ANTECEDENTES PENALES O ADICCIÓN A ALGÚN ESTUPEFACIENTE O CUALQUIER TIPO DE TÓXICOS: {0}. \n\n ESPECIFIQUE: {1}", _DatosEstudioTSFederal.CARACT_FP_ANTECE_PENALES_ADIC == "S" ? "SI" : "NO", _DatosEstudioTSFederal.CARACT_FP_ANTECEDENTES_PENALES);
                        CamposBase.ConceptoTieneFamInterno = string.Format("CONCEPTO QUE TIENE LA FAMILIA DEL INTERNO: {0} ", _DatosEstudioTSFederal.CARACT_FP_CONCEPTO);
                        CamposBase.HijosUnionesAnt = _DatosEstudioTSFederal.CARACT_FS_HIJOS_ANT;
                        CamposBase.GrupoFamSec = string.Format("FUNCIONAL ( {0} )  DISFUNCIONAL ( {1} ) ", _DatosEstudioTSFederal.CARACT_FS_GRUPO == "F" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_GRUPO == "D" ? "X" : string.Empty);
                        CamposBase.RelacionesInterfamSecundario = string.Format("ADECUADAS ( {0} ) INADECUADAS ( {1} )", _DatosEstudioTSFederal.CARACT_FS_RELACIONES_INTERFAM == "A" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_RELACIONES_INTERFAM == "I" ? "X" : string.Empty);
                        CamposBase.ViolenciaIntraFamGrupoSecundario = _DatosEstudioTSFederal.CARACT_FS_VIOLENCIA_INTRAFAM == "S" ? "SI" : "NO";
                        CamposBase.EspecificViolenciaGrupoSecundario = _DatosEstudioTSFederal.CARACT_FS_VIOLENCIA_INTRAFAM_E;
                        CamposBase.NivelSocioEconomicoCulturalGrupoSecundario = string.Format("ALTO ( {0} ) MEDIO ( {1} ) BAJO ( {2} )", _DatosEstudioTSFederal.CARACT_FS_NIVEL_SOCIO_CULTURAL == "A" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_NIVEL_SOCIO_CULTURAL == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_NIVEL_SOCIO_CULTURAL == "B" ? "X" : string.Empty);

                        CamposBase.NumHabitacionesTotal = string.Format("NÚMERO DE HABITACIONES EN TOTAL (SALA, COMEDOR, COCINA, RECAMARAS, BAÑO, CUARTO DE SERVICIO, ETC.): {0}", _DatosEstudioTSFederal.CARACT_FS_VIVIEN_NUM_HABITACIO.HasValue ? _DatosEstudioTSFederal.CARACT_FS_VIVIEN_NUM_HABITACIO.Value.ToString() : string.Empty);
                        CamposBase.DescripcionVivienda = string.Format("CÓMO ES SU VIVIENDA (DESCRIPCIÓN, MATERIALES DE LOS QUE ESTÁ CONSTRUIDA): \n\n {0}", _DatosEstudioTSFederal.CARACT_FS_VIVIEN_DESCRIPCION);
                        CamposBase.TransporteCercaVivienda = string.Format("EL TRANSPORTE ESTÁ CERCA DE SU VIVIENDA O TIENE QUE CAMINAR PARA TOMARLO:\n\n {0}", _DatosEstudioTSFederal.CARACT_FS_VIVIEN_TRANSPORTE);
                        CamposBase.EnseresMobiliario = string.Format("{0}", _DatosEstudioTSFederal.CARACT_FS_VIVIEN_MOBILIARIO);
                        CamposBase.CaractZonaGrupoSec = string.Format("URBANA ( {0} )  SUB - URBANA ( {1} )  RURAL ( {2} )  CRIMINOGENA (Existencia de bandas o pandillas, sobrepoblación, prostíbulos, cantinas, billares, etc.) ( {3} )",
                            _DatosEstudioTSFederal.CARACT_FS_ZONA == "U" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_ZONA == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_ZONA == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_ZONA == "N" ? "X" : string.Empty);
                        CamposBase.RelacionMedioExterno = string.Format("RELACIÓN CON MEDIO EXTERNO: {0}", _DatosEstudioTSFederal.CARACT_FS_RELACION_MEDIO_EXT);

                        CamposBase.AlgunMiembroPresentaProbConductaPara = string.Format("ALGÚN MIEMBRO DE LA FAMILIA PRESENTA PROBLEMAS DE CONDUCTA PARA Ó ANTISOCIAL: {0} ", _DatosEstudioTSFederal.CARACT_FS_PROBLEMAS_CONDUCTA == "S" ? "SI" : "NO");
                        CamposBase.DescrConducta = string.Format("ESPECIFIQUE: {0}", _DatosEstudioTSFederal.CARACT_FS_PROBLEMAS_CONDUCTA_E);
                        CamposBase.NoPersonasVividoManeraEstable = _DatosEstudioTSFederal.NUM_PAREJAS_ESTABLE;
                        CamposBase.TrabajoAntesReclusion = _DatosEstudioTSFederal.TRABAJO_DESEMP_ANTES;
                        CamposBase.TiempoLaborar = _DatosEstudioTSFederal.TIEMPO_LABORAR;
                        CamposBase.SueldoPercibidoGrupoSecundario = _DatosEstudioTSFederal.SUELDO_PERCIBIDO.HasValue ? _DatosEstudioTSFederal.SUELDO_PERCIBIDO.Value.ToString("c") : string.Empty;
                        CamposBase.OtrasAportacionesDeLaFamilia = string.Format("APARTE DEL INTERNO, SEÑALE OTRAS APORTACIONES ECONÓMICAS DE LA FAMILIA, QUIEN LAS REALIZA Y A CUÁNTO ASCIENDEN: {0}", _DatosEstudioTSFederal.APORTACIONES_FAM);
                        CamposBase.DistribucionGastoFamiliar = string.Format("DISTRIBUCIÓN DEL GASTO FAMILIAR: {0}", _DatosEstudioTSFederal.DISTRIBUCION_GASTO_FAM);
                        CamposBase.AlimentacionFamiliar = string.Format("LA ALIMENTACIÓN FAMILIAR EN QUE CONSISTE: {0}", _DatosEstudioTSFederal.ALIMENTACION_FAM);
                        CamposBase.ServiciosCuenta = string.Format("CON QUE SERVICIOS PÚBLICOS CUENTA (LUZ, AGUA, DRENAJE, ETC.): {0}", _DatosEstudioTSFederal.SERVICIOS_PUBLICOS);
                        CamposBase.CuentaOfertaTrabajoTexto = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.OFERTA_TRABAJO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.OFERTA_TRABAJO == "N" ? "X" : string.Empty);
                        CamposBase.EspecifiqueOfertaG = string.Format("EN QUÉ CONSISTE: {0}", _DatosEstudioTSFederal.OFERTA_TRABAJO_CONSISTE);
                        CamposBase.CuentaApoyoFamiliaAlgunaPersona = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.APOYO_FAM_OTROS == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.APOYO_FAM_OTROS == "N" ? "X" : string.Empty);
                        CamposBase.RecibeVisitaFam = string.Format("RECIBE VISITAS DE FAMILIARES: \t SI ( {0} )  NO ( {1} ) \n RADICAN EN EL ESTADO: SI( {2} )  NO( {3} )", _DatosEstudioTSFederal.VISITA_FAMILIARES == "S" ? "X" : string.Empty,
                          _DatosEstudioTSFederal.VISITA_FAMILIARES == "N" ? "X" : string.Empty,
                          _DatosEstudioTSFederal.RADICAN_ESTADO == "S" ? "X" : string.Empty,
                          _DatosEstudioTSFederal.RADICAN_ESTADO == "N" ? "X" : string.Empty);
                        CamposBase.FrecuenciaG = _DatosEstudioTSFederal.VISITA_FRECUENCIA;
                        CamposBase.LugarDesc = _DatosEstudioTSFederal.LUGAR;
                        CamposBase.VisitadoOtrasPersonas = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.VISITAS_OTROS == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.VISITAS_OTROS == "S" ? "X" : string.Empty);
                        CamposBase.QuienesVisitasTexto = string.Format("QUIENES: {0}", _DatosEstudioTSFederal.VISITA_OTROS_QUIIEN);
                        CamposBase.CuentaAvalMoralTexto = string.Format("AVAL MORAL (Nombre): {0}", _DatosEstudioTSFederal.AVAL_MORAL);
                        CamposBase.ConQuienViviraAlSerExternado = string.Format("CON QUIEN VA A VIVIR AL SER EXTERNADO: {0}", _DatosEstudioTSFederal.EXTERNADO_VIVIR_NOMBRE);
                        CamposBase.OpinionInternamiento = string.Format("CUÁL ES SU OPINIÓN ACERCA DE SU INTERNAMIENTO: {0}", _DatosEstudioTSFederal.OPINION_INTERNAMIENTO);
                        CamposBase.DeQueFormaInfluenciaEstarPrision = string.Format("DE QUÉ MANERA LE HA INFLUENCIADO SU ESTANCIA EN PRISIÓN: {0}", _DatosEstudioTSFederal.INFLUENCIADO_ESTANCIA_PRISION);
                        CamposBase.Diagnostico = string.Format("DIAGNÓSTICO SOCIAL Y PRONÓSTICO DE EXTERNACIÓN: {0}", _DatosEstudioTSFederal.DIAG_SOCIAL_PRONOS);
                        CamposBase.OpinionSobreOtorgamientoBeneficio = string.Format("ANOTE SU OPINIÓN SOBRE LA CONCESIÓN DE BENEFICIOS AL INTERNO EN ESTUDIO: {0}", _DatosEstudioTSFederal.OPINION_CONCESION_BENEFICIOS);
                        CamposBase.DirectorCRS = _DatosEstudioTSFederal.DIRECTOR_CENTRO;
                        CamposBase.MatricesRavenTexto = _DatosEstudioTSFederal.TRABAJADORA_SOCIAL;
                        CamposBase.TextoGenerico2 = _DatosEstudioTSFederal.EXTERNADO_CALLE;
                        CamposBase.TextoGenerico3 = _DatosEstudioTSFederal.EXTERNADO_NUMERO;
                        CamposBase.TextoGenerico5 = _DatosEstudioTSFederal.EXTERNADO_CP;
                        if (_DatosEstudioTSFederal.EXTERNADO_PARENTESCO.HasValue)
                        {
                            var _parentescoExternado = new cTipoReferencia().GetData(x => x.ID_TIPO_REFERENCIA == _DatosEstudioTSFederal.EXTERNADO_PARENTESCO).FirstOrDefault();
                            if (_parentescoExternado != null)
                                CamposBase.TextoGenerico9 = !string.IsNullOrEmpty(_parentescoExternado.DESCR) ? _parentescoExternado.DESCR.Trim() : string.Empty;
                        };

                        if (_DatosEstudioTSFederal.VISTA_PARENTESCO.HasValue)
                        {
                            var _parentescoExternado = new cTipoReferencia().GetData(x => x.ID_TIPO_REFERENCIA == _DatosEstudioTSFederal.VISTA_PARENTESCO).FirstOrDefault();
                            if (_parentescoExternado != null)
                                CamposBase.TextoGenerico1 = !string.IsNullOrEmpty(_parentescoExternado.DESCR) ? _parentescoExternado.DESCR.Trim() : string.Empty;
                        };

                        if (_DatosEstudioTSFederal.EXTERNADO_ENTIDAD.HasValue)
                        {
                            var _Entidad = new cEntidad().GetData(x => x.ID_ENTIDAD == _DatosEstudioTSFederal.EXTERNADO_ENTIDAD).FirstOrDefault();
                            CamposBase.TextoGenerico8 = CamposBase.TextoGenerico7 = _Entidad != null ? !string.IsNullOrEmpty(_Entidad.DESCR) ? _Entidad.DESCR.Trim() : string.Empty : string.Empty;
                            if (_Entidad != null)
                            {
                                var _Municipio = new cMunicipio().GetData(x => x.ID_ENTIDAD == _Entidad.ID_ENTIDAD && x.ID_MUNICIPIO == _DatosEstudioTSFederal.EXTERNADO_MUNICIPIO).FirstOrDefault();
                                CamposBase.TextoGenerico6 = _Municipio != null ? !string.IsNullOrEmpty(_Municipio.MUNICIPIO1) ? _Municipio.MUNICIPIO1.Trim() : string.Empty : string.Empty;

                                if (_Municipio != null)
                                {
                                    var _Colonia = new cColonia().GetData(x => x.ID_ENTIDAD == _Entidad.ID_ENTIDAD && x.ID_MUNICIPIO == _Municipio.ID_MUNICIPIO).FirstOrDefault();
                                    CamposBase.TextoGenerico4 = _Colonia != null ? !string.IsNullOrEmpty(_Colonia.DESCR) ? _Colonia.DESCR.Trim() : string.Empty : string.Empty;
                                };
                            };
                        };
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosGrupoFamiliarPrimarioFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var Padre = new cEstudioTrabajoSocialFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleToxicos = new List<cGrupoFamiliarPrimarioDatos>();
                if (Padre != null)
                {
                    var Datos = new cGrupoFamiliarFueroFederal().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO && x.ID_GRUPO_FAMILIAR == (short)eTipopGrupoTrabajoSocial.PRIMARIO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleToxicos.Add(new cGrupoFamiliarPrimarioDatos
                            {
                                Edad = item.EDAD,
                                EdoCivil = item.ESTADO_CIVIL,
                                Nombre = item.NOMBRE,
                                Ocupacion = item.OCUPACION,
                                PArentesco = item.PARENTESCO
                            });
                        };
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosGrupoFamiliarSecundarioFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var Padre = new cEstudioTrabajoSocialFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleToxicos = new List<cGrupoFamiliarPrimarioDatos>();
                if (Padre != null)
                {
                    var Datos = new cGrupoFamiliarFueroFederal().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO && x.ID_GRUPO_FAMILIAR == (short)eTipopGrupoTrabajoSocial.SECUNDARIO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleToxicos.Add(new cGrupoFamiliarPrimarioDatos
                            {
                                Edad = item.EDAD,
                                EdoCivil = item.ESTADO_CIVIL,
                                Nombre = item.NOMBRE,
                                Ocupacion = item.OCUPACION,
                                PArentesco = item.PARENTESCO
                            });
                        };
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet4";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        #endregion
        #region CAPACITACION FEDERAL
        private void CapacitacionFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                View.Report.LocalReport.DataSources.Add(DatosCuerpoActividadesProductCapacFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosDiasLaboradosCapacitacionFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosCursosCapacitacionFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rInformeActivProducCapFF.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoActividadesProductCapacFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                if (_ing != null && _ing.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioTSFederal = new cCapacitacionFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioTSFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioTSFederal.NOMBRE;
                        CamposBase.NoD = string.Format("{0}-{1}{2}-{3}", _ing.CAMA != null ?
                            _ing.CAMA.CELDA != null ? _ing.CAMA.CELDA.SECTOR != null ? _ing.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(_ing.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? _ing.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                            _ing.CAMA != null ? _ing.CAMA.CELDA != null ? _ing.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(_ing.CAMA.CELDA.SECTOR.DESCR) ? _ing.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                            _ing.CAMA != null ? _ing.CAMA.CELDA != null ? !string.IsNullOrEmpty(_ing.CAMA.CELDA.ID_CELDA) ? _ing.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty, _ing.ID_UB_CAMA);
                        CamposBase.OficioActivDesempenadaAntesReclucion = _DatosEstudioTSFederal.OFICIO_ANTES_RECLUSION.HasValue ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.OCUPACION.DESCR) ? _DatosEstudioTSFederal.OCUPACION.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.SueldoPercibidoGrupoSecundario = _DatosEstudioTSFederal.SALARIO_DEVENGABA_DETENCION.HasValue ? _DatosEstudioTSFederal.SALARIO_DEVENGABA_DETENCION.Value.ToString("c") : string.Empty;
                        CamposBase.ActividadProductivaActualDentroCentro = _DatosEstudioTSFederal.ACTIVIDAD_PRODUC_ACTUAL;
                        CamposBase.AtiendeIndicacionesSuperiores = _DatosEstudioTSFederal.ATIENDE_INDICACIONES == "S" ? "SI" : "NO";
                        CamposBase.SatisfaceActividad = _DatosEstudioTSFederal.SATISFACE_ACTIVIDAD == "S" ? "SI" : "NO";
                        CamposBase.DescuidadoCumplimientoLabores = _DatosEstudioTSFederal.DESCUIDADO_LABORES == "S" ? "SI" : "NO";
                        CamposBase.MotivosTiempoInterrupcionActiv = string.Format("MOTIVOS Y TIEMPO DE LAS INTERRUPCIONES EN LA ACTIVIDAD: {0}", _DatosEstudioTSFederal.MOTIVO_TIEMPO_INTERRUP_ACT);
                        CamposBase.RecibioConstancia = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.RECIBIO_CONSTANCIA == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.RECIBIO_CONSTANCIA == "N" ? "X" : string.Empty);
                        CamposBase.EspecifiqueAsistenciaGruposFederal = string.Format("EN CASO DE NO HABER ASISTIDO A CURSOS, ESPECIFIQUE EL MOTIVO: {0}", _DatosEstudioTSFederal.NO_CURSOS_MOTIVO);
                        CamposBase.CambiadoActividades = string.Format("¿HA CAMBIADO DE ACTIVIDAD? {0}. \t ¿POR QUÉ? {1} ", _DatosEstudioTSFederal.CAMBIO_ACTIVIDAD == "S" ? "SI" : "NO", _DatosEstudioTSFederal.CAMBIO_ACTIVIDAD_POR_QUE);
                        CamposBase.ActitudesHaciaDesempenoActivProduct = string.Format("ACTITUDES HACIA EL DESEMPEÑO DE ACTIVIDADES PRODUCTIVAS: {0}", _DatosEstudioTSFederal.ACTITUDES_DESEMPENO_ACT);
                        CamposBase.CuentaFondoAhorroTexto = string.Format("SI ( {0} )\t NO ( {1} )", _DatosEstudioTSFederal.FONDO_AHORRO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.FONDO_AHORRO == "N" ? "X" : string.Empty);
                        CamposBase.CompensacionRecibeActualmen = _DatosEstudioTSFederal.FONDO_AHORRO_COMPESACION_ACTUA;
                        CamposBase.TotalDiasLaboradosEfect = _DatosEstudioTSFederal.A_TOTAL_DIAS_LABORADOS.HasValue ? _DatosEstudioTSFederal.A_TOTAL_DIAS_LABORADOS.Value.ToString() : string.Empty;
                        CamposBase.DiasOtrosCentros = _DatosEstudioTSFederal.B_DIAS_LABORADOS_OTROS_CERESOS.HasValue ? _DatosEstudioTSFederal.B_DIAS_LABORADOS_OTROS_CERESOS.Value.ToString() : string.Empty;
                        CamposBase.DiasTotalLaborados = _DatosEstudioTSFederal.TOTAL_A_B.HasValue ? _DatosEstudioTSFederal.TOTAL_A_B.Value.ToString() : string.Empty;
                        CamposBase.Conclusion = _DatosEstudioTSFederal.CONCLUSIONES;
                        CamposBase.DirectorCRS = _DatosEstudioTSFederal.DIRECTOR_CENTRO;
                        CamposBase.LugarDesc = _DatosEstudioTSFederal.LUGAR;
                        CamposBase.TextoGenerico1 = _DatosEstudioTSFederal.JEFE_SECC_INDUSTRIAL;
                        CamposBase.HaProgresadoOficio = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.HA_PROGRESADO_OFICIO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.HA_PROGRESADO_OFICIO == "N" ? "X" : string.Empty);
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosCursosCapacitacionFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                var _DetalleToxicos = new List<cCursoCapacitacionFueroFederal>();
                var Padre = new cCapacitacionFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cCapacitacionCursoFueroFederal().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleToxicos.Add(new cCursoCapacitacionFueroFederal
                            {
                                Curso = item.CURSO,
                                FechaFin = item.FECHA_TERMINO.HasValue ? item.FECHA_TERMINO.Value.ToString("dd/MM/yyyy") : string.Empty,
                                FechaInicio = item.FECHA_INICIO.HasValue ? item.FECHA_INICIO.Value.ToString("dd/MM/yyyy") : string.Empty
                            });
                        };
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet4";
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return _respuesta;
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosDiasLaboradosCapacitacionFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                var _DetalleToxicos = new List<cDiasLaboradosFueroFed>();
                var Padre = new cCapacitacionFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cDiasLaboradosFueroFederal().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                            {
                                Mes = MesString(item.MES),
                                Anio = item.ANIO.ToString(),
                                DiasLab = item.DIAS_TRABAJADOS.ToString()
                            });
                        };
                    }
                    else
                    {
                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                            {
                                Mes = "ENERO",
                                DiasLab = decimal.Zero.ToString()
                            });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "FEBRERO",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "MARZO",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "ABRIL",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "MAYO",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "JUNIO",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "JULIO",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "AGOSTO",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "SEPTIEMBRE",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "OCTUBRE",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "NOVIEMBRE",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "DICIEMBRE",
                            DiasLab = decimal.Zero.ToString()
                        });
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet3";
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _respuesta;
        }

        #endregion
        #region EDUCATIVAS FEDERAL
        private void EducativasFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                View.Report.LocalReport.DataSources.Add(DatosCuerpoInformeActivEducCultFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosParticipacionesFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rInformeActEducCultDepRecCivFF.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoInformeActivEducCultFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                if (_ing != null && _ing.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioTSFederal = new cActividadesFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioTSFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioTSFederal.NOMBRE;
                        CamposBase.EscolaridadAnteriorIngreso = _DatosEstudioTSFederal.ESCOLARIDAD_MOMENTO.HasValue ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.ESCOLARIDAD.DESCR) ? _DatosEstudioTSFederal.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.EstudiosHaRealizadoInternamiento = _DatosEstudioTSFederal.ESTUDIOS_EN_INTERNAMIENTO;
                        CamposBase.EstudiosCursaActualmente = _DatosEstudioTSFederal.ESTUDIOS_ACTUALES;
                        CamposBase.AsisteEscuelaVoluntPuntualidadAsist = string.Format("SI ( {0} ) \t NO ( {1} ) \t  EN CASO NEGATIVO ¿PORQUE? {2}", _DatosEstudioTSFederal.ASISTE_PUNTUAL == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.ASISTE_PUNTUAL == "N" ? "X" : string.Empty, _DatosEstudioTSFederal.ASISTE_PUNTUAL_NO_POR_QUE);
                        CamposBase.AvanceRendimientoAcademico = string.Format("¿CUÁL ES SU AVANCE Y RENDIMIENTO ACADÉMICO? {0}", _DatosEstudioTSFederal.AVANCE_RENDIMIENTO_ACADEMINCO);
                        CamposBase.HaSidoPromovido = string.Format("SI ( {0} ) \t NO ( {1} )", _DatosEstudioTSFederal.PROMOVIDO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.PROMOVIDO == "N" ? "X" : string.Empty);
                        CamposBase.Abdomen = string.Format("( {0} ) ", _DatosEstudioTSFederal.ALFABE_PRIMARIA == "S" ? "X" : string.Empty);
                        CamposBase.Actitud = string.Format("( {0} ) ", _DatosEstudioTSFederal.PRIMARIA_SECU == "S" ? "X" : string.Empty);
                        CamposBase.ActitudesHaciaDesempenoActivProduct = string.Format("( {0} ) ", _DatosEstudioTSFederal.SECU_BACHILLER == "S" ? "X" : string.Empty);
                        CamposBase.ActitudTomadaAntesEntrevista = string.Format("( {0} ) ", _DatosEstudioTSFederal.BACHILLER_UNI == "S" ? "X" : string.Empty);
                        CamposBase.ActividadProductivaActualDentroCentro = string.Format("( {0} ) ", _DatosEstudioTSFederal.OTRO == "S" ? "X" : string.Empty);
                        CamposBase.EspecifiqueOtrasPromociones = _DatosEstudioTSFederal.ESPECIFIQUE;
                        CamposBase.EnsenanzaRecibe = _DatosEstudioTSFederal.OTRA_ENSENANZA;
                        CamposBase.HaImpartidoEnsenanza = string.Format("SI ( {0} ) \t NO ( {1} )", _DatosEstudioTSFederal.IMPARTIDO_ENSENANZA == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.IMPARTIDO_ENSENANZA == "N" ? "X" : string.Empty);
                        CamposBase.DeQueTipoEnsenanza = _DatosEstudioTSFederal.IMPARTIDO_ENSENANZA_TIPO;
                        CamposBase.CuantoTiempoEnsenanzaImpartida = _DatosEstudioTSFederal.IMPARTIDO_ENSENANZA_TIEMPO;
                        CamposBase.Conclusion = _DatosEstudioTSFederal.CONCLUSIONES;
                        CamposBase.DirectorCRS = _DatosEstudioTSFederal.DIRECTOR_CENTRO;
                        CamposBase.MatricesRavenTexto = _DatosEstudioTSFederal.JEFE_SECC_EDUCATIVA;
                        CamposBase.LugarDesc = _DatosEstudioTSFederal.LUGAR;
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                };
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return _respuesta;

        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosParticipacionesFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                var _DetalleToxicos = new List<cParticipacionFueroFederal>();
                var Padre = new cActividadesFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cActividadParticipacionFueroFederal().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleToxicos.Add(new cParticipacionFueroFederal
                            {
                                AnioD = item.FECHA_1.HasValue ? item.FECHA_1.Value.Year.ToString() : string.Empty,
                                AnioE = item.FECHA_2.HasValue ? item.FECHA_2.Value.Year.ToString() : string.Empty,
                                Programa = item.TIPO_PROGRAMA != null ? !string.IsNullOrEmpty(item.TIPO_PROGRAMA.NOMBRE) ? item.TIPO_PROGRAMA.NOMBRE.Trim() : string.Empty : string.Empty,
                                Particip = string.Format("SI({0})  NO ({1})", item.PARTICIPACION == "S" ? "X" : string.Empty, item.PARTICIPACION == "N" ? "X" : string.Empty),
                                FecInicio = item.FECHA_1.HasValue ? item.FECHA_1.Value.ToString("dd/MM/yyyy") : string.Empty,
                                FecFin = item.FECHA_2.HasValue ? item.FECHA_2.Value.ToString("dd/MM/yyyy") : string.Empty,
                                MesD = item.FECHA_1.HasValue ? MesString(short.Parse(item.FECHA_1.Value.Month.ToString())) : string.Empty,
                                MesE = item.FECHA_2.HasValue ? MesString(short.Parse(item.FECHA_2.Value.Month.ToString())) : string.Empty,
                                DiaD = item.FECHA_1.HasValue ? DiaStringByDayOfWeek((int)(item.FECHA_1.Value.DayOfWeek)) : string.Empty,
                                DiaE = item.FECHA_2.HasValue ? DiaStringByDayOfWeek((int)(item.FECHA_2.Value.DayOfWeek)) : string.Empty
                            });
                        };
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet3";
            }
            catch (Exception ex)
            {
            }

            return _respuesta;
        }
        #endregion
        #region VIGILANCIA FEDERAL
        private void VigilanciaFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                View.Report.LocalReport.DataSources.Add(DatosCuerpoInformeVigilanciaFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosCorrectivosVigiFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rInformeSeccVigilanciaFF.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoInformeVigilanciaFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                if (_ing != null && _ing.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioTSFederal = new cVigilanciaFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioTSFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioTSFederal.NOMBRE;
                        CamposBase.FechaIngreso = _DatosEstudioTSFederal.FECHA_INGRESO.HasValue ? _DatosEstudioTSFederal.FECHA_INGRESO.Value.ToString("dd/MM/yyyy") : string.Empty;
                        CamposBase.CeresoProcede = _DatosEstudioTSFederal.CENTRO_DONDE_PROCEDE;
                        CamposBase.ConductaObservoEnElMismo = string.Format("EXCELENTE ( {0} ) BUENA ( {1} ) REGULAR ( {2} ) MALA ( {3} )", _DatosEstudioTSFederal.CONDUCTA == "E" ? "X" : string.Empty
                            , _DatosEstudioTSFederal.CONDUCTA == "B" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA == "M" ? "X" : string.Empty);
                        CamposBase.MotivoTraslado = _DatosEstudioTSFederal.MOTIVO_TRASLADO;
                        CamposBase.ConductaSuperioresTexto = string.Format("EXCELENTE ( {0} ) BUENA ( {1} ) REGULAR ( {2} ) MALA ( {3} )", _DatosEstudioTSFederal.CONDUCTA_SUPERIORES == "E" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_SUPERIORES == "B" ? "X" : string.Empty
                            , _DatosEstudioTSFederal.CONDUCTA_SUPERIORES == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_SUPERIORES == "M" ? "X" : string.Empty);
                        CamposBase.RelacionCompanieros = _DatosEstudioTSFederal.RELACION_COMPANEROS;
                        CamposBase.DescrConducta = _DatosEstudioTSFederal.DESCRIPCION_CONDUCTA;
                        CamposBase.HigienePersonalTexto = string.Format("BUENA ( {0} ) REGULAR ( {1} ) MALA ( {2} )", _DatosEstudioTSFederal.HIGIENE_PERSONAL == "B" ? "X" : string.Empty, _DatosEstudioTSFederal.HIGIENE_PERSONAL == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.HIGIENE_PERSONAL == "M" ? "X" : string.Empty);
                        CamposBase.HigieneCeldaTexto = string.Format("BUENA ( {0} ) REGULAR ( {1} ) MALA ( {2} )", _DatosEstudioTSFederal.HIGIENE_CELDA == "B" ? "X" : string.Empty, _DatosEstudioTSFederal.HIGIENE_CELDA == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.HIGIENE_CELDA == "M" ? "X" : string.Empty);
                        CamposBase.RecibeVisText = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.VISITA_RECIBE == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.VISITA_RECIBE == "N" ? "X" : string.Empty);
                        CamposBase.FrecuenciaG = string.Format("FRECUENCIA: {0}", _DatosEstudioTSFederal.VISITA_FRECUENCIA);
                        CamposBase.DeQuienesVisita = string.Format("DE QUIÉNES : {0}", _DatosEstudioTSFederal.VISITA_QUIENES);
                        CamposBase.ConductaFamilia = string.Format("BUENA ( {0} ) REGULAR ( {1} ) MALA ( {2} )", _DatosEstudioTSFederal.CONDUCTA_FAMILIA == "B" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_FAMILIA == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_FAMILIA == "M" ? "X" : string.Empty);
                        CamposBase.EstimulosBuenaCond = _DatosEstudioTSFederal.ESTIMULOS_BUENA_CONDUCTA;
                        CamposBase.ClasificConsudtaGral = string.Format("EXCELENTE ( {0} ) BUENA ( {1} ) REGULAR ( {2} ) MALA ( {3} )", _DatosEstudioTSFederal.CONDUCTA_GENERAL == "E" ? "X" : string.Empty
                            , _DatosEstudioTSFederal.CONDUCTA_GENERAL == "B" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_GENERAL == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_GENERAL == "M" ? "X" : string.Empty);
                        CamposBase.Conclusion = string.Format("CONCLUSIONES: {0}", _DatosEstudioTSFederal.CONCLUSIONES);
                        CamposBase.DirectorCRS = _DatosEstudioTSFederal.DIRECTOR_CENTRO;
                        CamposBase.TextoGenerico1 = _DatosEstudioTSFederal.JEFE_VIGILANCIA;
                        CamposBase.LugarDesc = _DatosEstudioTSFederal.LUGAR;
                        CamposBase.RecibeVisitasTexto = string.Format("SI ( {0} ) \t NO ( {1} )", _DatosEstudioTSFederal.VISITA_RECIBE == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.VISITA_RECIBE == "N" ? "X" : string.Empty);
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosCorrectivosVigiFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                var _DetalleToxicos = new List<cCorrectivosVigilanciaFF>();
                var Padre = new cVigilanciaFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cCorrectivoFueroFederal().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleToxicos.Add(new cCorrectivosVigilanciaFF
                            {
                                Anio = item.FECHA.HasValue ? item.FECHA.Value.Year.ToString() : string.Empty,
                                Motivo = item.MOTIVO,
                                ResolucionH = item.RESOLUCION,
                                Mes = item.FECHA.HasValue ? MesString(short.Parse(item.FECHA.Value.Month.ToString())) : string.Empty,
                                Dia = item.FECHA.HasValue ? DiaStringByDayOfWeek((int)(item.FECHA.Value.DayOfWeek)) : string.Empty
                            });
                        };
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet3";
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return _respuesta;
        }

        #endregion

        #region CRIMINOLOICO FEDERAL
        private void CriminologicoFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                View.Report.LocalReport.DataSources.Add(DatosCuerpoCriminologicoFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioCriminologicoFF.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                TextControlView tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoCriminologicoFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
            try
            {
                if (_ing != null && _ing.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioTSFederal = new cCriminologicoFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioTSFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioTSFederal.NOMBRE;
                        CamposBase.AliasInterno = _DatosEstudioTSFederal.SOBRENOMBRE;
                        CamposBase.VersionDelitoSegunInterno = _DatosEstudioTSFederal.P1_VERSION_INTERNO;
                        CamposBase.CaractPersonalidadRelDel = _DatosEstudioTSFederal.P2_PERSONALIDAD;
                        CamposBase.ReqValoracionVictim = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.P3_VALORACION == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.P3_VALORACION == "N" ? "X" : string.Empty);
                        CamposBase.AntecedentesParasoc = _DatosEstudioTSFederal.ANTECEDENTES_PARA_ANTI_SOCIALE;
                        CamposBase.ClasificCriminologTexto = string.Format("PRIMODELINCUENTE: {0} \n REINCIDENTE ESPECÍFICO: {1} \t\t HABITUAL: {2} \n REINCIDENTE GENÉRICO: {3} \t\t PROFESIONAL: {4}",
                            _DatosEstudioTSFederal.P5_PRIMODELINCUENTE == "S" ? "X" : string.Empty, 
                            _DatosEstudioTSFederal.P5_ESPECIFICO == "S" ? "X" : string.Empty, 
                            _DatosEstudioTSFederal.P5_HABITUAL == "S" ? "X" : string.Empty,
                            _DatosEstudioTSFederal.P5_GENERICO == "S" ? "X" : string.Empty, 
                            _DatosEstudioTSFederal.P5_PROFESIONAL == "S" ? "X" : string.Empty);
                        CamposBase.TextoGenerico1 = _DatosEstudioTSFederal.P5_PRIMODELINCUENTE == "S" ? "X" : string.Empty;
                        CamposBase.TextoGenerico2 = _DatosEstudioTSFederal.P5_ESPECIFICO == "S" ? "X" : string.Empty;
                        CamposBase.TextoGenerico3 = _DatosEstudioTSFederal.P5_HABITUAL == "S" ? "X" : string.Empty;
                        CamposBase.TextoGenerico4 = _DatosEstudioTSFederal.P5_GENERICO == "S" ? "X" : string.Empty;
                        CamposBase.TextoGenerico5 = _DatosEstudioTSFederal.P5_PROFESIONAL == "S" ? "X" : string.Empty;
                        CamposBase.Criminogenesis = _DatosEstudioTSFederal.P6_CRIMINOGENESIS;
                        CamposBase.EgocentrismoTexto = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _DatosEstudioTSFederal.P7_EGOCENTRISMO == "A" ? "X" : string.Empty,
                        _DatosEstudioTSFederal.P7_EGOCENTRISMO == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.P7_EGOCENTRISMO == "B" ? "X" : string.Empty);
                        CamposBase.LabilidadAfectivaTexto = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _DatosEstudioTSFederal.P7_LABILIDAD == "A" ? "X" : string.Empty,
                        _DatosEstudioTSFederal.P7_LABILIDAD == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.P7_LABILIDAD == "B" ? "X" : string.Empty);
                        CamposBase.AgresividadTexto = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _DatosEstudioTSFederal.P7_AGRESIVIDAD == "A" ? "X" : string.Empty,
                        _DatosEstudioTSFederal.P7_AGRESIVIDAD == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.P7_AGRESIVIDAD == "B" ? "X" : string.Empty);
                        CamposBase.IndiferenciaAfectTexto = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _DatosEstudioTSFederal.P7_INDIFERENCIA == "A" ? "X" : string.Empty,
                        _DatosEstudioTSFederal.P7_INDIFERENCIA == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.P7_INDIFERENCIA == "B" ? "X" : string.Empty);
                        CamposBase.ResultTratamInstitucional = _DatosEstudioTSFederal.P8_RESULTADO_TRATAMIENTO;
                        CamposBase.EstadoPeligrosidadActual = string.Format("MINIMO ( {0} )  MÍNIMO MEDIO ( {1} )  MEDIO ( {2} )  MEDIO ALTO ( {3} )  ALTO ( {4} )", _DatosEstudioTSFederal.P8_ESTADO_PELIGRO == Convert.ToString("1") ? "X" : string.Empty
                            , _DatosEstudioTSFederal.P8_ESTADO_PELIGRO == Convert.ToString("2") ? "X" : string.Empty, _DatosEstudioTSFederal.P8_ESTADO_PELIGRO == Convert.ToString("3") ? "X" : string.Empty, _DatosEstudioTSFederal.P8_ESTADO_PELIGRO == Convert.ToString("4") ? "X" : string.Empty
                            , _DatosEstudioTSFederal.P8_ESTADO_PELIGRO == Convert.ToString("5") ? "X" : string.Empty);
                        CamposBase.OpinionSobreOtorgamientoBeneficio = _DatosEstudioTSFederal.P10_OPINION;
                        CamposBase.ReqContinuacionTratTexto = string.Format("SI ( {0} ) \t NO ( {1} )", _DatosEstudioTSFederal.P10_CONTINUAR_TRATAMIENTO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.P10_CONTINUAR_TRATAMIENTO == "N" ? "X" : string.Empty);
                        CamposBase.Abdomen = string.Concat("EN CASO AFIRMATIVO ESPECIFICAR: ", _DatosEstudioTSFederal.P10_CONTINUAR_SI_ESPECIFICAR);
                        CamposBase.Actitud = string.Concat("EN CASO NEGATIVO ESPECIFICAR: ", _DatosEstudioTSFederal.P10_CONTINUAR_NO_ESPECIFICAR);
                        CamposBase.DirectorCRS = _DatosEstudioTSFederal.DIRECTOR_CENTRO;
                        CamposBase.MatricesRavenTexto = _DatosEstudioTSFederal.CRIMINOLOGO;
                        CamposBase.LugarDesc = _DatosEstudioTSFederal.LUGAR;
                        CamposBase.ProbabilidadReincidencia = string.Format("ALTA ( {0} )  MEDIA ( {1} )  BAJA ( {2} )", _DatosEstudioTSFederal.P9_PRONOSTICO == "A" ? "X" : string.Empty,
                        _DatosEstudioTSFederal.P9_PRONOSTICO == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.P9_PRONOSTICO == "B" ? "X" : string.Empty);
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion
        #endregion


        private void MuestraFormatoCierreEstudiosPersonalidadArchivero(PERSONALIDAD Entity)
        {
            try
            {
                if (Entity != null)
                {
                    var View = new ReportesView();
                    #region Iniciliza el entorno para mostrar el reporte al usuario
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    View.Owner = PopUpsViewModels.MainWindow;
                    View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    //View.Show();

                    #endregion

                    string NombreCentro = string.Empty;
                    string NombreMunicipio = string.Empty;
                    var Centro = new cCentro().GetData(c => c.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (Centro != null)
                    {
                        NombreCentro = !string.IsNullOrEmpty(Centro.DESCR) ? Centro.DESCR.Trim() : string.Empty;
                        NombreMunicipio = string.Format("{0} {1}", Centro.ID_MUNICIPIO.HasValue ? !string.IsNullOrEmpty(Centro.MUNICIPIO.MUNICIPIO1) ? Centro.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty,
                            Centro.ID_MUNICIPIO.HasValue ? Centro.MUNICIPIO.ENTIDAD != null ? !string.IsNullOrEmpty(Centro.MUNICIPIO.ENTIDAD.DESCR) ? Centro.MUNICIPIO.ENTIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty);
                    };

                    string ListaFavorable = string.Empty;
                    string ListaAplazados = string.Empty;
                    short _dummy = 0;
                    short _dummy2 = 0;
                    IQueryable<PERSONALIDAD> lstBrig;
                    string _alia = string.Empty;
                    var detalleByMuestra = new cEstudioPersonalidad().GetData(x => x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                    if (detalleByMuestra != null)
                    {
                        string NombreBrigada = string.Empty;
                        string NombreFolio = string.Empty;

                        NombreBrigada = detalleByMuestra.PROG_NOMBRE;
                        NombreFolio = detalleByMuestra.NUM_OFICIO;

                        lstBrig = new cEstudioPersonalidad().ObtenerDatosBrigada(NombreBrigada, NombreFolio);
                        foreach (var item in lstBrig)
                        {
                            _alia = string.Empty;
                            if (item.RESULT_ESTUDIO == "S")
                            {
                                _dummy++;
                                var _aliasImputado = new cApodo().ObtenerTodosXImputado(item.ID_CENTRO, item.ID_ANIO, item.ID_IMPUTADO);
                                if (_aliasImputado != null && _aliasImputado.Any())
                                    foreach (var item2 in _aliasImputado)
                                        _alia += string.Format(" (a) {0}", !string.IsNullOrEmpty(item2.APODO1) ? item2.APODO1.Trim() : string.Empty);

                                ListaFavorable += _dummy + ". " + string.Format("{0} {1} {2} {3}", item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.NOMBRE) ? item.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.PATERNO) ? item.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.MATERNO) ? item.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                                                !string.IsNullOrEmpty(_alia) ? _alia.Trim() : string.Empty) + "\n";
                            }
                            else
                            {
                                _dummy2++;
                                var _aliasImputado = new cApodo().ObtenerTodosXImputado(item.ID_CENTRO, item.ID_ANIO, item.ID_IMPUTADO);
                                if (_aliasImputado != null && _aliasImputado.Any())
                                    foreach (var item2 in _aliasImputado)
                                        _alia += string.Format(" (a) {0}", !string.IsNullOrEmpty(item2.APODO1) ? item2.APODO1.Trim() : string.Empty);

                                ListaAplazados += _dummy2 + ". " + string.Format("{0} {1} {2} {3}", item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.NOMBRE) ? item.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.PATERNO) ? item.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.MATERNO) ? item.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                                                !string.IsNullOrEmpty(_alia) ? _alia.Trim() : string.Empty) + "\n";
                            };
                        };

                        string NombreJ = string.Empty;
                        var NombreJuridico = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 30).FirstOrDefault();
                        if (NombreJuridico != null)
                            NombreJ = NombreJuridico.USUARIO != null ? NombreJuridico.USUARIO.EMPLEADO != null ? NombreJuridico.USUARIO.EMPLEADO.PERSONA != null ?
                                string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;

                        string _NombreAreasTecnicas = string.Empty;
                        var NombreAreasTecnicas = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 1).FirstOrDefault();
                        if (NombreAreasTecnicas != null)
                            _NombreAreasTecnicas = NombreAreasTecnicas.USUARIO != null ? NombreAreasTecnicas.USUARIO.EMPLEADO != null ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA != null ?
                                string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;


                        string CuerpoDescripcion = string.Concat("  En atención al oficio " + Entity.NUM_OFICIO + " de fecha " + (Entity.SOLICITUD_FEC.HasValue ? Fechas.fechaLetra(Entity.SOLICITUD_FEC.Value, false) : string.Empty) + " remito a usted " + lstBrig.Count() + " estudios de personalidad del fuero común con su respectivo dictamen, para el trámite de beneficio que corresponda, los cuales comprenden de las semanas ");
                        string Fecha = string.Format("{0} a {1} ", NombreMunicipio, Fechas.fechaLetra(Fechas.GetFechaDateServer, false));
                        var _dato = new cFormatoRemisionEstudiosPersonalidad()
                        {
                            Descripcion = CuerpoDescripcion,
                            Fecha = Fecha,
                            InternosAplazados = ListaAplazados,
                            InternosFavorables = ListaFavorable,
                            NombreCereso = NombreCentro,
                            NombreEntidad = NombreMunicipio,
                            NombreJefeAreasTecnicas = _NombreAreasTecnicas,
                            NombreJefeJuridico = NombreJ,
                            Generico1 = detalleByMuestra != null ? detalleByMuestra.NUM_OFICIO1 : string.Empty
                        };

                        cEncabezado Encabezado = new cEncabezado();
                        Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                        Encabezado.TituloDos = Parametro.ENCABEZADO2;
                        Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO2;
                        Encabezado.NombreReporte = NombreCentro;
                        Encabezado.ImagenFondo = Parametro.REPORTE_LOGO_ISO;
                        Encabezado.ImagenDerecha = Parametro.LOGO_ESTADO_BC;
                        Encabezado.PieUno = Parametro.DESCR_ISO_1.Replace(";", ";\n");

                        #region Inicializacion de reporte
                        View.Report.LocalReport.ReportPath = "Reportes/rFormatoRemisionEstudiosPersonalidad.rdlc";
                        View.Report.LocalReport.DataSources.Clear();
                        #endregion


                        #region Definicion d origenes de datos

                        var ds2 = new List<cEncabezado>();
                        ds2.Add(Encabezado);
                        Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds2.Name = "DataSet1";
                        rds2.Value = ds2;
                        View.Report.LocalReport.DataSources.Add(rds2);

                        var ds1 = new List<cFormatoRemisionEstudiosPersonalidad>();
                        Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        ds1.Add(_dato);
                        rds1.Name = "DataSet2";
                        rds1.Value = ds1;
                        View.Report.LocalReport.DataSources.Add(rds1);

                        #endregion
                        View.Report.RefreshReport();
                        byte[] renderedBytes;

                        Microsoft.Reporting.WinForms.Warning[] warnings;
                        string[] streamids;
                        string mimeType;
                        string encoding;
                        string extension;

                        renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                        var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                        System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                        renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                        var tc = new TextControlView();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.editor.Loaded += (s, e) =>
                        {
                            tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                            tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;
                            tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                            tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                            foreach (var item in tc.editor.Paragraphs)
                            {
                                var _parrafo = item as TXTextControl.Paragraph;
                                if (!string.IsNullOrEmpty(_parrafo.Text))
                                {
                                    if (_parrafo.Text.Contains(".:."))
                                    {
                                        _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                        _parrafo.Text.Replace(".:.", " ");
                                    };

                                    if (_parrafo.Text.Contains("~.:.~"))
                                    {
                                        _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                        _parrafo.Text.Replace("~.:.~", " ");
                                    };
                                }
                            };
                        };

                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Owner = PopUpsViewModels.MainWindow;
                        tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Show();
                    };
                };
            }
            catch (Exception exc)
            {
                throw;
            }
        }


        private string MesString(short Mes)
        {
            try
            {
                string[] meses = { "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE" };
                int d = int.Parse(Mes.ToString());
                return meses[d];
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private string DiaString(short Dia)
        {
            try
            {
                string[] semana = { "DOMINGO", "LUNES", "MARTES", "MIÉRCOLES", "JUEVES", "VIERNES", "SÁBADO" };
                int d = int.Parse(Dia.ToString());
                return semana[d];
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private string DiaStringByDayOfWeek(int Dia)
        {
            try
            {
                string[] semana = { "DOMINGO", "LUNES", "MARTES", "MIÉRCOLES", "JUEVES", "VIERNES", "SÁBADO" };
                return semana[Dia];
            }
            catch (Exception exc)
            {
                throw;
            }
        }
    }
}