using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;

namespace CustomBA
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        enum Step
        {
            QuitForNewVersionInstalled,
            Install,
            Uninstall,
            Process,
            Complete
        }

        CustomBootstrapperApplication _app;
        string _installFolder;

        RelatedOperation _relatedOperation;
        LaunchAction _action;
        IntPtr _windowPtr;

        bool _isSuccess = true;
        bool _isExecuting = false;

        public MainWindow(CustomBootstrapperApplication app)
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            Title = $"{SysParam.ProductName} - 安装程序";
            TxtTitle.Text = $"{SysParam.ProductName}1.0正式版";

            _app = app;
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _installFolder = Path.Combine(folder, SysParam.InstallFolder);

            //if (Environment.Is64BitOperatingSystem)
            //{
            //}
            //else
            //{
            //    _installFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            //}
        }

        internal IntPtr WindowPtr { get { return _windowPtr; } }

        internal string InstallFolder { get { return _installFolder; } }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_windowPtr == IntPtr.Zero)
            {
                _windowPtr = new WindowInteropHelper(this).Handle;
            }
            Trace.WriteLine($"_windowPtr: {_windowPtr}");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isExecuting)
            {
                e.Cancel = true;
            }
        }

        internal void Initialize(RelatedOperation relatedOperation, LaunchAction action)
        {
            _relatedOperation = relatedOperation;
            _action = action;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_action == LaunchAction.Uninstall)
                {
                    MoveTo(Step.Uninstall);
                }
                else if (_action == LaunchAction.Unknown)
                {
                    MoveTo(Step.QuitForNewVersionInstalled);
                }
                else
                {
                    MoveTo(Step.Install);
                }
            }));
        }

        void MoveTo(Step step)
        {
            switch (step)
            {
                case Step.Install:
                    SetInstallFolder(_installFolder);

                    _isExecuting = false;
                    BtnClose.IsEnabled = true;

                    GridInstall.Visibility = Visibility.Visible;
                    GridUninstall.Visibility = Visibility.Collapsed;
                    GridProgress.Visibility = Visibility.Collapsed;
                    GridComplete.Visibility = Visibility.Collapsed;
                    ImageProgressBackground.Visibility = Visibility.Visible;
                    UninstallTextWrapper.Visibility = Visibility.Collapsed;
                    TxtProgress.Text = "正在安装，请稍候...";
                    break;
                case Step.Uninstall:
                    _isExecuting = false;
                    BtnClose.IsEnabled = true;

                    GridInstall.Visibility = Visibility.Collapsed;
                    GridUninstall.Visibility = Visibility.Visible;
                    GridProgress.Visibility = Visibility.Collapsed;
                    GridComplete.Visibility = Visibility.Collapsed;
                    ImageProgressBackground.Visibility = Visibility.Collapsed;
                    UninstallTextWrapper.Visibility = Visibility.Visible;
                    TxtProgress.Text = "正在卸载，请稍候...";
                    break;
                case Step.Process:
                    _isExecuting = true;
                    BtnClose.IsEnabled = false;

                    GridInstall.Visibility = Visibility.Collapsed;
                    GridUninstall.Visibility = Visibility.Collapsed;
                    GridProgress.Visibility = Visibility.Visible;
                    GridComplete.Visibility = Visibility.Collapsed;
                    break;
                case Step.Complete:
                    _isExecuting = false;
                    BtnClose.IsEnabled = true;

                    var message = "";
                    switch (_action)
                    {
                        case LaunchAction.Uninstall:
                            if (_isSuccess)
                            {
                                message = "卸载完成，期待再见~";
                                ImageSuccess.Visibility = Visibility.Collapsed;
                                ImageFailure.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                message = "卸载失败！";
                                ImageSuccess.Visibility = Visibility.Collapsed;
                                ImageFailure.Visibility = Visibility.Visible;
                            }
                            CheckBoxAutoStart.Visibility = Visibility.Collapsed;
                            BtnOk.Content = "完成";
                            break;
                        case LaunchAction.Install:
                            if (_isSuccess)
                            {
                                message = "安装成功！";
                                ImageSuccess.Visibility = Visibility.Visible;
                                ImageFailure.Visibility = Visibility.Collapsed;
                                CheckBoxAutoStart.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ImageSuccess.Visibility = Visibility.Collapsed;
                                ImageFailure.Visibility = Visibility.Visible;
                                message = "安装失败！";
                            }
                            BtnOk.Content = "完成安装";
                            break;
                    }

                    TxtComplete.Text = message;
                    GridInstall.Visibility = Visibility.Collapsed;
                    GridUninstall.Visibility = Visibility.Collapsed;
                    GridProgress.Visibility = Visibility.Collapsed;
                    GridComplete.Visibility = Visibility.Visible;
                    break;

                case Step.QuitForNewVersionInstalled:
                    _isExecuting = false;
                    BtnClose.IsEnabled = true;

                    ImageSuccess.Visibility = Visibility.Collapsed;
                    ImageFailure.Visibility = Visibility.Visible;

                    BtnOk.Content = "完成";

                    TxtComplete.Text = "已安装更高版本";
                    GridInstall.Visibility = Visibility.Collapsed;
                    GridUninstall.Visibility = Visibility.Collapsed;
                    GridProgress.Visibility = Visibility.Collapsed;
                    GridComplete.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        internal void ReportProgress(int percentage, string message)
        {
            Dispatcher.BeginInvoke(new Action(() =>
             {
                 ProgressBarMain.Value = percentage;
                 //TxtProgress.Text = message;
             }));
        }

        internal void ApplyComplete(bool isSuccess)
        {
            _isSuccess = isSuccess;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                MoveTo(Step.Complete);
            }));
        }

        void SetInstallFolder(string folder)
        {
            _installFolder = folder;
            TxtInstallFolder.Text = _installFolder;
            _app.Engine.StringVariables["InstallFolder"] = _installFolder;
        }

        private void BtnInstall_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxPolicy.IsChecked == true)
            {
                MoveTo(Step.Process);
                _app.Engine.Plan(_action);
            }
            else
            {
                var dialog = new DialogMessage(MessageBoxButton.OK, MessageBoxImage.Asterisk, "请阅读《用户协议》！", "", false)
                {
                    Owner = this
                };
                dialog.ShowDialog();
            }
        }

        private void BtnChangeInstallFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = _installFolder
            };

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SetInstallFolder(folderBrowserDialog.SelectedPath);
            }
        }

        private void BtnUninstall_Click(object sender, RoutedEventArgs e)
        {
            MoveTo(Step.Process);
            _app.Engine.Plan(_action);
        }

        private void BtnUninstallBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (_action == LaunchAction.Install && _isSuccess)
            {
                try
                {
                    var fileName = Path.Combine(_installFolder, "MarketingPlatForm.Client.exe");
                    if (CheckBoxAutoStart.IsChecked == true)
                    {
                        Util.RegisterAutoStart(fileName, $"{SysParam.InternalName}_MarketingPlatform.Client", true);
                    }

                    Process.Start(fileName);
                }
                catch
                {

                }
            }
            Close();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void HyperlinkPolicy_Click(object sender, RoutedEventArgs e)
        {
            //if (!string.IsNullOrEmpty(SysParam.PolicyLink))
            //{
            //    Process.Start(SysParam.PolicyLink);
            //}

            var dialog = new DialogPolicy
            {
                Owner = this
            };
            var result = dialog.ShowDialog();
            if (result == true)
            {
                CheckBoxPolicy.IsChecked = true;
            }
        }

        private void HyperlinkInstallFolder_Click(object sender, RoutedEventArgs e)
        {
            GridInstallFolder.Visibility = Visibility.Visible;
        }

        private void BtnBackInstallFolder_Click(object sender, RoutedEventArgs e)
        {
            GridInstallFolder.Visibility = Visibility.Collapsed;
        }

        private void BtnOkInstallFolder_Click(object sender, RoutedEventArgs e)
        {
            var folder = TxtInstallFolder.Text;
            try
            {
                var driveName = Path.GetPathRoot(Path.GetFullPath(folder));
                var drive = DriveInfo.GetDrives().Where(p => p.DriveType == DriveType.Fixed && p.IsReady == true &&
                    string.Equals(p.Name, driveName, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
                if (drive == null)
                {
                    System.Windows.Forms.MessageBox.Show("指定的目录不可用！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("指定的目录不可用！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SetInstallFolder(TxtInstallFolder.Text);
            BtnInstall_Click(sender, e);
        }
    }
}
