using DataBaseHelper;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var tables = new DBAnalyze("server=.;uid=sa;pwd=hiwits;database=GaoKe_HeatingV3;").Excute();
            bool isSoFirst = true;
            foreach (var table in tables)
            {
                if (isSoFirst)
                {
                    isSoFirst = false;
                    Console.WriteLine($" {new String('_', 99)}");
                }
                else
                {
                    Console.WriteLine($"|{new String('_', 40)}|{new String('_', 40)}|{new String('_', 17)}|");
                }
                bool isFirstRow = true;
                foreach (var cols in table.Value)
                {
                    Console.Write(isFirstRow ? $"|{table.Key}{new String(' ', 40 - table.Key.Length)}" : $"|{new String(' ', 40)}");
                    Console.Write($"|{cols.Key}{new String(' ', 40 - cols.Key.Length)}");
                    Console.Write($"|{cols.Value}{new String(' ', 17 - cols.Value.Length)}|");
                    if (isFirstRow)
                    {
                        isFirstRow = false;
                    }
                    foreach (var table1 in tables)
                    {
                        if (table.Key == table1.Key) continue;
                        foreach (var row in table1.Value)
                        {
                            if (row.Key == cols.Key && row.Key.ToUpper().EndsWith("ID"))
                            {
                                Console.Write($" {table1.Key},");
                            }
                        }
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine($"|{new String('_', 40)}|{new String('_', 40)}|{new String('_', 17)}|");
            Console.WriteLine();
            Console.Read();
        }
    }
}
