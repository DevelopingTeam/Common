using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace test
{
    public class txtImport
    {
        private txtImport() { }

        private static txtImport instance;

        private static readonly object sybcObj = new object();
        public static txtImport Instance()
        {
            if (instance == null)
            {
                lock(sybcObj)
                {
                    if (instance == null)
                        instance = new txtImport();
                }
            }

            return instance;
        }

        /// <summary>
        /// 获取Table表头配置信息
        /// </summary>
        /// <param name="Table">表名</param>
        /// <returns>表头信息(以字典存储)</returns>
        public Dictionary<string,string> getTabHeader(string table)
        {
            string[] keys = ConfigurationManager.AppSettings.AllKeys;
            Dictionary<string, string> fieldsDict = new Dictionary<string, string>();

            foreach (var key in keys)
            {
                if (key.Split('_')[0] == table)
                {
                    string val = ConfigurationManager.AppSettings[key];
                    fieldsDict.Add(key, val);
                }                
            }

            return fieldsDict;
        }


        /// <summary>
        /// 导入数据文件
        /// </summary>
        /// <param name="path">文件及其路径</param>
        /// <param name="dict">保存为Dictionary结果集</param>
        public void importFile(string path, ref Dictionary<string, List<string[]>> dict)
        {
            StreamReader objReader = new StreamReader(path);
            String sLine = "";
            dict.Clear();

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (!string.IsNullOrEmpty(sLine))
                {
                    sLine = sLine.Trim();

                    List<string[]> arr = new List<string[]>();
                    var myArr = sLine.Split('\t');
                    arr.Add(myArr);

                    if (dict.ContainsKey(myArr[0].ToUpper()))
                    {
                        dict[myArr[0].ToUpper()].Add(sLine.Split('\t'));
                    }
                    else
                    {
                        dict.Add(myArr[0].ToUpper(), arr);
                    }
                }
            }
            objReader.Close();
        }

        /// <summary>
        /// 初始化datagriview表头
        /// </summary>
        /// <param name="dw">datagriview</param>
        /// <param name="dataList">数据集</param>
        public void initDataGriew(DataGridView dw, Dictionary<string, List<string[]>> dataList)
        {
            foreach (var key in dataList.Keys)
            {
                StringBuilder str = new StringBuilder();
                foreach (var list in dataList[key])
                    foreach (var val in list)
                    {
                        dw.Columns.Add(val, val);
                    }
                break;
            }
        }

        /// <summary>
        /// 填充DataGriew数据
        /// </summary>
        /// <param name="dw">DataGriew</param>
        /// <param name="dataList">数据集</param>
        public void FillDataGriew(DataGridView dw, Dictionary<string, List<string[]>> dataList)
        {
            initDataGriew(dw, dataList);
            
            int count = -1;
            foreach (var key in dataList.Keys)
            {
                count++;
                if (count == 0)
                    continue;
                foreach (var list in dataList[key])
                {
                    int index = dw.Rows.Add();
                    for (int i = 0; i < list.Length; i++)
                        dw.Rows[index].Cells[i].Value = list[i];
                }
            }         
        }
        /// <summary>
        /// DataGridView绘制行号
        /// </summary>
        /// <param name="dw">DataGridView</param>
        /// <param name="e">事件e</param>
        public void dwRowPostPaint(DataGridView dw, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dw.RowHeadersWidth - 4, e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
            dw.RowHeadersDefaultCellStyle.Font,
            rectangle, dw.RowHeadersDefaultCellStyle.ForeColor,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

    }
}
