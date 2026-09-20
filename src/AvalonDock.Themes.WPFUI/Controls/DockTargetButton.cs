using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AvalonDock.Controls;

namespace AvalonDock.Themes.WPFUI.Controls
{
    public enum DockTarget
    {
        Center,
        SplitLeft,
        SplitTop,
        SplitRight,
        SplitBottom,
        DockLeft,
        DockTop,
        DockRight,
        DockBottom
    }

    public partial class DockTargetButton : Button
    {
        private struct PointI
        {
            public int X;

            public int Y;
        }

#if NET8_0_OR_GREATER
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool GetCursorPos(out PointI lpPoint);
#else
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out PointI lpPoint);
#endif

        public static readonly DependencyProperty TargetDockProperty;
        public static readonly DependencyProperty CornerRadiusProperty;
        public static readonly DependencyProperty GlyphBorderBrushProperty;
        public static readonly DependencyProperty GlyphBackgroundProperty;
        public static readonly DependencyProperty OuterBorderBrushProperty;
        public static readonly DependencyProperty OuterBackgroundProperty;
        public static readonly DependencyProperty GlyphArrowBrushProperty;
        public static readonly DependencyProperty IsTargetedProperty;
        public static readonly DependencyProperty IsOuterProperty;

        private static readonly Dictionary<OverlayWindow, OverlayState> overlayStates = new Dictionary<OverlayWindow, OverlayState>();

        private OverlayWindow overlayWindow;

        private sealed class OverlayState
        {
            private readonly Path previewBox;
            private readonly List<DockTargetButton> targets = new List<DockTargetButton>();
            private DockTargetButton currentTarget;

            public OverlayState(Path previewBox)
            {
                this.previewBox = previewBox;
                previewBox.IsVisibleChanged += OnPreviewBoxVisibleChanged;
            }

            public void Add(DockTargetButton target)
            {
                targets.Add(target);
            }

            public void Remove(DockTargetButton target)
            {
                if (currentTarget == target)
                {
                    ClearCurrentTarget();
                }

                targets.Remove(target);
            }

            public bool IsEmpty => targets.Count == 0;

            public void Dispose()
            {
                ClearCurrentTarget();
                previewBox.IsVisibleChanged -= OnPreviewBoxVisibleChanged;
            }

            private void OnPreviewBoxVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                if ((bool)e.NewValue)
                {
                    UpdateCurrentTarget();
                }
                else
                {
                    ClearCurrentTarget();
                }
            }

            private void UpdateCurrentTarget()
            {
                ClearCurrentTarget();
                var mousePosition = GetMousePosition();

                foreach (var target in targets)
                {
                    if (!target.IsVisible || !target.IsEnabled)
                    {
                        continue;
                    }

                    var rect = new Rect(0, 0, target.RenderSize.Width + 2, target.RenderSize.Height + 2);
                    if (rect.Contains(target.PointFromScreen(mousePosition)))
                    {
                        currentTarget = target;
                        currentTarget.IsTargeted = true;
                        return;
                    }
                }
            }

