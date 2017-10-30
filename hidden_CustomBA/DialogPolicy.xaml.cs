﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CustomBA
{
    /// <summary>
    /// DialogPolicy.xaml 的交互逻辑
    /// </summary>
    public partial class DialogPolicy : Window
    {
        public DialogPolicy()
        {
            InitializeComponent();

            TxtPolicy.Text =
            @"重要须知：在此特别提醒用户认真阅读、充分理解本《软件许可及安装协议》（下称《协议》）---- 用户应认真阅读、充分理解本《协议》中各条款，包括免除或者限制公司责任的免责条款及对用户的权利限制条款。请您审慎阅读并选择接受或不接受本《协议》。除非您接受本《协议》所有条款，否则您无权安装或使用本软件及其相关服务。您的安装、使用、License获取和登录等行为将视为对本《协议》的接受，并同意接受本《协议》各项条款的约束。 
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnAgree_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

    }
}