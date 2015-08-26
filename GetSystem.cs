using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Management;
using System.Net;

namespace PcInfo
{
    public class Configuration
    {
        [DllImport("kernel32")]
        public static extern void GetWindowsDirectory(StringBuilder winDir, int count);

        [DllImport("kernel32")]
        public static extern void GetSystemDirectory(StringBuilder sysDir, int count);

        [DllImport("kernel32")]
        public static extern void GetSystemInfo(ref CPU_INFO cpuInfo);

        [DllImport("kernel32")]
        public static extern void GlobalMemoryStatus(ref MEMORY_INFO memInfo);

        [DllImport("kernel32")]
        public static extern void GetSystemTime(ref SYSTEMTIME_INFO sysInfo);

        [StructLayout(LayoutKind.Sequential)]
        public struct CPU_INFO
        {
            public uint dwOemId;
            public uint dwPageSize;
            public uint lpMinAppAddress;
            public uint lpMaxAppAddress;
            public uint dwActiveProcessorMask;
            public uint dwNumOFProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public uint dwProcessorLevel;
            public uint dwProcessorRevision;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_INFO
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public uint dwTotalPhys;
            public uint dwAvailPhys;
            public uint dwTotalPageFile;
            public uint dwAvailPageFile;
            public uint dwTotalVirtual;
            public uint dwAvailVirtual;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME_INFO
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSencond;
            public ushort wMilliseconds;
        }

        /// <summary>
        /// 获取操作系统设置
        /// </summary>
        /// <returns></returns>
        public string initSysInfoData()
        {
            string info = "";
            info = string.Format("计算机名：{0}\n", SystemInformation.ComputerName);
            info += string.Format("是否连接网络：{0}\n", SystemInformation.Network);
            info += string.Format("当前线程用户：{0}\n", SystemInformation.Network);
            info += string.Format("启动方式：{0}\n", SystemInformation.Network);
            info += string.Format("菜单的字体：{0}\n", SystemInformation.MonitorCount);
            info += string.Format("鼠标已安装：{0}\n", SystemInformation.MouseWheelPresent);
            info += string.Format("鼠标按钮数：{0}\n", SystemInformation.MouseButtons);
            info += string.Format("是否交互模式：{0}\n", SystemInformation.UserInteractive);
            info += string.Format("屏幕界限：{0}\n", SystemInformation.VirtualScreen);

            return info;
        }

        /// <summary>
        /// 获取程序运行的相关信息
        /// </summary>
        /// <returns></returns>
        public string initEnvData()
        {
            string info = "";
            info = string.Format("命令行：{0}\n", Environment.CommandLine);
            info += string.Format("命令行参数：{0}\n", Environment.GetCommandLineArgs());
            info += string.Format("当前目录：{0}\n", Environment.CurrentDirectory);
            info += string.Format("操作系统版本：{0}\n", Environment.OSVersion);
            info += string.Format("系统目录：{0}\n", Environment.SystemDirectory);
            info += string.Format("堆栈信息：{0}\n", Environment.StackTrace);
            info += string.Format("用户域：{0}\n", Environment.UserDomainName);
            info += string.Format("CLR版本：{0}\n", Environment.Version);
            info += string.Format("系统启动时间（ms）：{0}\n", Environment.TickCount);
            info += string.Format("进程上下文的物理内存量：{0}\n", Environment.WorkingSet);
            info += string.Format("本机磁盘驱动器：{0}\n", string.Join(",", Environment.GetLogicalDrives()));

            info += string.Format("本机的所有环境变量：\n");
            IDictionary envs = Environment.GetEnvironmentVariables();

            foreach(DictionaryEntry de in envs)
            {
                info += string.Format("\t{0}={1}\t", de.Key, de.Value);
            }

            return info;
        }

        /// <summary>
        /// 通过注册表获取信息
        /// </summary>
        /// <returns></returns>
        public string initRegKeyData()
        {
            string info = "";
            RegistryKey rKey = Registry.LocalMachine;
            rKey = rKey.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");
            info = string.Format("处理器信息：{0}\n\n", 
                rKey.GetValue("ProcessorNameString").ToString().Trim());

            rKey = rKey.OpenSubKey("SOFTWARE\\Microsoft\\WindowsNT\\CurrentVersion\\NetworkCards\\1");
            info += string.Format("网卡信息：{0}\n",
                rKey.GetValue("Description").ToString().Trim());

            return info;
        }