            private void ClearCurrentTarget()
            {
                if (currentTarget != null)
                {
                    currentTarget.IsTargeted = false;
                    currentTarget = null;
                }
            }
        }

        static DockTargetButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockTargetButton), new FrameworkPropertyMetadata(typeof(DockTargetButton)));

            TargetDockProperty = DependencyProperty.Register(nameof(TargetDock),
                                                             typeof(DockTarget),
                                                             typeof(DockTargetButton),
                                                             new FrameworkPropertyMetadata(DockTarget.Center));

            CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius),
                                                               typeof(CornerRadius),
                                                               typeof(DockTargetButton),
                                                               new FrameworkPropertyMetadata(new CornerRadius(0)));

            GlyphBorderBrushProperty = DependencyProperty.Register(nameof(GlyphBorderBrush),
                                                                   typeof(Brush),
                                                                   typeof(DockTargetButton),
                                                                   new FrameworkPropertyMetadata(Brushes.Transparent));

            GlyphBackgroundProperty = DependencyProperty.Register(nameof(GlyphBackground),
                                                                  typeof(Brush),
                                                                  typeof(DockTargetButton),
                                                                  new FrameworkPropertyMetadata(Brushes.Transparent));

            OuterBorderBrushProperty = DependencyProperty.Register(nameof(OuterBorderBrush),
                                                                   typeof(Brush),
                                                                   typeof(DockTargetButton),
                                                                   new FrameworkPropertyMetadata(Brushes.Transparent));

            OuterBackgroundProperty = DependencyProperty.Register(nameof(OuterBackground),
                                                                  typeof(Brush),
                                                                  typeof(DockTargetButton),
                                                                  new FrameworkPropertyMetadata(Brushes.Transparent));

            GlyphArrowBrushProperty = DependencyProperty.Register(nameof(GlyphArrowBrush),
                                                                  typeof(Brush),
                                                                  typeof(DockTargetButton),
                                                                  new FrameworkPropertyMetadata(Brushes.Transparent));

            IsTargetedProperty = DependencyProperty.Register(nameof(IsTargeted),
                                                             typeof(bool),
                                                             typeof(DockTargetButton),
                                                             new FrameworkPropertyMetadata(false));

            IsOuterProperty = DependencyProperty.Register(nameof(IsOuter),
                                                          typeof(bool),
                                                          typeof(DockTargetButton),
                                                          new FrameworkPropertyMetadata(false));
        }

        public DockTargetButton()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public DockTarget TargetDock
        {
            get => (DockTarget)GetValue(TargetDockProperty);
            set => SetValue(TargetDockProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public Brush GlyphBorderBrush
        {
            get => (Brush)GetValue(GlyphBorderBrushProperty);
            set => SetValue(GlyphBorderBrushProperty, value);
        }

        public Brush GlyphBackground
        {
            get => (Brush)GetValue(GlyphBackgroundProperty);
            set => SetValue(GlyphBackgroundProperty, value);
        }

        public Brush GlyphArrowBrush
        {
            get => (Brush)GetValue(GlyphArrowBrushProperty);
            set => SetValue(GlyphArrowBrushProperty, value);
        }

        public Brush OuterBorderBrush
        {
            get => (Brush)GetValue(OuterBorderBrushProperty);
            set => SetValue(OuterBorderBrushProperty, value);
        }

        public Brush OuterBackground
        {
            get => (Brush)GetValue(OuterBackgroundProperty);
            set => SetValue(OuterBackgroundProperty, value);
        }

        public bool IsTargeted
        {
            get => (bool)GetValue(IsTargetedProperty);
            set => SetValue(IsTargetedProperty, value);
        }

        public bool IsOuter
        {
            get => (bool)GetValue(IsOuterProperty);
            set => SetValue(IsOuterProperty, value);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            overlayWindow = TemplatedParent as OverlayWindow;
            if (overlayWindow == null || !(overlayWindow.Template.FindName("PART_PreviewBox", overlayWindow) is Path previewBox))
            {
                return;
            }

            if (!overlayStates.TryGetValue(overlayWindow, out var state))
            {
                state = new OverlayState(previewBox);
                overlayStates.Add(overlayWindow, state);
            }

            state.Add(this);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (overlayWindow == null || !overlayStates.TryGetValue(overlayWindow, out var state))
            {
                overlayWindow = null;
                return;
            }

            state.Remove(this);
            if (state.IsEmpty)
            {
                state.Dispose();
                overlayStates.Remove(overlayWindow);
            }

            overlayWindow = null;
        }

        private static Point GetMousePosition()
        {
            if (GetCursorPos(out PointI point))
            {
                return new Point(point.X, point.Y);
            }

            return default;
        }
    }
}
