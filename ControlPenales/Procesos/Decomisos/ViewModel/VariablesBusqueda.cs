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
using ControlPenales.Clases;
using SSP.Servidor;

namespace ControlPenales
{
    partial class RegistroDecomisoViewModel
    {
        private short? tipoB = 0;
        public short? TipoB
        {
            get { return tipoB; }
            set { tipoB = value; OnPropertyChanged("TipoB"); }
        }

        private string paternoB;
        public string PaternoB
        {
            get { return paternoB; }
            set { paternoB = value; OnPropertyChanged("PaternoB"); }
        }

        private string maternoB;
        public string MaternoB
        {
            get { return maternoB; }
            set { maternoB = value; OnPropertyChanged("MaternoB"); }
        }

        private string nombreB;
        public string NombreB
        {
            get { return nombreB; }
            set { nombreB = value; OnPropertyChanged("NombreB"); }
        }

        //private ObservableCollection<cDecomisos> lstDecomisos;
        //public ObservableCollection<cDecomisos> LstDecomisos
        //{
        //    get { return lstDecomisos; }
        //    set { 
        //        lstDecomisos = value;
        //        if (value != null)
        //        {
        //            DecomisosEmpty = value.Any() ? Visibility.Collapsed : Visibility.Visible;
        //        }
        //        else
        //            DecomisosEmpty = Visibility.Visible;
        //        OnPropertyChanged("LstDecomisos"); }
        //}

        private RangeEnabledObservableCollection<DECOMISO> lstDecomisos;
        public RangeEnabledObservableCollection<DECOMISO> LstDecomisos
        {
            get { return lstDecomisos; }
            set { 
                lstDecomisos = value;
                if (value != null)
                {
                    DecomisosEmpty = value.Any() ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                    DecomisosEmpty = Visibility.Visible;
                OnPropertyChanged("LstDecomisos"); }
        }
        
        //private cDecomisos selectedDecomiso;
        //public cDecomisos SelectedDecomiso
        //{
        //    get { return selectedDecomiso; }
        //    set { selectedDecomiso = value;
        //    if (value != null)
        //    {
        //        MenuGuardarEnabled = true;
        //        if (PImprimir)
        //            MenuReporteEnabled = true;
        //        if (value.ImagenVisitante != null)
        //        {
        //            ImagenIngresoB = value.ImagenVisitante;
        //        }
        //        else
        //            ImagenIngresoB = new Imagenes().getImagenPerson();
        //    }
        //    else
        //    {
        //        if (!PInsertar)
        //            MenuGuardarEnabled = false;
        //        MenuReporteEnabled = false;
        //        ImagenIngresoB = new Imagenes().getImagenPerson(); 
        //    }
        //    if (value != null)
        //        MenuReporteEnabled = true;
        //    else
        //        MenuReporteEnabled = false;
        //        OnPropertyChanged("SelectedDecomiso"); }
        //}

        private DECOMISO selectedDecomiso;
        public DECOMISO SelectedDecomiso
        {
            get { return selectedDecomiso; }
            set { selectedDecomiso = value;
            if (value != null)
                Editar = false;
            else
                Editar = true;
                OnPropertyChanged("SelectedDecomiso"); }
        }


        private Visibility decomisosEmpty = Visibility.Visible;
        public Visibility DecomisosEmpty
        {
            get { return decomisosEmpty; }
            set { decomisosEmpty = value; OnPropertyChanged("DecomisosEmpty"); }
        }

        private byte[] imagenIngresoB = new Imagenes().getImagenPerson();
        public byte[] ImagenIngresoB
        {
            get { return imagenIngresoB; }
            set
            {
                imagenIngresoB = value;
                OnPropertyChanged("ImagenIngresoB");
            }
        }
    }
}
