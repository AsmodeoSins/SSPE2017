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
        private long? noV;
        public long? NoV
        {
            get { return noV; }
            set { noV = value; OnPropertyChanged("NoV"); }
        }

        private string paternoV;
        public string PaternoV
        {
            get { return paternoV; }
            set { paternoV = value; OnPropertyChanged("PaternoV"); }
        }

        private string maternoV;
        public string MaternoV
        {
            get { return maternoV; }
            set { maternoV = value; OnPropertyChanged("MaternoV"); }
        }

        private string nombreV;
        public string NombreV
        {
            get { return nombreV; }
            set { nombreV = value; OnPropertyChanged("NombreV"); }
        }

        private bool visitanteEmpty;
        public bool VisitanteEmpty
        {
            get { return visitanteEmpty; }
            set { visitanteEmpty = value; OnPropertyChanged("VisitanteEmpty"); }
        }

        //private ObservableCollection<VISITANTE> lstVisitantePop;
        //public ObservableCollection<VISITANTE> LstVisitantePop
        //{
        //    get { return lstVisitantePop; }
        //    set { lstVisitantePop = value; OnPropertyChanged("LstVisitantePop"); }
        //}
        private RangeEnabledObservableCollection<VISITANTE> lstVisitantePop;
        public RangeEnabledObservableCollection<VISITANTE> LstVisitantePop
        {
            get { return lstVisitantePop; }
            set { lstVisitantePop = value; OnPropertyChanged("LstVisitantePop"); }
        }
        private int PaginaVisitante { get; set; }
        private bool SeguirCargandoVisitante { get; set; }

        private VISITANTE selectedVisitantePop;
        public VISITANTE SelectedVisitantePop
        {
            get { return selectedVisitantePop; }
            set { selectedVisitantePop = value;
            if (value != null)
            {
                if (value.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenVisitantePop = value.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenVisitantePop = new Imagenes().getImagenPerson();
            }
            else
                ImagenVisitantePop = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectedVisitantePop"); }
        }

        private ObservableCollection<DECOMISO_PERSONA> lstVisitante;
        public ObservableCollection<DECOMISO_PERSONA> LstVisitante
        {
            get { return lstVisitante; }
            set { lstVisitante = value; OnPropertyChanged("LstVisitante"); }
        }

        private DECOMISO_PERSONA selectedVisitante;
        public DECOMISO_PERSONA SelectedVisitante
        {
            get { return selectedVisitante; }
            set { selectedVisitante = value;
            if (value != null)
            {
                if (value.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenVisitante = value.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenVisitante = new Imagenes().getImagenPerson();
            }
            else
                ImagenVisitante = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectedVisitante"); }
        }

        private byte[] imagenVisitantePop = new Imagenes().getImagenPerson();
        public byte[] ImagenVisitantePop
        {
            get { return imagenVisitantePop; }
            set
            {
                imagenVisitantePop = value;
                OnPropertyChanged("ImagenVisitantePop");
            }
        }

        private byte[] imagenVisitante = new Imagenes().getImagenPerson();
        public byte[] ImagenVisitante
        {
            get { return imagenVisitante; }
            set
            {
                imagenVisitante = value;
                OnPropertyChanged("ImagenVisitante");
            }
        }
        
    }
}
