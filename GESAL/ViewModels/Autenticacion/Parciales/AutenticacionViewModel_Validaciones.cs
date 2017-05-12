using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GESAL.ViewModels
{
    public partial class AutenticacionViewModel
    {
        private void setAutenticaContrasena()
        {
            base.ClearRules();
            base.AddRule(() => Password, () => (Password!=null && Password.Length>0), "ESCRIBA SU CONTRASEÑA");
            RaisePropertyChanged("Password");
        }
    }
}
