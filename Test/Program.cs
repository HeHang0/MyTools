using FileHelper;
using StringHelper;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new ExcelImportHelper("C:\\Users\\hehan\\Desktop\\水表项目信息导入模板.xlsx").Excute();
            foreach (var item in a)
            {
                for (int i = 0; i < item.Count; i++)
                {
                    Console.Write(item[i] + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.Read();
        }
    }
}
