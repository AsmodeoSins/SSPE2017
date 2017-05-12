using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ControlPenales
{
    public class Dialogos
    {
        public Dialogos() { }

        public async void NotificacionDialog(string sTitulo, string sMensaje)
        {
            var metro = Application.Current.Windows[0] as MetroWindow;
            var dialog = (BaseMetroDialog)metro.Resources["NotificacionDialog"];
            PrincipalViewModel.DialogTitulo = sTitulo;
            PrincipalViewModel.DialogMensaje = sMensaje;
            await metro.ShowMetroDialogAsync(dialog);
            await TaskEx.Delay(1500);
            await metro.HideMetroDialogAsync(dialog);
        }

        public async void ConfirmacionDialogo(string sTitulo, string sMensaje)
        {
            try
            {
                var metro = Application.Current.Windows[0] as MetroWindow;
                var mySettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Cerrar"
                };
                await metro.ShowMessageAsync(sTitulo, sMensaje, MessageDialogStyle.Affirmative, mySettings);
            }
            catch (Exception ex) { }
        }

        public async Task<int> ConfirmacionDialogoReturn(string sTitulo, string sMensaje)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Cerrar"
            };
            var metro = Application.Current.Windows[0] as MetroWindow;
            MessageDialogResult result = await metro.ShowMessageAsync(sTitulo, sMensaje,
                MessageDialogStyle.Affirmative, mySettings);

            return 1;
        }

        public async Task<int> ConfirmarSalida(string sTitulo, string sMensaje)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Si",
                NegativeButtonText = "No",
                FirstAuxiliaryButtonText = "Cancelar"
            };
            var metro = Application.Current.Windows[0] as MetroWindow;
            MessageDialogResult result = await metro.ShowMessageAsync(sTitulo, sMensaje,
                MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);

            if (result == MessageDialogResult.FirstAuxiliary)
                return 2;
            else if (result == MessageDialogResult.Affirmative)
                return 1;
            else if (result == MessageDialogResult.Negative)
                return 0;
            return -1;
        }

        public async Task<int> ConfirmarEliminar(string sTitulo, string sMensaje)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Si",
                NegativeButtonText = "No"

            };
            var metro = Application.Current.Windows[0] as MetroWindow;
            MessageDialogResult result = await metro.ShowMessageAsync(sTitulo, sMensaje,
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Affirmative)
                return 1;
            else if (result == MessageDialogResult.Negative)
                return 0;
            return -1;
        }

        public async Task<int> ConfirmacionDosBotonesCustom(string sTitulo, string sMensaje, string btnAfirma, int retAfirma, string btnNegat, int retNegat)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = btnAfirma,
                NegativeButtonText = btnNegat
            };
            var metro = Application.Current.Windows[0] as MetroWindow;
            MessageDialogResult result = await metro.ShowMessageAsync(sTitulo, sMensaje,
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Negative)
                return retNegat;
            else if (result == MessageDialogResult.Affirmative)
                return retAfirma;
            return -1;
        }

        public async Task<int> ConfirmacionTresBotonesCustom(string sTitulo, string sMensaje, int retAfirma, int retNegat, int retAux)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Si",
                NegativeButtonText = "No",
                FirstAuxiliaryButtonText = "Cancelar"
            };
            var metro = Application.Current.Windows[0] as MetroWindow;
            MessageDialogResult result = await metro.ShowMessageAsync(sTitulo, sMensaje,
                MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);

            if (result == MessageDialogResult.Negative)
                return retNegat;
            else if (result == MessageDialogResult.Affirmative)
                return retAfirma;
            else if (result == MessageDialogResult.FirstAuxiliary)
                return retAux;
            return -1;
        }

        public async Task<int> ConfirmacionTresBotonesDinamico(string sTitulo, string sMensaje, string btnAfirma, int retAfirma, string btnNegat, int retNegat, string firstAux, int retAux)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = string.IsNullOrWhiteSpace(btnAfirma) ? "Si" : btnAfirma,
                NegativeButtonText = string.IsNullOrWhiteSpace(btnNegat) ? "No" : btnNegat,
                FirstAuxiliaryButtonText = string.IsNullOrWhiteSpace(firstAux) ? "Cancelar" : firstAux, 
            };
            var metro = Application.Current.Windows[0] as MetroWindow;
            MessageDialogResult result = await metro.ShowMessageAsync(sTitulo, sMensaje,
                MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);

            if (result == MessageDialogResult.Negative)
                return retNegat;
            else if (result == MessageDialogResult.Affirmative)
                return retAfirma;
            else if (result == MessageDialogResult.FirstAuxiliary)
                return retAux;
            return -1;
        }

        public async Task<int> ConfirmacionCuatroBotonesCustom(string sTitulo, string sMensaje, string btnAfirma, int retAfirma, string btnNegat, int retNegat, string firstAux, int retAux, string secondAux, int retSecAux)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = btnAfirma,
                NegativeButtonText = btnNegat,
                FirstAuxiliaryButtonText = firstAux,
                SecondAuxiliaryButtonText = secondAux
            };
            var metro = Application.Current.Windows[0] as MetroWindow;
            MessageDialogResult result = await metro.ShowMessageAsync(sTitulo, sMensaje,
                MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary, mySettings);

            if (result == MessageDialogResult.Negative)
                return retNegat;
            else if (result == MessageDialogResult.Affirmative)
                return retAfirma;
            else if (result == MessageDialogResult.FirstAuxiliary)
                return retAux;
            else if (result == MessageDialogResult.SecondAuxiliary)
                return retSecAux;
            return -1;
        }
    }
}