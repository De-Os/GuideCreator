using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GuideCreator.UI
{
    public class BottomMenu : Grid
    {
        public delegate void ClearEvent();

        public event ClearEvent Clear;

        public delegate void SaveEvent();

        public event SaveEvent Save;

        public delegate void LoadEvent();

        public event LoadEvent Load;

        public BottomMenu()
        {
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            HorizontalAlignment = HorizontalAlignment.Right;
            Margin = new Thickness(0, 5, 0, 5);

            var clearbtn = new Button
            {
                Content = new TextBlock
                {
                    Text = "Clear"
                },
                Margin = new Thickness(5, 0, 5, 0)
            };
            clearbtn.Click += (a, b) => Clear?.Invoke();
            SetColumn(clearbtn, 0);
            Children.Add(clearbtn);

            var loadbtn = new Button
            {
                Content = new TextBlock
                {
                    Text = "Load"
                },
                Margin = new Thickness(0, 0, 5, 0)
            };
            loadbtn.Click += (a, b) => Load?.Invoke();
            SetColumn(loadbtn, 1);
            Children.Add(loadbtn);

            var savebtn = new Button
            {
                Content = new TextBlock
                {
                    Text = "Save"
                },
                Margin = new Thickness(0, 0, 5, 0)
            };
            savebtn.Click += (a, b) => Save?.Invoke();
            SetColumn(savebtn, 2);
            Children.Add(savebtn);
        }
    }
}