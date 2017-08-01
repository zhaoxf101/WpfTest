using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfTestApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();



        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var installer = new UpdateService.ProjectInstaller();
            var state = new Dictionary<string, object>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var context = new InstallContext();
            context.Parameters["assemblypath"] = @"D:\Repos\WpfTest\WpfTestApp\bin\Debug\UpdateService.exe";
            //installer.ServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            //installer.ServiceProcessInstaller.Install(state);


            installer.ServiceInstaller.Context = context;


            var service = ServiceController.GetServices().Where(p => p.ServiceName == "UpdateService").SingleOrDefault();
            if (service == null)
            {
                installer.ServiceInstaller.Install(state);
                MessageBox.Show("Install OK.");
            }
            else
            {
                installer.ServiceInstaller.Uninstall(null);
                MessageBox.Show("Uninstall ok.");
            }


            //installer.ServiceProcessInstaller.Uninstall(state);

        }
    }
}
