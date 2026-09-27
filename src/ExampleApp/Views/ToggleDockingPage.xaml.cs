using System.Windows.Controls;

namespace ExampleApp.Views
{
    public partial class ToggleDockingPage : UserControl
    {
        public ToggleDockingPage()
        {
            InitializeComponent();
            SampleWorkspace.Populate(Manager, true);
        }
    }
}