using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program2
    {
        //static void Main(string[] args)
        //{
        //    //var s = "http://item.YHD.com/1217836.html";

        //    //if (Uri.TryCreate(s, UriKind.Absolute, out Uri uri))
        //    //{
        //    //    var host = uri.Host;
        //    //}


        //    Test();

        //    Console.ReadKey();
        //}


        static void Test()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var pipe = new NamedPipeServerStream("Hello", PipeDirection.InOut, -1,
                        PipeTransmissionMode.Message, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
                    pipe.WaitForConnection();

                    using (pipe)
                    {
                        var data = new byte[1000];
                        int bytesRead = pipe.Read(data, 0, data.Length);
                        var message = Encoding.UTF8.GetString(data, 0, bytesRead);
                        Console.WriteLine("pipe connected. message: " + message);
                    }
                }

            }, TaskCreationOptions.LongRunning);

        }

    }


}
