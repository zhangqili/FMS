using FMS.Commands;
using FMS.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.ViewModels
{
    public class AddNameItemWindowViewModel : NotificationObject
    {
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public DelegateCommand OKCommand { get; set; }

        private void OK(object parameter)
        {
            Global.Core.AddNameItem(Name);
        }
        public AddNameItemWindowViewModel()
        {
            OKCommand = new DelegateCommand(OK);
            Name = "";
        }
    }
}
