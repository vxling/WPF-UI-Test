using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfUiTest.Themes;

public class RowBackgroundAdorner : Adorner
{
    private SolidColorBrush _backgroundBrush;

    public RowBackgroundAdorner(UIElement adornedElement, Brush background) : base(adornedElement)
    {
        _backgroundBrush = background as SolidColorBrush ?? new SolidColorBrush(Colors.Transparent);
        IsHitTestVisible = false;
    }

    public void UpdateBackground(Brush background)
    {
        _backgroundBrush = background as SolidColorBrush ?? new SolidColorBrush(Colors.Transparent);
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        drawingContext.DrawRectangle(_backgroundBrush, null,
            new Rect(0, 0, AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height));
    }
}
