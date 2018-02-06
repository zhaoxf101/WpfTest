using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CheckUrl
{
    class Program
    {
        static void Main(string[] args)
        {
            //var files = Directory.GetFiles($@"D:\product\History\OEM4_一网推_7");

            //foreach (var file in files)
            //{
            //    Check(file);
            //}

            //Check(@"D:\product\History\OEM4_一网推_7\一网推_1.0.28.0_2018_01_25_12.28.exe");

            //Check(@"D:\product\Client\Bundle\Release\IMS.exe");
            //Check(@"D:\product\Client\Bundle\SetupProjectOEM4\content\Xyh.Crawlers.KeyWordsRank.dll");
            //Check(@"D:\product\Client\Bundle\Release\IMS云合景从客户端.exe");
            //Check(@"C:\Users\Worker\Downloads\ywt_1.0.28.0(1).exe");
        }


        static void Check(string fileName)
        {
            Debug.WriteLine("=================================================");
            Console.WriteLine("=================================================");

            Debug.WriteLine($"file: {fileName}");
            Console.WriteLine($"file: {fileName}");

            var data = new byte[1024];

            var builder = new StringBuilder();
            var length = 0;

            using (var stream = File.OpenRead(fileName))
            {
                while ((length = stream.Read(data, 0, data.Length)) > 0)
                {
                    for (int i = 0; i < length; i++)
                    {
                        if (data[i] > 31 && data[i] < 127)
                        {
                            builder.Append((char)data[i]);
                        }
                        else
                        {
                            builder.Append("?");
                        }
                    }
                }
                var matches = new[] { ".COM", ".CN" };

                var original = builder.ToString();
                var result = original.ToUpper();

                foreach (var item in matches)
                {
                    var index = -1;
                    while ((index = result.IndexOf(item, index + 1)) != -1)
                    {
                        var startIndex = index - 20 < 0 ? 0 : index - 20;
                        var length2 = startIndex + 40 > result.Length ? result.Length - startIndex : 40;

                        var checkIndex = index - 2;
                        if (checkIndex < 0 || original[checkIndex] == '?' || original[checkIndex + 1] == '?')
                        {
                            continue;
                        }

                        var substring = original.Substring(startIndex, length2);
                        if (substring.Contains(".wosign.com") || substring.Contains("Windows.Common") || substring.Contains("ocsp-certum.com"))
                        {
                            continue;
                        }
                        Debug.WriteLine(substring);
                        Console.WriteLine(substring);
                    }


                }

            }

        }
    }


}
