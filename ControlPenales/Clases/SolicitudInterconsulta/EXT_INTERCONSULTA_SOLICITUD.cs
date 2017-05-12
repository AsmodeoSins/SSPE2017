using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public class EXT_INTERCONSULTA_SOLICITUD:ValidationViewModelBase
    {
        public INTERCONSULTA_SOLICITUD  INTERCONSULTA_SOLICITUD { get; set; }
        private Visibility isCanalizacionEspecialidad;
        public Visibility IsCanalizacionEspecialidad
        {
            get { return isCanalizacionEspecialidad; }
            set { isCanalizacionEspecialidad = value; RaisePropertyChanged("IsCanalizacionEspecialidad"); }
        }
        private Visibility isCanalizacionServAux;
        public Visibility IsCanalizacionServAux
        {
            get { return isCanalizacionServAux; }
            set { isCanalizacionServAux = value; RaisePropertyChanged("IsCanalizacionServAux"); }
        }
    }
}
