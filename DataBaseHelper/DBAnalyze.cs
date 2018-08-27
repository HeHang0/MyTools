using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseHelper
{
    /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
    *                                                                                                                                                *
    *            var tables = new DBAnalyze("server=.;uid=sa;pwd=***;database=***;").Excute();                                                       *
    *            bool isSoFirst = true;                                                                                                              *
    *            foreach (var table in tables)                                                                                                       *
    *            {                                                                                                                                   *
    *                if (isSoFirst)                                                                                                                  *
    *                {                                                                                                                               *
    *                    isSoFirst = false;                                                                                                          *
    *                    Console.WriteLine($" {new String('_', 99)}");                                                                               *
    *                }                                                                                                                               *
    *                else                                                                                                                            *
    *                {                                                                                                                               *
    *                    Console.WriteLine($"|{new String('_', 40)}|{new String('_', 40)}|{new String('_', 17)}|");                                  *
    *                }                                                                                                                               *
    *                bool isFirstRow = true;                                                                                                         *
    *                foreach (var cols in table.Value)                                                                                               *
    *                {                                                                                                                               *
    *                    Console.Write(isFirstRow ? $"|{table.Key}{new String(' ', 40 - table.Key.Length)}" : $"|{new String(' ', 40)}");            *
    *                    Console.Write($"|{cols.Key}{new String(' ', 40 - cols.Key.Length)}");                                                       *
    *                    Console.Write($"|{cols.Value}{new String(' ', 17 - cols.Value.Length)}|");                                                  *
    *                    if (isFirstRow)                                                                                                             *
    *                    {                                                                                                                           *
    *                        isFirstRow = false;                                                                                                     *
    *                    }                                                                                                                           * 
    *                    foreach (var table1 in tables)                                                                                              *
    *                    {                                                                                                                           *
    *                        if (table.Key == table1.Key) continue;                                                                                  *
    *                        foreach (var row in table1.Value)                                                                                       *
    *                        {                                                                                                                       *
    *                            if (row.Key == cols.Key && row.Key.ToUpper().EndsWith("ID"))                                                        *
    *                            {                                                                                                                   *
    *                                Console.Write($" {table1.Key},");                                                                               *
    *                            }                                                                                                                   *
    *                        }                                                                                                                       *
    *                    }                                                                                                                           *
    *                    Console.WriteLine();                                                                                                        *
    *                }                                                                                                                               *
    *            }                                                                                                                                   *
    *            Console.WriteLine($"|{new String('_', 40)}|{new String('_', 40)}|{new String('_', 17)}|");                                          *
    *                                                                                                                                                *
    * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
    public class DBAnalyze
    {
        private string connectionString;

        public DBAnalyze(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Dictionary<string, Dictionary<string, string>> Excute()
        {
            DataRowCollection rows = new DBHelper(connectionString).ExecuteDataTable(querySqlString).Rows;
            Dictionary<string, Dictionary<string, string>> tables = new Dictionary<string, Dictionary<string, string>>();
            foreach (DataRow item in rows)
            {
                if (!tables.ContainsKey(item["TABLE_NAME"].ToString()))
                {
                    tables.Add(item["TABLE_NAME"].ToString(), new Dictionary<string, string>());
                }
                Dictionary<string, string> table = tables[item["TABLE_NAME"].ToString()];
                table.Add(item["COLUMN_NAME"].ToString(), item["DATA_TYPE"].ToString());
            }
            return tables;
        }

        private static string querySqlString = @"
            SELECT 
                TABLE_NAME,
                COLUMN_NAME,
                DATA_TYPE
            FROM
                INFORMATION_SCHEMA.COLUMNS 
            ORDER BY TABLE_NAME
        ";
    }
}
