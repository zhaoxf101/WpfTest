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


            TxtPolicy.Text =
            @"重要须知：在此特别提醒用户认真阅读、充分理解本《软件许可及安装协议》（下称《协议》）---- 用户应认真阅读、充分理解本《协议》中各条款，包括免除或者限制公司责任的免责条款及对用户的权利限制条款。请您审慎阅读并选择接受或不接受本《协议》。除非您接受本《协议》所有条款，否则您无权安装或使用本软件及其相关服务。您的安装、使用、License获取和登录等行为将视为对本《协议》的接受，并同意接受本《协议》各项条款的约束。 
本《协议》是您（下称“用户”）与公司之间关于用户安装、使用软件，注册、使用、管理软件；以及使用公司公司提供的相关服务所订立的协议。本《协议》描述公司与用户之间关于“软件”许可使用及服务相关方面的权利义务。“用户”是指通过公司提供的获取软件授权和License注册的途径获得软件产品及号码授权许可以及使用公司公司提供的相关服务的个人或组织。 
本《协议》可由公司随时更新，更新后的协议条款一旦公布即代替原来的协议条款，恕不再另行通知。用户可重新下载安装本软件或网站查阅最新版协议条款。在公司修改《协议》条款后，如果用户不接受修改后的条款，请立即停止使用公司提供的软件和服务，用户继续使用公司提供的软件和服务将被视为已接受了修改后的协议。 除本《协议》有明确规定外，本《协议》并未对利用本“软件”使用的公司或合作单位的其他服务规定相关的服务条款。对于这些服务，一般有单独的服务条款加以规范，用户须在使用有关服务时另行了解与确认。单独的服务条款与本协议有冲突的地方，以单独的服务条款为准。如用户使用该服务，视为对相关服务条款的接受。 

3、关于您的资料的规则。 

您同意，“公司资料”和您提供在云合景从上交易的任何“产品”（泛指一切可供依法交易的、有形的或无形的、以各种形态存在的某种具体的物品，或某种权利或利益，或某种票据或证券，或某种服务或行为。“本条款”中“产品”一词均含此义） 

（a）不会有欺诈成份，与售卖伪造或盗窃无涉； 

（b）不会侵犯任何第三者对该物品享有的物权，或版权、专利、商标、商业秘密或其他知识产权，或隐私权、名誉权； 

（c）不会违反任何法律、法规、条例或规章 (包括但不限于关于规范出口管理、贸易配额、保护消费者、不正当竞争或虚假广告的法律、法规、条例或规章)； 

（d）不会含有诽谤（包括商业诽谤）、非法恐吓或非法骚扰的内容； 

（e）不会含有淫秽、或包含任何儿童色情内容； 

（f）不会含有蓄意毁坏、恶意干扰、秘密地截取或侵占任何系统、数据或个人资料的任何病毒、伪装破坏程序、电脑蠕虫、定时程序炸弹或其他电脑程序； 

（g）不会直接或间接与“本条款”项下禁止的货物/服务以及您无权连接或包含的货物/服务连接，或包含对上述各项货物或服务的描述； 

（h） 不会在与任何连锁信件、大量胡乱邮寄的电子邮件、滥发电子邮件或任何复制或多余的信息有关的方面使用“服务”； 

（i）不会未经其他人士同意，利用“服务”收集其他人士的电子邮件地址及其他资料； 

（j）不会利用“服务”制作虚假的电子邮件地址，或以其他形式试图在发送人的身份或信息的来源方面误导其他人士。 

4、被禁止物品。 

您不得在云合景从的网站公布： 
(a) 可能使云合景从违反任何相关法律、法规、条例或规章的任何物品； 
(b) 云合景从认为应禁止或不适合通过本网站发布的任何物品。";

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var drivers = System.IO.DriveInfo.GetDrives();

            var info = Path.GetPathRoot("/Helel");
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
        
        private void HyperlinkPolicy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HyperlinkInstallFolder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAgree_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnBackInstallFolder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnOkInstallFolder_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
