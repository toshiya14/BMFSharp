using System.Linq;
using System.Windows;
using BMFViewer.Events;
using Stylet;

namespace BMFViewer.Pages
{
    public partial class ShellView : Window
    {
        private IEventAggregator events;

        public ShellView(IEventAggregator events)
        {
            this.events = events;
            InitializeComponent();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
                events.Publish(new DropFileEvent { Handler = typeof(ShellViewModel), FileName = files.First() });
            }
        }
    }
}
