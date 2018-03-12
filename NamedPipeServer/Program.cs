using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamedPipeServer
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {

                try
                {
                    PipeConnection.Disconnect();

                    PipeConnection.Connect();

                    stringrequest = PipeConnection.Read();

                    if (!string.IsNullOrEmpty(request))
                    {

                        Console.WriteLine("get:" + request);

                        PipeConnection.Write("get:" + request);

                        if (request.ToLower() == "break") break;
                    }

                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                    break;

                }
            }
            PipeConnection.Dispose();

            Console.Write("pressanykeytoexit..");
            Console.Read();
        }
    }
}
