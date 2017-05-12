using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    class RecepcionActuariosViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region variables
        public string Name
        {
            get
            {
                return "recepcion_actuarios";
            }
        }
        #endregion

        #region metodos
                void IPageViewModel.inicializa()
                { }
        #endregion

        #region constructor
        public RecepcionActuariosViewModel() { }
        #endregion
    }
}
