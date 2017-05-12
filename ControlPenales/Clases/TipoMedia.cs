using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class TipoMedia : ValidationViewModelBase
    {
        public TipoMedia() { }
        private short id_tipo;
        public short ID_TIPO
        {
            get { return id_tipo; }
            set
            {
                id_tipo = value;
                OnPropertyChanged("ID_TIPO");
            }
        }
        private short id_media;
        public short ID_MEDIA
        {
            get { return id_media; }
            set
            {
                id_media = value;
                OnPropertyChanged("ID_MEDIA");
            }
        }
        private string tipo_descripcion;
        public string TIPO_FILIACION
        {
            get { return tipo_descripcion; }
            set
            {
                tipo_descripcion = value;
                OnPropertyChanged("TIPO_FILIACION");
            }
        }
        private string media_descripcion;
        public string MEDIA_FILIACION
        {
            get { return media_descripcion; }
            set
            {
                media_descripcion = value;
                OnPropertyChanged("MEDIA_FILIACION");
            }
        }
    }
}
