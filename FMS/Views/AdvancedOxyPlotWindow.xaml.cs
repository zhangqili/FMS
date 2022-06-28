using FMS.Models;
using FMS.ViewModels;
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

namespace FMS.Views
{
    /// <summary>
    /// AdvancedOxyPlotWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AdvancedOxyPlotWindow : Window
    {
        public AdvancedOxyPlotWindow()
        {
            InitializeComponent();
            DataContext = new AdvancedOxyPlotWIndowViewModel();
        }
        public AdvancedOxyPlotWindow(NameItem nameItem)
        {
            InitializeComponent();
            DataContext = new AdvancedOxyPlotWIndowViewModel(nameItem);
        }
    }
}
