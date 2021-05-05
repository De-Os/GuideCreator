using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace GuideCreator.UI
{
    [Bindable]
    public class InfinityStack : StackPanel, IInteractiveElement
    {
        public InfinityStack() => this.Loaded += OnRender;

        private void OnRender(object sender, RoutedEventArgs e)
        {
            var button = new AddButton(false)
            {
                Content = new TextBlock
                {
                    Text = "+",
                    FontSize = 30
                },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            button.AddElement += (element) => AddElement(element, Children.IndexOf(button));
            button.DeleteElement += () => DeleteElement(Children.IndexOf(button));
            Children.Add(button);
        }

        public Dictionary<string, dynamic> GetContents()
        {
            var content = new List<Dictionary<string, dynamic>>();

            foreach (var children in Children.Where(i => i is IInteractiveElement)) content.Add((children as IInteractiveElement).GetContents());

            return new Dictionary<string, dynamic> {
                {"type", "list"},
                {"content", content}
            };
        }

        public void AddElement(UIElement element, int index = -1)
        {
            if (index == -1)
            {
                if (Children.Count == 0)
                {
                    index = 0;
                }
                else
                {
                    if (Children[Children.Count - 1] is Button)
                    {
                        index = Children.Count - 1;
                    }
                    else index = Children.Count;
                }
            }

            var addBtn = new AddButton(true)
            {
                Content = new TextBlock
                {
                    Text = "|",
                    FontSize = 15
                },
                Background = new SolidColorBrush(Colors.Transparent),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            addBtn.PointerEntered += (a, b) => (addBtn.Content as TextBlock).Text = "+";
            addBtn.PointerExited += (a, b) => (addBtn.Content as TextBlock).Text = "|";
            addBtn.AddElement += (e) => AddElement(e, Children.IndexOf(addBtn));
            addBtn.DeleteElement += () => DeleteElement(Children.IndexOf(addBtn));
            Children.Insert(index, addBtn);
            index++;
            Children.Insert(index, element);
        }

        private void DeleteElement(int index)
        {
            Children.RemoveAt(index);
            Children.RemoveAt(index);
        }
    }

    [Bindable]
    public class LinedInfinityStack : ContentControl, IInteractiveElement
    {
        public LinedInfinityStack()
        {
            var border = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            border.BorderThickness = new Thickness(3, 3, 0, 3);
            border.BorderBrush = new SolidColorBrush(Colors.Gray);
            border.Child = new InfinityStack();

            Content = border;
            Margin = new Thickness(3, 3, 0, 3);
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
        }

        public Dictionary<string, dynamic> GetContents() => ((Content as Border).Child as InfinityStack).GetContents();
    }

    [Bindable]
    public class AddButton : Button
    {
        public delegate void AddElementEvent(UIElement element);

        public delegate void DeleteElementEvent();

        public event AddElementEvent AddElement;

        public event DeleteElementEvent DeleteElement;

        public AddButton(bool deleteAction)
        {
            var addMenu = new AddChoose(deleteAction);
            addMenu.AddElement += (e) => AddElement?.Invoke(e);
            addMenu.DeleteElement += () => DeleteElement?.Invoke();
            Flyout = addMenu;
        }

        [Bindable]
        public class AddChoose : MenuFlyout
        {
            public delegate void AddElementEvent(UIElement element);

            public delegate void DeleteElementEvent();

            public event AddElementEvent AddElement;

            public event DeleteElementEvent DeleteElement;

            public AddChoose(bool deleteAction)
            {
                GenerateMenu(deleteAction);

                AddElement += (a) => GenerateMenu(deleteAction);
            }

            private void GenerateMenu(bool deleteAction)
            {
                Items.Clear();

                Add(new MenuFlyoutItem
                {
                    Text = "Title",
                    Icon = new FontIcon
                    {
                        Glyph = "\uE736"
                    }
                }, new InteractiveTitle());

                Add(new MenuFlyoutItem
                {
                    Text = "Text",
                    Icon = new FontIcon
                    {
                        Glyph = "\uEE56"
                    }
                }, new InteractiveTextBox());

                Add(new MenuFlyoutItem
                {
                    Text = "Image",
                    Icon = new FontIcon
                    {
                        Glyph = "\uEB9F"
                    }
                }, new InteractivePhoto());

                Add(new MenuFlyoutItem
                {
                    Text = "Stack",
                    Icon = new FontIcon
                    {
                        Glyph = "\uE8FD"
                    }
                }, new LinedInfinityStack());

                if (deleteAction)
                {
                    Items.Add(new MenuFlyoutSeparator());

                    var delete = new MenuFlyoutItem
                    {
                        Text = "Delete element",
                        Icon = new FontIcon
                        {
                            Glyph = "\uE711"
                        }
                    };
                    delete.Click += (a, b) => DeleteElement?.Invoke();
                    Items.Add(delete);
                }

                void Add(MenuFlyoutItem item, UIElement elementToAdd)
                {
                    Items.Add(item);
                    item.Click += (a, b) => AddElement?.Invoke(elementToAdd);
                }
            }
        }
    }
}