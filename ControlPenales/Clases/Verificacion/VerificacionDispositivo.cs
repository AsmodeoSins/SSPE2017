using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace ControlPenales
{
    public class VerificacionDispositivo
    {
        public static IEnumerable<NIC> GetMacAddress()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration where IPEnabled=true");
            IEnumerable<ManagementObject> objects = searcher.Get().Cast<ManagementObject>();
            return objects.Select(s => new NIC
            {
                mac = s.Properties["MACAddress"].Value.ToString().Replace(":","-"),
                obj_ips = s.Properties["IPAddress"].Value
            }); ;
        }

        public static List<HardDrive> GETHDSerial()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            var hdCollection = new List<HardDrive>();
            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                HardDrive hd = new HardDrive();
                hd.Model = wmi_HD["Model"].ToString();
                hd.InterfaceType = wmi_HD["InterfaceType"].ToString();
                hd.Caption = wmi_HD["Caption"].ToString();

                hd.SerialNo = wmi_HD.GetPropertyValue("SerialNumber").ToString();//get the serailNumber of diskdrive

                hdCollection.Add(hd);
            }
            return hdCollection;
        }
    }

    public class NIC
    {
        public string mac { get; set; }
        public object obj_ips;
        public string[] ips
        {
            get
            {
                return (string[]) obj_ips;
            }
        }
    }

    public class HardDrive
    {
        public string Model { get; set; }
        public string InterfaceType { get; set; }
        public string Caption { get; set; }
        public string SerialNo { get; set; }
    }
}
