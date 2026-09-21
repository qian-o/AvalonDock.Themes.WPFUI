using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using AvalonDock.Controls;

namespace AvalonDock.Themes.WPFUI.Controls
{
    public static class DockFloatingWindowAssist
    {
        private const int DwmwaWindowCornerPreference = 33;
        private const int DwmwaBorderColor = 34;
        private const int DwmwcpRound = 2;
        private const int DwmwaColorNone = unchecked((int)0xFFFFFFFE);

        private static readonly DependencyPropertyDescriptor MinWidthDescriptor =
            DependencyPropertyDescriptor.FromProperty(FrameworkElement.MinWidthProperty, typeof(Window));
        private static readonly DependencyPropertyDescriptor MinHeightDescriptor =
            DependencyPropertyDescriptor.FromProperty(FrameworkElement.MinHeightProperty, typeof(Window));

        public static readonly DependencyProperty UseFluentChromeProperty = DependencyProperty.RegisterAttached(
            "UseFluentChrome",
            typeof(bool),
            typeof(DockFloatingWindowAssist),
            new PropertyMetadata(false, OnUseFluentChromeChanged));

        public static bool GetUseFluentChrome(DependencyObject element) =>
            (bool)element.GetValue(UseFluentChromeProperty);

        public static void SetUseFluentChrome(DependencyObject element, bool value) =>
            element.SetValue(UseFluentChromeProperty, value);

        private static Size GetMinimumSize(Window window)
        {
            if (window is LayoutDocumentFloatingWindowControl)
            {
                return new Size(280, 180);
            }
            if (window is LayoutAnchorableFloatingWindowControl)
            {
                return new Size(200, 140);
            }

            return new Size(0, 0);
        }

        private static void StartMinimumSizeTracking(Window window)
        {
            if (GetMinimumSize(window) == new Size(0, 0))
            {
                return;
            }

            window.Closed -= OnMinimumSizeWindowClosed;
            window.Closed += OnMinimumSizeWindowClosed;
            if (new WindowInteropHelper(window).Handle != IntPtr.Zero)
            {
                MinWidthDescriptor.RemoveValueChanged(window, OnWindowMinimumChanged);
                MinHeightDescriptor.RemoveValueChanged(window, OnWindowMinimumChanged);
                MinWidthDescriptor.AddValueChanged(window, OnWindowMinimumChanged);
                MinHeightDescriptor.AddValueChanged(window, OnWindowMinimumChanged);
            }

            ApplyMinimumSize(window);
        }

        private static void OnWindowMinimumChanged(object sender, EventArgs args) => ApplyMinimumSize((Window)sender);

        private static void OnMinimumSizeWindowClosed(object sender, EventArgs args) => StopMinimumSizeTracking((Window)sender);

        private static void StopMinimumSizeTracking(Window window)
        {
            window.Closed -= OnMinimumSizeWindowClosed;
            MinWidthDescriptor.RemoveValueChanged(window, OnWindowMinimumChanged);
            MinHeightDescriptor.RemoveValueChanged(window, OnWindowMinimumChanged);
        }

        private static void ApplyMinimumSize(Window window)
        {
            if (!GetUseFluentChrome(window))
            {
                return;
            }

            var minimum = GetMinimumSize(window);
            if (window.MinWidth < minimum.Width)
            {
                window.SetCurrentValue(FrameworkElement.MinWidthProperty, minimum.Width);
            }
            if (window.MinHeight < minimum.Height)
            {
                window.SetCurrentValue(FrameworkElement.MinHeightProperty, minimum.Height);
            }
        }

        private static void OnUseFluentChromeChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            if (!(element is Window window))
            {
                return;
            }

            window.SourceInitialized -= OnWindowReady;
            window.Loaded -= OnWindowReady;
            StopMinimumSizeTracking(window);
            if (!(bool)args.NewValue)
            {
                var minimum = GetMinimumSize(window);
                if (minimum.Width > 0 && window.MinWidth == minimum.Width)
                {
                    window.InvalidateProperty(FrameworkElement.MinWidthProperty);
                }
                if (minimum.Height > 0 && window.MinHeight == minimum.Height)
                {
                    window.InvalidateProperty(FrameworkElement.MinHeightProperty);
                }
                return;
            }

            window.SourceInitialized += OnWindowReady;
            window.Loaded += OnWindowReady;
            StartMinimumSizeTracking(window);
            if (window.IsInitialized)
            {
                Apply(window);
            }
        }

        private static void OnWindowReady(object sender, EventArgs args)
        {
            if (!(sender is Window window) || !GetUseFluentChrome(window))
            {
                return;
            }

            StartMinimumSizeTracking(window);
            Apply(window);
            window.Dispatcher.BeginInvoke(new Action(() => Apply(window)), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private static void Apply(Window window)
        {
            if (!GetUseFluentChrome(window))
            {
                return;
            }

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
