using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program_Main3
    {
        static void Main()
        {
            object o = "asdf";

            var i = 0;

            try
            {
            i = (int)o;

            }
            catch (Exception ex)
            {

            }

        }

    }
}
