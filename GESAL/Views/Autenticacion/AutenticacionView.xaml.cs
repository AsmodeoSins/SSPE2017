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
using MahApps.Metro.Controls;
using MVVMShared.Extensiones;
using MVVMShared.Manager;
namespace GESAL.Views
{
    /// <summary>
    /// Interaction logic for Autenticacion.xaml
    /// </summary>
    public partial class AutenticacionView : MetroWindow
    {
        public AutenticacionView()
        {
            InitializeComponent();
            this.SetBinding(ListaImagenPropiedad, new Binding("ListaImagenes") { Mode = BindingMode.OneWay });
            this.SetBinding(IniciaHuellaAnimacionPropiedad, new Binding("IniciaAnimacion") { Mode = BindingMode.OneWay });
        }

        private void pbContrasena_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox pBox = (PasswordBox)(sender);
            PasswordBoxMVVMAttachedProperties.SetEncryptedPassword(pBox, pBox.SecurePassword);
        }

        public static readonly DependencyProperty ListaImagenPropiedad = DependencyProperty.Register("ImagenData", typeof(List<byte[]>),
            typeof(AutenticacionView), new FrameworkPropertyMetadata(new PropertyChangedCallback(ListaImagenProperty_Changed)));

        private static void ListaImagenProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (AutenticacionView)sender;
            if (e.NewValue != null && ((List<byte[]>)(e.NewValue)).Count>0)
            {
                var fileManagerProvider=new FileManagerProvider();
                var lista_bitmaps = new List<ImageSource>();
                foreach (var item in (List<byte[]>)(e.NewValue))
                    lista_bitmaps.Add(fileManagerProvider.byteToImageSource(item));
                obj.animacionHuella.Load(lista_bitmaps);
                obj.animacionHuella.FrozeenImage();
            }

        }

        public List<byte[]> ImagenData
        {
            get { return (List<byte[]>)GetValue(ListaImagenPropiedad); }
            set { SetValue(ListaImagenPropiedad, value); }
        }


        private static readonly DependencyProperty IniciaHuellaAnimacionPropiedad = DependencyProperty.Register("IniciaHuellaAnimacion", typeof(bool?), 
            typeof(AutenticacionView), new FrameworkPropertyMetadata(new PropertyChangedCallback(IniciaHuellaProperty_Changed)));

        private static void IniciaHuellaProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (AutenticacionView)sender;
            if (e.NewValue!=null)
            {
                if (((bool?)e.NewValue).Value)
                    obj.animacionHuella.Play();
                else
                    obj.animacionHuella.Stop();
            }
        }
        public bool? IniciaHuellaAnimacion
        {
            get { return (bool?)GetValue(IniciaHuellaAnimacionPropiedad); }
            set { SetValue(IniciaHuellaAnimacionPropiedad, value); }
        }


    }
}
