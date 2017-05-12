using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class EXT_SECTOR_CLASIFICACION:ValidationViewModelBase
    {
        public SECTOR_CLASIFICACION SECTOR_CLASIFICACION { get; set; }
        private bool is_checked = false;
        public bool IS_CHECKED 
        {
            get { return is_checked; }
            set { is_checked = value; OnPropertyValidateChanged("IS_CHECKED"); }
        }
    }
}
