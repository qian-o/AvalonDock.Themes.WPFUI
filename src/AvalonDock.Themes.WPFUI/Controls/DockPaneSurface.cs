using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AvalonDock.Themes.WPFUI.Controls
{
    public class DockPaneSurface : FrameworkElement
    {
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            nameof(Fill), typeof(Brush), typeof(DockPaneSurface), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            nameof(Stroke), typeof(Brush), typeof(DockPaneSurface), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty ContentPanelProperty = DependencyProperty.Register(
            nameof(ContentPanel), typeof(FrameworkElement), typeof(DockPaneSurface), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius), typeof(double), typeof(DockPaneSurface), new FrameworkPropertyMetadata(4.0, FrameworkPropertyMetadataOptions.AffectsRender));

        private Rect contentBounds = Rect.Empty;
        private Rect tabBounds = Rect.Empty;
        private bool bottomTab;

        public DockPaneSurface()
        {
            IsHitTestVisible = false;
            ClipToBounds = true;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        public FrameworkElement ContentPanel
        {
            get => (FrameworkElement)GetValue(ContentPanelProperty);
            set => SetValue(ContentPanelProperty, value);
        }

        public double CornerRadius
        {
            get => (double)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            LayoutUpdated += OnLayoutUpdated;
            OnLayoutUpdated(this, EventArgs.Empty);
        }

        private void OnUnloaded(object sender, RoutedEventArgs args)
        {
            LayoutUpdated -= OnLayoutUpdated;
            contentBounds = Rect.Empty;
            tabBounds = Rect.Empty;
            InvalidateVisual();
        }

        private void OnLayoutUpdated(object sender, EventArgs args)
        {
            var nextContent = Rect.Empty;
            var nextTab = Rect.Empty;
            var nextBottom = false;
            if (TemplatedParent is TabControl pane && pane.HasItems && IsVisible
                && RenderSize.Width > 1 && RenderSize.Height > 1
                && ContentPanel != null && ContentPanel.IsVisible)
            {
                nextContent = new Rect(ContentPanel.TranslatePoint(new Point(), this), ContentPanel.RenderSize);
                nextContent.Intersect(new Rect(RenderSize));
                nextBottom = pane.TabStripPlacement == Dock.Bottom;
                if (pane.SelectedItem != null && pane.ItemContainerGenerator.ContainerFromItem(pane.SelectedItem) is TabItem tab && tab.IsVisible)
                {
                    nextTab = new Rect(tab.TranslatePoint(new Point(), this), tab.RenderSize);
                }
            }

            if (nextContent != contentBounds || nextTab != tabBounds || nextBottom != bottomTab)
            {
                contentBounds = nextContent;
                tabBounds = nextTab;
                bottomTab = nextBottom;
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (RenderSize.Width <= 1 || RenderSize.Height <= 1
                || contentBounds.IsEmpty || contentBounds.Width <= 1 || contentBounds.Height <= 1)
            {
                return;
            }

            var body = contentBounds;
            var tab = tabBounds;
            if (bottomTab)
            {
                body.Y = ActualHeight - body.Bottom;
                if (!tab.IsEmpty)
                {
                    tab.Y = ActualHeight - tab.Bottom;
                }
            }
            body.Inflate(-0.5, -0.5);
            var radius = Math.Max(0, Math.Min(CornerRadius, Math.Min(body.Width, body.Height) / 2));
            var outline = CreateOutline(body, tab, radius);
            if (bottomTab)
            {
                outline.Transform = new MatrixTransform(1, 0, 0, -1, 0, ActualHeight);
            }
            outline.Freeze();
            drawingContext.DrawGeometry(Fill, new Pen(Stroke, 1), outline);
        }

        private static Geometry CreateOutline(Rect body, Rect tab, double radius)
        {
            var tabLeft = tab.IsEmpty ? 0 : Math.Max(body.Left, tab.Left + 0.5);
            var tabRight = tab.IsEmpty ? 0 : Math.Min(body.Right - radius * 2, tab.Right - 0.5);
            var tabTop = tab.IsEmpty ? 0 : tab.Top + 0.5;
            if (tab.IsEmpty || tabRight - tabLeft < radius * 2 || body.Top - tabTop < radius * 2)
            {
                return new RectangleGeometry(body, radius, radius);
            }

            var touchesLeftEdge = tabLeft - body.Left < radius * 2;
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                if (touchesLeftEdge)
                {
                    context.BeginFigure(new Point(tabLeft + radius, tabTop), true, true);
                }
                else
                {
                    context.BeginFigure(new Point(body.Left + radius, body.Top), true, true);
                    context.LineTo(new Point(tabLeft - radius, body.Top), true, false);
                    context.QuadraticBezierTo(new Point(tabLeft, body.Top), new Point(tabLeft, body.Top - radius), true, false);
                    context.LineTo(new Point(tabLeft, tabTop + radius), true, false);
                    context.QuadraticBezierTo(new Point(tabLeft, tabTop), new Point(tabLeft + radius, tabTop), true, false);
                }
                context.LineTo(new Point(tabRight - radius, tabTop), true, false);
                context.QuadraticBezierTo(new Point(tabRight, tabTop), new Point(tabRight, tabTop + radius), true, false);
                context.LineTo(new Point(tabRight, body.Top - radius), true, false);
                context.QuadraticBezierTo(new Point(tabRight, body.Top), new Point(tabRight + radius, body.Top), true, false);
                context.LineTo(new Point(body.Right - radius, body.Top), true, false);
                context.QuadraticBezierTo(body.TopRight, new Point(body.Right, body.Top + radius), true, false);
                context.LineTo(new Point(body.Right, body.Bottom - radius), true, false);
                context.QuadraticBezierTo(body.BottomRight, new Point(body.Right - radius, body.Bottom), true, false);
                context.LineTo(new Point(body.Left + radius, body.Bottom), true, false);
                context.QuadraticBezierTo(body.BottomLeft, new Point(body.Left, body.Bottom - radius), true, false);
                if (touchesLeftEdge)
                {
                    var leftRadius = Math.Min(radius, tabLeft - body.Left);
                    context.LineTo(new Point(body.Left, body.Top + leftRadius), true, false);
                    context.QuadraticBezierTo(body.TopLeft, new Point(body.Left + leftRadius, body.Top), true, false);
                    context.LineTo(new Point(tabLeft, body.Top), true, false);
                    context.LineTo(new Point(tabLeft, tabTop + radius), true, false);
                    context.QuadraticBezierTo(new Point(tabLeft, tabTop), new Point(tabLeft + radius, tabTop), true, false);
                }
                else
                {
                    context.LineTo(new Point(body.Left, body.Top + radius), true, false);
                    context.QuadraticBezierTo(body.TopLeft, new Point(body.Left + radius, body.Top), true, false);
                }
            }
            return geometry;
        }
    }
}