using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using MahApps.Metro.Controls;
using SSP.Servidor;

namespace ControlPenales
{
    public class MyDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(
            object item,
            DependencyObject container)
        {
            try
            {
                var wnd = Application.Current.Windows[0] as MetroWindow;
                if (item is string)
                {
                    // SOLO FUNCIONA EN NotaMedicaView
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is NotaMedicaView)
                        return ((NotaMedicaView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("WaitTemplate") as DataTemplate;
                    // SOLO FUNCIONA EN NotaEvolucionView
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is NotaEvolucionView)
                        return ((NotaEvolucionView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("WaitTemplate") as DataTemplate;
                    // SOLO FUNCIONA EN ProcedimientosMaterialesView
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is ProcedimientosMaterialesView)
                        return ((ProcedimientosMaterialesView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("WaitTemplate") as DataTemplate;
                    // SOLO FUNCIONA EN BandejaEntradaView
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is BandejaEntradaView)
                        return ((BandejaEntradaView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("WaitTemplate") as DataTemplate;
                    // SOLO FUNCIONA EN CAPTURA DE NOTIFICACION A TS
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is CapturaNotificacionView)
                        return ((CapturaNotificacionView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("WaitTemplate") as DataTemplate;
                    // SOLO FUNCIONA EN CAPTURA DE SOLUCIONES EN HOJA DE ENFERMERIA
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is HojaEnfermeriaView)
                        return ((HojaEnfermeriaView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("WaitTemplate") as DataTemplate;
                    // SOLO FUNCIONA EN CAPTURA DE DEFUNCIONES
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is CapturaDefuncionView)
                        return ((CapturaDefuncionView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("WaitTemplate") as DataTemplate;
                    // SOLO FUNCIONA EN BITACORA DE HOSPITALIZACIONES
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is BitacoraIngresosEgresosHospitalizacionView)
                        return ((BitacoraIngresosEgresosHospitalizacionView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("WaitTemplate") as DataTemplate;

                }
                else if (item is ENFERMEDAD)
                {
                    // SOLO FUNCIONA EN NotaMedicaView
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is NotaMedicaView)
                        return ((NotaMedicaView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("TheItemTemplate") as DataTemplate;
                    // SOLO FUNCIONA EN NotaEvolucionView
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is NotaEvolucionView)
                        return ((NotaEvolucionView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("TheItemTemplate") as DataTemplate;
                    // SOLO FUNCIONA EN CAPTURA DE NOTIFICACION A TS
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is CapturaNotificacionView)
                        return ((CapturaNotificacionView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("TheItemTemplate") as DataTemplate;
                    // SOLO FUNCIONA EN CAPTURA DE DEFUNCIONES
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is CapturaDefuncionView)
                        return ((CapturaDefuncionView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("TheItemTemplate") as DataTemplate;
                    // SOLO FUNCIONA EN BITACORA DE HOSPITALIZACIONES
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is BitacoraIngresosEgresosHospitalizacionView)
                        return ((BitacoraIngresosEgresosHospitalizacionView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("TheItemTemplate") as DataTemplate;
                }

                else if (item is RecetaMedica)
                {
                    // SOLO FUNCIONA EN NotaMedicaView
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is NotaMedicaView)
                        return ((NotaMedicaView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("TheItemTemplateReceta") as DataTemplate;
                    // SOLO FUNCIONA EN NotaEvolucionView
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is NotaEvolucionView)
                        return ((NotaEvolucionView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("TheItemTemplateReceta") as DataTemplate;
                    // SOLO FUNCIONA EN ProcedimientosMaterialesView
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is ProcedimientosMaterialesView)
                        return ((ProcedimientosMaterialesView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("TheItemTemplateReceta") as DataTemplate;
                    // SOLO FUNCIONA EN HojaEnfermeriaViewModel
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is HojaEnfermeriaView)
                        return ((HojaEnfermeriaView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("TheItemTemplateReceta") as DataTemplate;
                    // SOLO FUNCIONA EN BITACORA DE HOSPITALIZACIONES
                    if ((((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content) is BitacoraIngresosEgresosHospitalizacionView)
                        return ((BitacoraIngresosEgresosHospitalizacionView)(((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content)).FindResource("TheItemTemplateReceta") as DataTemplate;
                }
                else
                {

                }
                return new DataTemplate();
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }
    }
}
