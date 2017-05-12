using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.Office.Interop.Word;
using System.IO;
using System.Collections.Generic;

//using MvvmFramework;

namespace ControlPenales
{
    partial class RegistroIngresoViewModel
    {
        private void PopulateFunciones()
        {
            if (TiposDocumentos == null || TiposDocumentos.Count < 1)
            TiposDocumentos = (new cImputadoTipoDocumento()).ObtenerTodos();
        }

        private void PopulateImputadosDocumentos() 
        {
            //if (ImputadoDocumentos == null || ImputadoDocumentos.Count<1)
            //    ImputadoDocumentos = new ObservableCollection<IMPUTADO_DOCUMENTO>((new cImputadoDocumento()).Obtener(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO));
            //VisibleDocumentoVacio = false;
            //if (ImputadoDocumentos != null)
            //{
            //    if (ImputadoDocumentos.Count == 0)
            //        VisibleDocumentoVacio = true;
            //}
            //else
            //    VisibleDocumentoVacio = true;
        }

        private void LimpiarModalDocumentos()
        {
            FechaD = Fechas.GetFechaDateServer;
            ActividadD = string.Empty;
            SelectedTipoDocumento = null;
        }

        private bool GuardarDocumento(byte[] contenido)
        {
            try
            {
                    var fecha = Fechas.GetFechaDateServer;
                    var obj = new IMPUTADO_DOCUMENTO();
                    obj.ID_CENTRO = SelectIngreso.ID_CENTRO;
                    obj.ID_ANIO = SelectIngreso.ID_ANIO;
                    obj.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                    obj.ID_IM_TIPO_DOCTO = (short)enumTipoDocumentoImputado.SALA_CABOS;
                    obj.FEC_CREACION = fecha;
                    obj.FEC_MODIFICACION = fecha;
                    obj.DOCUMENTO = contenido;
                    obj.ACTIVIDAD = string.Empty;
                    obj.ID_INGRESO = SelectIngreso.ID_INGRESO;
                    if (new cImputadoDocumento().Insertar(obj))
                        return true;
                #region comentado
                //if (!base.HasErrors)
                //{
                //    cImputadoDocumento obj = new cImputadoDocumento();
                //    if (ImputadoDocumentos == null)
                //        ImputadoDocumentos = new ObservableCollection<IMPUTADO_DOCUMENTO>();

                //    var impDoc = new IMPUTADO_DOCUMENTO();
                //    impDoc.ID_CENTRO = Imputado.ID_CENTRO;
                //    impDoc.ID_ANIO = Imputado.ID_ANIO;
                //    impDoc.ID_IMPUTADO = Imputado.ID_IMPUTADO;
                //    impDoc.ID_IM_TIPO_DOCTO = SelectedTipoDocumento.ID_IM_TIPO_DOCTO;
                //    //impDoc.IMPUTADO_TIPO_DOCUMENTO = SelectedTipoDocumento;
                //    impDoc.FEC_CREACION = impDoc.FEC_MODIFICACION = FechaD;
                //    impDoc.DOCUMENTO = SelectedTipoDocumento.DOCUMENTO;
                //    impDoc.ACTIVIDAD = ActividadD;

                //    if (obj.Insertar(impDoc) > 0)
                //    {
                //        this.LimpiarModalDocumentos();
                //        this.PopulateImputadosDocumentos();
                //        SetValidacionesGenerales();
                //        return true;
                //    }
                //    else
                //    {
                //        (new Dialogos()).ConfirmacionDialogo("Error", "Ha ocurrido un Error al Guardar la Información");
                //        return false;
                //    }
                //}
                #endregion
            }
            catch ( Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar documento.", ex);
            }
            return false;
        }

        private async void EliminarDocumento() {
            try
            {
                if (SelectedImputadoDocumento != null)
                {
                    int respuesta = await (new Dialogos()).ConfirmarEliminar("Confirmación", "¿Esta Seguro que Desea Eliminar este Documento?");
                    if (respuesta == 1)
                    {
                        var cID = new cImputadoDocumento();
                        if (cID.Eliminar(SelectedImputadoDocumento.ID_CENTRO, SelectedImputadoDocumento.ID_ANIO, SelectedImputadoDocumento.ID_IMPUTADO, SelectedImputadoDocumento.ID_IM_TIPO_DOCTO))
                        {
                            this.PopulateImputadosDocumentos();
                        }
                    }
                }
                else
                    (new Dialogos()).ConfirmacionDialogo("Error", "Debe Seleccionar un Documento");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar documento.", ex);
            }
        }

