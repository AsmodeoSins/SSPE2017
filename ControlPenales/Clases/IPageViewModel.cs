using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public interface IPageViewModel
    {
        string Name { get; }
        
        void inicializa();

        //void Dispose();
    }
}
