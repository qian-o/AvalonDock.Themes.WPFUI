using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Docking;

public class AnchorableDocument : ContentControl
{
    private const string PartDragThumb = "PART_DragThumb";
    private const string PartCloseButton = "PART_CloseButton";

    public static readonly DependencyProperty TitleProperty;
    public static readonly DependencyProperty CanCloseProperty;

    static AnchorableDocument()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(AnchorableDocument),
                                                 new FrameworkPropertyMetadata(typeof(AnchorableDocument)));

        TitleProperty = DependencyProperty.Register(nameof(Title),
                                                    typeof(object),
                                                    typeof(AnchorableDocument),
                                                    new PropertyMetadata(null));

        CanCloseProperty = DependencyProperty.Register(nameof(CanClose),
                                                      typeof(bool),
                                                      typeof(AnchorableDocument),
                                                      new PropertyMetadata(true));
    }

    private Thumb dragThumb = null!;
    private Button closeButton = null!;

    public object? Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

    public bool CanClose { get => (bool)GetValue(CanCloseProperty); set => SetValue(CanCloseProperty, value); }

    public override void OnApplyTemplate()
    {
        dragThumb = (Thumb)GetTemplateChild(PartDragThumb);
        closeButton = (Button)GetTemplateChild(PartCloseButton);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        Focus();
    }
}
