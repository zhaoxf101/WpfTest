using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using MarketingPlatform.Client;

namespace ConsoleApp1
{
    public class PerformanceRecord
    {
        public int CPUUsage { get; set; }

        public int MemoryUsage { get; set; }

        public int MemoryAvailable { get; set; }
    }

    class Program
    {


        public static byte[] Compress(byte[] data)
        {
            var output = new MemoryStream();
            using (GZipStream zipStream = new GZipStream(output, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        public static byte[] Decompress(byte[] data)
        {
            var output = new MemoryStream(data);
            using (GZipStream zipStream = new GZipStream(output, CompressionMode.Decompress))
            {
                zipStream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }


        static double GetSimi(string string1, string string2)
        {
            double stepPercentage = 0.5;

            // 如果有一个串为空，另一个不为空，不相似。
            // 如果两个串为空，相等。
            int length1 = 0;
            int length2 = 0;
            if (string.IsNullOrWhiteSpace(string1))
            {
                length1 = 0;
            }
            else
            {
                length1 = string1.Length;
            }

            if (string.IsNullOrWhiteSpace(string2))
            {
                length2 = 0;
            }
            else
            {
                length2 = string2.Length;
            }

            if (length1 == 0 || length2 == 0)
            {
                if (length1 == length2)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            if (length1 > length2)
            {
                // swap the input strings to consume less memory
                var tmp = string1;
                string1 = string2;
                string2 = tmp;
                length1 = length2;
                length2 = string2.Length;
            }

            var step = (int)(length1 * stepPercentage);
            if (step < 2)
            {
                step = 2;
            }

            var index = 0;
            var minum = int.MaxValue;
            while (index <= length2 - length1)
            {
                minum = Math.Min(minum, LevenshteinDistance.GetLevenshteinDistance(string1, string2.Substring(index, length1)));
                index += step;
            }

            return (double)minum / length1;
        }


        public static double GetTextSimilarity(string string1, string string2)
        {
            var distance = LevenshteinDistance.GetLevenshteinDistance(string1, string2);
            var length = Math.Max(string1?.Length ?? 0, string2?.Length ?? 0);

            if (length == 0)
            {
                return 1;
            }
            else
            {
                return 1 - (double)distance / length;
            }
        }

        public static int GetTextLengthWithTrimedAndNewLineRemoved(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0;
            }
            else
            {
                var count = 0;
                var countWhiteSpace = 0;
                bool started = false;
                foreach (var c in text)
                {
                    if (char.IsWhiteSpace(c))
                    {
                        if (started && c != '\r' && c != '\n')
                        {
                            countWhiteSpace++;
                        }
                    }
                    else
                    {
                        if (started)
                        {
                            count += countWhiteSpace + 1;
                            countWhiteSpace = 0;
                        }
                        else
                        {
                            started = true;
                            count++;
                        }
                    }
                }
                return count;
            }
        }

        public static int GetTextLengthWithTrimed(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0;
            }
            else
            {
                var count = 0;
                var countWhiteSpace = 0;
                bool started = false;
                foreach (var c in text)
                {
                    if (char.IsWhiteSpace(c))
                    {
                        if (started)
                        {
                            countWhiteSpace++;
                        }
                    }
                    else
                    {
                        if (started)
                        {
                            count += countWhiteSpace + 1;
                            countWhiteSpace = 0;
                        }
                        else
                        {
                            started = true;
                            count++;
                        }
                    }
                }
                return count;
            }
        }

        static void Main(string[] args)
        {

            //var string1 = "";
            //var string2 = "";

            //            string string1 = "121313";
            //            string string2 = @"单人二级A2生物安全柜BSC-1100IIA2-X
            //国内销量,适用单人操作,30%外排70%循环
            //一、技术参数
            //1、安全柜基本参数：
            //（1）分类：A2型，30%外排，70%循环
            //（2）外部尺寸（L×D×H）1100mm×750mm×2250mm；
            //（3）内部尺寸（L×D×H）940mm×600mm×660mm。
            //（4）台面距离地面高度：750mm（尺寸可根据要求订制修改）
            //（5）风速：平均下降风速：0.33±0.025m/s；平均吸入口风速0.53±0.025m/s
            //（6）系统排风总量：360m3/h
            //（7）额定功率：1100W（包含操作区插座负载500W）
            //（8）噪音等级：≤65dB（A）
            //（9）照明：≥1000lx
            //（10）过滤效率:送风和排风过滤器均采用世界知名品牌的硼硅酸盐玻璃纤维材质的HEPA（ULPA）高效过滤器，对0.3μm（0.12）颗粒过滤效率≥99.999%（99.9995%）
            //（11）注册证号：国械注册20163540412
            //（12）产品标准：YZB/国6408-2012 （YY0569-2011 II级生物安全柜）
            //（13）重量：毛重243KG 净重227KG
            //（14）使用人数：单人
            //2、生物安全性：
            //（1）人员安全性：用碘化钾（KI）法测试，前窗操作口的保护因子应不小于1×105
            //（2）产品安全性：菌落数≤5CFU/次 
            //（3）交叉污染安全性：菌落数≤2CFU/次
            //二、结构功能特点：
            //1、山东博科生物产业有限公司是中华人民共和国医药行业YY0569-2011生物安全柜产品标准的主要起草单位；
            //2、柜体采用10°倾斜角设计，符合人体工程学原理，视角更大，操作方便且更加人性化；  
            //3、安全柜裸露工作区三侧壁板采用优质304#不锈钢一体化结构，内部可清洗部位采用8mm大圆角处理，不留死角，易于清洁；
            //4、工作区采用四面（左右二侧、后部、底部）负压环绕设计工作区内，保护性更好、更安全；
            //5、工作台面材质为优质304#不锈钢，采用盆状式设计，即使实验有废液溢出，也不会流入积液槽中，便于清理；
            //6、福马脚轮设计：脚轮与支架一体化设计，安全柜即可通过脚轮安全移动，也可以通过调节脚轮支脚进行固定和调平；
            //7、柜体和支架可分离，支架高度可根据实际情况订制修改；
            //8、合理的结构设计：安全柜过滤器和风机的维修、更换，都可在安全柜的前侧进行，更加方便、快捷。
            //9、前窗玻璃采用双层夹胶安全玻璃；即使玻璃破损，也不会伤人，并且生物安全柜还能正常工作，直到实验结束，更好的保护了人员及实验的安全；
            //10、高亮度LCD显示屏,实时动态显示操作区的下降气流流速和流入气流流速，显示安全柜的整体运行时间，UV灯的运行时间，操作区的温度和湿度，送风和排风过滤器的阻力，显示过滤器的使用时间并由条码显示过滤器的使用寿命，条码全部点亮是过滤器寿命到期，运行状态全部显示,一目了然；
            //11、电动控制前窗玻璃门，可同时采用脚踏控制、按键控制或遥控控制，玻璃门升降到安全操作高度时，自动停止升降，使操作更加方便；且玻璃门升降时不用直接接触玻璃，使实验人员更安全；
            //12、遥控控制：安全柜的所有按键操作，都可通过遥控控制实现，使安全柜的使用更加快捷方便；且遥控器的使用，大大减少了使用者与安全柜的直接接触，更加保护了使用者的人身安全；
            // 13、具有预约定时功能，能自动设定安全柜定时开机、关机及紫外灯消毒时间，大大节省了工作时间，提高了工作效率；济南鑫贝西生物技术有限公司（简称鑫贝西公司）成立于1999年9月，注册地为济南市天桥区，注册资本为2001万元。济南鑫贝西公司致力于为客户在临床诊断、实验室、空气安全等领域提供系统、专业、创新的解决方案。济南鑫贝西公司拥有专利28项，其中发明专利6项。是中国早生产生物安全柜的厂家之一，是中国家通过国家食品药品监督检测并获得生物安全柜注册证的厂家。公司现为《Ⅱ级生物安全柜YY0569-2011》起草单位，是全球第六家、亚济南鑫贝西洲第二家、中国家通过美国NSF生物安全柜生产的企业，同时也是目前中国国内家同时通过美国NSF49、欧盟EN12469和中国CFDA认证的企业。
            //公司于2004年10月在济南启用我国目前的生物安全柜生产基地，占地面积达2万平方米。并坚持实施“走出去”战略，加强国际合作，与美国、德国、加拿大等国外知名企业建立了战略合作关系，吸收全球医疗先进技术，提高自主创新能力。公司产品主要零部件均进口于美国、意大利、德国等发达国家，采用意大利MARIO公司开卷机生产线、日本AMADA株式数控机床、意大利GASPARINI公司的液压折弯机、瑞士GEMA公司静电喷粉设备，全部使用计算机控制，自动化程度非常高，操作方便。
            //公司坚持科学管理，贯彻实施卓越绩效评价准则，建立大质量概念下的质量标准体系，持续改进经营管理模式，提高顾客满意度，提升竞争力。推进六西格玛管理，引进先进管理思想和方法。在行业打造了完善的信息化管理平台，提高了决策水平、管理效率和质量。坚持“满足并努力超越顾客的需求”的质量方针，追求顾客满意。在行业已通过ISO9001：2008质量管理体系认证，ISO13485:2003质量管理体系认证，ISO14001:2004环境管理体系认证。生产的产品已获得欧美的认可，通过了欧盟的CE认证，美国FDA认证。";

            //            string1 = "67##############";
            //            string2 = "67!!!!!!!!!!!!!!!!!!!!!!!";


            //            var distance = GetTextSimilarity(string1, string2);

            //            var d = 1.5m;

            //            var dd = decimal.Round(d, 0, MidpointRounding.AwayFromZero);


            var updateResult = Update.StartUpdate();

            Console.WriteLine();

            //Available MBytes

            //% Committed Bytes In Use

            //Available KBytes

            //_Total


            //PerformanceCounterFun("Processor", "% Processor Time" ,"_Total");

            //var items = new[] { "Memory", "PhysicalDisk", "Processor Information",
            //    "Storage Spaces Virtual Disk", "System", "Process" };

            //GetInstanceNameListANDCounterNameList("Processor Information");

            //PerformanceCounterFun("System", "System Up Time");

            //PerformanceCounterFun("Processor Information", "_Total", "% Processor Time");




            //PerformanceCounterFun("Memory", "_Total", "");

            //PerformanceCounterFun("Memory", "% Committed Bytes In Use", "");


            //PerformanceCounterFun("Memory", "% Committed Bytes In Use", "");
            //PerformanceCounterFun("Memory", "% Committed Bytes In Use", "");
            //PerformanceCounterFun("Memory", "% Committed Bytes In Use", "");
            //PerformanceCounterFun("Memory", "% Committed Bytes In Use", "");


            //PerformanceCounterFun("Processor Information", "% Processor Time", "_Total");
            //PerformanceCounterFun("Memory", "Available MBytes");
            //PerformanceCounterFun("Memory", "% Committed Bytes In Use");

            //PerformanceCounter pc1 = new PerformanceCounter("Memory", "Available MBytes");
            //PerformanceCounter pc2 = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            //PerformanceCounter pc3 = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");

            //while (true)
            //{
            //    Console.WriteLine(pc1.NextValue() + "MBytes " + pc2.NextValue() + "% " + pc3.NextValue() + "%");
            //    Thread.Sleep(1000); // wait for 1 second   
            //}

            //var list = new List<int>();

            //var item = list.FirstOrDefault(); 



            //var str = "https";

            //str = str.Remove("http".Length, 1);

            //Console.ReadKey();


            //var str = "{\"msg\":\"{\\\"CPUUsage\\\":0,\\\"MemoryUsage\\\":0,\\\"MemoryAvailable\\\":0}\"}";

            //dynamic data = JsonConvert.DeserializeObject(str);
            //var data2 = data.msg.Value;



            //PerformanceCounterFun("Processor Information", "% Processor Performance", "_Total");


            //GetCategoryNameList();
            //foreach (var item in items)
            //{
            //    Console.WriteLine();
            //    Debug.WriteLine("");

            //    Console.WriteLine(item);
            //    Debug.WriteLine(item);
            //    GetInstanceNameListANDCounterNameList(item);
            //}

            //新建一个Stopwatch变量用来统计程序运行时间

            //PerformanceUtil.ReportRecord();


            //PerformanceUtil.StartRecord();

            //Console.ReadLine();
            //Console.ReadLine();
            //Console.ReadLine();
            //Console.ReadLine();
            //Console.ReadLine();



            Console.ReadLine();
        }
        static void M()
        {
            Stopwatch watch = Stopwatch.StartNew();
            //获取本机运行的所有进程ID和进程名,并输出哥进程所使用的工作集和私有工作集
            foreach (Process ps in Process.GetProcesses())
            {
                PerformanceCounter pf1 = new PerformanceCounter("Process", "Working Set - Private", ps.ProcessName);
                PerformanceCounter pf2 = new PerformanceCounter("Process", "Working Set", ps.ProcessName);
                Console.WriteLine("{0}:{1} {2:N}KB", ps.ProcessName, "工作集(进程类)", ps.WorkingSet64 / 1024);
                Console.WriteLine("{0}:{1} {2:N}KB", ps.ProcessName, "工作集    ", pf2.NextValue() / 1024);
                //私有工作集
                Console.WriteLine("{0}:{1} {2:N}KB", ps.ProcessName, "私有工作集  ", pf1.NextValue());
            }
            watch.Stop();
            Console.WriteLine(watch.Elapsed);
        }


        public static void GetCategoryNameList()
        {
            PerformanceCounterCategory[] myCat2;
            myCat2 = PerformanceCounterCategory.GetCategories();
            for (int i = 0; i < myCat2.Length; i++)
            {
                Console.WriteLine(myCat2[i].CategoryName);
            }
        }

        public static void GetInstanceNameListANDCounterNameList(string categoryName)
        {
            string[] instanceNames;
            var counters = new List<PerformanceCounter>();
            var mycat = new PerformanceCounterCategory(categoryName);
            try
            {
                instanceNames = mycat.GetInstanceNames();
                if (instanceNames.Length == 0)
                {
                    counters.AddRange(mycat.GetCounters());
                }
                else
                {
                    Console.WriteLine("instance");
                    Debug.WriteLine("instance");
                    Console.WriteLine("-----------------------");
                    Debug.WriteLine("-----------------------");

                    for (int i = 0; i < instanceNames.Length; i++)
                    {
                        counters.AddRange(mycat.GetCounters(instanceNames[i]));
                    }
                }

                for (int i = 0; i < instanceNames.Length; i++)
                {
                    Console.WriteLine(instanceNames[i]);
                    Debug.WriteLine(instanceNames[i]);
                }

                Console.WriteLine("++++++++++++++++++++++++++++++++++++++");
                Debug.WriteLine("++++++++++++++++++++++++++++++++++++++");

                foreach (PerformanceCounter counter in counters)
                {
                    Console.WriteLine(counter.CounterName);
                    Debug.WriteLine(counter.CounterName);
                }

                Console.WriteLine();
                Debug.WriteLine("");
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to list the counters for this category");
                Debug.WriteLine("Unable to list the counters for this category");
            }
        }

        private static void PerformanceCounterFun(string CategoryName, string CounterName, string InstanceName = "")
        {


            PerformanceCounter pc = new PerformanceCounter(CategoryName, CounterName, InstanceName);
            while (true)
            {
                Thread.Sleep(1000); // wait for 1 second   
                float cpuLoad = pc.NextValue();
                //Console.WriteLine("CPU load = " + cpuLoad + " %.");
                Console.WriteLine(cpuLoad);
            }
        }


        //private static void PerformanceCounterFun(string CategoryName, string InstanceName, string CounterName)
        //{
        //    PerformanceCounter pc = new PerformanceCounter(CategoryName, CounterName, InstanceName);
        //    while (true)
        //    {
        //        Thread.Sleep(1000); // wait for 1 second   
        //        float cpuLoad = pc.NextValue();
        //        //Console.WriteLine("CPU load = " + cpuLoad + " %.");
        //        Console.WriteLine(cpuLoad);
        //    }
        //}

    }
}
