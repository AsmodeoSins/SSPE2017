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
using System.Windows.Shapes;
using MVVMShared.Data.WPF;
namespace GESAL.Views.Principal
{
    /// <summary>
    /// Interaction logic for PrincipalView.xaml
    /// </summary>
    public partial class PrincipalView  : MahApps.Metro.Controls.MetroWindow
    {
        public PrincipalView()
        {
            InitializeComponent();
            //hace el binding de la propiedad dependiente en la vista hacia la propiedad correspondiente en la vista modelo.
            string propertyInViewModel = "ContentControlBag";
            var bindingViewMode = new Binding(propertyInViewModel) { Mode = BindingMode.TwoWay };
            this.SetBinding(ContentControlBagPropiedad, propertyInViewModel);
            
        }

         //Using a DependencyProperty as the backing store for MapCenter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentControlBagPropiedad = DependencyProperty.Register("ContentControlBag", typeof(ContentControlBag),
            typeof(PrincipalView), new PropertyMetadata(null, (sender, e) => ((PrincipalView)sender).UpdateContentControl()));

        private void UpdateContentControl()
        {

            if (ContentControlBag != null)
            {
                contenedorVistas.Content = null;
                GC.Collect();
                contenedorVistas.Content = ContentControlBag.View;
                contenedorVistas.DataContext = ContentControlBag.ViewModel;
            }
        }

        public ContentControlBag ContentControlBag
        {
            get { return (ContentControlBag)GetValue(ContentControlBagPropiedad); }
            set { SetValue(ContentControlBagPropiedad, value); }
        }
    }
}
