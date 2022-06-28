using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FMS.Lib;
using FMS.ViewModels;

namespace FMS.Views
{
    /// <summary>
    /// OxyPlotWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OxyPlotWindow : Window
    {
        public OxyPlotWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Global.OxyPlotWindowViewModel = null;
        }
    }
}