        /// <summary>
        /// 调用API获取系统相关信息
        /// </summary>
        /// <returns></returns>
        public string initAPIData()
        {
            string info = "";
            //获取Windows路径
            const int nChars = 128;
            StringBuilder buff = new StringBuilder(nChars);
            GetWindowsDirectory(buff, nChars);
            info = string.Format("Windows路径：{0}\n", buff.ToString());

            //获取系统路径
            GetSystemDirectory(buff, nChars);
            info += string.Format("系统路径：{0}\n", buff.ToString());

            //获取CPU信息
            CPU_INFO cpuInfo = new CPU_INFO();
            GetSystemInfo(ref cpuInfo);
            info += string.Format(
                "\n本机中有{0}个CPU\nCPU类型为：{1}\nCPU等级为：{2}\nCPU的OEM ID为：{3}CPU的页面大小为：{4}\n", 
                cpuInfo.dwNumOFProcessors.ToString(), cpuInfo.dwProcessorType.ToString(),
                cpuInfo.dwProcessorLevel.ToString(), cpuInfo.dwOemId.ToString(), cpuInfo.dwPageSize.ToString());

            //获取内存信息
            MEMORY_INFO memInfo = new MEMORY_INFO();
            GlobalMemoryStatus(ref memInfo);
            info += string.Format("内存正在使用{0}K\n",Math.Round(memInfo.dwMemoryLoad / (1.0*1024),2).ToString());
            info += string.Format("物理内存共有{0}M\n", Math.Round(memInfo.dwTotalPhys / (1.0 * 1024 * 1024), 2).ToString());
            info += string.Format("可使用的物理内存共有{0}M\n", Math.Round(memInfo.dwAvailPhys / (1.0 * 1024 * 1024), 2).ToString());
            info += string.Format("交换文件总大小为{0}M\n", Math.Round(memInfo.dwTotalPageFile / (1.0 * 1024 * 1024), 2).ToString());
            info += string.Format("尚可交换文件大小为{0}M\n", Math.Round(memInfo.dwAvailPageFile / (1.0 * 1024 * 1024), 2).ToString());
            info += string.Format("总虚拟内存为{0}M\n", Math.Round(memInfo.dwTotalVirtual / (1.0 * 1024 * 1024), 2).ToString());
            info += string.Format("未用虚拟内存为{0}M\n", Math.Round(memInfo.dwAvailVirtual / (1.0 * 1024 * 1024), 2).ToString());

            //获取系统时间信息
            SYSTEMTIME_INFO stInfo = new SYSTEMTIME_INFO();
            GetSystemTime(ref stInfo);
            info += string.Format("\n系统时间：{0}年{1}月{2}日 星期{6} {3}时{4}分{5}秒",
                stInfo.wYear, stInfo.wMonth, stInfo.wDay, stInfo.wDayOfWeek,
                stInfo.wHour, stInfo.wMinute, stInfo.wMilliseconds);

            return info;
        }
    }
   
