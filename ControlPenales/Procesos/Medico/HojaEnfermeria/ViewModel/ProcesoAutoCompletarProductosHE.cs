using System.Linq;
namespace ControlPenales
{
    public partial class HojaEnfermeriaViewModel
    {
        private void MouseUpReceta(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var popup = AutoCompleteReceta.Template.FindName("PART_Popup", AutoCompleteReceta) as System.Windows.Controls.Primitives.Popup;
                AutoCompleteRecetaLB = AutoCompleteReceta.Template.FindName("PART_ListBox", AutoCompleteReceta) as System.Windows.Controls.ListBox;
                var dep = (System.Windows.DependencyObject)e.OriginalSource;
                while ((dep != null) && !(dep is System.Windows.Controls.ListBoxItem))
                    dep = System.Windows.Media.VisualTreeHelper.GetParent(dep);
                if (dep == null) return;
                var item = AutoCompleteRecetaLB.ItemContainerGenerator.ItemFromContainer(dep);
                if (item == null) return;
                if (item is RecetaMedica)
                {
                    ListRecetas = ListRecetas ?? new System.Collections.ObjectModel.ObservableCollection<RecetaMedica>();
                    if (!ListRecetas.Any(a => a.PRODUCTO.ID_PRODUCTO == ((RecetaMedica)item).PRODUCTO.ID_PRODUCTO))
                    {
                        ListRecetas.Insert(0, new RecetaMedica
                        {
                            CANTIDAD = new decimal?(),
                            DURACION = new short?(),
                            HORA_MANANA = false,
                            HORA_NOCHE = false,
                            HORA_TARDE = false,
                            MEDIDA = ((RecetaMedica)item).MEDIDA,
                            OBSERVACIONES = string.Empty,
                            PRESENTACION = 0,
                            PRODUCTO = ((RecetaMedica)item).PRODUCTO,
                        });
                        AutoCompleteReceta.Text = string.Empty;
                        AutoCompleteReceta.Focus();
                    }
                    else
                        popup.IsOpen = false;
                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar una enfermedad.", ex);
            }
        }

        private void KeyDownReceta(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Return)
                {
                    var popup = AutoCompleteReceta.Template.FindName("PART_Popup", AutoCompleteReceta) as System.Windows.Controls.Primitives.Popup;
                    AutoCompleteRecetaLB = AutoCompleteReceta.Template.FindName("PART_ListBox", AutoCompleteReceta) as System.Windows.Controls.ListBox;
                    var dep = (System.Windows.DependencyObject)e.OriginalSource;
                    while ((dep != null) && !(dep is System.Windows.Controls.ListBoxItem))
                        dep = System.Windows.Media.VisualTreeHelper.GetParent(dep);
                    if (dep == null) return;
                    var item = AutoCompleteRecetaLB.ItemContainerGenerator.ItemFromContainer(dep);
                    if (item == null) return;
                    if (item is RecetaMedica)
                    {
                        ListRecetas = ListRecetas ?? new System.Collections.ObjectModel.ObservableCollection<RecetaMedica>();
                        if (!ListRecetas.Any(a => a.PRODUCTO.ID_PRODUCTO == ((RecetaMedica)item).PRODUCTO.ID_PRODUCTO))
                        {
                            ListRecetas.Insert(0, (RecetaMedica)item);
                            AutoCompleteReceta.Text = string.Empty;
                            AutoCompleteReceta.Focus();
                        }
                        else
                            popup.IsOpen = false;
                    }
                }
                else if (e.Key == System.Windows.Input.Key.Tab) { }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar un producto.", ex);
            }
        }

    }
}