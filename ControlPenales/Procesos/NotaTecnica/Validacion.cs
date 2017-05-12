using ControlPenales;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using ControlPenales.Clases;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Interop;
using System.IO;
using System.Windows.Controls;
using ControlPenales.BiometricoServiceReference;
using System.Windows.Media;


namespace ControlPenales
{
    partial class NotaTecnicaViewModel : ValidationViewModelBase
    {
        public void SetValidaciones()
        {
            base.ClearRules();
            base.AddRule(() => AreaTecnica, () => AreaTecnica != -1, "ÁREA TÉCNICA ES REQUERIDO");
            base.AddRule(() => AtencionTxt, () => !string.IsNullOrEmpty(AtencionTxt), "ATENCIÓN RECIBIDA ES REQUERIDO");
            OnPropertyChanged("AreaTecnica");
            OnPropertyChanged("AtencionTxt");
        }
    }
}
