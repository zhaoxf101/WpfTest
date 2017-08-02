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

     
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            if (Environment.Is64BitOperatingSystem)
            {
                SetInstallFolder(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            }
            else
            {
                SetInstallFolder(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            }

          
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        void Initialize()
        {
        }

        void SetInstallFolder(string folder)
        {
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void BtnInstall_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SelectFile_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnUninstall_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
