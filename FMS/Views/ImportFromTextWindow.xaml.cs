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
using FMS.Models;
using FMS.ViewModels;

namespace FMS.Views
{
    /// <summary>
    /// ImportFromTextWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImportFromTextWindow : Window
    {
        public List<Item> ReturnList { get; set; }
        public ImportFromTextWindow()
        {
            InitializeComponent();
            DataContext= new ImportFromTextWindowViewModel();
        }

        private void oKButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnList = Core.ImportListFromText(textBox.Text);
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
