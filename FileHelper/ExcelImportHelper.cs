using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHelper
{
    public class ExcelImportHelper
    {
        private Stream excelStream;
        public ExcelImportHelper(string path)
        {
            excelStream = FileToStream(path);
        }

        public ExcelImportHelper(Stream stream)
        {
            excelStream = stream ?? throw new ArgumentException("Stream不可为空！");
        }

        public List<Dictionary<int, object>> Excute()
        {
            List<Dictionary<int, object>> list = new List<Dictionary<int, object>>();
            using (excelStream)
            using (ExcelPackage package = new ExcelPackage(excelStream))
            {
                ExcelWorksheets workSheet = package.Workbook.Worksheets;
                var a = workSheet.FirstOrDefault();
                var array = a.Cells.Value as object[,];
                
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    Dictionary<int, object> dic = new Dictionary<int, object>();
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        dic.Add(j, array[i, j]);
                    }
                    list.Add(dic);
                }
            }
            return list;
        }


        /// <summary> 
        /// 从文件读取 Stream
        /// </summary> 
        private Stream FileToStream(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            // 把 byte[] 转换成 Stream 
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        ~ExcelImportHelper()
        {
            excelStream?.Close();
        }
    }

    class ExcelRow
    {
        ExcelRow(object[,] values, bool isFirstRowAsIndex = false)
        {
            if (isFirstRowAsIndex)
            {
                Dictionary<string, int> dic = new Dictionary<string, int>();
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    string index = values[0, j].ToString();
                    if (!dic.ContainsKey(index))
                    {
                        dic.Add(values[0, j].ToString(), j);
                    }
                }
                rowIndex = dic;

            }
        }
        public object this[int groupnum] => collection[groupnum];
        public object this[string groupname] => collection[rowIndex[groupname]];

        private object[] collection;

        private readonly Dictionary<string, int> rowIndex;
    }
}
