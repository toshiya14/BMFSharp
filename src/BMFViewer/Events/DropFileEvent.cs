using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMFViewer.Events;

public class DropFileEvent
{
    public string FileName { get; set; }
    public Type Handler { get; set; }
}
