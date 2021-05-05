using GuideCreator.UI;
using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace GuideCreator
{
    public sealed partial class MainPage : Page
    {
        public InfinityStack InfinityStack = new InfinityStack();

        public MainPage()
        {
            InitializeComponent();

            var menu = new BottomMenu();

            menu.Save += Save;
            menu.Load += Load;
            menu.Clear += () =>
            {
                InfinityStack = new InfinityStack();
                (ContentControl.Content as BlurView).Content = InfinityStack;
            };

            ContentControl.Content = new BlurView
            {
                Content = InfinityStack,
                BottomMenu = menu
            };
        }

        private async void Save()
        {
            var loadpopup = new Popup
            {
                Content = new ProgressRing
                {
                    IsActive = true,
                    Width = 50,
                    Height = 50
                }
            };
            Popup.Children.Add(loadpopup);

            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("JSON", new List<string> { ".json" });
            picker.FileTypeChoices.Add("MessagePack", new List<string> { ".msgpack" });
            var file = await picker.PickSaveFileAsync();

            if (file != null)
            {
                if (file.FileType == ".msgpack")
                {
                    await FileIO.WriteBytesAsync(file, MessagePackSerializer.Serialize(((ContentControl.Content as BlurView).Content as IInteractiveElement).GetContents()));

                }
                else
                {
                    await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(((ContentControl.Content as BlurView).Content as IInteractiveElement).GetContents(), Formatting.Indented));
                }
            }

            loadpopup.Hide();
        }

        private async void Load()
        {
            var loadpopup = new Popup
            {
                Content = new ProgressRing
                {
                    IsActive = true,
                    Width = 50,
                    Height = 50
                }
            };
            Popup.Children.Add(loadpopup);

            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".json");
            picker.FileTypeFilter.Add(".msgpack");
            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                string content;
                if (file.FileType == ".json")
                {
                    content = await FileIO.ReadTextAsync(file);
                }
                else
                {
                    using (var bytesStream = await file.OpenStreamForReadAsync())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            bytesStream.CopyTo(memoryStream);
                            content = MessagePackSerializer.ConvertToJson(memoryStream.ToArray());
                        }
                    }
                }

                var json = JToken.Parse(content);

                if (json["content"] is JArray array) foreach (var element in array) Load(element, InfinityStack);

                void Load(JToken val, InfinityStack stack)
                {
                    switch (val["type"].ToString())
                    {
                        case "list":
                            var infs = new LinedInfinityStack();
                            stack.AddElement(infs);
                            foreach (var element in val["content"]) Load(element, (infs.Content as Border).Child as InfinityStack);
                            break;

                        case "text":
                            stack.AddElement(new InteractiveTextBox(val["content"].ToString()));
                            break;

                        case "title":
                            stack.AddElement(new InteractiveTitle
                            {
                                Text = val["content"].ToString()
                            });
                            break;

                        case "image":
                            stack.AddElement(new InteractivePhoto(val["content"].ToString()));
                            break;
                    }
                }
            }

            loadpopup.Hide();
        }
    }
}