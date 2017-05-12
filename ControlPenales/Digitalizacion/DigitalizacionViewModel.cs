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
using System.IO;
using System.Drawing;

namespace ControlPenales
{
    class DigitalizacionViewModel : ValidationViewModelBase, IPageViewModel, INotifyPropertyChanged
    {
        #region variables
        private int valueBrillo;

        public int ValueBrillo
        {
            get { return valueBrillo; }
            set { valueBrillo = value; OnPropertyChanged("ValueBrillo"); }
        }

        private int valueContraste;

        public int ValueContraste
        {
            get { return valueContraste; }
            set { valueContraste = value; OnPropertyChanged("ValueContraste"); }
        }

        private int valueResolucion;

        public int ValueResolucion
        {
            get { return valueResolucion; }
            set { valueResolucion = value; Resolucion = string.Format("{0} dpi", valueResolucion); OnPropertyChanged("ValueResolucion"); }
        }

        private string resolucion;

        public string Resolucion
        {
            get { return resolucion; }
            set { resolucion = value; OnPropertyChanged("Resolucion"); }
        }

        private byte[] digitalizacion;

        public byte[] Digitalizacion
        {
            get { return digitalizacion; }
            set { digitalizacion = value; OnPropertyChanged("Digitalizacion"); }
        }

        public string Name
        {
            get
            {
                return "digitalizacion";
            }
        }
        #endregion

        #region constructor
        public DigitalizacionViewModel()
        {
           
        }
        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        {
            ValueBrillo = 0;
            ValueContraste = 0;
            ValueResolucion = 150;


            Image img = Image.FromFile(@"C:\Users\Eduardo\Pictures\ACTA.jpg");
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            Digitalizacion = ms.ToArray();
        }
       
        private void clickSwitch(Object obj)
        {
            //switch (obj.ToString())
            //{
      
            //}
        }
        #endregion

        #region commands
        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(clickSwitch));
            }
        }
        #endregion
    }
}
