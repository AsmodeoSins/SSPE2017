using Microsoft.Office.Interop.Word;
using Novacode;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace ControlPenales
{
    public class cWord
    {
        public cWord() { }

        public byte[] FillFields(byte[] template, Dictionary<string, string> values,byte[] Logo1,byte[] Logo2,byte[] Foto)
        {
            try
            {
                Microsoft.Office.Interop.Word.Application application = new Microsoft.Office.Interop.Word.Application();
                var tmpFile = Path.GetTempFileName();
                var tmpFileStream = File.OpenWrite(tmpFile);
                tmpFileStream.Write(template, 0, template.Length);
                tmpFileStream.Close();
                Document document = application.Documents.Open(tmpFile);
                File.WriteAllBytes(Path.GetTempPath() + "foto.png", Foto);
                var f = document.Shapes.AddPicture(Path.GetTempPath() + "foto.png");
                f.Width = 50;
                f.Height = 50;
                f.Left = (float)Microsoft.Office.Interop.Word.WdShapePosition.wdShapeRight;

                foreach (Microsoft.Office.Interop.Word.Section section in document.Sections)
                {
                    //HEADER
                    var headers = section.Headers;
                    foreach (Microsoft.Office.Interop.Word.HeaderFooter header in headers)
                    {

                        if (header.IsHeader)
                        {
                            File.WriteAllBytes(Path.GetTempPath() + "logo1.png",Logo1);
                            var l1 = header.Shapes.AddPicture(Path.GetTempPath() + "logo1.png");
                            l1.Width = 50;
                            l1.Height = 50;
                            l1.Left = (float)Microsoft.Office.Interop.Word.WdShapePosition.wdShapeLeft;

                            File.WriteAllBytes(Path.GetTempPath() + "logo2.png", Logo2);
                            var l2 = header.Shapes.AddPicture(Path.GetTempPath() + "logo2.png");
                            l2.Width = 50;
                            l2.Height = 50;
                            l2.Left = (float)Microsoft.Office.Interop.Word.WdShapePosition.wdShapeRight;

                        }

                        Fields fields = header.Range.Fields;
                        foreach (Field mergeField in fields)
                        {
                            string fieldText = mergeField.Code.Text;
                            string fieldName = GetFieldName(fieldText);
                            if (values.ContainsKey(fieldName))
                            {
                                mergeField.Select();
                                application.Selection.TypeText(values[fieldName]);
                            }
                        }
                    }
                    //FOOTER
                    var footers = section.Footers;
                    foreach (Microsoft.Office.Interop.Word.HeaderFooter footer in footers)
                    {
                        Fields fields = footer.Range.Fields;
                        foreach (Field mergeField in fields)
                        {
                            string fieldText = mergeField.Code.Text;
                            string fieldName = GetFieldName(fieldText);
                            if (values.ContainsKey(fieldName))
                            {
                                mergeField.Select();
                                application.Selection.TypeText(values[fieldName]);
                            }
                        }
                    }
                }

                //TEXT BOX
                foreach (Microsoft.Office.Interop.Word.Range otherStoryRange in document.StoryRanges)
                {
                    if (otherStoryRange.StoryType != Microsoft.Office.Interop.Word.WdStoryType.wdMainTextStory)
                    {
                        foreach (Field mergeField in otherStoryRange.Fields)
                        {
                            if (mergeField.Type == WdFieldType.wdFieldMergeField)
                            {
                                string fieldText = mergeField.Code.Text;
                                string fieldName = GetFieldName(fieldText);
                                if (values.ContainsKey(fieldName))
                                {
                                    mergeField.Select();
                                    application.Selection.TypeText(values[fieldName]);
                                }
                            }
                        }
                    }

                    // 'Now search all next stories of other stories (doc.storyRanges dont seem to cascades in sub story)
                    Microsoft.Office.Interop.Word.Range nextStoryRange = otherStoryRange.NextStoryRange;
                    while (nextStoryRange != null)
                    {
                        foreach (Field mergeField in nextStoryRange.Fields)
                        {
                            if (mergeField.Type == WdFieldType.wdFieldMergeField)
                            {
                                string fieldText = mergeField.Code.Text;
                                string fieldName = GetFieldName(fieldText);
                                if (values.ContainsKey(fieldName))
                                {
                                    mergeField.Select();
                                    application.Selection.TypeText(values[fieldName]);
                                }
                            }
                        }
                        nextStoryRange = nextStoryRange.NextStoryRange;
                    }
                }
                //BODY
                foreach (Field mergeField in document.Fields)
                {
                    if (mergeField.Type == WdFieldType.wdFieldMergeField)
                    {
                        string fieldText = mergeField.Code.Text;
                        string fieldName = GetFieldName(fieldText);
                        if (values.ContainsKey(fieldName))
                        {
                            mergeField.Select();
                            application.Selection.TypeText(values[fieldName]);
                        }
                    }
                }

                document.Save();
                document.Close();
                return System.IO.File.ReadAllBytes(tmpFile);
            }
            catch (Exception ex)
            {
                //StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar documento." + ex.InnerException.Message, ex);
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", ex.InnerException.Message, ex);
                return null;
            }
        }


        public byte[] FillFieldsDocx(byte[] template, Dictionary<string, string> values)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    ms.Write(template, 0, template.Length);
                    using (DocX doc = DocX.Load(ms))
                    {
                        #region Configuracion del Documento
                        doc.MarginLeft = 40;
                        doc.MarginRight = 40;
                        #endregion

                        foreach (var d in values)
                        {
                            doc.ReplaceText(d.Key, !string.IsNullOrWhiteSpace(d.Value)?d.Value:" ");
                        }
                        doc.Save();
                    }
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar documento.", ex);
                return null;
            }
        }

        public byte[] FillFields(byte[] template, Dictionary<string, string> values)
        {
            try
            {
                Microsoft.Office.Interop.Word.Application application = new Microsoft.Office.Interop.Word.Application();
                var tmpFile = Path.GetTempFileName();
                var tmpFileStream = File.OpenWrite(tmpFile);
                tmpFileStream.Write(template, 0, template.Length);
                tmpFileStream.Close();
                Document document = application.Documents.Open(tmpFile);

                foreach (Microsoft.Office.Interop.Word.Section section in document.Sections)
                {
                    //HEADER
                    var headers = section.Headers;
                    foreach (Microsoft.Office.Interop.Word.HeaderFooter header in headers)
                    {

                        Fields fields = header.Range.Fields;
                        foreach (Field mergeField in fields)
                        {
                            string fieldText = mergeField.Code.Text;
                            string fieldName = GetFieldName(fieldText);
                            if (values.ContainsKey(fieldName))
                            {
                                mergeField.Select();
                                application.Selection.TypeText(values[fieldName]);
                            }
                        }
                    }
                    //FOOTER
                    var footers = section.Footers;
                    foreach (Microsoft.Office.Interop.Word.HeaderFooter footer in footers)
                    {
                        Fields fields = footer.Range.Fields;
                        foreach (Field mergeField in fields)
                        {
                            string fieldText = mergeField.Code.Text;
                            string fieldName = GetFieldName(fieldText);
                            if (values.ContainsKey(fieldName))
                            {
                                mergeField.Select();
                                application.Selection.TypeText(values[fieldName]);
                            }
                        }
                    }
                }

                //TEXT BOX
                foreach (Microsoft.Office.Interop.Word.Range otherStoryRange in document.StoryRanges)
                {
                    if (otherStoryRange.StoryType != Microsoft.Office.Interop.Word.WdStoryType.wdMainTextStory)
                    {
                       foreach (Field mergeField in otherStoryRange.Fields)
                       {
                           if (mergeField.Type == WdFieldType.wdFieldMergeField)
                           {
                               string fieldText = mergeField.Code.Text;
                               string fieldName = GetFieldName(fieldText);
                               if (values.ContainsKey(fieldName))
                               {
                                   mergeField.Select();
                                   application.Selection.TypeText(values[fieldName]);
                               }
                           }
                       }
                    }

                    // 'Now search all next stories of other stories (doc.storyRanges dont seem to cascades in sub story)
                    Microsoft.Office.Interop.Word.Range nextStoryRange = otherStoryRange.NextStoryRange;
                    while (nextStoryRange != null)
                    {
                        foreach (Field mergeField in nextStoryRange.Fields)
                        {
                            if (mergeField.Type == WdFieldType.wdFieldMergeField)
                            {
                                string fieldText = mergeField.Code.Text;
                                string fieldName = GetFieldName(fieldText);
                                if (values.ContainsKey(fieldName))
                                {
                                    mergeField.Select();
                                    application.Selection.TypeText(values[fieldName]);
                                }
                            }
                        }
                        nextStoryRange = nextStoryRange.NextStoryRange;
                    }
                }
                //BODY
                foreach (Field mergeField in document.Fields)
                {
                    if (mergeField.Type == WdFieldType.wdFieldMergeField)
                    {
                        string fieldText = mergeField.Code.Text;
                        string fieldName = GetFieldName(fieldText);
                        if (values.ContainsKey(fieldName))
                        {
                            mergeField.Select();
                            application.Selection.TypeText(values[fieldName]);
                        }
                    }
                }

                document.Save();
                document.Close();
                return System.IO.File.ReadAllBytes(tmpFile);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar documento.", ex);
                return null;
            }
            
        }

        private string GetFieldName(string fieldText)
        {
            if (fieldText.StartsWith(" MERGEFIELD"))
            {
                Int32 endMerge = fieldText.IndexOf("\\");
                Int32 fieldNameLength = fieldText.Length - endMerge;
                String fieldName = fieldText.Substring(11, endMerge - 11);
                return fieldName.Trim();
            }
            return string.Empty;
        }

        private void FindAndReplace(Microsoft.Office.Interop.Word.Application doc, object findText, object replaceWithText)
        {
            //options
            object matchCase = false;
            object matchWholeWord = true;
            object matchWildCards = false;
            object matchSoundsLike = false;
            object matchAllWordForms = false;
            object forward = true;
            object format = false;
            object matchKashida = false;
            object matchDiacritics = false;
            object matchAlefHamza = false;
            object matchControl = true;
            object read_only = false;
            object visible = true;
            object replace = 2;
            object wrap = 1;
            //execute find and replace
            doc.Selection.Find.Execute(ref findText, ref matchCase, ref matchWholeWord,
                ref matchWildCards, ref matchSoundsLike, ref matchAllWordForms, ref forward, ref wrap, ref format, ref replaceWithText, ref replace,
                ref matchKashida, ref matchDiacritics, ref matchAlefHamza, ref matchControl);
        }
    }
}
