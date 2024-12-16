using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Docking;

public class AnchorableDocument : ContentControl
{
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

    public object? Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

    public bool CanClose { get => (bool)GetValue(CanCloseProperty); set => SetValue(CanCloseProperty, value); }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        Focus();
    }
}