    public class HardwareInfo
    {
        public enum NCBCONST
        {
            NCBNAMSZ = 16,
            MAX_LANA = 254,
            NCBENUM = 0x37,
            NRC_GOODRET = 0x00,
            NCBRESET = 0x32,
            NCBASTAT = 0x33,
            NUM_NAMEBUF = 30
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ADAPTER_STATUS
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] adapter_address;
            public byte rev_major;
            public byte reserved0;
            public byte adapter_type;
            public byte rev_minor;
            public ushort duration;
            public ushort frmr_recv;
            public ushort frmr_xmit;
            public ushort iframe_recv_err;
            public ushort xmit_aborts;
            public uint xmit_success;
            public uint recv_success;
            public ushort iframe_xmit_err;
            public ushort recv_buff_unavail;
            public ushort t1_timeouts;
            public ushort ti_timeouts;
            public uint reserved1;
            public ushort free_ncbs;
            public ushort max_cfg_ncbs;
            public ushort max_ncbs;
            public ushort xmit_buf_unavail;
            public ushort max_dgram_size;
            public ushort pending_sess;
            public ushort max_cfg_sess;
            public ushort max_sess;
            public ushort max_sess_pkt_size;
            public ushort name_count;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NAME_BUFFER
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NCBNAMSZ)]
            public byte[] name;
            public byte name_num;
            public byte name_flags;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct NCB
        {
            public byte ncb_command;
            public byte ncb_retcode;
            public byte ncb_lsn;
            public byte ncb_num;
            public IntPtr ncb_buffer;
            public ushort ncb_length;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NCBNAMSZ)]
            public byte[] ncb_callname;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NCBNAMSZ)]
            public byte[] ncb_name;
            public byte ncb_rto;
            public byte ncb_sto;
            public IntPtr ncb_post;
            public byte ncb_lana_num;
            public byte ncb_cmd_cplt;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] ncb_reserve;
            public IntPtr ncb_event;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LANA_ENUM
        {
            public byte length;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.MAX_LANA)]
            public byte[] lana;
        }
        [StructLayout(LayoutKind.Auto)]
        public struct ASTAT
        {
            public ADAPTER_STATUS adapt;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NUM_NAMEBUF)]
            public NAME_BUFFER[] NameBuff;
        }

        public class Win32API
        {
            [DllImport("NETAPI32.DLL")]
            public static extern char Netbios(ref NCB ncb);
        }
        public string getHostName()
        {
            return Dns.GetHostName();
        }

        public string getCpuID()
        {
            try
            {
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();

                string strCpuID = "";
                foreach (ManagementObject mo in moc)
                {
                    strCpuID += mo.Properties["ProcessorId"].Value.ToString() + "\n";
                    //break;
                }
                return strCpuID;
            }
            catch
            {
                return "";
            }
        }

        public string getHardDiskID()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher
                    (" select * from Win32_PhysicalMedia");

                string strHardDiskID = "";
                foreach (ManagementObject mo in searcher.Get())
                {
                    strHardDiskID += mo["SerialNumber"].ToString().Trim() + "\n";
                    //break;
                }

                //方法二
                ManagementClass ci = new ManagementClass("Win32_DiskDrive");
                ManagementObjectCollection mc = ci.GetInstances();

                string HDid = "";
                foreach (ManagementObject mo in mc)
                {
                    HDid = mo.Properties["Model"].Value.ToString();
                    strHardDiskID += string.Format("硬盘序列号：{0}\n", HDid);
                }

                return strHardDiskID;
            }
            catch
            {
                return "";
            }
        }

        public string getMacAddress()
        {
            string addr = "";
            try
            {
                int cb;
                ASTAT adapter;
                NCB Ncb = new NCB();
                char uRetCode;
                LANA_ENUM lenum;

                Ncb.ncb_command = (byte)NCBCONST.NCBENUM;
                cb = Marshal.SizeOf(typeof(LANA_ENUM));
                Ncb.ncb_buffer = Marshal.AllocHGlobal(cb);
                Ncb.ncb_length = (ushort)cb;
                uRetCode = Win32API.Netbios(ref Ncb);
                lenum = (LANA_ENUM)Marshal.PtrToStructure(Ncb.ncb_buffer, typeof(LANA_ENUM));
                Marshal.FreeHGlobal(Ncb.ncb_buffer);

                if (uRetCode != (short)NCBCONST.NRC_GOODRET)
                    return "";

                for (int i = 0; i < lenum.length; i++)
                {
                    Ncb.ncb_command = (byte)NCBCONST.NCBRESET;
                    Ncb.ncb_lana_num = lenum.lana[i];
                    uRetCode = Win32API.Netbios(ref Ncb);

                    if (uRetCode != (short)NCBCONST.NRC_GOODRET)
                        return "";

                    Ncb.ncb_command = (byte)NCBCONST.NCBASTAT;
                    Ncb.ncb_lana_num = lenum.lana[i];
                    Ncb.ncb_callname[0] = (byte)'*';
                    cb = Marshal.SizeOf(typeof(ADAPTER_STATUS)) 
                        + Marshal.SizeOf(typeof(NAME_BUFFER)) 
                        * (int)NCBCONST.NUM_NAMEBUF;
                    Ncb.ncb_buffer = Marshal.AllocHGlobal(cb);
                    Ncb.ncb_length = (ushort)cb;
                    uRetCode = Win32API.Netbios(ref Ncb);
                    adapter.adapt = (ADAPTER_STATUS)Marshal.PtrToStructure(
                        Ncb.ncb_buffer, typeof(ADAPTER_STATUS));
                    Marshal.FreeHGlobal(Ncb.ncb_buffer);

                    if (uRetCode == (short)NCBCONST.NRC_GOODRET)
                    {
                        if (i > 0)
                            addr += ":";
                        addr = string.Format(
                            "{0,2:X}{1,2:X}{2,2:X}{3,2:X}{4,2:X}{5,2:X}", 
                            adapter.adapt.adapter_address[0], 
                            adapter.adapt.adapter_address[1], 
                            adapter.adapt.adapter_address[2], 
                            adapter.adapt.adapter_address[3], 
                            adapter.adapt.adapter_address[4],
                            adapter.adapt.adapter_address[5]);
                    }
                }
            }
            catch { }

            //方法二
            string str = "\n";
            try
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                        str += string.Format("MAC adress: {0}", mo["MacAddress"].ToString());
                    mo.Dispose();
                }
            }
            catch{  }

            //方法三
            IList<string> MAC = new List<string>();
            string mac = "\n";

            try
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                        MAC.Add(mo["MacAddress"].ToString());
                    mo.Dispose();
                }

                bool Bool = true;
                for (int i = 0; i < MAC.Count; i++)
                {
                    for (int j = 0; j < MAC.Count; j++)
                    {
                        if (i == j)
                            continue;

                        if (MAC[i].Equals(MAC[j]))
                            Bool = true;
                        else
                            Bool = false;

                        if (!Bool)
                            break;

                        if (Bool)
                            mac = MAC[0];
                        else
                        {
                            for (int k = 0; k < MAC.Count; k++)
                            {
                                bool isM = true;
                                for (int m = 0; m <MAC.Count; m++)
                                {
                                    if (k == m)
                                        continue;

                                    if (MAC[k].Equals(MAC[m]))
                                    {
                                        isM = false;
                                        break;
                                    }
                                    else
                                        isM = true;

                                    if (isM)
                                    {
                                        mac = MAC[k];
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return string.Format("MacAddress: {0}\n{1}\nmacAddress:{2}", addr.Replace(' ', '0') , str , mac);
        }
    }
}
