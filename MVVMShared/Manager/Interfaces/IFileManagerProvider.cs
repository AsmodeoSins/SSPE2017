using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace MVVMShared.Manager.Interfaces
{
    public interface IFileManagerProvider
    {
        bool isFileMaxSize(string filename, int maxSize);
        byte[] fileToByteArray(string filename);
        ImageSource fileToImageSource(string filename);
        BitmapImage fileToBitmapImage(string filename);
        byte[] bitmapToByte(BitmapSource imageBMP);
        ImageSource byteToImageSource(byte[] imageData);
    }
}
