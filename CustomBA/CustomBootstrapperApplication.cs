using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Windows;
using System.Diagnostics;

namespace CustomBA
{
    public class CustomBootstrapperApplication : BootstrapperApplication
    {
        protected override void Run()
        {
            //Debug.Listeners.Add(new TextWriterTraceListener(new FileStream("C:\\setup_light.log", FileMode.Append, FileAccess.Write)));

            //MessageBox.Show("waiting!");
           
            var window = new MainWindow(this);
            window.ShowDialog();

            Engine.Quit(0);
        }
    }
}
