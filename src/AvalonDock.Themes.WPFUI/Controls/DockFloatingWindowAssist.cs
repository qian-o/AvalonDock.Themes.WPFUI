using System;
using System.ComponentModel;
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

        private static readonly DependencyPropertyDescriptor MinWidthDescriptor =
            DependencyPropertyDescriptor.FromProperty(FrameworkElement.MinWidthProperty, typeof(Window));
        private static readonly DependencyPropertyDescriptor MinHeightDescriptor =
            DependencyPropertyDescriptor.FromProperty(FrameworkElement.MinHeightProperty, typeof(Window));

        public static readonly DependencyProperty MinimumSizeProperty = DependencyProperty.RegisterAttached(
            "MinimumSize",
            typeof(Size),
            typeof(DockFloatingWindowAssist),
            new PropertyMetadata(new Size(0, 0), OnMinimumSizeChanged),
            value => value is Size size && !size.IsEmpty &&
                !double.IsNaN(size.Width) && !double.IsInfinity(size.Width) &&
                !double.IsNaN(size.Height) && !double.IsInfinity(size.Height));

        public static Size GetMinimumSize(DependencyObject element) =>
            (Size)element.GetValue(MinimumSizeProperty);

        public static void SetMinimumSize(DependencyObject element, Size value) =>
            element.SetValue(MinimumSizeProperty, value);

        public static readonly DependencyProperty UseFluentChromeProperty = DependencyProperty.RegisterAttached(
            "UseFluentChrome",
            typeof(bool),
            typeof(DockFloatingWindowAssist),
            new PropertyMetadata(false, OnUseFluentChromeChanged));

        public static bool GetUseFluentChrome(DependencyObject element) =>
            (bool)element.GetValue(UseFluentChromeProperty);

        public static void SetUseFluentChrome(DependencyObject element, bool value) =>
            element.SetValue(UseFluentChromeProperty, value);

        private static void OnMinimumSizeChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            if (!(element is Window window))
            {
                return;
            }

            StopMinimumSizeTracking(window);
            var previous = (Size)args.OldValue;
            if (previous.Width > 0 && window.MinWidth == previous.Width)
            {
                window.InvalidateProperty(FrameworkElement.MinWidthProperty);
            }
            if (previous.Height > 0 && window.MinHeight == previous.Height)
            {
                window.InvalidateProperty(FrameworkElement.MinHeightProperty);
            }

            if ((Size)args.NewValue == new Size(0, 0))
            {
                return;
            }

            window.SourceInitialized += OnMinimumSizeWindowReady;
            window.Closed += OnMinimumSizeWindowClosed;
            if (new WindowInteropHelper(window).Handle != IntPtr.Zero)
            {
                OnMinimumSizeWindowReady(window, EventArgs.Empty);
            }
            else
            {
                ApplyMinimumSize(window);
            }
        }

        private static void OnMinimumSizeWindowReady(object sender, EventArgs args)
        {
            var window = (Window)sender;
            MinWidthDescriptor.RemoveValueChanged(window, OnWindowMinimumChanged);
            MinHeightDescriptor.RemoveValueChanged(window, OnWindowMinimumChanged);
            MinWidthDescriptor.AddValueChanged(window, OnWindowMinimumChanged);
            MinHeightDescriptor.AddValueChanged(window, OnWindowMinimumChanged);
            ApplyMinimumSize(window);
        }

        private static void OnWindowMinimumChanged(object sender, EventArgs args) => ApplyMinimumSize((Window)sender);

        private static void OnMinimumSizeWindowClosed(object sender, EventArgs args) => StopMinimumSizeTracking((Window)sender);

        private static void StopMinimumSizeTracking(Window window)
        {
            window.SourceInitialized -= OnMinimumSizeWindowReady;
            window.Closed -= OnMinimumSizeWindowClosed;
            MinWidthDescriptor.RemoveValueChanged(window, OnWindowMinimumChanged);
            MinHeightDescriptor.RemoveValueChanged(window, OnWindowMinimumChanged);
        }

        private static void ApplyMinimumSize(Window window)
        {
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
            if (!(bool)args.NewValue)
            {
                return;
            }

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
