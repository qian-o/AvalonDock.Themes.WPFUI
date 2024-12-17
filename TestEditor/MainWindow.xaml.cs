using System.Windows;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace TestEditor;

public partial class MainWindow : FluentWindow
{
    public MainWindow()
    {
        InitializeComponent();

        SystemThemeWatcher.Watch(this);

        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Maximized;
    }

    private void ToggleSwitch_Click(object sender, RoutedEventArgs e)
    {
        ToggleSwitch toggleSwitch = (ToggleSwitch)sender;

        if (toggleSwitch.IsChecked is true)
        {
            ApplicationThemeManager.Apply(ApplicationTheme.Dark);
        }
        else
        {
            ApplicationThemeManager.Apply(ApplicationTheme.Light);
        }
    }
}