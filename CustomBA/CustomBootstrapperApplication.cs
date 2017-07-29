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
            MessageBox.Show("waiting!");

            Dispatcher.Run();

            Engine.Quit(0);

        }

        public static void Plan(LaunchAction action)
        {
            
        }



        public static void PlanLayout()
        {
            //// Either default or set the layout directory
            //if (String.IsNullOrEmpty(Model.Command.LayoutDirectory))
            //{
            //    Model.LayoutDirectory = Directory.GetCurrentDirectory();

            //    // Ask the user for layout folder if one wasn't provided and we're in full UI mode
            //    if (Model.Command.Display == Display.Full)
            //    {
            //        Dispatcher.Invoke((Action)delegate ()
            //        {
            //            WinForms.FolderBrowserDialog browserDialog = new WinForms.FolderBrowserDialog();
            //            browserDialog.RootFolder = Environment.SpecialFolder.MyComputer;

            //            // Default to the current directory.
            //            browserDialog.SelectedPath = Model.LayoutDirectory;
            //            WinForms.DialogResult result = browserDialog.ShowDialog();

            //            if (WinForms.DialogResult.OK == result)
            //            {
            //                Model.LayoutDirectory = browserDialog.SelectedPath;
            //                Plan(Model.Command.Action);
            //            }
            //            else
            //            {
            //                View.Close();
            //            }
            //        }
            //        );
            //    }
            //}
            //else
            //{
            //    Model.LayoutDirectory = Model.Command.LayoutDirectory;
            //    Plan(Model.Command.Action);
            //}
        }
    }
}
