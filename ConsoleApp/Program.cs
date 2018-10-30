using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var r1 = CExtern.runParser("cmd");
            var r11 = CExtern.runParser("cmd false");
            var r12 = CExtern.runParser("cmd 4");
            var r13 = CExtern.runParser("cmd \"str\"");
            //var r14 = CExtern.runParser("cmd 'str'"); --fix
            var r2 = CExtern.runParser("cmd cmd");
            var r3 = CExtern.runParser("cmd -p");
            var r4 = CExtern.runParser("cmd -p arg");
            var r5 = CExtern.runParser("cmd -p | cmd \"str\"");
            Console.ReadKey();
        }
    }
}
