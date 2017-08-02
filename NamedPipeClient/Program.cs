using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;

namespace NamedPipeClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using (NamedPipeClientStream client
                = new NamedPipeClientStream("testing"))
            {
                // 连接命名管道服务器，管道名称为testing
                client.Connect();

                // 向服务器发送一个字符串
                using (StreamWriter sw = new StreamWriter(client))
                {
                    sw.WriteLine("来自客户端的信息");
                    sw.Flush();
                }
            }

            // 按任意键退出程序
            Console.ReadKey();
        }
    }
}
