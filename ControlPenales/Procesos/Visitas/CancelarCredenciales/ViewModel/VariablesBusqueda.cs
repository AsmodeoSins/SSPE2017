using SSP.Servidor;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
//using MvvmFramework;

namespace ControlPenales
{
    partial class CancelacionVisitasViewModel
    {
        private byte[] _ImagenAcompanante = new Imagenes().getImagenPerson();
        public byte[] ImagenAcompanante
        {
            get
            {
                return _ImagenAcompanante;
            }
            set
            {
                _ImagenAcompanante = value;
                OnPropertyChanged("ImagenAcompanante");
            }
        }

        private byte[] ImagenIngresoDCredencial = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get
            {
                return ImagenIngresoDCredencial;
            }
            set
            {
                ImagenIngresoDCredencial = value;

                OnPropertyChanged("ImagenIngreso");
            }
        }

        private byte[] imagenPersonaCredencial = new Imagenes().getImagenPerson();
        public byte[] ImagenPersona
        {
            get
            {
                return imagenPersonaCredencial;
            }
            set
            {
                imagenPersonaCredencial = value;
                OnPropertyChanged("ImagenPersona");
            }
        }

        private ImageSource _FotoVisitaCredencial = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
        public ImageSource FotoVisita
        {
            get
            {
                return _FotoVisitaCredencial;
            }
            set
            {
                _FotoVisitaCredencial = value;
                OnPropertyChanged("FotoVisita");
            }
        }
    }
}
