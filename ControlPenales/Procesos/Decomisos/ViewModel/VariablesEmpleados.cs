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
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class RegistroDecomisoViewModel
    {

        private string nipE;
        public string NipE
        {
            get { return nipE; }
            set { nipE = value; OnPropertyChanged("NipE"); }
        }

        private string paternoE;
        public string PaternoE
        {
            get { return paternoE; }
            set { paternoE = value; OnPropertyChanged("PaternoE"); }
        }

        private string maternoE;
        public string MaternoE
        {
            get { return maternoE; }
            set { maternoE = value; OnPropertyChanged("MaternoE"); }
        }

        private string nombreE;
        public string NombreE
        {
            get { return nombreE; }
            set { nombreE = value; OnPropertyChanged("NombreE"); }
        }

        private bool empleadoEmpty = false;
        public bool EmpleadoEmpty
        {
            get { return empleadoEmpty; }
            set { empleadoEmpty = value; OnPropertyChanged("EmpleadoEmpty"); }
        }

        private RangeEnabledObservableCollection<SSP.Servidor.PERSONA> lstEmpleadoPop;
        public RangeEnabledObservableCollection<SSP.Servidor.PERSONA> LstEmpleadoPop
        {
            get { return lstEmpleadoPop; }
            set { lstEmpleadoPop = value; OnPropertyChanged("LstEmpleadoPop"); }
        }
        private int Pagina { get; set; }
        private bool SeguirCargandoPersonas { get; set; }
        private bool _HeaderSortin;
        public bool HeaderSortin
        {
            get { return _HeaderSortin; }
            set { _HeaderSortin = value; }
        }

        private SSP.Servidor.PERSONA selectedEmpleadoPop;
        public SSP.Servidor.PERSONA SelectedEmpleadoPop
        {
            get { return selectedEmpleadoPop; }
            set
            {
                selectedEmpleadoPop = value;
                if (value != null)
                {
                    if (value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                        ImagenEmpleadoPop = value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    else
                        ImagenEmpleadoPop = new Imagenes().getImagenPerson();
                }
                else
                    ImagenEmpleadoPop = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectedEmpleadoPop");
            }
        }

        private ObservableCollection<DECOMISO_PERSONA> lstEmpleado;
        public ObservableCollection<DECOMISO_PERSONA> LstEmpleado
        {
            get { return lstEmpleado; }
            set { lstEmpleado = value; OnPropertyChanged("LstEmpleado"); }
        }

        private DECOMISO_PERSONA selectedEmpleado;
        public DECOMISO_PERSONA SelectedEmpleado
        {
            get { return selectedEmpleado; }
            set
            {
                selectedEmpleado = value;
                if (value != null)
                {
                    if (value.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                        ImagenEmpleado = value.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    else
                        ImagenEmpleado = new Imagenes().getImagenPerson();
                }
                else
                    ImagenEmpleado = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectedEmpleado");
            }
        }

        private byte[] imagenEmpleadoPop = new Imagenes().getImagenPerson();
        public byte[] ImagenEmpleadoPop
        {
            get { return imagenEmpleadoPop; }
            set
            {
                imagenEmpleadoPop = value;
                OnPropertyChanged("ImagenEmpleadoPop");
            }
        }

        private byte[] imagenEmpleado = new Imagenes().getImagenPerson();
        public byte[] ImagenEmpleado
        {
            get { return imagenEmpleado; }
            set
            {
                imagenEmpleado = value;
                OnPropertyChanged("ImagenEmpleado");
            }
        }
    }
}
