using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
   public partial class BusquedaInternoProgramasViewModel
    {

       public ICommand LoadBuscarExpediente
       {
           get { return new DelegateCommand<BuscarInternosProgramas>(OnLoad); }
       }


       private ICommand buscarEnter;
       public ICommand BuscarEnter
       {
           get
           {
               return buscarEnter ?? (buscarEnter = new RelayCommand(EnterKeyPressed));
           }
       }


       private ICommand _onClick;
       public ICommand OnClick
       {
           get
           {
               return _onClick ?? (_onClick = new RelayCommand(ClickSwitch));
           }
       }


       private ICommand buttonMouseEnter;
       public ICommand ButtonMouseEnter
       {
           get { return buttonMouseEnter ?? (buttonMouseEnter = new RelayCommand(MouseEnterSwitch)); }
       }


       private ICommand buttonMouseLeave;
       public ICommand ButtonMouseLeave
       {
           get { return buttonMouseLeave ?? (buttonMouseLeave = new RelayCommand(MouseLeaveSwitch)); }
       }

    }
}
