using GESAL.Clases.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GESAL.Views
{
    /// <summary>
    /// Interaction logic for Almacen_GrupoPopUpView.xaml
    /// </summary>
    public partial class Almacen_GrupoPopUpView : UserControl
    {
        public Almacen_GrupoPopUpView()
        {
            InitializeComponent();
            this.SetBinding(RealizarAccionPropiedad, new Binding("RealizarAccion") {Mode=BindingMode.OneWay });
        }

        //Using a DependencyProperty as the backing store for MapCenter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RealizarAccionPropiedad = DependencyProperty.Register("RealizarAccion", typeof(AccionSalvar),
            typeof(Almacen_GrupoPopUpView), new FrameworkPropertyMetadata(new PropertyChangedCallback(RealizarAccionProperty_Changed)));

        private static void RealizarAccionProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (Almacen_GrupoPopUpView)sender;
            if (e.NewValue != null)
            {
                if ((AccionSalvar)e.NewValue == AccionSalvar.Actualizar)
                    obj.ID.IsEnabled = false;
                else
                    obj.ID.IsEnabled = true;
            }

        }

        public AccionSalvar RealizarAccion
        {
            get { return (AccionSalvar)GetValue(RealizarAccionPropiedad); }
            set { SetValue(RealizarAccionPropiedad, value); }
        }
    }
}
