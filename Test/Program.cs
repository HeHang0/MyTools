using StringHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string a = "abcsklajdlkabcaskjaklabc";
            var b = RegexHelper.Matches(a, "abc");
            Console.WriteLine(b.Count);
            Console.Read();
        }
    }
}
