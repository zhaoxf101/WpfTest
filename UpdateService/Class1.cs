using System;
using System.Threading;

class Program
{
    static void Main2(string[] args)
    {
        new Thread(A).Start();
        new Thread(B).Start();
        new Thread(C).Start();
        Console.ReadLine();
    }

    static object lockObj = new object();

    static void A()
    {
        lock (lockObj)
        //进入就绪队列 
        {
            Thread.Sleep(1000);
            Monitor.Pulse(lockObj);
            Monitor.Wait(lockObj);
            //自我流放到等待队列 
        }
        Console.WriteLine("A exit...");
    }

    static void B()
    {
        Thread.Sleep(500);

        lock (lockObj)
        //进入就绪队列 
        {
            Monitor.Pulse(lockObj);
        }
        Console.WriteLine("B exit...");
    }
    static void C()
    {
        Thread.Sleep(800);
        lock (lockObj)
        //进入就绪队列 
        {
        }
        Console.WriteLine("C exit...");
    }
}

