using System;
using System.Collections;
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
using FMS.Models;
using FMS.ViewModels;
using MahApps.Metro.Controls;

namespace FMS.Views
{
    /// <summary>
    /// ExportWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExportWindow : MetroWindow
    {
        public ExportWindow()
        {
            InitializeComponent();
            DateItemListBox.SelectAll();
            NameItemListBox.SelectAll();
            DataContext = new ExportWindowViewModel();
        }

        private void oKButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DateItemCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if ((bool)checkBox.IsChecked)
            {
                DateItemListBox.SelectAll();
            }
            else
            {
                DateItemListBox.UnselectAll();
            }
        }

        private void NameItemCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if ((bool)checkBox.IsChecked)
            {
                NameItemListBox.SelectAll();
            }
            else
            {
                NameItemListBox.UnselectAll();
            }
        }
    }
}
