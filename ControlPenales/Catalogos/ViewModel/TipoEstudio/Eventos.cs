using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;

namespace ControlPenales
{
    partial class CatalogoTipoEstudioViewModel
    {
        private ICommand _buscarClick;
        public ICommand BuscarClick
        {
            get { return _buscarClick ?? (_buscarClick = new RelayCommand(ClickEnter)); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }

        public ICommand CatalogoSimpleLoading
        {
            get { return new DelegateCommand<CatalogoSimpleView>(TipoEstudioLoad); }
        }
    }

    //public partial class MainWindow : Window
    //{
    //    public class FileInfo { public string Name { get; set; } public DateTime LastModified { get; set; } public FileInfo(string name) { Name = name; LastModified = DateTime.Now; } } ObservableCollection<FileInfo> mFileNames = new ObservableCollection<FileInfo>(); public ObservableCollection<FileInfo> FileNames { get { return mFileNames; } } public MainWindow() { DataContext = this; InitializeComponent(); }
    //    private void Window_Loaded(object sender, RoutedEventArgs e)
    //    {
    //        ThreadPool.QueueUserWorkItem((x) =>
    //        {
    //            while (true)
    //            {
    //                Dispatcher.BeginInvoke((Action)(() =>
    //                    {
    //                        mFileNames.Add(new FileInfo("X"));
    //                    })); Thread.Sleep(500);
    //            }
    //        });
    //    }
    //}
}
