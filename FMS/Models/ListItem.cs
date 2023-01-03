using System.Collections.Generic;
using System.Windows.Documents;

namespace FMS.Models;

public interface IListItem
{
    public List<object> List { get; set; }
    public List EfficientList { get; set; }
    public int Count { get; set; }

}