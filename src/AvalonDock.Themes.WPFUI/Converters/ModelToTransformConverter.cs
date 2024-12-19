using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using AvalonDock.Layout;

namespace AvalonDock.Themes.WPFUI.Converters
{
    [ValueConversion(typeof(LayoutAnchorable), typeof(Transform))]
    public class ModelToTransformConverter : MarkupExtension, IValueConverter
    {
        private static ModelToTransformConverter converter = null;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LayoutAnchorable layoutAnchorable && layoutAnchorable.FindParent<LayoutAnchorSide>() is LayoutAnchorSide layoutAnchorSide)
            {
                DockingManager manager = layoutAnchorable.Root.Manager;

                double x = 0.0;
                double y = 0.0;

                switch (layoutAnchorSide.Side)
                {
                    case AnchorSide.Left:
                        x += manager.GridSplitterWidth / 2;
                        break;
                    case AnchorSide.Top:
                        y += manager.GridSplitterHeight / 2;
                        break;
                    case AnchorSide.Right:
                        x -= manager.GridSplitterWidth / 2;
                        break;
                    case AnchorSide.Bottom:
                        y -= manager.GridSplitterHeight / 2;
                        break;
                    default:
                        break;
                }

                return new TranslateTransform(x, y);
            }

            return Transform.Identity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
#if NET6_0_OR_GREATER
            converter ??= new ModelToTransformConverter();
#else
            converter = converter ?? new ModelToTransformConverter();
#endif

            return converter;
        }
    }
}
