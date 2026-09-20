using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Wpf.Ui.Appearance;

namespace AvalonDock.Themes.WPFUI.Controls
{
    public static class DockOverlayWindowAssist
    {
        private const int DwmwaSystemBackdropType = 38;
        private const int DwmwaMicaEffect = 1029;
        private const int DwmsbtNone = 1;

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(DockOverlayWindowAssist),
            new PropertyMetadata(false, OnIsEnabledChanged));

        static DockOverlayWindowAssist()
        {
            ApplicationThemeManager.Changed += (theme, accent) =>
            {
                if (Application.Current == null)
                {
                    return;
                }

                foreach (Window window in Application.Current.Windows)
                {
                    if (GetIsEnabled(window) && window.IsVisible)
                    {
                        QueueApply(window);
                    }
                }
            };
        }

        public static bool GetIsEnabled(DependencyObject element) =>
            (bool)element.GetValue(IsEnabledProperty);

        public static void SetIsEnabled(DependencyObject element, bool value) =>
            element.SetValue(IsEnabledProperty, value);

        private static void OnIsEnabledChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            if (!(element is Window window))
            {
                return;
            }

            window.Loaded -= OnWindowLoaded;
            window.IsVisibleChanged -= OnIsVisibleChanged;
            if (!(bool)args.NewValue)
            {
                return;
            }

            window.Loaded += OnWindowLoaded;
            window.IsVisibleChanged += OnIsVisibleChanged;
            Refresh(window);
        }

        private static void OnWindowLoaded(object sender, RoutedEventArgs args) => Refresh((Window)sender);

        private static void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args) => Refresh((Window)sender);

        private static void Refresh(Window window)
        {
            if (!window.IsVisible)
            {
                return;
            }

            Apply(window);
            QueueApply(window);
        }

        private static void QueueApply(Window window) =>
            window.Dispatcher.BeginInvoke(new Action(() => Apply(window)), DispatcherPriority.Loaded);

        private static void Apply(Window window)
        {
            if (!GetIsEnabled(window) || !window.IsVisible || !window.AllowsTransparency)
            {
                return;
            }

            var handle = new WindowInteropHelper(window).Handle;
            if (handle != IntPtr.Zero)
            {
                var backdrop = DwmsbtNone;
                if (DwmSetWindowAttribute(handle, DwmwaSystemBackdropType, ref backdrop, sizeof(int)) < 0)
                {
                    var disabled = 0;
                    DwmSetWindowAttribute(handle, DwmwaMicaEffect, ref disabled, sizeof(int));
                }
            }
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);
    }
}