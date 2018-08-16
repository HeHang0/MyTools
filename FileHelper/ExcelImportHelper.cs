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

        public List<ExcelRow> Excute(bool isFirstRowAsIndex = false)
        {
            using (excelStream)
            using (ExcelPackage package = new ExcelPackage(excelStream))
            {
                ExcelWorksheets workSheet = package.Workbook.Worksheets;
                var a = workSheet.FirstOrDefault();
                var array = a.Cells.Value as object[,];
                if (isFirstRowAsIndex)
                {
                    return GetExcelRowCollectionWithRowIndex(array);
                }
                else
                {
                    return GetExcelRowCollectionWith(array);
                }
                
            }
        }
        private List<ExcelRow> GetExcelRowCollectionWith(object[,] values)
        {
            List<ExcelRow> list = new List<ExcelRow>();
            for (int i = 0; i < values.GetLength(0); i++)
            {
                List<object> ol = new List<object>();
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    ol.Add(values[i, j]);
                }
                list.Add(new ExcelRow(ol.ToArray()));
            }
            return list;
        }

        private List<ExcelRow> GetExcelRowCollectionWithRowIndex(object[,] values)
        {
            Dictionary<string,int> rowIndexList = new Dictionary<string, int>();
            for (int i = 0; i < values.GetLength(1); i++)
            {
                string value = values[0, i].ToString();
                if (!rowIndexList.ContainsKey(value))
                {
                    rowIndexList.Add(value, i);
                }
            }
            List<ExcelRow> list = new List<ExcelRow>();
            for (int i = 1; i < values.GetLength(0); i++)
            {
                Dictionary<string, object> soDic = new Dictionary<string, object>();
                foreach (var item in rowIndexList)
                {
                    soDic.Add(item.Key, values[i, item.Value]);
                }
                list.Add(new ExcelRow(soDic));
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

    public class ExcelRow
    {
        public ExcelRow(object[] values)
        {
            collection = values;
        }

        public ExcelRow(Dictionary<string, object> values)
        {
            collection = new object[values.Count];
            rowIndex = new Dictionary<string, int>();
            int i = 0;
            foreach (var item in values)
            {
                collection[i] = item.Value;
                rowIndex.Add(item.Key, i++);
            }
        }

        public int Count => collection?.Length ?? 0;

        public object this[int groupnum] => collection[groupnum];
        public object this[string groupname]
        {
            get
            {
                if (collection != null && rowIndex != null && rowIndex.ContainsKey(groupname))
                {
                    return collection[rowIndex[groupname]];
                }
                else
                {
                    return null;
                }
            }
        }

        private object[] collection;

        private readonly Dictionary<string, int> rowIndex;
    }
}
