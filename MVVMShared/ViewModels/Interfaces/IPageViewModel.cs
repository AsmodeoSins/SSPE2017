using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVMShared.ViewModels.Interfaces
{
    public interface IPageViewModel
    {
        string Name { get; }
        
        void inicializa();

        //void Dispose();
    }
}
