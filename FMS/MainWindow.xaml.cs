using FMS.Lib;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FMS.Libs;
using MahApps.Metro.Controls;

namespace FMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            //Global.Core = new Core(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "core.xlsx");
            //Global.Core = new Core(@"D:\FMS\FMS\bin\Debug\net6.0-windows\" + "core.xlsx");

            Global.Core = new Core(new DataBase());
            InitializeComponent();
        }
    }
}
