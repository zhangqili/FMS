using FMS.Commands;
using FMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Lib;
using Microsoft.Win32;
using NPOI.HPSF;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace FMS.ViewModels
{
    internal class WelcomeViewModel : NotificationObject
    {
        public DelegateCommand NewCommand { get; set; }
        private void New(object parameter)
        {
            Global.MainWindowViewModel.NewDatabase(null);
        }

        public DelegateCommand OpenCommand { get; set; }
        private void Open(object parameter)
        {
            Global.MainWindowViewModel.OpenDatabase(null);
        }
        public WelcomeViewModel()
        {
            NewCommand = new DelegateCommand(New);
            OpenCommand = new DelegateCommand(Open);
        }
    }
}
