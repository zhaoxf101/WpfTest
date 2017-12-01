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
        MainWindow _mainWindow;
        RelatedOperation _relatedOperation;
        LaunchAction _action;

        string _installFolder;

        int _progress;
        string _message;

        protected override void Run()
        {
            Report.ReportInfoAsync();
            var logFile = Path.Combine(Path.GetTempPath(), $"IMS_Setup_{DateTime.Now.ToString("yyyyMMddHHmmssff")}.log");
            Trace.Listeners.Add(new TextWriterTraceListener(logFile));
            Trace.AutoFlush = true;

            Trace.WriteLine($"Command.Action: {Command.Action} Display: {Command.Display} LayoutDirectory: {Command.LayoutDirectory} Passthrough: {Command.Passthrough} Relation: {Command.Relation} Restart: {Command.Restart} Restart: {Command.Restart} Resume: {Command.Resume} SplashScreen: {Command.SplashScreen} CommandLineArgs: {string.Join("|", Command.GetCommandLineArgs())}");

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            DetectRelatedBundle += CustomBootstrapperApplication_DetectRelatedBundle;
            DetectPackageComplete += CustomBootstrapperApplication_DetectPackageComplete;

            PlanComplete += CustomBootstrapperApplication_PlanComplete;
            ApplyComplete += CustomBootstrapperApplication_ApplyComplete;

            ExecuteMsiMessage += CustomBootstrapperApplication_ExecuteMsiMessage;
            ExecuteProgress += CustomBootstrapperApplication_ExecuteProgress;

            if (Command.Display == Display.Full)
            {
                _mainWindow = new MainWindow(this);

                Engine.Detect();
                _mainWindow.ShowDialog();
                Engine.Quit(0);
            }
            else
            {
                var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                _installFolder = Path.Combine(folder, "XYUNHUI");

                Engine.Detect();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Log("Program. UnhandledException.");

            var exception = e.ExceptionObject as Exception;
            var level = 0;
            while (exception != null)
            {
                var padding = new string(' ', level * 2);
                Trace.WriteLine($"{padding}Message: {exception.Message}");
                Trace.WriteLine($"{padding}StackTrace:");
                Trace.WriteLine(exception.StackTrace);

                if (++level > 10)
                {
                    break;
                }
                exception = exception.InnerException;
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Logger.Log("Program. ProcessExit.");
        }

        private void CustomBootstrapperApplication_DetectRelatedBundle(object sender, DetectRelatedBundleEventArgs e)
        {
            Trace.WriteLine($"DetectRelatedBundle. Operation: {e.Operation} BundleTag: {e.BundleTag} PerMachine: {e.PerMachine} ProductCode: {e.ProductCode} RelationType: {e.RelationType} Result: {e.Result} Version: {e.Version}");

            _relatedOperation = e.Operation;
        }

        private void CustomBootstrapperApplication_DetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
            Trace.WriteLine(string.Format("DetectPackageComplete. PackageId: {0} State: {1} Status: {2}",
                e.PackageId, e.State, e.Status));

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
                    if (_relatedOperation == RelatedOperation.MajorUpgrade ||
                        _relatedOperation == RelatedOperation.MinorUpdate)
                    {
                        _action = LaunchAction.UpdateReplace;
                    }
                    else if (_relatedOperation == RelatedOperation.Downgrade)
                    {
                        _action = LaunchAction.Unknown;
                    }
                    else
                    {
                        _action = LaunchAction.Uninstall;
                    }
                }

                Trace.WriteLine($"_action: {_action}");

                if (_mainWindow != null)
                {
                    _mainWindow.Initialize(_relatedOperation, _action);
                }
                else
                {
                    if (_action != LaunchAction.Uninstall &&
                        _action != LaunchAction.Unknown)
                    {
                        Engine.StringVariables["InstallFolder"] = _installFolder;
                    }
                    Engine.Plan(_action);
                }
            }
        }

        private void CustomBootstrapperApplication_PlanComplete(object sender, PlanCompleteEventArgs e)
        {
            Trace.WriteLine($"PlanComplete. Status: {e.Status} _action: {_action}");

            if (_action == LaunchAction.Uninstall)
            {
                CustomAction.KillRelativeProcesses(_mainWindow?.InstallFolder ?? _installFolder);

                // 无窗口卸载，假定是升级安装。因为是先安装升级 msi，再卸载低版本 bundle，所以这里不再执行 Backup 操作。
                if (_mainWindow != null)
                {
                    // 注意，这里的安装目录是固定的
                    CustomAction.Backup(_mainWindow.InstallFolder);
                }
            }
            else if (_action != LaunchAction.Unknown)
            {
                // 注意，这里的备份目录是固定的
                CustomAction.RemoveBackup(_mainWindow?.InstallFolder ?? _installFolder);
            }

            Engine.Apply(_mainWindow?.WindowPtr ?? IntPtr.Zero);
        }

        private void CustomBootstrapperApplication_ExecuteMsiMessage(object sender, ExecuteMsiMessageEventArgs e)
        {
            Trace.WriteLine(string.Format("ExecuteMsiMessage. Data: {0} DisplayParameters: {1} Message: {2} MessageType: {3} PackageId: {4} Result: {5}",
                string.Join(",", e.Data ?? new string[] { "" }), e.DisplayParameters, e.Message, e.MessageType, e.PackageId, e.Result));

            if (e.MessageType == InstallMessage.ActionStart)
            {
                _message = e.Message;
                _mainWindow?.ReportProgress(_progress, _message);
            }
        }

        private void CustomBootstrapperApplication_ExecuteProgress(object sender, ExecuteProgressEventArgs e)
        {
            Trace.WriteLine(string.Format("ExecuteProgress. OverallPercentage: {0} PackageId: {1} ProgressPercentage: {2} Result: {3}",
                e.OverallPercentage, e.PackageId, e.ProgressPercentage, e.Result));

            _progress = e.OverallPercentage;
            _mainWindow?.ReportProgress(_progress, _message);
        }

        private void CustomBootstrapperApplication_ApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            Trace.WriteLine(string.Format("ApplyComplete. Restart: {0} Result: {1} Status: {2}",
                e.Restart, e.Result, e.Status));

            if (e.Status == 0)
            {
                CustomAction.CleanUp(_mainWindow?.InstallFolder ?? _installFolder);
            }

            if (_mainWindow != null)
            {
                _mainWindow.ApplyComplete(e.Status == 0);
            }
            else
            {
                Engine.Quit(0);
            }
        }

    }
}
