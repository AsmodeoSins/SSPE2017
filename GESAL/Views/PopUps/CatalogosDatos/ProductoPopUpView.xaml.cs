using Microsoft.Win32;
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
using MVVMShared.Manager;

namespace GESAL.Views
{
    /// <summary>
    /// Interaction logic for ProductoPopUpView.xaml
    /// </summary>
    public partial class ProductoPopUpView : UserControl
    {
        public ProductoPopUpView()
        {
            InitializeComponent();
            //hace el binding de la propiedad dependiente en la vista hacia la propiedad correspondiente en la vista modelo.
            string propertyInViewModel = "ImagenData";
            var bindingViewMode = new Binding(propertyInViewModel) { Mode = BindingMode.TwoWay };
            this.SetBinding(ImagenDataPropiedad, propertyInViewModel);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Seleccione una imagen";
            dlg.Filter = "Formatos Validos|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (dlg.ShowDialog() == true)
            {
                txtfilename.Text = dlg.FileName;
                imgProducto.Source = new FileManagerProvider().fileToImageSource(dlg.FileName);
            }
        }

        //Using a DependencyProperty as the backing store for MapCenter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImagenDataPropiedad = DependencyProperty.Register("ImagenData", typeof(byte[]),
            typeof(ProductoPopUpView), new FrameworkPropertyMetadata(new PropertyChangedCallback(ImagenDataProperty_Changed)));

        private static void ImagenDataProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ProductoPopUpView)sender;
            if (e.NewValue != null)
            {
                obj.imgProducto.Source = new FileManagerProvider().byteToImageSource((byte[])e.NewValue);
                obj.imgProducto.Stretch = Stretch.Uniform;
            }

        }

        public byte[] ImagenData
        {
            get { return (byte[])GetValue(ImagenDataPropiedad); }
            set { SetValue(ImagenDataPropiedad, value); }
        }
    }
}
