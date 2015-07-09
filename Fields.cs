using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Analysis
{
    /// <summary>
    /// Fields操作类
    /// by zhangbc 2015-07-06
    /// modify by zhangbc 2015-07-09 单例模式，双重锁机制
    /// </summary>
    class Fields
    {
        /// <summary>
        /// 定义一个私有的静态全局变量来保存该类的唯一实例
        /// </summary>
        private static Fields fieldsInstance;

        /// <summary>
        /// 构造函数必须是私有的
        /// 这样在外部便无法使用 new 来创建该类的实例
        /// </summary>
        private Fields()
        {

        }

        /// <summary>
        /// 定义一个只读静态对象,只是在程序运行时创建的
        /// </summary>
        private static readonly object syncObj = new object();

        /// <summary>
        ///  定义一个全局访问点，设置为静态方法
        ///  则在类的外部便无需实例化就可以调用该方法
        /// </summary>
        /// <returns>返回单例fieldsInstance</returns>
        public static Fields GetInstance()
        {
            //第一重 singleton == null
            if(fieldsInstance==null)
            {
                lock (syncObj)
                {
                    //第二重 singleton == null
                    if (fieldsInstance == null)
                    {
                        fieldsInstance = new Fields();
                    }
                }
            }
            return fieldsInstance;
        }

        static iniFile filedReader = new iniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config\Fields.ini");

        /// <summary>
        /// 读取指定的结点下对应key的value值
        /// </summary>
        /// <param name="Session">结点名称</param>
        /// <param name="key">key值</param>
        /// <returns>返回的value值</returns>
        public string getValue(string Session,string key)
        {
            return filedReader.readIniValues(Session, key);
        }
        
        /// <summary>
        /// 读取ini文件的所有的section结点
        /// </summary>
        /// <returns>所有结点名称</returns>
        public string[] getSections()
        {
            return filedReader.readiniSections();
        }

        /// <summary>
        /// 读取ini文件指定section结点所有条目
        /// </summary>
        /// <param name="Session">结点名称</param>
        /// <returns>返回Key-value键值对</returns>
        public string[] keyVals(string Session)
        {
            return filedReader.readIniKeyVals(Session);
        }

        /// <summary>
        /// 读取ini文件指定section结点所有Keys列表
        /// </summary>
        /// <param name="Session">结点名称</param>
        /// <returns>返回keys列表</returns>
        public string[] getKeys(string Session)
        {
            return filedReader.readIniKeys(Session);
        }

        /// <summary>
        /// 动态获取层次性属性对象
        /// </summary>
        /// <returns>返回属性对象</returns>
        public PropertyManageCls getPmc()
        {
            PropertyManageCls pmc = new PropertyManageCls();
            string[] sections=getSections();
            foreach(var section in sections)
            {
                string[] kvs = keyVals(section);
                foreach (var kv in kvs)
                {
                    Property pp = new Property(kv.Split('=')[0], kv.Split('=')[1], false, true);
                    pp.Category = section;
                    pp.DisplayName = kv.Split('=')[0];
                    pmc.Add(pp);
                }
            }
            return pmc;
        }
    }
}
