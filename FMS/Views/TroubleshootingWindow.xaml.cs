using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace FMS.Views
{
    /// <summary>
    /// TroubleshootingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TroubleshootingWindow : MetroWindow
    {
        public TroubleshootingWindow()
        {
            InitializeComponent();
        }

        public TroubleshootingWindow(string message)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
