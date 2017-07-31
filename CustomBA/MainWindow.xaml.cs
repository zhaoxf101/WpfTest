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
            Install,
            Repair,
            Progress,
            Complete
        }

        CustomBootstrapperApplication _app;
        string _installFolder;

        LaunchAction _action;
        IntPtr _windowPtr;

        int _progress;
        string _message;

        bool _isSuccess = true;

        public MainWindow(CustomBootstrapperApplication app)
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;

            if (Environment.Is64BitOperatingSystem)
            {
                _installFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            }
            else
            {
                _installFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            }

            _app = app;

            app.DetectPackageComplete += CustomBootstrapperApplication_DetectPackageComplete;
            app.PlanComplete += CustomBootstrapperApplication_PlanComplete;
            app.ExecuteMsiMessage += CustomBootstrapperApplication_ExecuteMsiMessage;
            app.ExecuteProgress += CustomBootstrapperApplication_ExecuteProgress;
            app.Progress += CustomBootstrapperApplication_Progress;
            app.PlanBegin += CustomBootstrapperApplication_PlanBegin;
            app.CacheAcquireProgress += CustomBootstrapperApplication_CacheAcquireProgress;

            app.PlanPackageComplete += CustomBootstrapperApplication_PlanPackageComplete;
            app.CacheComplete += CustomBootstrapperApplication_CacheComplete;

            app.ApplyBegin += CustomBootstrapperApplication_ApplyBegin;
            app.ApplyComplete += CustomBootstrapperApplication_ApplyComplete;
            app.ExecutePackageBegin += CustomBootstrapperApplication_ExecutePackageBegin;
            app.ExecutePackageComplete += CustomBootstrapperApplication_ExecutePackageComplete;

            app.Engine.Detect();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _windowPtr = new WindowInteropHelper(this).Handle;
        }

        void Initialize()
        {
            if (_action == LaunchAction.Install)
            {
                MoveTo(Step.Install);
            }
            else
            {
                MoveTo(Step.Repair);
            }
        }

        void MoveTo(Step step)
        {
            switch (step)
            {
                case Step.Install:
                    GridInstall.Visibility = Visibility.Visible;
                    GridRepair.Visibility = Visibility.Collapsed;
                    GridProgress.Visibility = Visibility.Collapsed;
                    GridComplete.Visibility = Visibility.Collapsed;
                    break;
                case Step.Repair:
                    GridInstall.Visibility = Visibility.Collapsed;
                    GridRepair.Visibility = Visibility.Visible;
                    GridProgress.Visibility = Visibility.Collapsed;
                    GridComplete.Visibility = Visibility.Collapsed;
                    break;
                case Step.Progress:
                    GridInstall.Visibility = Visibility.Collapsed;
                    GridRepair.Visibility = Visibility.Collapsed;
                    GridProgress.Visibility = Visibility.Visible;
                    GridComplete.Visibility = Visibility.Collapsed;
                    break;
                case Step.Complete:

                    var message = "";
                    if (!_isSuccess)
                    {
                        switch (_action)
                        {
                            case LaunchAction.Uninstall:
                                message = "卸载";
                                break;
                            case LaunchAction.Install:
                                message = "安装";
                                break;
                            case LaunchAction.Repair:
                                message = "修复";
                                break;
                        }

                        message += "失败！";
                    }

                    TxtComplete.Text = message;
                    GridInstall.Visibility = Visibility.Collapsed;
                    GridRepair.Visibility = Visibility.Collapsed;
                    GridProgress.Visibility = Visibility.Collapsed;
                    GridComplete.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        void ReportProgress(int percentage, string message)
        {
            Dispatcher.BeginInvoke(new Action(() =>
             {
                 ProgressBarMain.Value = percentage;
                 TxtProgress.Text = message;
             }));
        }

        private void BtnInstall_Click(object sender, RoutedEventArgs e)
        {
            MoveTo(Step.Progress);
            _app.Engine.Plan(LaunchAction.Install);
            _action = LaunchAction.Install;
        }

        private void SelectFile_OnClick(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = _installFolder
            };

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _installFolder = folderBrowserDialog.SelectedPath;
                _app.Engine.StringVariables["InstallFolder"] = _installFolder;
            }
        }

        private void BtnUninstall_Click(object sender, RoutedEventArgs e)
        {
            _app.Engine.Plan(LaunchAction.Uninstall);
            _action = LaunchAction.Uninstall;
            MoveTo(Step.Progress);
        }

        private void BtnRepair_Click(object sender, RoutedEventArgs e)
        {
            _app.Engine.Plan(LaunchAction.Repair);
            _action = LaunchAction.Repair;
            MoveTo(Step.Progress);
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CustomBootstrapperApplication_DetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
            Debug.WriteLine("DetectPackageComplete. PackageId: {0} State: {1} Status: {2}",
                e.PackageId, e.State, e.Status);

            // DetectPackageComplete.PackageId: NetFx40Web State: Present Status: 0
            // DetectPackageComplete.PackageId: Setup.msi State: Present Status: 0

            if (e.PackageId == "Setup.msi")
            {
                if (e.State == PackageState.Absent)
                {
                    _action = LaunchAction.Install;
                }
                else
                {
                    _action = LaunchAction.Repair;
                }
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                Initialize();
            }));
        }


        private void CustomBootstrapperApplication_PlanBegin(object sender, PlanBeginEventArgs e)
        {
            Debug.WriteLine("PlanBegin. PackageCount: {0} Result: {1}",
                e.PackageCount, e.Result);
        }

        private void CustomBootstrapperApplication_PlanComplete(object sender, PlanCompleteEventArgs e)
        {
            Debug.WriteLine("PlanComplete. Status: {0}",
                e.Status);

            _app.Engine.Apply(_windowPtr);
        }

        private void CustomBootstrapperApplication_CacheComplete(object sender, CacheCompleteEventArgs e)
        {
            Debug.WriteLine("CacheComplete. e.Status: " + e.Status);
        }

        private void CustomBootstrapperApplication_Progress(object sender, ProgressEventArgs e)
        {
            Debug.WriteLine("Progress. Progress: {0} Overall: {1}", e.ProgressPercentage, e.OverallPercentage);

            //_progress = e.OverallPercentage;
            //ReportProgress(_progress, _message);
        }

        private void CustomBootstrapperApplication_PlanPackageComplete(object sender, PlanPackageCompleteEventArgs e)
        {
            Debug.WriteLine("PlanPackageComplete. Action: {0} Package: {1} RequestedState: {2} Rollback: {3} State: {4} Status: {5}",
                e.Execute, e.PackageId, e.Requested, e.Rollback, e.State, e.Status);
        }

        private void CustomBootstrapperApplication_ExecuteMsiMessage(object sender, ExecuteMsiMessageEventArgs e)
        {
            Debug.WriteLine("ExecuteMsiMessage. Data: {0} DisplayParameters: {1} Message: {2} MessageType: {3} PackageId: {4} Result: {5}",
                string.Join(",", e.Data ?? new string[] { "" }), e.DisplayParameters, e.Message, e.MessageType, e.PackageId, e.Result);

            if (e.MessageType == InstallMessage.ActionStart)
            {
                _message = e.Message;
                ReportProgress(_progress, _message);
            }
        }

        private void CustomBootstrapperApplication_ExecuteProgress(object sender, ExecuteProgressEventArgs e)
        {
            Debug.WriteLine("ExecuteProgress. OverallPercentage: {0} PackageId: {1} ProgressPercentage: {2} Result: {3}",
                e.OverallPercentage, e.PackageId, e.ProgressPercentage, e.Result);

            _progress = e.OverallPercentage;
            ReportProgress(_progress, _message);
        }

        private void CustomBootstrapperApplication_CacheAcquireProgress(object sender, CacheAcquireProgressEventArgs e)
        {
            Debug.WriteLine("CacheAcquireProgress. OverallPercentage: {0} PackageOrContainerId: {1} PayloadId: {2} Progress: {3} Result: {4} Total: {5}",
                e.OverallPercentage, e.PackageOrContainerId, e.PayloadId, e.Progress, e.Result, e.Total);
        }


        private void CustomBootstrapperApplication_ExecutePackageComplete(object sender, ExecutePackageCompleteEventArgs e)
        {
            Debug.WriteLine("ExecutePackageComplete. PackageId: {0} Restart: {1} Result: {2} Status: {3}",
                e.PackageId, e.Restart, e.Result, e.Status);
        }

        private void CustomBootstrapperApplication_ExecutePackageBegin(object sender, ExecutePackageBeginEventArgs e)
        {
            Debug.WriteLine("ExecutePackageBegin. PackageId: {0} Result: {1} ShouldExecute: {2}",
                e.PackageId, e.Result, e.ShouldExecute);
        }

        private void CustomBootstrapperApplication_ApplyBegin(object sender, ApplyBeginEventArgs e)
        {
            Debug.WriteLine("ApplyBegin. Result: {0}",
                e.Result);
        }

        private void CustomBootstrapperApplication_ApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            Debug.WriteLine("ApplyComplete. Restart: {0} Result: {1} Status: {2}",
                e.Restart, e.Result, e.Status);

            _isSuccess = e.Status == 0;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                MoveTo(Step.Complete);
            }));
        }
    }
}
