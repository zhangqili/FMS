using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Lib;

namespace FMS.ViewModels
{
    internal class RawViewWindowViewModel : NotificationObject
    {
        private Core core;

        public Core Core
        {
            get { return core; }
            set
            {
                core = value;
                OnPropertyChanged(nameof(Core));
            }
        }

        public RawViewWindowViewModel()
        {
            Core = Global.Core;
        }
    }
}
