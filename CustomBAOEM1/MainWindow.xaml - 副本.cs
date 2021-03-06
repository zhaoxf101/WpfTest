﻿using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
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
            Uninstall,
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
        bool _isExecuting = false;

        public MainWindow(CustomBootstrapperApplication app)
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            Trace.Listeners.Add(new TextWriterTraceListener("d:\\log.log"));

            Trace.AutoFlush = true;

            _app = app;

            _installFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            //if (Environment.Is64BitOperatingSystem)
            //{
            //}
            //else
            //{
            //    _installFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            //}
            SetInstallFolder(Path.Combine(_installFolder, "XYUNHUI"));

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

            _app.DetectRelatedMsiPackage += _app_DetectRelatedMsiPackage;
            _app.DetectCompatiblePackage += _app_DetectCompatiblePackage;
            _app.DetectForwardCompatibleBundle += _app_DetectForwardCompatibleBundle;
            _app.DetectMsiFeature += _app_DetectMsiFeature;
            _app.DetectPackageBegin += _app_DetectPackageBegin;
            _app.DetectPackageComplete += _app_DetectPackageComplete;
            _app.DetectPriorBundle += _app_DetectPriorBundle;
            _app.DetectRelatedBundle += _app_DetectRelatedBundle;
            _app.DetectTargetMsiPackage += _app_DetectTargetMsiPackage;
            _app.DetectUpdate += _app_DetectUpdate;
            _app.DetectUpdateBegin += _app_DetectUpdateBegin;
            _app.DetectUpdateComplete += _app_DetectUpdateComplete;

            app.Engine.Detect();
        }

        private void _app_DetectUpdateComplete(object sender, DetectUpdateCompleteEventArgs e)
        {
            Trace.WriteLine($"DetectUpdateComplete. Status: {e.Status} UpdateLocation: {e.UpdateLocation}");
        }

        private void _app_DetectUpdateBegin(object sender, DetectUpdateBeginEventArgs e)
        {
            Trace.WriteLine($"DetectUpdateBegin. Result: {e.Result} UpdateLocation{e.UpdateLocation}");
        }

        private void _app_DetectUpdate(object sender, DetectUpdateEventArgs e)
        {
            Trace.WriteLine($"DetectUpdate. Result: {e.Result} Content: {e.Content} ContentType: {e.ContentType} Size: {e.Size} Summary: { e.Summary} Title: { e.Title} UpdateLocation: { e.UpdateLocation} Version: {e.Version}");
        }

        private void _app_DetectTargetMsiPackage(object sender, DetectTargetMsiPackageEventArgs e)
        {
            Trace.WriteLine($"DetectTargetMsiPackage. PackageId: {e.PackageId} ProductCode: {e.ProductCode} Result: {e.Result} State: {e.State}");
        }

        private void _app_DetectRelatedBundle(object sender, DetectRelatedBundleEventArgs e)
        {
            Trace.WriteLine($"DetectRelatedBundle. Operation: {e.Operation} BundleTag: {e.BundleTag} PerMachine: {e.PerMachine} ProductCode: {e.ProductCode} RelationType: {e.RelationType} Result: {e.Result} Version: {e.Version}");
        }

        private void _app_DetectPriorBundle(object sender, DetectPriorBundleEventArgs e)
        {
            Trace.WriteLine($"DetectPriorBundle. BundleId: {e.BundleId} Result: {e.Result}");
        }

        private void _app_DetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
            Trace.WriteLine($"DetectPackageComplete. PackageId: {e.PackageId} State: {e.State} Status: {e.Status}");

        }

        private void _app_DetectPackageBegin(object sender, DetectPackageBeginEventArgs e)
        {
            Trace.WriteLine($"DetectPackageBegin. PackageId: {e.PackageId} Result: {e.Result}");
        }

        private void _app_DetectMsiFeature(object sender, DetectMsiFeatureEventArgs e)
        {
            Trace.WriteLine($"DetectMsiFeature. FeatureId: {e.FeatureId} PackageId: {e.PackageId} Result: {e.Result} State: {e.State}");

        }

        private void _app_DetectForwardCompatibleBundle(object sender, DetectForwardCompatibleBundleEventArgs e)
        {
            Trace.WriteLine($"DetectForwardCompatibleBundle. BundleId: {e.BundleId} BundleTag:{e.BundleTag} PerMachine: {e.PerMachine} RelationType: {e.RelationType} Result: {e.Result} Version: {e.Version}");
        }

        private void _app_DetectCompatiblePackage(object sender, DetectCompatiblePackageEventArgs e)
        {
            Trace.WriteLine($"DetectCompatiblePackage. CompatiblePackageId: {e.CompatiblePackageId} PackageId: {e.PackageId} Result: {e.Result}");

        }

        private void _app_DetectRelatedMsiPackage(object sender, DetectRelatedMsiPackageEventArgs e)
        {
            Trace.WriteLine($"DetectRelatedMsiPackage. Operation: {e.Operation} PackageId: {e.PackageId} PerMachine: {e.PerMachine} ProductCode: {e.ProductCode} Result: {e.Result} Version: {e.Version}");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _windowPtr = new WindowInteropHelper(this).Handle;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isExecuting)
            {
                e.Cancel = true;
            }
        }

        void Initialize()
        {
            //if (_action == LaunchAction.Install)
            //{
                MoveTo(Step.Install);
            //}
            //else
            //{
            //    MoveTo(Step.Uninstall);
            //}
        }

        void MoveTo(Step step)
        {
            switch (step)
            {
                case Step.Install:
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
                case Step.Progress:
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
                            }
                            else
                            {
                                message = "卸载失败！";
                                ImageSuccess.Visibility = Visibility.Visible;
                                ImageSuccess.Visibility = Visibility.Collapsed;
                                ImageFailure.Visibility = Visibility.Visible;
                            }
                            BtnOk.Content = "完成";
                            break;
                        case LaunchAction.Install:
                            if (_isSuccess)
                            {
                                message = "安装成功！";
                                ImageSuccess.Visibility = Visibility.Visible;
                                ImageSuccess.Visibility = Visibility.Visible;
                                ImageFailure.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                ImageSuccess.Visibility = Visibility.Visible;
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
                default:
                    break;
            }
        }

        void ReportProgress(int percentage, string message)
        {
            Dispatcher.BeginInvoke(new Action(() =>
             {
                 ProgressBarMain.Value = percentage;
                 //TxtProgress.Text = message;
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
            MoveTo(Step.Progress);
            _app.Engine.Plan(LaunchAction.UpdateReplaceEmbedded);
            _action = LaunchAction.Install;
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
            _app.Engine.Plan(LaunchAction.Uninstall);
            _action = LaunchAction.Uninstall;
            MoveTo(Step.Progress);
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
                    Process.Start(Path.Combine(_installFolder, "MarketingPlatForm.Client.exe"));
                }
                catch
                {

                }
            }
            Close();
        }

        private void CustomBootstrapperApplication_DetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
            Trace.WriteLine(string.Format("DetectPackageComplete. PackageId: {0} State: {1} Status: {2}",
                e.PackageId, e.State, e.Status));

            // DetectPackageComplete.PackageId: NetFx40Web State: Present Status: 0
            // DetectPackageComplete.PackageId: Setup.msi State: Present Status: 0


            Trace.WriteLine($"e.State: {e.State} Status: {e.Status} PackageId: {e.PackageId}");

            
            if (e.PackageId == "Setup.msi")
            {
                if (e.State == PackageState.Absent)
                {
                    _action = LaunchAction.Install;
                }
                else
                {
                    _action = LaunchAction.Uninstall;
                }
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                Initialize();
            }));
        }


        private void CustomBootstrapperApplication_PlanBegin(object sender, PlanBeginEventArgs e)
        {
            Trace.WriteLine($"PlanBegin. PackageCount: {e.PackageCount} Result: {e.Result}");
        }

        private void CustomBootstrapperApplication_PlanComplete(object sender, PlanCompleteEventArgs e)
        {
            Trace.WriteLine(string.Format("PlanComplete. Status: {0}", e.Status));

            _app.Engine.Apply(_windowPtr);
        }

        private void CustomBootstrapperApplication_CacheComplete(object sender, CacheCompleteEventArgs e)
        {
            Trace.WriteLine("CacheComplete. e.Status: " + e.Status);
        }

        private void CustomBootstrapperApplication_Progress(object sender, ProgressEventArgs e)
        {
            Trace.WriteLine(string.Format("Progress. Progress: {0} Overall: {1}", e.ProgressPercentage, e.OverallPercentage));

            //_progress = e.OverallPercentage;
            //ReportProgress(_progress, _message);
        }

        private void CustomBootstrapperApplication_PlanPackageComplete(object sender, PlanPackageCompleteEventArgs e)
        {
            Trace.WriteLine(string.Format("PlanPackageComplete. Action: {0} Package: {1} RequestedState: {2} Rollback: {3} State: {4} Status: {5}",
                e.Execute, e.PackageId, e.Requested, e.Rollback, e.State, e.Status));
        }

        private void CustomBootstrapperApplication_ExecuteMsiMessage(object sender, ExecuteMsiMessageEventArgs e)
        {
            Trace.WriteLine(string.Format("ExecuteMsiMessage. Data: {0} DisplayParameters: {1} Message: {2} MessageType: {3} PackageId: {4} Result: {5}",
                string.Join(",", e.Data ?? new string[] { "" }), e.DisplayParameters, e.Message, e.MessageType, e.PackageId, e.Result));

            if (e.MessageType == InstallMessage.ActionStart)
            {
                _message = e.Message;
                ReportProgress(_progress, _message);
            }
        }

        private void CustomBootstrapperApplication_ExecuteProgress(object sender, ExecuteProgressEventArgs e)
        {
            Trace.WriteLine(string.Format("ExecuteProgress. OverallPercentage: {0} PackageId: {1} ProgressPercentage: {2} Result: {3}",
                e.OverallPercentage, e.PackageId, e.ProgressPercentage, e.Result));

            _progress = e.OverallPercentage;
            ReportProgress(_progress, _message);
        }

        private void CustomBootstrapperApplication_CacheAcquireProgress(object sender, CacheAcquireProgressEventArgs e)
        {
            Trace.WriteLine(string.Format("CacheAcquireProgress. OverallPercentage: {0} PackageOrContainerId: {1} PayloadId: {2} Progress: {3} Result: {4} Total: {5}",
                e.OverallPercentage, e.PackageOrContainerId, e.PayloadId, e.Progress, e.Result, e.Total));
        }


        private void CustomBootstrapperApplication_ExecutePackageComplete(object sender, ExecutePackageCompleteEventArgs e)
        {
            Trace.WriteLine(string.Format("ExecutePackageComplete. PackageId: {0} Restart: {1} Result: {2} Status: {3}",
                e.PackageId, e.Restart, e.Result, e.Status));
        }

        private void CustomBootstrapperApplication_ExecutePackageBegin(object sender, ExecutePackageBeginEventArgs e)
        {
            Trace.WriteLine(string.Format("ExecutePackageBegin. PackageId: {0} Result: {1} ShouldExecute: {2}",
                e.PackageId, e.Result, e.ShouldExecute));
        }

        private void CustomBootstrapperApplication_ApplyBegin(object sender, ApplyBeginEventArgs e)
        {
            Trace.WriteLine(string.Format("ApplyBegin. Result: {0}",
                e.Result));
        }

        private void CustomBootstrapperApplication_ApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            Trace.WriteLine(string.Format("ApplyComplete. Restart: {0} Result: {1} Status: {2}",
                e.Restart, e.Result, e.Status));

            _isSuccess = e.Status == 0;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                MoveTo(Step.Complete);
            }));
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
