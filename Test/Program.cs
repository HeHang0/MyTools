using FileHelper;
using StringHelper;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new ExcelImportHelper("C:\\Users\\hehan\\Desktop\\水表项目信息导入模板.xlsx");
            a.Excute();
            Console.WriteLine();
            Console.Read();
        }
    }
}
