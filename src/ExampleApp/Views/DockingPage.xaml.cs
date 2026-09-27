using System.Windows.Controls;

namespace ExampleApp.Views
{
    public partial class DockingPage : UserControl
    {
        public DockingPage()
        {
            InitializeComponent();
            SampleWorkspace.Populate(Manager, false);
        }
    }
}