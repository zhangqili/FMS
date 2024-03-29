﻿using FMS.ViewModels;
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
    /// ImportWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImportWindow : Window
    {
        public ImportWindow()
        {
            InitializeComponent();
            DataContext= new ImportWindowViewModel();

        }

        public ImportWindow(string filePath)
        {
            InitializeComponent();
            DataContext = new ImportWindowViewModel(filePath);

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void oKButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
