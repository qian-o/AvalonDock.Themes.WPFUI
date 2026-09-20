using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace AvalonDock.Themes.WPFUI.Controls
{
    public static class DockFloatingWindowAssist
    {
        private const int DwmwaWindowCornerPreference = 33;
        private const int DwmwaBorderColor = 34;
        private const int DwmwcpRound = 2;
        private const int DwmwaColorNone = unchecked((int)0xFFFFFFFE);

        public static readonly DependencyProperty UseFluentChromeProperty = DependencyProperty.RegisterAttached(
            "UseFluentChrome",
            typeof(bool),
            typeof(DockFloatingWindowAssist),
            new PropertyMetadata(false, OnUseFluentChromeChanged));

        public static bool GetUseFluentChrome(DependencyObject element) =>
            (bool)element.GetValue(UseFluentChromeProperty);

        public static void SetUseFluentChrome(DependencyObject element, bool value) =>
            element.SetValue(UseFluentChromeProperty, value);

        private static void OnUseFluentChromeChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            if (!(element is Window window) || !(bool)args.NewValue)
            {
                return;
            }

            window.SourceInitialized -= OnWindowReady;
            window.Loaded -= OnWindowReady;
            window.SourceInitialized += OnWindowReady;
            window.Loaded += OnWindowReady;
            if (window.IsInitialized)
            {
                Apply(window);
            }
        }

        private static void OnWindowReady(object sender, EventArgs args)
        {
            if (!(sender is Window window))
            {
                return;
            }

            Apply(window);
            window.Dispatcher.BeginInvoke(new Action(() => Apply(window)), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private static void Apply(Window window)
        {
            var handle = new WindowInteropHelper(window).Handle;
            if (handle == IntPtr.Zero)
            {
                return;
            }

            var corner = DwmwcpRound;
            DwmSetWindowAttribute(handle, DwmwaWindowCornerPreference, ref corner, sizeof(int));

            var color = DwmwaColorNone;
            DwmSetWindowAttribute(handle, DwmwaBorderColor, ref color, sizeof(int));
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);
    }
}
