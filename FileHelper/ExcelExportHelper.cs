using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 *          Dictionary<string, string> col = new Dictionary<string, string>() {
 *              { "Col1","列1"}, { "Col2", "列2" },{ "Col3", "列3" }
 *          };
 *          byte[] fileContent = ExcelExportHelper.ExportExcel(result.OrderBy(m => m.SortOrder).ToList(), "", false, col);
 *          using (FileStream fs = new FileStream("file.xlsx", FileMode.Create))
 *          {
 *              fs.Write(fileContent, 0, fileContent.Length);
 *          } 
 */

namespace FileHelper
{
    public class ExcelExportHelper
    {
        public static string ExcelContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        #region 【List转DataTable】

        /// <summary>
        /// List转DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="ColumnsChangeName">根据Dictionary替换属性Title</param>
        /// <returns></returns>
        private static DataTable ListToDataTable<T>(List<T> data, Dictionary<string, string> ColumnsChangeName = null)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dataTable = new DataTable();
            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor property = properties[i];
                var PropertyType = property.PropertyType;
                if (ColumnsChangeName != null && ColumnsChangeName.Keys.Contains(property.Name))
                {
                    dataTable.Columns.Add(ColumnsChangeName[property.Name], Nullable.GetUnderlyingType(PropertyType) ?? PropertyType);
                }
                else
                {
                    dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(PropertyType) ?? PropertyType);
                }
            }
            object[] values = new object[properties.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (properties[i].PropertyType.Name == typeof(Byte[]).Name)
                    {
                        values[i] = properties[i].GetValue(item) == null ? string.Empty : ByteToTwo(properties[i].GetValue(item) as Byte[]);
                    }
                    else
                    {
                        values[i] = properties[i].GetValue(item);
                        if (values[i] != null && values[i].GetType() == typeof(bool))
                        {
                            if ((bool)values[i])
                            {
                                values[i] = "是";
                            }
                            else
                            {
                                values[i] = "否";
                            }
                        }
                    }
                }

                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        /// <summary>
        /// Byte数组转换为字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string ByteToTwo(Byte[] bytes)
        {
            string strResult = string.Empty;
            string strTemp = string.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i != 0)
                {
                    strTemp = strTemp + ",";
                }
                strTemp = strTemp + bytes[i].ToString();
            }
            strResult = strTemp;
            return strResult;
        }

        #endregion

        #region 【DataTable转为Excel类型的Byte数组】

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dataTable">数据源</param>
        /// <param name="heading">工作簿Worksheet</param>
        /// <param name="showSrNo">是否显示行编号</param>
        /// <param name="columnsToTake">要导出的列</param>
        /// <returns></returns>
        private static byte[] ExportExcel(DataTable dataTable, string heading = "", bool showSrNo = false, Dictionary<string, string> columnsToTake = null)
        {
            byte[] result = null;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(string.Format("{0}Data", heading));
                int startRowFrom = string.IsNullOrEmpty(heading) ? 1 : 3;  //开始的行
                //是否显示行编号
                if (showSrNo)
                {
                    DataColumn dataColumn = dataTable.Columns.Add("#", typeof(int));
                    dataColumn.SetOrdinal(0);
                    int index = 1;
                    foreach (DataRow item in dataTable.Rows)
                    {
                        item[0] = index;
                        index++;
                    }
                }

                //Add Content Into the Excel File
                workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);
                // autofit width of cells with small content  
                int columnIndex = 1;
                foreach (DataColumn item in dataTable.Columns)
                {
                    ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                    int maxLength = columnCells.Max(cell => (cell.Value == null ? "" : cell.Value.ToString()).Count());
                    if (maxLength < 150)
                    {
                        workSheet.Column(columnIndex).AutoFit();
                    }
                    columnIndex++;
                }
                // format header - bold, yellow on black  
                using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                {
                    r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#DCE0E2"));

                    r.Style.Font.Bold = true;
                    r.Style.Font.Size = 12;
                    r.Style.Font.Name = "等线";
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                }

                // format cells - add borders  
                using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                {
                    r.Style.Font.Name = "等线";
                    r.Style.Font.Size = 11;
                    r.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                }

                //
                for (int i = 1; i < dataTable.Rows.Count; i++)
                {
                    if (i % 2 != 0)
                    {
                        using (ExcelRange r = workSheet.Cells[startRowFrom + i + 1, 1, startRowFrom + i + 1, dataTable.Columns.Count])
                        {
                            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#DCE0E2"));
                        }
                    }

                }

                // removed ignored columns  
                for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                {
                    if (i == 0 && showSrNo)
                    {
                        continue;
                    }
                    if (columnsToTake != null && !columnsToTake.Values.Contains(dataTable.Columns[i].ColumnName))
                    {
                        workSheet.DeleteColumn(i + 1);
                    }
                }

                if (!String.IsNullOrEmpty(heading))
                {
                    workSheet.Cells["A1"].Value = heading;
                    workSheet.Cells["A1"].Style.Font.Size = 20;

                    workSheet.InsertColumn(1, 1);
                    workSheet.InsertRow(1, 1);
                    workSheet.Column(1).Width = 5;
                }

                result = package.GetAsByteArray();

            }
            return result;
        }

        #endregion

        #region 【导出Excel】

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">数据源</param>
        /// <param name="heading">工作簿Worksheet</param>
        /// <param name="showSrNo">是否显示行编号</param>
        /// <param name="columnsToTake">要导出的列</param>
        /// <returns></returns>
        public static byte[] ExportExcel<T>(List<T> data, string heading = "", bool isShowSlNo = false, Dictionary<string, string> columnsToTake = null)
        {
            if (data.Count <= 0)
            {
                return new byte[] { new byte() };
            }
            return ExportExcel(ListToDataTable<T>(data, columnsToTake), heading, isShowSlNo, columnsToTake);
        }

        #endregion

    }
}
