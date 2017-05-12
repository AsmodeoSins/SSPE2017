using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMShared.Manager.Interfaces;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace MVVMShared.Manager
{
    public class FileManagerProvider:IFileManagerProvider
    {
        public bool isFileMaxSize(string filename, int maxSize)
        {
            if (new FileInfo(filename).Length <= maxSize)
                return true;
            return false;
        }

        public byte[] fileToByteArray(string filename)
        {
            return bitmapToByte(fileToBitmapImage(filename));
        }

        public ImageSource fileToImageSource(string filename)
        {
            var imgSrc = fileToBitmapImage(filename) as ImageSource;
            return imgSrc; 
        }

        public BitmapImage fileToBitmapImage(string filename)
        {
            var biImg = new BitmapImage();
            biImg.BeginInit();
            biImg.UriSource = new Uri(filename, UriKind.Absolute);
            biImg.EndInit();
            return biImg;
        }
        public byte[] bitmapToByte(BitmapSource imageBMP)
        {
            byte[] data;
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageBMP));
            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }

        public ImageSource byteToImageSource(byte[] imageData)
        {
            using (MemoryStream mStream= new MemoryStream(imageData))
            {
                var biImg = new BitmapImage();
                biImg.BeginInit();
                biImg.CacheOption = BitmapCacheOption.OnLoad;
                biImg.StreamSource = mStream;
                biImg.EndInit();
                var imgSrc = biImg as ImageSource;
                return imgSrc;
            }
        }
    }
}
