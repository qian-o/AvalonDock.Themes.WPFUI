using System;
using System.Windows;
using Fluent;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace ExampleApp
{
    public partial class MainWindow : FluentWindow, IRibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            SystemThemeWatcher.Watch(this);
        }

        public RibbonTitleBar TitleBar => RibbonTitleBar;

        private void FluentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FluentWindow_StateChanged(sender, e);
        }

        private void FluentWindow_StateChanged(object sender, EventArgs e)
        {
            Manager.Margin = new Thickness(WindowState == WindowState.Normal ? 2 : 0);
        }
    }
}