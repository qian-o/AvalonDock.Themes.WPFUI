using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AvalonDock.Controls;

namespace AvalonDock.Themes.WPFUI.Controls
{
    public class UniformAnchorablePaneTabPanel : AnchorablePaneTabPanel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            double width = availableSize.Width;
            double height = availableSize.Height;
            UIElement[] enumerable = All().ToArray();

            if (double.IsInfinity(width) || double.IsInfinity(height) || enumerable.Length is 0)
            {
                return base.MeasureOverride(availableSize);
            }

            double enumerableWidth = width / enumerable.Length;

            foreach (UIElement item in enumerable)
            {
                item.Measure(new Size(enumerableWidth, height));

                height = Math.Max(height, item.DesiredSize.Height);
            }

            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = finalSize.Width;
            UIElement[] enumerable = All().ToArray();

            if (double.IsInfinity(width) || enumerable.Length is 0)
            {
                return base.MeasureOverride(finalSize);
            }

            double enumerableWidth = width / enumerable.Length;

            double offset = 0.0;

            foreach (UIElement item in enumerable)
            {
                item.Arrange(new Rect(offset, 0.0, enumerableWidth, finalSize.Height));

                offset += enumerableWidth;
            }

            return finalSize;
        }

        private IEnumerable<UIElement> All()
        {
            foreach (UIElement ch in Children)
            {
                if (ch.Visibility != Visibility.Collapsed)
                {
                    yield return ch;
                }
            }
        }
    }
}
