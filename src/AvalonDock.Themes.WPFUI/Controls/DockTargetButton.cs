using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AvalonDock.Controls;

namespace AvalonDock.Themes.WPFUI.Controls;

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

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out PointI lpPoint);

    public static readonly DependencyProperty TargetDockProperty;
    public static readonly DependencyProperty CornerRadiusProperty;
    public static readonly DependencyProperty GlyphBorderBrushProperty;
    public static readonly DependencyProperty GlyphBackgroundProperty;
    public static readonly DependencyProperty OuterBorderBrushProperty;
    public static readonly DependencyProperty OuterBackgroundProperty;
    public static readonly DependencyProperty GlyphArrowBrushProperty;
    public static readonly DependencyProperty IsTargetedProperty;
    public static readonly DependencyProperty IsOuterProperty;

    private static readonly List<DockTargetButton> dockTargets = new();

    private static Path? previewBox;
    private static DockTargetButton? current;

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
        if (previewBox is null)
        {
            if (sender is DockTargetButton dockTargetButton && dockTargetButton.TemplatedParent is OverlayWindow overlayWindow)
            {
                if (overlayWindow.Template.FindName("PART_PreviewBox", overlayWindow) is Path element)
                {
                    previewBox = element;
                    previewBox.IsVisibleChanged += OnIsVisibleChanged;
                }
            }
        }

        dockTargets.Add(this);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (previewBox is not null)
        {
            previewBox.IsVisibleChanged -= OnIsVisibleChanged;
            previewBox = null;
        }

        dockTargets.Remove(this);
    }

    private static void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue)
        {
            foreach (DockTargetButton item in dockTargets)
            {
                Rect rect = new(0,
                                0,
                                item.RenderSize.Width + 2,
                                item.RenderSize.Height + 2);

                Point point = item.PointFromScreen(GetMousePosition());

                if (rect.Contains(point))
                {
                    current = item;

                    current.IsTargeted = true;

                    return;
                }
            }
        }
        else
        {
            if (current is not null)
            {
                current.IsTargeted = false;

                current = null;
            }
        }
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
