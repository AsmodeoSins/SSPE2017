using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MVVMShared.Views
{
    public class Dialogos
    {
        public Dialogos(){ }

        public async void NotificacionDialog(string sTitulo, string sMensaje )
        {
            var metro = Application.Current.Windows[0] as MetroWindow;
            var dialog = (BaseMetroDialog)metro.Resources["NotificacionDialog"];
            //hay que ver como podemos hacer referencia a esta propiedad
            //PrincipalViewModel.DialogTitulo = sTitulo;
            //PrincipalViewModel.DialogMensaje = sMensaje;
            await metro.ShowMetroDialogAsync(dialog);
            await TaskEx.Delay(1500);
            await metro.HideMetroDialogAsync(dialog);
        }

        public async void ConfirmacionDialogo(string sTitulo, string sMensaje)
        {
            var metro = Application.Current.Windows[0] as MetroWindow;
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Cerrar"
            };
            await metro.ShowMessageAsync(sTitulo, sMensaje, MessageDialogStyle.Affirmative, mySettings);
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
    }
}