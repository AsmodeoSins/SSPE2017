using System.Linq;

namespace ControlPenales
{
    partial class NotificacionTrabajoSocialViewModel
    {
        #region ENFERMEDAD

        private void listBox_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                AutoCompleteLB = AutoCompleteTB.Template.FindName("PART_ListBox", AutoCompleteTB) as System.Windows.Controls.ListBox;
                var dep = (System.Windows.DependencyObject)e.OriginalSource;
                while ((dep != null) && !(dep is System.Windows.Controls.ListBoxItem))
                    dep = System.Windows.Media.VisualTreeHelper.GetParent(dep);
                if (dep == null) return;
                var item = AutoCompleteLB.ItemContainerGenerator.ItemFromContainer(dep);
                if (item == null) return;
                //new ControlPenales.Controls.AutoCompleteTextBox().SetTextValueBySelection(item, false);
                if (item is SSP.Servidor.ENFERMEDAD)
                {
                    ListEnfermedades = ListEnfermedades ?? new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ENFERMEDAD>();
                    if (!ListEnfermedades.Any(a => a.ID_ENFERMEDAD == ((SSP.Servidor.ENFERMEDAD)item).ID_ENFERMEDAD))
                    {
                        ListEnfermedades.Insert(0, (SSP.Servidor.ENFERMEDAD)item);
                        AutoCompleteTB.Text = string.Empty;
                    }
                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar una enfermedad.", ex);
            }
        }

        private void listBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Return)
                {
                    var popup = AutoCompleteTB.Template.FindName("PART_Popup", AutoCompleteTB) as System.Windows.Controls.Primitives.Popup;
                    AutoCompleteLB = AutoCompleteTB.Template.FindName("PART_ListBox", AutoCompleteTB) as System.Windows.Controls.ListBox;
                    var dep = (System.Windows.DependencyObject)e.OriginalSource;
                    while ((dep != null) && !(dep is System.Windows.Controls.ListBoxItem))
                        dep = System.Windows.Media.VisualTreeHelper.GetParent(dep);
                    if (dep == null) return;
                    var item = AutoCompleteLB.ItemContainerGenerator.ItemFromContainer(dep);
                    if (item == null) return;
                    if (item is SSP.Servidor.ENFERMEDAD)
                    {
                        ListEnfermedades = ListEnfermedades ?? new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ENFERMEDAD>();
                        if (!ListEnfermedades.Any(a => a.ID_ENFERMEDAD == ((SSP.Servidor.ENFERMEDAD)item).ID_ENFERMEDAD))
                        {
                            ListEnfermedades.Insert(0, (SSP.Servidor.ENFERMEDAD)item);
                            AutoCompleteTB.Text = string.Empty;
                            AutoCompleteTB.Focus();
                        }
                        else
                            popup.IsOpen = false;
                    }
                }
                else if (e.Key == System.Windows.Input.Key.Tab) { }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar una enfermedad.", ex);
            }
        }

        #endregion
    }
}