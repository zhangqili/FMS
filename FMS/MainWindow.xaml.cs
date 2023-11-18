using FMS.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using FMS;
using MahApps.Metro.Controls;
using FMS.Views;
using System.Windows.Threading;
using System.Data.SQLite;

namespace FMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private StartWindow startWindow;
        public MainWindow()
        {
            
            //NewWindowHandler(null,null);
            //Hide();
            /*
            try
            {
                //Global.Core = new Core(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "core.xlsx");
                //Global.Core = new Core(@"D:\FMS\FMS\bin\Debug\net6.0-windows\" + "core.xlsx");
                Global.Core = new Core(new DataBase());
            }
            catch (Exception e)
            {
                //CloseWindowSafe(startWindow);
                new TroubleshootingWindow(e.Message).ShowDialog();

            }
            */
            InitializeComponent();
            //Show();
            //Activate();
            //CloseWindowSafe(startWindow);
        }
        private void NewWindowHandler(object sender, RoutedEventArgs e)
        {
            Thread newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
            newWindowThread.SetApartmentState(ApartmentState.STA);
            newWindowThread.IsBackground = true;
            newWindowThread.Start();
        }

        private void ThreadStartingPoint()
        {
            startWindow = new StartWindow();
            startWindow.Show();
            Dispatcher.Run();
        }

        void CloseWindowSafe(Window w)
        {
            if (w.Dispatcher.CheckAccess())
                w.Close();
            else
                w.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(w.Close));
        }
    }
}
