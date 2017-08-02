using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;

namespace NamedPipeServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (NamedPipeServerStream server
               = new NamedPipeServerStream("testing"))
            {
                // 等待客户端连接
                server.WaitForConnection();

                // 接收来自客户端的数据并打印出来
                using (StreamReader sr = new StreamReader(server))
                {
                    string message = sr.ReadToEnd();
                    Console.WriteLine(message);
                }
            }

            // 按任意键退出程序
            Console.ReadKey();
        }
    }
}
