using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;


namespace DataPlantingForm
{
    /// <summary>
    /// 操作INI文件类
    /// by zhangbc 2015-07-06
    /// </summary>
    class iniFile
    {
        /// <summary>
        /// 内存缓冲区的长度
        /// </summary>
        const int DATA_SIZE = 1024;
        /// <summary>
        /// 默认的内存缓冲区大小
        /// </summary>
        const uint MAX_BUFFER = 32767;
        private string _path;
        /// <summary>
        /// ini文件及其路径
        /// </summary>
        private string iniPath
        {
            get {return _path;}
            set { _path = value; }
        }
        [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Auto)]
        public struct StringBuffer
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DATA_SIZE)]
            public string sText;
        }

        #region API声明
        /// <summary>
        /// 将结点信息写入ini文件
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">key值</param>
        /// <param name="val">value值</param>
        /// <param name="filePath">ini文件及其路径</param>
        /// <returns>实际写入的长度</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
     
        /// <summary>
        /// 读取ini文件中指定的Key值(法一)
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">key值</param>
        /// <param name="def">读取失败时的默认值</param>
        /// <param name="retVal">返回的value值(自定义结构型)</param>
        /// <param name="size">内存缓冲区的长度</param>
        /// <param name="filePath">ini文件及其路径</param>
        /// <returns>实际读取到的长度</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetPrivateProfileString(string section, string key, string def, out StringBuffer retVal, int size, string filePath);

        /// <summary>
        /// 读取ini文件中指定的Key值(法二)
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">key值</param>
        /// <param name="def">读取失败时的默认值</param>
        /// <param name="retVal">返回的value值(字符组型)</param>
        /// <param name="size">内存缓冲区的长度</param>
        /// <param name="filePath">ini文件及其路径</param>
        /// <returns>实际读取到的长度</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileString(string section, string key, string def, [In,Out] char[] retVal, uint size, string filePath);
      
        /// <summary>
        /// 获取所有的结点名称
        /// </summary>
        /// <param name="szReturnBuffer">存放节点名称的内存地址,每个结点用\0间隔</param>
        /// <param name="size">内存大小</param>
        /// <param name="filePath">ini文件及其路径</param>
        /// <returns>内容的实际长度,为0表示没有内容,为size-2表示内存大小不够</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileSectionNames(IntPtr szReturnBuffer, uint size, string filePath);
       
        /// <summary>
        /// 获取某个结点下的所有的Key和Value
        /// </summary>
        /// <param name="section">结点名称</param>
        /// <param name="szReturnBuffer">存放节点名称的内存地址,每个结点用\0间隔</param>
        /// <param name="size">内存大小</param>
        /// <param name="filePath">ini文件及其路径</param>
        /// <returns>内容的实际长度,为0表示没有内容,为size-2表示内存大小不够</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileSection(string section,IntPtr szReturnBuffer, uint size, string filePath);
        #endregion
       
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sPath">文件名及其路径</param>
        public iniFile(string sPath)
        {
            this._path = sPath;
            string path = iniPath.Substring(0, iniPath.LastIndexOf('\\'));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!File.Exists(_path))
                createIniFile();
        }

        /// <summary>
        /// 写文件操作
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">key值</param>
        /// <param name="val">value值</param>
        public void writeIniValue(string section,string key,string val)
        {
            WritePrivateProfileString(section,key,val,this._path);
        }

        /// <summary>
        /// 读取ini文件的所有的section结点
        /// </summary>
        /// <returns>返回结点值列表</returns>
        public string[] readiniSections()
        {
            string[] sections = new string[0];
            //申请内存
            IntPtr pString = Marshal.AllocCoTaskMem((int)MAX_BUFFER * sizeof(char));
            uint byteReturned = GetPrivateProfileSectionNames(pString,MAX_BUFFER,this._path);
            if(byteReturned!=0)
            {
                string local = Marshal.PtrToStringAuto(pString,(int)byteReturned).ToString();
                sections = local.Split(new char[]{'\0'},StringSplitOptions.RemoveEmptyEntries);
            }
            //释放内存
            Marshal.FreeCoTaskMem(pString);
            return sections;
        }

        /// <summary>
        /// 读取ini文件下指定结点下Key值列表
        /// </summary>
        /// <param name="section">节点名</param>
        /// <returns>返回Key值列表</returns>
        public string[] readIniKeys(string section)
        {
            string[] keys = new string[0];
            const int SIZE = DATA_SIZE * 10;

            if(string.IsNullOrEmpty(section))
            {
                throw new ArgumentException("必须指定节点名称","section");
            }
            char[] chars = new char[SIZE];
            uint byteReturned = GetPrivateProfileString(section, null,null,chars, SIZE, this._path);
            if (byteReturned != 0)
            {
                keys = new string(chars).Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            }
            chars = null;
            return keys;
        }

        /// <summary>
        /// 读取ini文件下指定key值的value
        /// </summary>
        /// <param name="section">节点名</param>
        /// <param name="key">指定的key值</param>
        /// <returns>value值</returns>
        public string readIniValues(string section,string key)
        {
            StringBuffer retVal;
            GetPrivateProfileString(section,key,null,out retVal,DATA_SIZE,this._path);
            return retVal.sText.Trim();
        }

        /// <summary>
        /// 读取ini文件下指定values值,重载
        /// </summary>
        /// <param name="section">节点名</param>
        /// <param name="key">指定的key值</param>
        /// <returns>value值</returns>
        public string readIniValues(string section, string key, string defaultVal)
        {
            StringBuffer retVal;
            GetPrivateProfileString(section, key, null, out retVal, DATA_SIZE, this._path);
            string tmp=retVal.sText.Trim();
            return tmp==""?defaultVal:tmp;
        }

        /// <summary>
        /// 读取ini文件下指定结点的所有条目(key=value)
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public string[] readIniKeyVals(string section)
        {
            string[] items = new string[0];
            //申请内存
            IntPtr pString = Marshal.AllocCoTaskMem((int)MAX_BUFFER * sizeof(char));
            uint byteReturned = GetPrivateProfileSection(section,pString, MAX_BUFFER, this._path);
            if (!(byteReturned==MAX_BUFFER-2)||(byteReturned == 0))
            {
                string local = Marshal.PtrToStringAuto(pString, (int)byteReturned).ToString();
                items = local.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            }
            //释放内存
            Marshal.FreeCoTaskMem(pString);
            return items;
        }

        /// <summary>
        /// 创建空文件ini
        /// </summary>
        public void createIniFile()
        {
            StreamWriter wr = File.CreateText(this._path);
            wr.Write("");
            wr.Flush();
            wr.Close();
        }
    }
}
