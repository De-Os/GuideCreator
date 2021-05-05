using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace GuideCreator.UI
{
    internal interface IInteractiveElement
    {
        Dictionary<string, dynamic> GetContents();
    }

    [Bindable]
    public class InteractiveTextBox : RichEditBox, IInteractiveElement
    {
        public InteractiveTextBox(string text = null)
        {
            if (text != null) Document.SetText(TextSetOptions.None, text);
            AcceptsReturn = true;
        }

        public Dictionary<string, dynamic> GetContents()
        {
            Document.GetText(TextGetOptions.None, out string text);
            if (text.EndsWith("\r")) text = text.Substring(0, text.Length - 1);
            return new Dictionary<string, dynamic>
            {
                {"type", "text"},
                {"content", text.Replace("\u000b", "").Replace("\r", "\n")}
            };
        }
    }

    [Bindable]
    public class InteractivePhoto : Grid, IInteractiveElement
    {
        private string Image;

        public InteractivePhoto(string image = null)
        {
            Loaded += (a, b) =>
            {
                if (image == null)
                {
                    Choose();
                }
                else LoadFromBytes(image);
            };
            PointerPressed += (a, b) => Choose();

            HorizontalAlignment = HorizontalAlignment.Center;
            MinHeight = 50;
            MaxHeight = 500;
        }

        private async void Choose()
        {
            var ring = new ProgressRing
            {
                IsActive = true,
                Width = 50,
                Height = 50
            };
            Children.Add(ring);

            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    var bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    Children.Add(new Image
                    {
                        Source = bitmapImage,
                    });
                }

                using (var bytesStream = await file.OpenStreamForReadAsync())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        bytesStream.CopyTo(memoryStream);
                        Image = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
                Children.Remove(ring);
            }
            else
            {
                Children.Remove(ring);
                Choose();
            }
        }

        private async void LoadFromBytes(string image)
        {
            var bitmap = new BitmapImage();
            using (var stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(Convert.FromBase64String(image).AsBuffer());
                stream.Seek(0);
                await bitmap.SetSourceAsync(stream);
            }
            Children.Add(new Image
            {
                Source = bitmap,
            });
            Image = image;
        }

        public Dictionary<string, dynamic> GetContents()
        {
            return new Dictionary<string, dynamic>
            {
                {"type", "image"},
                {"content", Image}
            };
        }
    }

    [Bindable]
    public class InteractiveTitle : TextBox, IInteractiveElement
    {
        public InteractiveTitle()
        {
            FontWeight = FontWeights.Bold;
            MaxWidth = 500;
        }

        public Dictionary<string, dynamic> GetContents()
        {
            return new Dictionary<string, dynamic>
            {
                {"type", "title"},
                {"content", Text}
            };
        }
    }

    public struct ElementInfo
    {
        [JsonProperty("type")]
        public readonly string Type;

        [JsonProperty("content")]
        public readonly dynamic Content;
    }
}