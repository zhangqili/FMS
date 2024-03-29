﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.ViewModels;

namespace FMS.Lib
{
    internal class Global
    { 
        public static Core Core { get; set; }

        public static OxyPlotWindowViewModel OxyPlotWindowViewModel { get; set; }
        public static DateItemViewModel DateItemViewModel { get; set; }
        public static NameItemViewModel NameItemViewModel { get; set; }
        public static MainWindowViewModel MainWindowViewModel { get; set; }
    }
}
