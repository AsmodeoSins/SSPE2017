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
    public partial class TextControlGuardarView : MahApps.Metro.Controls.MetroWindow
    {
        public byte[] Documento;

        public TextControlGuardarView(Object obj = null, byte[] docto = null)
        {
            //if(obj != null)
            //    this.obj = obj;
            //if (docto != null)
            //    doc = docto;

            InitializeComponent();
        }

        private ReporteIngreso repIngreso;
        internal ReporteIngreso RepIngreso
        {
            get { return repIngreso; }
            set { repIngreso = value; }
        }
        //private void editor_Loaded(object sender, EventArgs e) 
        //{
        //   // editor.Load(doc, TXTextControl.BinaryStreamType.MSWord);
        //}

        private void Imprimir(object sender, RoutedEventArgs e)
        {
            editor.Print("Documento", true);
        }
       

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            editor.Height = e.NewSize.Height - 150;
        }

        private void Guardar(object sender, RoutedEventArgs e)
        {
            editor.Save(out Documento, BinaryStreamType.WordprocessingML);
            this.Close();
        }
    }
}
