using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AvalonDock.Controls;
using AvalonDock.Core;
using AvalonDock.Layout;
using Wpf.Ui.Appearance;

namespace AvalonDock.Themes.WPFUI.Controls
{
    internal sealed class DockToggleDragOverlay : Window
    {
        private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly Type OverlayType = typeof(DockingManager).Assembly.GetType("AvalonDock.Controls.ToggleDockDragOverlay");
        private static readonly FieldInfo ManagerField = OverlayType?.GetField("_manager", PrivateInstance);
        private static readonly FieldInfo AnchorableField = OverlayType?.GetField("_sourceAnchorable", PrivateInstance);
        private static readonly FieldInfo ZonesField = OverlayType?.GetField("_dropZones", PrivateInstance);
        private static readonly Type ZoneType = OverlayType?.GetNestedType("DropZone", BindingFlags.NonPublic);
        private static readonly FieldInfo RectField = ZoneType?.GetField("Rect");
        private static readonly FieldInfo ZoneField = ZoneType?.GetField("Zone");
        private static readonly FieldInfo LabelField = ZoneType?.GetField("Label");
        private static readonly FieldInfo LineStartField = ZoneType?.GetField("InsertionLineStart");
        private static readonly FieldInfo LineEndField = ZoneType?.GetField("InsertionLineEnd");
        private static readonly DependencyProperty OriginalOpacityProperty = DependencyProperty.RegisterAttached(
            "OriginalOpacity", typeof(double?), typeof(DockToggleDragOverlay), new PropertyMetadata(null));

        private readonly Window inputWindow;
        private readonly ToggleDockingManager manager;
        private readonly PreviewSurface surface;
        private readonly double originalOpacity;
        private readonly IList sourceZones;
        private readonly object[] originalZones;

        private DockToggleDragOverlay(Window inputWindow, ToggleDockingManager manager, LayoutAnchorable anchorable, List<DropZone> zones, IList sourceZones, object[] originalZones)
        {
            this.inputWindow = inputWindow;
            this.manager = manager;
            originalOpacity = (double?)inputWindow.GetValue(OriginalOpacityProperty) ?? inputWindow.Opacity;
            this.sourceZones = sourceZones;
            this.originalZones = originalZones;
            Owner = inputWindow;
            Left = inputWindow.Left;
            Top = inputWindow.Top;
            Width = inputWindow.Width;
            Height = inputWindow.Height;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
            ShowActivated = false;
            Topmost = true;
            Focusable = false;
            IsHitTestVisible = false;
            Resources = manager.Resources;
            surface = new PreviewSurface(manager, anchorable.Title, zones);
            Content = surface;
            DockOverlayWindowAssist.SetIsEnabled(this, true);
            inputWindow.MouseMove += OnInputMouseMove;
            inputWindow.IsVisibleChanged += OnInputVisibilityChanged;
            manager.IsVisibleChanged += OnManagerVisibilityChanged;
            manager.Unloaded += OnManagerUnloaded;
            ApplicationThemeManager.Changed += OnThemeChanged;
        }

        internal static void OnInputSizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (!(sender is Window input) || input.GetType() != OverlayType
                || input.GetValue(OriginalOpacityProperty) != null
                || !(ManagerField?.GetValue(input) is ToggleDockingManager manager)
                || !DockOverlayWindowAssist.GetIsEnabled(manager)
                || !TryGetZones(input, out _, out var zones) || zones.Count == 0)
            {
                return;
            }

