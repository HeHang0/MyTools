using StringHelper;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string a = "abcsklajdlkabcaskjaklabc";
            var b = RegexHelper.MatchesGroups(a, "a(b)(c)");
            Console.WriteLine(b);
            Console.Read();
        }
    }
}
