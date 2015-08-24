using System;
using System.Management;
using System.Windows.Forms;
using Microsoft.Win32;

namespace PcInfo
{
    class Program
    {
        /// <summary>
        /// 读取CPU数量
        /// </summary>
        /// <returns>返回CPU数量</returns>
        private static UInt32 countPhysicalProcessor()
        {
            ManagementObjectSearcher objects = new ManagementObjectSearcher(
                " select * from Win32_ComputerSystem ");
            ManagementObjectCollection coll = objects.Get();

            foreach(ManagementObject obj in coll)
            {
                return (UInt32)obj["NumberOfProcessors"];
            }
            return 0;
        }

        /// <summary>
        /// 读取内存容量
        /// </summary>
        /// <returns>返回内存容量</returns>
        private static UInt64 countPhysicalMemory()
        {
            ManagementObjectSearcher objects = new ManagementObjectSearcher(
                " select * from Win32_PhysicalMemory ");
            ManagementObjectCollection coll = objects.Get();
            UInt64 total = 0;

            foreach (ManagementObject obj in coll)
            {
                total += (UInt64)obj["Capacity"];
            }
            return total;
        }
        
        static void Main(string[] args)
        {
            #region 读取OS和CLR版本

            OperatingSystem os = Environment.OSVersion;

            string str = string.Format("Platform: {0}\nService Pack: {1}\nVersion: {2}\nVersionString: {3}\nCLR Version: {4}\n\n",
                os.Platform, os.ServicePack, os.Version, os.VersionString, Environment.Version);

            //Console.WriteLine(str);
            #endregion

            #region 读取CPU数量和内存容量
            str = string.Format("Machine: {0}\n#of processors(logical): {1}\n#of processors(phyical): {2}\n",
                Environment.MachineName, Environment.ProcessorCount, countPhysicalProcessor());
            str += string.Format("RAM installed: {0:N0}bytes.\nIs OS 64-bit? {1}\nIs process 64-bit? {2}\nLittle-endian: {3}\n\n",
                countPhysicalMemory(), Environment.Is64BitOperatingSystem, Environment.Is64BitProcess, BitConverter.IsLittleEndian);
            
            foreach(Screen screen in Screen.AllScreens)
            {
                str += string.Format("Screen:{0}\n\tPrimary:{1}\n\tBounds:{2}\n\tWorking Area:{3}\n\tBitPrePixel:{4}\n\n",
                    screen.DeviceName, screen.Primary, screen.Bounds, screen.WorkingArea, screen.BitsPerPixel);
            }

            //Console.WriteLine(str);
            #endregion

            #region 读取注册表键值对

            using (RegistryKey keyRun = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run"))
            {
                str = "";
                foreach(string value in keyRun.GetValueNames())
                {
                    str += string.Format("Name: {0}\nValue:  {1}\n\n", value, keyRun.GetValue(value));
                }
            }

            //Console.WriteLine(str);
            #endregion

            Console.ReadKey();

        }
    }
}