            input.SetValue(OriginalOpacityProperty, input.Opacity);
            input.SetCurrentValue(OpacityProperty, 0d);
            input.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!input.OwnedWindows.OfType<DockToggleDragOverlay>().Any())
                {
                    RestorePendingOpacity(input);
                }
            }), DispatcherPriority.ContextIdle);
        }

        internal static void OnMouseCaptured(object sender, MouseEventArgs args)
        {
            if (!(sender is Window input) || input.GetType() != OverlayType || !ReferenceEquals(Mouse.Captured, input)
                || !(ManagerField?.GetValue(input) is ToggleDockingManager manager)
                || !DockOverlayWindowAssist.GetIsEnabled(manager))
            {
                return;
            }

            try
            {
                TryShow(input, manager);
            }
            finally
            {
                if (!input.OwnedWindows.OfType<DockToggleDragOverlay>().Any())
                {
                    RestorePendingOpacity(input);
                }
            }
        }

        private static void RestorePendingOpacity(Window input)
        {
            if (input.GetValue(OriginalOpacityProperty) is double opacity)
            {
                input.SetCurrentValue(OpacityProperty, opacity);
                input.ClearValue(OriginalOpacityProperty);
            }
        }

        internal static void CloseForManager(ToggleDockingManager manager)
        {
            if (Application.Current == null)
            {
                return;
            }

            foreach (var overlay in Application.Current.Windows.OfType<DockToggleDragOverlay>().Where(window => window.manager == manager).ToArray())
            {
                overlay.Close();
            }
        }

        private static void TryShow(Window input, ToggleDockingManager manager)
        {
            if (!input.IsVisible || !ReferenceEquals(Mouse.Captured, input) || !DockOverlayWindowAssist.GetIsEnabled(manager)
                || input.OwnedWindows.OfType<DockToggleDragOverlay>().Any()
                || !(AnchorableField?.GetValue(input) is LayoutAnchorable anchorable)
                || !TryGetZones(input, out var sourceZones, out var zones) || zones.Count == 0)
            {
                return;
            }

            var originalZones = sourceZones.Cast<object>().ToArray();
            AlignZones(input, manager, zones);
            for (var index = 0; index < zones.Count; index++)
            {
                var source = sourceZones[index];
                RectField.SetValue(source, zones[index].Bounds);
                LineStartField.SetValue(source, zones[index].LineStart);
                LineEndField.SetValue(source, zones[index].LineEnd);
                sourceZones[index] = source;
            }

            var overlay = new DockToggleDragOverlay(input, manager, anchorable, zones, sourceZones, originalZones);
            input.SetCurrentValue(OpacityProperty, 0d);
            try
            {
                overlay.Show();
            }
            catch
            {
                overlay.Close();
                throw;
            }
            if (!ReferenceEquals(Mouse.Captured, input))
            {
                overlay.Close();
                return;
            }

            overlay.surface.Update(Mouse.GetPosition(input));
        }

        private static bool TryGetZones(Window input, out IList sourceZones, out List<DropZone> zones)
        {
            sourceZones = ZonesField?.GetValue(input) as IList;
            zones = new List<DropZone>();
            if (sourceZones == null || sourceZones.IsReadOnly
                || !(AnchorableField?.GetValue(input) is LayoutAnchorable)
                || RectField?.FieldType != typeof(Rect) || ZoneField?.FieldType != typeof(DockZone)
                || LabelField?.FieldType != typeof(string) || LineStartField?.FieldType != typeof(Point?)
                || LineEndField?.FieldType != typeof(Point?))
            {
                return false;
            }

            foreach (var source in sourceZones)
            {
                if (source == null || source.GetType() != ZoneType)
                {
                    return false;
                }

                zones.Add(new DropZone
                {
                    Bounds = (Rect)RectField.GetValue(source),
                    Zone = (DockZone)ZoneField.GetValue(source),
                    Description = (string)LabelField.GetValue(source),
                    IsSidebar = LabelField.GetValue(source) == null,
                    LineStart = (Point?)LineStartField.GetValue(source),
                    LineEnd = (Point?)LineEndField.GetValue(source)
                });
            }
            return true;
        }

        private static void AlignZones(Window input, ToggleDockingManager manager, List<DropZone> zones)
        {
            if (!(manager.LayoutRootPanel is FrameworkElement root) || !root.IsVisible)
            {
                return;
            }

            var content = BoundsInWindow(root, input);
            if (content.IsEmpty || content.Width <= 0 || content.Height <= 0)
            {
                return;
            }

            var leftWidth = content.Width / 4;
            var rightWidth = content.Width / 4;
            var bottomHeight = content.Height / 4;
            var sideBottom = double.NaN;
            foreach (var pane in VisualChildren<LayoutAnchorablePaneControl>(root).Where(control => control.IsVisible && control.HasItems))
            {
                var bounds = BoundsInWindow(pane, input);
                switch (((LayoutAnchorablePane)pane.Model).GetSide())
                {
                    case AnchorSide.Left:
                        leftWidth = bounds.Width;
                        break;
                    case AnchorSide.Right:
                        rightWidth = bounds.Width;
                        break;
                    case AnchorSide.Bottom:
                        bottomHeight = content.Bottom - bounds.Top;
                        sideBottom = bounds.Top - manager.GridSplitterHeight;
                        break;
                }
            }

            leftWidth = Math.Min(leftWidth, content.Width);
            rightWidth = Math.Min(rightWidth, content.Width);
            bottomHeight = Math.Min(bottomHeight, content.Height);
            var bottomTop = content.Bottom - bottomHeight;
            var sideHeight = Math.Max(0, (double.IsNaN(sideBottom) ? bottomTop : sideBottom) - content.Top) / 2;

            foreach (var zone in zones)
            {
                if (zone.IsSidebar)
                {
                    var right = zone.Zone == DockZone.RightTop || zone.Zone == DockZone.RightBottom || zone.Zone == DockZone.BottomRight;
                    var navigation = manager.Template.FindName(right ? "RightNavigationSurface" : "LeftNavigationSurface", manager) as Canvas;
                    if (navigation == null)
                    {
                        continue;
                    }

                    if (!navigation.Children.OfType<Border>().Any(frame => frame.IsVisible))
                    {
                        zone.Bounds = Rect.Empty;
                        zone.LineStart = null;
                        zone.LineEnd = null;
                        continue;
                    }

                    var sidebar = BoundsInWindow(navigation, input);
                    var bounds = zone.Bounds;
                    bounds.X = sidebar.Left;
                    bounds.Width = sidebar.Width;
                    bounds.Intersect(sidebar);
                    zone.Bounds = bounds;
                    if (zone.LineStart.HasValue && zone.LineEnd.HasValue)
                    {
                        zone.LineStart = new Point(sidebar.Left + 6, zone.LineStart.Value.Y);
                        zone.LineEnd = new Point(sidebar.Right - 6, zone.LineEnd.Value.Y);
                    }
                    continue;
                }

                switch (zone.Zone)
                {
                    case DockZone.LeftTop:
                        zone.Bounds = new Rect(content.Left, content.Top, leftWidth, sideHeight);
                        break;
                    case DockZone.LeftBottom:
                        zone.Bounds = new Rect(content.Left, content.Top + sideHeight, leftWidth, sideHeight);
                        break;
                    case DockZone.RightTop:
                        zone.Bounds = new Rect(content.Right - rightWidth, content.Top, rightWidth, sideHeight);
                        break;
                    case DockZone.RightBottom:
                        zone.Bounds = new Rect(content.Right - rightWidth, content.Top + sideHeight, rightWidth, sideHeight);
                        break;
                    case DockZone.BottomLeft:
                        zone.Bounds = new Rect(content.Left, bottomTop, content.Width / 2, bottomHeight);
                        break;
                    case DockZone.BottomRight:
                        zone.Bounds = new Rect(content.Left + content.Width / 2, bottomTop, content.Width / 2, bottomHeight);
                        break;
                }
            }
        }

        private static Rect BoundsInWindow(FrameworkElement element, Window window) =>
            new Rect(window.PointFromScreen(element.PointToScreen(new Point())),
                window.PointFromScreen(element.PointToScreen(new Point(element.ActualWidth, element.ActualHeight))));

        private static IEnumerable<T> VisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            for (var index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
            {
                var child = VisualTreeHelper.GetChild(parent, index);
                if (child is T match)
                {
                    yield return match;
                }
                foreach (var descendant in VisualChildren<T>(child))
                {
                    yield return descendant;
                }
            }
        }

        private void OnInputMouseMove(object sender, MouseEventArgs args) => surface.Update(args.GetPosition(inputWindow));

        private void OnInputVisibilityChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (!(bool)args.NewValue)
            {
                Close();
            }
        }

        private void OnManagerVisibilityChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (!(bool)args.NewValue)
            {
                CancelDrag();
            }
        }

        private void OnManagerUnloaded(object sender, RoutedEventArgs args) => CancelDrag();

        private void CancelDrag()
        {
            if (inputWindow.IsVisible)
            {
                inputWindow.Close();
            }
            else if (IsVisible)
            {
                Close();
            }
        }

        private void OnThemeChanged(ApplicationTheme theme, Color accent) =>
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (IsVisible)
                {
                    surface.Update(Mouse.GetPosition(inputWindow));
                }
            }), DispatcherPriority.Render);

        protected override void OnClosed(EventArgs args)
        {
            inputWindow.MouseMove -= OnInputMouseMove;
            inputWindow.IsVisibleChanged -= OnInputVisibilityChanged;
            manager.IsVisibleChanged -= OnManagerVisibilityChanged;
            manager.Unloaded -= OnManagerUnloaded;
            ApplicationThemeManager.Changed -= OnThemeChanged;
            if (inputWindow.IsVisible)
            {
                for (var index = 0; index < originalZones.Length; index++)
                {
                    sourceZones[index] = originalZones[index];
                }
            }
            inputWindow.SetCurrentValue(OpacityProperty, originalOpacity);
            inputWindow.ClearValue(OriginalOpacityProperty);
            base.OnClosed(args);
        }

        private sealed class DropZone
        {
            internal Rect Bounds;
            internal DockZone Zone;
            internal string Description;
            internal bool IsSidebar;
            internal Point? LineStart;
            internal Point? LineEnd;
            internal TextBlock Label;
        }

        private sealed class PreviewSurface : Canvas
        {
            private readonly ToggleDockingManager manager;
            private readonly List<DropZone> zones;
            private readonly Border dragLabel;
            private DropZone hoveredZone;

            internal PreviewSurface(ToggleDockingManager manager, string title, List<DropZone> zones)
            {
                this.manager = manager;
                this.zones = zones;
                ClipToBounds = true;
                SnapsToDevicePixels = true;
                UseLayoutRounding = true;

                foreach (var zone in zones.Where(candidate => !candidate.IsSidebar))
                {
                    zone.Label = new TextBlock
                    {
                        Text = zone.Description,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Center,
                        TextTrimming = TextTrimming.CharacterEllipsis
                    };
                    zone.Label.SetResourceReference(TextBlock.FontFamilyProperty, "DockFontFamily");
                    zone.Label.SetResourceReference(TextBlock.FontSizeProperty, "DockFontSize");
                    zone.Label.SetResourceReference(TextBlock.ForegroundProperty, "DockSecondaryTextBrush");
                    var area = new Border { Width = zone.Bounds.Width, Height = zone.Bounds.Height, Padding = new Thickness(8), Child = zone.Label };
                    SetLeft(area, zone.Bounds.Left);
                    SetTop(area, zone.Bounds.Top);
                    Children.Add(area);
                }

                var text = new TextBlock { Text = title, MaxWidth = 160, TextTrimming = TextTrimming.CharacterEllipsis, VerticalAlignment = VerticalAlignment.Center };
                text.SetResourceReference(TextBlock.FontFamilyProperty, "DockFontFamily");
                text.SetResourceReference(TextBlock.FontSizeProperty, "DockFontSize");
                text.SetResourceReference(TextBlock.ForegroundProperty, "DockTextBrush");
                dragLabel = new Border { Height = 28, Padding = new Thickness(10, 0, 10, 0), BorderThickness = new Thickness(1), Child = text };
                dragLabel.SetResourceReference(Border.BackgroundProperty, "DockSurfaceBrush");
                dragLabel.SetResourceReference(Border.BorderBrushProperty, "DockBorderBrush");
                dragLabel.SetResourceReference(Border.CornerRadiusProperty, "DockControlCornerRadius");
                Children.Add(dragLabel);
            }

            internal void Update(Point position)
            {
                hoveredZone = null;
                for (var index = zones.Count - 1; index >= 0; index--)
                {
                    if (zones[index].Bounds.Contains(position))
                    {
                        hoveredZone = zones[index];
                        break;
                    }
                }

                foreach (var zone in zones)
                {
                    if (zone.Label != null)
                    {
                        var isTargeted = hoveredZone != null && hoveredZone.Zone == zone.Zone;
                        zone.Label.SetResourceReference(TextBlock.ForegroundProperty, isTargeted ? "DockTextBrush" : "DockSecondaryTextBrush");
                        zone.Label.FontWeight = isTargeted ? FontWeights.SemiBold : FontWeights.Normal;
                    }
                }

                dragLabel.Measure(new Size(double.PositiveInfinity, 28));
                SetLeft(dragLabel, Math.Max(4, Math.Min(position.X + 16, ActualWidth - dragLabel.DesiredSize.Width - 4)));
                SetTop(dragLabel, Math.Max(4, Math.Min(position.Y - 14, ActualHeight - 32)));
                InvalidateVisual();
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);
                var border = new Pen(Brush("DockBorderBrush"), 1) { DashStyle = new DashStyle(new[] { 4d, 4d }, 0) };
                var previewBorder = new Pen(Brush("DockPreviewBorderBrush"), 1);
                var radius = manager.TryFindResource("DockPaneCornerRadius") is CornerRadius corners ? corners.TopLeft : 4;

                foreach (var zone in zones)
                {
                    var isHovered = ReferenceEquals(zone, hoveredZone);
                    if (zone.Bounds.IsEmpty || (zone.IsSidebar && !isHovered))
                    {
                        continue;
                    }

                    var bounds = zone.Bounds;
                    bounds.Inflate(-Math.Min(4, bounds.Width / 4), -Math.Min(4, bounds.Height / 4));
                    if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
                    {
                        continue;
                    }

                    drawingContext.PushOpacity(0.65);
                    drawingContext.DrawRoundedRectangle(Brush("DockChromeBrush"), null, bounds, radius, radius);
                    drawingContext.Pop();
                    drawingContext.DrawRoundedRectangle(isHovered ? Brush("DockPreviewBrush") : null,
                        isHovered ? previewBorder : border, bounds, radius, radius);
                    if (isHovered && zone.LineStart.HasValue && zone.LineEnd.HasValue)
                    {
                        var insertion = new Pen(Brush("AccentFillColorDefaultBrush"), 2) { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
                        drawingContext.DrawLine(insertion, zone.LineStart.Value, zone.LineEnd.Value);
                    }
                }
            }

            private Brush Brush(string key) => manager.TryFindResource(key) as Brush ?? Brushes.Transparent;
        }
    }
}