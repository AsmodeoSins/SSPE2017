using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class EXT_SERV_AUX_DIAGNOSTICO:ValidationViewModelBase
    {
        public int ID_SERV_AUX { get; set; }
        public Nullable<short> ID_SUBTIPO_SADT { get; set; }
        public string DESCR { get; set; }
        public string ESTATUS { get; set; }
        public string SUBTIPO_DESCR { get; set; }
        private bool isChecked = false;
        public bool ISCHECKED
        {
            get { return isChecked; }
            set { isChecked = value; RaisePropertyChanged("ISCHECKED"); }
        }
        private List<INTERCONSULTA_TIPO> lstInterconsulta_Tipo;
        public List<INTERCONSULTA_TIPO> LstInterconsulta_Tipo
        {
            get { return lstInterconsulta_Tipo; }
            set { lstInterconsulta_Tipo = value; RaisePropertyChanged("LstInterconsulta_Tipo"); }
        }

        private short selectedInterconsulta_Tipo = -1;
        public short SelectedInterconsulta_Tipo
        {
            get { return selectedInterconsulta_Tipo; }
            set { selectedInterconsulta_Tipo = value; RaisePropertyChanged("SelectedInterconsulta_Tipo"); }
        }
        private List<INTERCONSULTA_NIVEL_PRIORIDAD> lstNivel_Prioridad;
        public List<INTERCONSULTA_NIVEL_PRIORIDAD> LstNivel_Prioridad
        {
            get { return lstNivel_Prioridad; }
            set { lstNivel_Prioridad = value; RaisePropertyChanged("LstNivel_Prioridad"); }
        }
        private short selectedNivel_Prioridad = -1;
        public short SelectedNivel_Prioridad
        {
            get { return selectedNivel_Prioridad; }
            set { selectedNivel_Prioridad = value; RaisePropertyChanged("SelectedNivel_Prioridad"); }
        }

        private bool isProgramado = false;
        public bool IsProgramado
        {
            get { return isProgramado; }
            set 
            {
                isProgramado = value;
                RaisePropertyChanged("IsProgramado");
                if (value)
                {
                    isEnabled = false;
                    RaisePropertyChanged("IsEnabled");
                }
                else
                {
                    isEnabled = true;
                    RaisePropertyChanged("IsEnabled");
                }
            }
        }

        private bool isEnabled=true;
        public bool IsEnabled
        {
            get { return isEnabled; }
        }

    }
}
