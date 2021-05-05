using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace GuideCreator.UI
{
    [Windows.UI.Xaml.Data.Bindable]
    public class BlurView : Grid
    {
        public ScrollViewer Scroll = new ScrollViewer
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        public FrameworkElement Content
        {
            get
            {
                return Scroll.Content as FrameworkElement;
            }
            set
            {
                Scroll.Content = value;
            }
        }

        protected Grid _topmenu = new Grid
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        public FrameworkElement TopMenu
        {
            get
            {
                if (_topmenu.Children.Count > 0) return _topmenu.Children[0] as FrameworkElement;
                return null;
            }
            set
            {
                _topmenu.Children.Clear();
                _topmenu.Children.Add(value);
            }
        }

        protected Grid _bottomMenu = new Grid
        {
            VerticalAlignment = VerticalAlignment.Bottom,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        public FrameworkElement BottomMenu
        {
            get
            {
                if (_bottomMenu.Children.Count > 0) return _bottomMenu.Children[0] as FrameworkElement;
                return null;
            }
            set
            {
                _bottomMenu.Children.Clear();
                _bottomMenu.Children.Add(value);
            }
        }

        public BlurView()
        {
            Children.Add(Scroll);
            Children.Add(_topmenu);
            Children.Add(_bottomMenu);
            UpdateColors();

            _topmenu.SizeChanged += (a, b) => ChangeScrollPadding(b.NewSize.Height, true);
            _bottomMenu.SizeChanged += (a, b) => ChangeScrollPadding(b.NewSize.Height, false);
        }

        private void ChangeScrollPadding(double height, bool top)
        {
            var content = Scroll.Content as FrameworkElement;
            var margin = content.Margin;
            if (top) margin.Top = height; else margin.Bottom = height;
            content.Margin = margin;
        }

        protected void UpdateColors()
        {
            var brush = new AcrylicBrush
            {
                BackgroundSource = AcrylicBackgroundSource.Backdrop,
                TintColor = new Windows.UI.ViewManagement.UISettings().GetColorValue(Windows.UI.ViewManagement.UIColorType.Background)
            };
            _topmenu.Background = brush;
            _bottomMenu.Background = brush;
        }
    }
}