        private void VerDocumento()
        {
            //if (SelectedImputadoDocumento != null)
            if (SelectIngreso == null)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de guardar antes de imprimir");
                return;
            }
            else
            {
                if (SelectIngreso.ID_ANIO == 0 && SelectIngreso.ID_IMPUTADO == 0 && SelectIngreso.ID_INGRESO == 0)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de guardar antes de imprimir");
                    return;
                }
            }
            if (SelectIngreso != null)
            {
                
                //SelectIngreso = new cIngreso().Obtener(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                //CARGAMOS DE NUEVO PARA MOSTRAR LA INFORMACION ACTUALIZADA
                SelectedImputadoDocumento = (new cImputadoDocumento()).Obtener(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO, (short)enumTipoDocumentoImputado.SALA_CABOS);
                if (SelectedImputadoDocumento == null)
                {
                    #region Reporte
                    ReporteIngreso reporte = new ReporteIngreso();
                    reporte.Nombre = string.Format("{0} {1} {2}", SelectIngreso.IMPUTADO.NOMBRE.Trim(),
                        !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty);
                    reporte.Alias = " ";
                    if (SelectIngreso.IMPUTADO.ALIAS != null)
                    {
                        string alias = string.Empty;
                        foreach (var a in SelectIngreso.IMPUTADO.ALIAS)
                        {
                            if (!string.IsNullOrEmpty(alias))
                                alias = string.Format("{0},", alias);
                            alias = alias + string.Format("{0} {1} {2}", a.NOMBRE.Trim(),
                                !string.IsNullOrEmpty(a.PATERNO) ? a.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(a.MATERNO) ? a.MATERNO.Trim() : string.Empty);
                        }
                    }
                    reporte.Apodo = " ";
                    if (SelectIngreso.IMPUTADO.APODO != null)
                    {
                        string apodos = string.Empty;
                        foreach (var a in SelectIngreso.IMPUTADO.APODO)
                        {
                            if (!string.IsNullOrEmpty(apodos))
                                apodos = string.Format("{0},", apodos);
                            apodos = apodos + a.APODO1.Trim();
                        }
                    }
                    //reporte.EstadoCivil = Imputado.ESTADO_CIVIL != null ? Imputado.ESTADO_CIVIL.DESCR : " ";
                    reporte.EstadoCivil = SelectIngreso.ESTADO_CIVIL != null ? SelectIngreso.ESTADO_CIVIL.DESCR : " ";
                    reporte.Conyugue = " ";
                    reporte.Originario = " ";
                    reporte.FecNacimiento = " ";
                    //reporte.Escolaridad = Imputado.ESCOLARIDAD != null ? Imputado.ESCOLARIDAD.DESCR : " ";
                    reporte.Escolaridad = SelectIngreso.ESCOLARIDAD != null ? SelectIngreso.ESCOLARIDAD.DESCR : " ";

                    reporte.DomicilioActual = " ";//string.Format("{0} {1},{2},{3},{4}", Imputado.DOMICILIO_CALLE, Imputado.DOMICILIO_NUM_EXT, Imputado.COLONIA.DESCR, Imputado.COLONIA.MUNICIPIO.MUNICIPIO1, Imputado.COLONIA.MUNICIPIO.ENTIDAD.DESCR);
                    var fechas = new Fechas();
                    reporte.Edad = " ";
                    if (Imputado.NACIMIENTO_FECHA != null)
                        reporte.Edad = string.Format("{0} AÑOS", fechas.CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA));
                    //reporte.TiempoBC = (Imputado.RESIDENCIA_ANIOS != null ? string.Format("{0} AÑOS",Imputado.RESIDENCIA_ANIOS) : string.Empty) + (Imputado.RESIDENCIA_MESES != null ? string.Format(" {0} MESES",Imputado.RESIDENCIA_MESES) : string.Empty);
                    reporte.TiempoBC = (SelectIngreso.RESIDENCIA_ANIOS != null ? string.Format("{0} AÑOS", SelectIngreso.RESIDENCIA_ANIOS) : string.Empty) + (SelectIngreso.RESIDENCIAS_MESES != null ? string.Format(" {0} MESES", SelectIngreso.RESIDENCIAS_MESES) : string.Empty);
                    //reporte.Telefono = Imputado.TELEFONO != null ? Imputado.TELEFONO.ToString() : " ";
                    reporte.Telefono = SelectIngreso.TELEFONO != null ? SelectIngreso.TELEFONO.ToString() : " ";
                    //reporte.Ocupacion = Imputado.OCUPACION != null ? Imputado.OCUPACION.DESCR : " ";
                    reporte.Ocupacion = SelectIngreso.OCUPACION != null ? SelectIngreso.OCUPACION.DESCR : " ";
                    //reporte.NombreMadre = string.Format("{0} {1} {2} {3}", Imputado.NOMBRE_MADRE, Imputado.PATERNO_MADRE, Imputado.MATERNO_MADRE, Imputado.MADRE_FINADO.Equals("S") ? "FINADO" : string.Empty);
                    //reporte.NombrePadre = string.Format("{0} {1} {2} {3}", Imputado.NOMBRE_PADRE, Imputado.PATERNO_PADRE, Imputado.MATERNO_PADRE, Imputado.PADRE_FINADO.Equals("S") ? "FINADO" : string.Empty);
                    reporte.NombreMadre = string.Format("{0} {1} {2} {3}",
                        Imputado.NOMBRE_MADRE.Trim(),
                        !string.IsNullOrEmpty(Imputado.PATERNO_MADRE) ? Imputado.PATERNO_MADRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(Imputado.MATERNO_MADRE) ? Imputado.MATERNO_MADRE.Trim() : string.Empty,
                        SelectIngreso.MADRE_FINADO.Equals("S") ? "FINADO" : string.Empty);
                    reporte.NombrePadre = string.Format("{0} {1} {2} {3}",
                        Imputado.NOMBRE_PADRE.Trim(),
                        !string.IsNullOrEmpty(Imputado.PATERNO_PADRE) ? Imputado.PATERNO_PADRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(Imputado.MATERNO_PADRE) ? Imputado.MATERNO_PADRE.Trim() : string.Empty,
                        SelectIngreso.PADRE_FINADO.Equals("S") ? "FINADO" : string.Empty);
                    reporte.DomicilioPadres = " ";

                    var diccionario = new Dictionary<string, string>();
                    diccionario.Add("<<nombre>>", reporte.Nombre);
                    diccionario.Add("<<alias>>", reporte.Alias);
                    diccionario.Add("<<apodo>>", reporte.Apodo);
                    diccionario.Add("<<estadoCivil>>", reporte.EstadoCivil);
                    diccionario.Add("<<edad>>", reporte.Edad);
                    diccionario.Add("<<conyugue>>", reporte.Conyugue);
                    diccionario.Add("<<originario>>", reporte.Originario);
                    diccionario.Add("<<fecNacimiento>>", reporte.FecNacimiento);
                    diccionario.Add("<<escolaridad>>", reporte.Escolaridad);
                    diccionario.Add("<<domicilioActual>>", reporte.DomicilioActual);
                    diccionario.Add("<<tiempoRadicacion>>", reporte.TiempoBC);
                    diccionario.Add("<<telefono>>", reporte.Telefono);
                    diccionario.Add("<<ocupacion>>", reporte.Ocupacion);
                    diccionario.Add("<<nombreMadre>>", reporte.NombreMadre);
                    diccionario.Add("<<nombrePadre>>", reporte.NombrePadre);
                    diccionario.Add("<<domicilioPadres>>", reporte.DomicilioPadres);
                    diccionario.Add("<<centro>>", centro.DESCR.Trim());
                     
                    var documento = new cImputadoTipoDocumento().Obtener((short)enumTipoDocumentoImputado.SALA_CABOS);

                    var contenido = new cWord().FillFieldsDocx(documento.DOCUMENTO, diccionario);
                    #endregion
                    GuardarDocumento(contenido);
                    SelectedImputadoDocumento = (new cImputadoDocumento()).Obtener(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO, (short)enumTipoDocumentoImputado.SALA_CABOS);
                }
                #region comentado
                //if (docto != null)
                //    if (docto.Count > 0)
                //        SelectedImputadoDocumento = docto[0];
                
