using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AvalonDock;
using AvalonDock.Controls;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace ExampleApp
{
    public partial class MainWindow : FluentWindow
    {
        private readonly HashSet<LayoutFloatingWindowControl> hiddenWindows = new HashSet<LayoutFloatingWindowControl>();

        public MainWindow()
        {
            InitializeComponent();

            SystemThemeWatcher.Watch(this);
        }

        private void OnLoaded(object sender, RoutedEventArgs args) => UpdateFloatingWindows();

        private void OnPageChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.OriginalSource == Pages && IsLoaded)
            {
                (Pages.SelectedItem as TabItem)?.Focus();
                UpdateFloatingWindows();
            }
        }

        private void UpdateFloatingWindows()
        {
            var selected = Pages.SelectedIndex == 0 ? DockingView.Manager : (DockingManager)ToggleView.Manager;
            foreach (var window in Application.Current.Windows.OfType<LayoutFloatingWindowControl>().ToArray())
            {
                var manager = window.Model.Root?.Manager;
                if (manager != DockingView.Manager && manager != ToggleView.Manager) continue;
                if (manager == selected && hiddenWindows.Remove(window))
                {
                    window.Closed -= OnFloatingClosed;
                    window.Show();
                }
                else if (manager != selected && window.IsVisible)
                {
                    hiddenWindows.Add(window);
                    window.Closed += OnFloatingClosed;
                    window.Hide();
                }
            }
        }

        private void OnFloatingClosed(object sender, EventArgs args)
        {
            var window = (LayoutFloatingWindowControl)sender;
            window.Closed -= OnFloatingClosed;
            hiddenWindows.Remove(window);
        }

        private void OnClosed(object sender, EventArgs args)
        {
            ((MainViewModel)DataContext).Dispose();
            foreach (var window in hiddenWindows) window.Closed -= OnFloatingClosed;
            hiddenWindows.Clear();
        }
    }
}