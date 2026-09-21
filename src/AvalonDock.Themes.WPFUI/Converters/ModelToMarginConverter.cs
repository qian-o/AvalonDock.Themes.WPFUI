using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using AvalonDock.Layout;

namespace AvalonDock.Themes.WPFUI.Converters
{
    [ValueConversion(typeof(LayoutAnchorable), typeof(Thickness))]
    public class ModelToMarginConverter : MarkupExtension, IValueConverter
    {
        private static readonly ModelToMarginConverter converter = new ModelToMarginConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LayoutAnchorable layoutAnchorable
                && layoutAnchorable.FindParent<LayoutAnchorSide>() is LayoutAnchorSide layoutAnchorSide
                && layoutAnchorable.Root?.Manager is DockingManager manager)
            {
                switch (layoutAnchorSide.Side)
                {
                    case AnchorSide.Left:
                        return new Thickness(0, 0, -manager.GridSplitterWidth, 0);
                    case AnchorSide.Top:
                        return new Thickness(0, 0, 0, -manager.GridSplitterHeight);
                    case AnchorSide.Right:
                        return new Thickness(-manager.GridSplitterWidth, 0, 0, 0);
                    case AnchorSide.Bottom:
                        return new Thickness(0, -manager.GridSplitterHeight, 0, 0);
                }
            }

            return new Thickness();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return converter;
        }
    }
}