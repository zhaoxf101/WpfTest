using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Windows;

namespace CustomBA
{
    public class CustomBootstrapperApplication : BootstrapperApplication
    {
        public static Dispatcher Dispatcher { get; set; }

        protected override void Run()
        {
            //MessageBox.Show("waiting!");


            var window = new MainWindow();
            window.ShowDialog();

            Engine.Quit(0);
        }

    }
}
