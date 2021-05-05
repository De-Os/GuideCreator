using System.Linq;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace GuideCreator.UI
{
    [Bindable]
    public class Popup : Grid
    {
        private readonly Grid _content = new Grid
        {
            Background = new AcrylicBrush
            {
                BackgroundSource = AcrylicBackgroundSource.Backdrop,
                TintColor = new Windows.UI.ViewManagement.UISettings().GetColorValue(Windows.UI.ViewManagement.UIColorType.Background)
            },
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(10),
            MaxHeight = 600
        };

        private readonly TextBlock _title;

        public Brush ContentBackground
        {
            get
            {
                return _content.Background;
            }
            set
            {
                _content.Background = value;
            }
        }

        public string Title
        {
            get
            {
                return _title.Text;
            }
            set
            {
                _title.Visibility = value != null && value.Length > 0 ? Visibility.Visible : Visibility.Collapsed;
                _title.Text = value;
            }
        }

        public FrameworkElement Content
        {
            get
            {
                if (_content.Children.Count > 0)
                {
                    return _content.Children[1] as FrameworkElement;
                }
                else return null;
            }
            set
            {
                if (_content.Children.Count > 1 && _content.Children[1] is FrameworkElement prev) _content.Children.Remove(prev);
                value.Transitions.Add(new EntranceThemeTransition { IsStaggeringEnabled = true });
                Grid.SetRow(value, 1);
                _content.Children.Add(value);
            }
        }

        public bool IsPointerOnContent { get; private set; } = false;

        public Popup()
        {
            Children.Add(_content);

            _title = new TextBlock
            {
                Margin = new Thickness(5, 0, 0, 5),
                FontWeight = FontWeights.Bold,
                TextTrimming = TextTrimming.CharacterEllipsis,
                FontSize = 20,
                Visibility = Visibility.Collapsed
            };

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            _content.HorizontalAlignment = HorizontalAlignment.Center;
            _content.VerticalAlignment = VerticalAlignment.Center;

            _content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            _content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            Grid.SetRow(_title, 0);
            _content.Children.Add(_title);

            Background = new SolidColorBrush(new Windows.UI.Color
            {
                A = 125,
                R = (byte)0,
                G = (byte)0,
                B = (byte)0
            });

            Transitions.Add(new PopupThemeTransition());

            _content.PointerEntered += (a, b) => IsPointerOnContent = true;
            _content.PointerExited += (a, b) => IsPointerOnContent = false;

            PointerPressed += (a, b) => { if (!IsPointerOnContent) Hide(); };

            Window.Current.CoreWindow.KeyDown += OnKeyDown;
        }

        public void OnKeyDown(object s, KeyEventArgs e)
        {
            if (e.VirtualKey == Windows.System.VirtualKey.Escape)
            {
                e.Handled = true;
                Hide(true);
            }
        }

        public void Hide(bool checkLast = false)
        {
            var parent = VisualTreeHelper.GetParent(this);
            if (parent == null) return;
            if (parent is Panel p)
            {
                if (checkLast && p.Children.Last() != this) return; // Don't hide popup if it is not last
                p.Children.Remove(this);
            }
            if (parent is ContentControl c)
            {
                c.Content = null;
            }

            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
        }
    }
}