                //if (reporte != null)
                //{
                   
                

                    //Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
                    //Document wordDoc = new Document();
                    //var tmpFile = Path.GetTempFileName();
                    //var tmpFileStream = File.OpenWrite(tmpFile);
                    //tmpFileStream.Write(SelectedImputadoDocumento.DOCUMENTO, 0, SelectedImputadoDocumento.DOCUMENTO.Length);
                    //tmpFileStream.Close();
                    //wordDoc = wordApp.Documents.Add(tmpFile);
                    //foreach (Field myMergeField in wordDoc.Fields)
                    //{
                    //    Range rngFieldCode = myMergeField.Code;
                    //    String fieldText = rngFieldCode.Text;
                    //    // ONLY GETTING THE MAILMERGE FIELDS
                    //    if (fieldText.StartsWith(" MERGEFIELD"))
                    //    {
                    //        Int32 endMerge = fieldText.IndexOf("\\");
                    //        Int32 fieldNameLength = fieldText.Length - endMerge;
                    //        String fieldName = fieldText.Substring(11, endMerge - 11);
                    //        fieldName = fieldName.Trim();

                    //        switch (fieldName)
                    //        {
                    //            case "nombre":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(reporte.Nombre);
                    //                break;
                    //            case "alias":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(reporte.Alias);
                    //                break;
                    //            case "apodo":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(reporte.Apodo);
                    //                break;
                    //            case "estadoCivil":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(reporte.EstadoCivil);
                    //                break;
                    //            case "edad":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(reporte.Edad);
                    //                break;
                    //            case "conyugue":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(reporte.Conyugue);
                    //                break;
                    //            case "originario":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(reporte.Originario);
                    //                break;
                    //            case "fecNacimiento":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(reporte.FecNacimiento);
                    //                break;
                    //            case "escolaridad":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(reporte.Escolaridad);
                    //                break;
                    //            case "domicilioActual":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(reporte.DomicilioActual);
                    //                break;
                    //            case "tiempoRadicacion":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(reporte.TiempoBC);
                    //                break;
                    //            case "telefono":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(reporte.Telefono);
                    //                break;
                    //            case "ocupacion":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(reporte.Ocupacion);
                    //                break;
                    //            case "nombreMadre":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(reporte.NombreMadre);
                    //                break;
                    //            case "nombrePadre":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(reporte.NombrePadre);
                    //                break;
                    //            case "domicilioPadres":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(reporte.DomicilioPadres);
                    //                break;
                    //            case "centro":
                    //                myMergeField.Select();
                    //                wordApp.Selection.TypeText(centro.DESCR.Trim());
                    //                break;
                    //        }
                    //    }
                    //}
                    //wordDoc.SaveAs(tmpFile, WdSaveFormat.wdFormatDocument97);
                    //wordDoc.Close();
                //byte[] bytes = System.IO.File.ReadAllBytes(tmpFile);
                #endregion
                var v = new EditorView(SelectedImputadoDocumento, SelectedImputadoDocumento.DOCUMENTO);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    v.Owner = PopUpsViewModels.MainWindow;
                    v.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    v.Show();
                //}
            }

        }

        #region ImprimirDocumento()
        private void ImprimirDocumento() 
        {
            try
            {
                if (SelectedImputadoDocumento != null)
                {
                    var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                    var parametros = new Dictionary<string,string>();
                    parametros.Add("<<centro>>", centro.DESCR.Trim());
                    parametros.Add("<<nombre>>", string.Format("{0} {1} {2}", 
                        !string.IsNullOrEmpty(Imputado.NOMBRE) ? Imputado.NOMBRE.Trim() : string.Empty, 
                        !string.IsNullOrEmpty(Imputado.PATERNO) ? Imputado.PATERNO.Trim() : string.Empty, 
                        !string.IsNullOrEmpty(Imputado.MATERNO) ? Imputado.MATERNO.Trim() : string.Empty));
                    if (Imputado.ALIAS != null)
                    {
                        string alias = string.Empty;
                        foreach (var a in Imputado.ALIAS)
                        {
                            if (!string.IsNullOrEmpty(alias))
                                alias = string.Format("{0},", alias);
                            alias = alias + string.Format("{0} {1} {2}", a.NOMBRE, a.PATERNO, a.MATERNO);
                        }
                        parametros.Add("<<alias>>", alias);
                    }
                    else
                        parametros.Add("<<alias>>", string.Empty);
                    if (Imputado.APODO != null)
                    {
                        string apodos = string.Empty;
                        foreach (var a in Imputado.APODO)
                        {
                            if (!string.IsNullOrEmpty(apodos))
                                apodos = string.Format("{0},", apodos);
                            apodos = apodos + a.APODO1;
                        }
                        parametros.Add("<<apodo>>", apodos);
                    }
                    else
                        parametros.Add("<<apodo>>", string.Empty);

                    //parametros.Add("<<estadoCivil>>", Imputado.ESTADO_CIVIL != null ? Imputado.ESTADO_CIVIL.DESCR.Trim() : string.Empty);
                    parametros.Add("<<estadoCivil>>", SelectIngreso.ESTADO_CIVIL != null ? SelectIngreso.ESTADO_CIVIL.DESCR.Trim() : string.Empty);
                    parametros.Add("<<edad>>", new Fechas().CalculaEdad(Imputado.NACIMIENTO_FECHA).ToString());
                    parametros.Add("<<conyugue>>", string.Empty);
                    parametros.Add("<<originario>>", string.Empty);
                    parametros.Add("<<fecNacimiento>>", Imputado.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy"));
                    //parametros.Add("<<escolaridad>>", Imputado.ESCOLARIDAD != null ? Imputado.ESCOLARIDAD.DESCR.Trim() : string.Empty);
                    parametros.Add("<<escolaridad>>", SelectIngreso.ESCOLARIDAD != null ? SelectIngreso.ESCOLARIDAD.DESCR.Trim() : string.Empty);

                    //parametros.Add("<<domicilioActual>>", string.Format("{0} {1},{2},{3},{4}", 
                    //    !string.IsNullOrEmpty(Imputado.DOMICILIO_CALLE) ? Imputado.DOMICILIO_CALLE.Trim() : string.Empty,
                    //    Imputado.DOMICILIO_NUM_EXT,
                    //    Imputado.COLONIA != null ? Imputado.COLONIA.DESCR.Trim() : string.Empty,
                    //    Imputado.COLONIA.MUNICIPIO != null ? Imputado.COLONIA.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty,
                    //    Imputado.COLONIA.MUNICIPIO.ENTIDAD != null ? Imputado.COLONIA.MUNICIPIO.ENTIDAD.DESCR.Trim() : string.Empty));
                    parametros.Add("<<domicilioActual>>", string.Format("{0} {1},{2},{3},{4}",
                       !string.IsNullOrEmpty(SelectIngreso.DOMICILIO_CALLE) ? SelectIngreso.DOMICILIO_CALLE.Trim() : string.Empty,
                       SelectIngreso.DOMICILIO_NUM_EXT,
                       SelectIngreso.COLONIA != null ? SelectIngreso.COLONIA.DESCR.Trim() : string.Empty,
                       SelectIngreso.COLONIA.MUNICIPIO != null ? SelectIngreso.COLONIA.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty,
                       SelectIngreso.COLONIA.MUNICIPIO.ENTIDAD != null ? SelectIngreso.COLONIA.MUNICIPIO.ENTIDAD.DESCR.Trim() : string.Empty));
                    //parametros.Add("<<TiempoRadicacion>>",(Imputado.RESIDENCIA_ANIOS != null ? string.Format("{0} AÑOS", Imputado.RESIDENCIA_ANIOS) : string.Empty) + (Imputado.RESIDENCIA_MESES != null ? string.Format(" {0} MESES", Imputado.RESIDENCIA_MESES) : string.Empty));
                    parametros.Add("<<TiempoRadicacion>>", (SelectIngreso.RESIDENCIA_ANIOS != null ? string.Format("{0} AÑOS", SelectIngreso.RESIDENCIA_ANIOS) : string.Empty) + (SelectIngreso.RESIDENCIAS_MESES != null ? string.Format(" {0} MESES", SelectIngreso.RESIDENCIAS_MESES) : string.Empty));
                    //parametros.Add("<<telefono>>", Imputado.TELEFONO != null ? Imputado.TELEFONO.ToString() : string.Empty);
                    parametros.Add("<<telefono>>", SelectIngreso.TELEFONO != null ? SelectIngreso.TELEFONO.ToString() : string.Empty);
                    //parametros.Add("<<ocupacion>>", Imputado.OCUPACION != null ? Imputado.OCUPACION.DESCR.Trim() : string.Empty);
                    parametros.Add("<<ocupacion>>", SelectIngreso.OCUPACION != null ? SelectIngreso.OCUPACION.DESCR.Trim() : string.Empty);
                    //parametros.Add("<<NombreMadre>>",string.Format("{0} {1} {2} {3}", 
                    //    Imputado.NOMBRE_MADRE, 
                    //    !string.IsNullOrEmpty(Imputado.PATERNO_MADRE) ? Imputado.PATERNO_MADRE.Trim() : string.Empty,
                    //    !string.IsNullOrEmpty(Imputado.MATERNO_MADRE) ? Imputado.MATERNO_MADRE.Trim() : string.Empty, 
                    //    Imputado.MADRE_FINADO.Equals("S") ? "FINADO" : string.Empty));
                    parametros.Add("<<NombreMadre>>", string.Format("{0} {1} {2} {3}",
                        Imputado.NOMBRE_MADRE,
                        !string.IsNullOrEmpty(Imputado.PATERNO_MADRE) ? Imputado.PATERNO_MADRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(Imputado.MATERNO_MADRE) ? Imputado.MATERNO_MADRE.Trim() : string.Empty,
                        SelectIngreso.MADRE_FINADO.Equals("S") ? "FINADO" : string.Empty));
                    //parametros.Add("<<nombrePadre>>", string.Format("{0} {1} {2} {3}", 
                    //    Imputado.NOMBRE_PADRE, 
                    //    !string.IsNullOrEmpty(Imputado.PATERNO_PADRE) ? Imputado.PATERNO_PADRE.Trim() : string.Empty,
                    //    !string.IsNullOrEmpty(Imputado.MATERNO_PADRE) ? Imputado.MATERNO_PADRE.Trim() : string.Empty, 
                    //    Imputado.PADRE_FINADO.Equals("S") ? "FINADO" : string.Empty));
                    parametros.Add("<<nombrePadre>>", string.Format("{0} {1} {2} {3}",
                        Imputado.NOMBRE_PADRE,
                        !string.IsNullOrEmpty(Imputado.PATERNO_PADRE) ? Imputado.PATERNO_PADRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(Imputado.MATERNO_PADRE) ? Imputado.MATERNO_PADRE.Trim() : string.Empty,
                        SelectIngreso.PADRE_FINADO.Equals("S") ? "FINADO" : string.Empty));
                    parametros.Add("<<domicilioPadres>>", string.Empty);
/*<<nombre>>
<<alias>>
<<apodo>>
<<estadoCivil>>
<<edad>>
<<conyugue>>
<<originario>>
<<fecNacimiento>>
<<escolaridad>>
<<domicilioActual>>
<<TiempoRadicacion>>
<<telefono>>
<<ocupacion>>
<<NombreMadre>>
<<nombrePadre>>
<<domicilioPadres>>*/


                    
                    
                    //CARGAMOS DE NUEVO PARA MOSTRAR LA INFORMACION ACTUALIZADA
                    SelectedImputadoDocumento = (new cImputadoDocumento()).Obtener(SelectedImputadoDocumento.ID_CENTRO, SelectedImputadoDocumento.ID_ANIO, SelectedImputadoDocumento.ID_IMPUTADO, SelectedImputadoDocumento.ID_IM_TIPO_DOCTO);
                    //if (docto != null)
                    //    if (docto.Count > 0)
                    //        SelectedImputadoDocumento = docto[0];


                    ReporteIngreso reporte = new ReporteIngreso();
                    reporte.Nombre = string.Format("{0} {1} {2}", Imputado.NOMBRE, Imputado.PATERNO, Imputado.MATERNO);
                    reporte.Alias = " ";
                    if (Imputado.ALIAS != null)
                    {
                        string alias = string.Empty;
                        foreach (var a in Imputado.ALIAS)
                        {
                            if (!string.IsNullOrEmpty(alias))
                                alias = string.Format("{0},", alias);
                            alias = alias + string.Format("{0} {1} {2}", a.NOMBRE, a.PATERNO, a.MATERNO);
                        }
                    }
                    reporte.Apodo = " ";
                    if (Imputado.APODO != null)
                    {
                        string apodos = string.Empty;
                        foreach (var a in Imputado.APODO)
                        {
                            if (!string.IsNullOrEmpty(apodos))
                                apodos = string.Format("{0},", apodos);
                            apodos = apodos + a.APODO1;
                        }
                    }
                    //reporte.EstadoCivil = Imputado.ESTADO_CIVIL != null ? Imputado.ESTADO_CIVIL.DESCR : " ";
                    reporte.EstadoCivil = SelectIngreso.ESTADO_CIVIL != null ? SelectIngreso.ESTADO_CIVIL.DESCR : " ";
                    reporte.Conyugue = " ";
                    reporte.Originario = " ";
                    reporte.FecNacimiento = " ";
                    //reporte.Escolaridad = Imputado.ESCOLARIDAD != null ? Imputado.ESCOLARIDAD.DESCR : " ";
                    reporte.Escolaridad = SelectIngreso.ESCOLARIDAD != null ? SelectIngreso.ESCOLARIDAD.DESCR : " ";

                    reporte.DomicilioActual = " ";//string.Format("{0} {1},{2},{3},{4}", Imputado.DOMICILIO_CALLE, Imputado.DOMICILIO_NUM_EXT, Imputado.COLONIA.DESCR, Imputado.COLONIA.MUNICIPIO.MUNICIPIO1, Imputado.COLONIA.MUNICIPIO.ENTIDAD.DESCR);
                    var fechas = new Fechas();
                    reporte.Edad = " ";
                    if (Imputado.NACIMIENTO_FECHA != null)
                        reporte.Edad = string.Format("{0} AÑOS", fechas.CalculaEdad(Imputado.NACIMIENTO_FECHA));
                    //reporte.TiempoBC = (Imputado.RESIDENCIA_ANIOS != null ? string.Format("{0} AÑOS", Imputado.RESIDENCIA_ANIOS) : string.Empty) + (Imputado.RESIDENCIA_MESES != null ? string.Format(" {0} MESES", Imputado.RESIDENCIA_MESES) : string.Empty);
                    reporte.TiempoBC = (SelectIngreso.RESIDENCIA_ANIOS != null ? string.Format("{0} AÑOS", SelectIngreso.RESIDENCIA_ANIOS) : string.Empty) + (SelectIngreso.RESIDENCIAS_MESES != null ? string.Format(" {0} MESES", SelectIngreso.RESIDENCIAS_MESES) : string.Empty);
                    //reporte.Telefono = Imputado.TELEFONO != null ? Imputado.TELEFONO.ToString() : " ";
                    reporte.Telefono = SelectIngreso.TELEFONO != null ? SelectIngreso.TELEFONO.ToString() : " ";
                    //reporte.Ocupacion = Imputado.OCUPACION != null ? Imputado.OCUPACION.DESCR : " ";
                    reporte.Ocupacion = SelectIngreso.OCUPACION != null ? SelectIngreso.OCUPACION.DESCR : " ";
                    //reporte.NombreMadre = string.Format("{0} {1} {2} {3}", Imputado.NOMBRE_MADRE, Imputado.PATERNO_MADRE, Imputado.MATERNO_MADRE, Imputado.MADRE_FINADO.Equals("S") ? "FINADO" : string.Empty);
                    reporte.NombreMadre = string.Format("{0} {1} {2} {3}", Imputado.NOMBRE_MADRE, Imputado.PATERNO_MADRE, Imputado.MATERNO_MADRE, SelectIngreso.MADRE_FINADO.Equals("S") ? "FINADO" : string.Empty);
                    //reporte.NombrePadre = string.Format("{0} {1} {2} {3}", Imputado.NOMBRE_PADRE, Imputado.PATERNO_PADRE, Imputado.MATERNO_PADRE, Imputado.PADRE_FINADO.Equals("S") ? "FINADO" : string.Empty); ;
                    reporte.NombrePadre = string.Format("{0} {1} {2} {3}", Imputado.NOMBRE_PADRE, Imputado.PATERNO_PADRE, Imputado.MATERNO_PADRE, SelectIngreso.PADRE_FINADO.Equals("S") ? "FINADO" : string.Empty); ;
                    reporte.DomicilioPadres = " ";
                    if (reporte != null)
                    {
                        Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
                        Document wordDoc = new Document();
                        var tmpFile = Path.GetTempFileName();
                        var tmpFileStream = File.OpenWrite(tmpFile);
                        tmpFileStream.Write(SelectedImputadoDocumento.DOCUMENTO, 0, SelectedImputadoDocumento.DOCUMENTO.Length);
                        tmpFileStream.Close();
                        wordDoc = wordApp.Documents.Add(tmpFile);
                        foreach (Field myMergeField in wordDoc.Fields)
                        {
                            Range rngFieldCode = myMergeField.Code;
                            String fieldText = rngFieldCode.Text;
                            // ONLY GETTING THE MAILMERGE FIELDS
                            if (fieldText.StartsWith(" MERGEFIELD"))
                            {
                                Int32 endMerge = fieldText.IndexOf("\\");
                                Int32 fieldNameLength = fieldText.Length - endMerge;
                                String fieldName = fieldText.Substring(11, endMerge - 11);
                                fieldName = fieldName.Trim();

                                switch (fieldName)
                                {
                                    case "nombre":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(reporte.Nombre);
                                        break;
                                    case "alias":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(reporte.Alias);
                                        break;
                                    case "apodo":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(reporte.Apodo);
                                        break;
                                    case "estadoCivil":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(reporte.EstadoCivil);
                                        break;
                                    case "edad":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(reporte.Edad);
                                        break;
                                    case "conyugue":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(reporte.Conyugue);
                                        break;
                                    case "originario":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(reporte.Originario);
                                        break;
                                    case "fecNacimiento":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(reporte.FecNacimiento);
                                        break;
                                    case "escolaridad":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(reporte.Escolaridad);
                                        break;
                                    case "domicilioActual":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(reporte.DomicilioActual);
                                        break;
                                    case "tiempoRadicacion":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(reporte.TiempoBC);
                                        break;
                                    case "telefono":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(reporte.Telefono);
                                        break;
                                    case "ocupacion":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(reporte.Ocupacion);
                                        break;
                                    case "nombreMadre":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(reporte.NombreMadre);
                                        break;
                                    case "nombrePadre":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(reporte.NombrePadre);
                                        break;
                                    case "domicilioPadres":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(reporte.DomicilioPadres);
                                        break;
                                    case "centro":
                                        myMergeField.Select();
                                        wordApp.Selection.TypeText(centro.DESCR.Trim());
                                        break;
                                }
                            }
                        }
                        wordDoc.SaveAs(tmpFile, WdSaveFormat.wdFormatDocument97);
                        wordDoc.Close();
                        byte[] bytes = System.IO.File.ReadAllBytes(tmpFile);
                        var v = new EditorView(SelectedImputadoDocumento, bytes);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        v.Owner = PopUpsViewModels.MainWindow;
                        v.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                        v.Show();
                    }
                }

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al imprimir documento.", ex);
            }
        }
        #endregion
    }
}
