using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Office.Interop.Word;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TXTextControl;

namespace ControlPenales
{
    /// <summary>
    /// Interaction logic for EditorView.xaml
    /// </summary>
    public partial class EditorView : MahApps.Metro.Controls.MetroWindow
    {
        Object obj = null;
        byte[] doc = null;
        public EditorView(Object obj = null, byte[] docto = null)
        {
            if(obj != null)
                this.obj = obj;
            if (docto != null)
                doc = docto;

            InitializeComponent();
            editor.Loaded +=editor_Loaded;
        }

        private ReporteIngreso repIngreso;
        internal ReporteIngreso RepIngreso
        {
            get { return repIngreso; }
            set { repIngreso = value; }
        }
        private void editor_Loaded(object sender, EventArgs e) 
        {
            //byte[] docto = System.IO.File.ReadAllBytes("E:\\Reportes\\PlantillaRegistro.doc");
            //var obj = new IMPUTADO_TIPO_DOCUMENTO();
            //obj.ID_IM_TIPO_DOCTO = 162;
            //obj.DESCR = "REPORTE - SALA CABOS";
            //obj.ID_FORMATO = 1;
            //obj.DOCUMENTO = docto;
            //(new cImputadoTipoDocumento()).Insertar(obj);

            //if (reporte != null)
            //{
             //   Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            //    Object oMissing = System.Reflection.Missing.Value;
            //    Object oTemplatePath = "E:\\Reportes\\PlantillaRegistro.doc";
            //    Document wordDoc = new Document();
            //    wordDoc = wordApp.Documents.Add(ref oTemplatePath, ref oMissing, ref oMissing, ref oMissing);
            //    foreach (Field myMergeField in wordDoc.Fields)
            //    {
            //        Range rngFieldCode = myMergeField.Code;
            //        String fieldText = rngFieldCode.Text;
            //        // ONLY GETTING THE MAILMERGE FIELDS
            //        if (fieldText.StartsWith(" MERGEFIELD"))
            //        {
            //            Int32 endMerge = fieldText.IndexOf("\\");
            //            Int32 fieldNameLength = fieldText.Length - endMerge;
            //            String fieldName = fieldText.Substring(11, endMerge - 11);
            //            fieldName = fieldName.Trim();

            //            switch (fieldName)
            //            {
            //                case "nombre":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText(reporte.Nombre);
            //                    break;
            //                case "alias":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText(reporte.Alias);
            //                    break;
            //                case "apodo":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText(reporte.Apodo);
            //                    break;
            //                case "estadoCivil":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText(reporte.EstadoCivil);
            //                    break;
            //                case "edad":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText(reporte.Edad);
            //                    break;
            //                case "conyugue":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText(reporte.Conyugue);
            //                    break;
            //                case "originario":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText(reporte.Originario);
            //                    break;
            //                case "fecNacimiento":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText(reporte.FecNacimiento);
            //                    break;
            //                case "escolaridad":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText(reporte.Escolaridad);
            //                    break;
            //                case "domicilioActual":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText(reporte.DomicilioActual);
            //                    break;
            //                case "tiempoRadicacion":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText(reporte.TiempoBC);
            //                    break;
            //                case "telefono":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText(reporte.Telefono);
            //                    break;
            //                case "ocupacion":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText(reporte.Ocupacion);
            //                    break;
            //                case "nombreMadre":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText(reporte.NombreMadre);
            //                    break;
            //                case "nombrePadre":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText(reporte.NombrePadre);
            //                    break;
            //                case "domicilioPadres":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText(reporte.DomicilioPadres);
            //                    break;
            //                case "centro":
            //                    myMergeField.Select();
            //                    wordApp.Selection.TypeText("Centro de Reinserción Social de Mexicali");
            //                    break;
            //            }
            //        }
            //    }
            //    wordDoc.SaveAs("E:\\Reportes\\reporteFicha.doc", WdSaveFormat.wdFormatDocument97);
            //    wordDoc.Close();
            //    byte[] bytes = System.IO.File.ReadAllBytes("E:\\Reportes\\reporteFicha.doc");
            //    editor.Load(bytes, TXTextControl.BinaryStreamType.MSWord);
            //}
            editor.Load(doc, TXTextControl.BinaryStreamType.WordprocessingML);
        }

        private void Imprimir(object sender, RoutedEventArgs e)
        {
            editor.Print("ReporteIngreso", true);
        }

        private async void Guardar(object sender, RoutedEventArgs e)
        {
            if (obj != null)
            {
                var tipo = obj.GetType();
                switch (tipo.BaseType.Name)
                { 
                    case "IMPUTADO_DOCUMENTO":
                          byte[] data;
                          editor.Save(out data,BinaryStreamType.MSWord);
                          var impDoc = (IMPUTADO_DOCUMENTO)obj;
                          var doc = new IMPUTADO_DOCUMENTO();
                          doc.ID_CENTRO = impDoc.ID_CENTRO;
                          doc.ID_ANIO = impDoc.ID_ANIO;
                          doc.ID_IMPUTADO = impDoc.ID_IMPUTADO;
                          doc.ID_INGRESO = impDoc.ID_INGRESO;
                          doc.ID_IM_TIPO_DOCTO = impDoc.ID_IM_TIPO_DOCTO;
                          doc.FEC_CREACION = impDoc.FEC_CREACION;
                          doc.FEC_MODIFICACION = Fechas.GetFechaDateServer;
                          doc.DOCUMENTO = data;
                          doc.ACTIVIDAD = impDoc.ACTIVIDAD;
                          if((new cImputadoDocumento()).Actualizar(doc))
                              
                              //new Dialogos().ConfirmacionDialogo("EXITO!", "Informacion Grabada Exitosamente!");
                              await this.ShowMessageAsync("ÉXITO!", "Información Grabada Exitosamente!");
                          else
                              //new Dialogos().ConfirmacionDialogo("ERROR!", "Ha ocurrido un error al guardar la informacion!");
                              await this.ShowMessageAsync("ERROR", "Ha Ocurrido un Error al Guardar la Información");
                        break;
                }
            }
        }

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            editor.Height = e.NewSize.Height - 150;
        }
    }
}
