using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class ControlInternosViewModel : ObservableObject, IPageViewModel
    {
        #region variables
        public string Name
        {
            get
            {
                return "control_internos";
            }
        }
        #endregion

        #region constructor
        public ControlInternosViewModel() { }
        #endregion
    }
}
