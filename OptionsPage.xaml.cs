using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Microsoft.UI.Xaml.Navigation;
using System;
using Windows.Storage.Pickers;
using WinRT.Interop;
using System.Xml;
using System.IO;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HttpUtility
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OptionsPage : Page
    {
        public List<HttpHeader> Headers = [];
        public string Body = string.Empty;
        private RootPage _root;

#pragma warning disable CS8618
        public OptionsPage()
        {
            this.InitializeComponent();

            Loaded += (s, e) =>
            {
                if (RequestMethodCombo.SelectedItem == null)
                RequestMethodCombo.SelectedIndex = 0;
            };
        }
#pragma warning restore

        private void EditHeadersButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                PrimaryButtonText = "OK",
                SecondaryButtonText = "Cancel",
                XamlRoot = XamlRoot,
                Width = 650,
                Height = 600,
                Title = "Edit HTTP Headers",
                DefaultButton = ContentDialogButton.Primary
            };

            var page = new HeaderPage();
            page.Headers = new ObservableCollection<HttpHeader>(Headers);
            dialog.Content = page;

            var task = dialog.ShowAsync();
            page.Headers.CollectionChanged += (s, e) =>
            {
                foreach (var header in page.Headers)
                {
                    header.PropertyChanged += (ss, ee) =>
                    {
                        var enabled = true;

                        foreach (var header in page.Headers)
                        {
                            if (string.IsNullOrEmpty(header.Key) || string.IsNullOrEmpty(header.Value))
                            {
                                enabled = false;
                                break;
                            }

                            var filtered = page.Headers.Where(x => x != header);
                            if (filtered.Where(x =>
                            {
                                return x.Key == header.Key;
                            }).Any())
                            {
                                enabled = false;
                                break;
                            }
                        }

                        dialog.IsPrimaryButtonEnabled = enabled;
                    };

                }

                var enabled = true;

                foreach (var header in page.Headers)
                {
                    if (string.IsNullOrEmpty(header.Key) || string.IsNullOrEmpty(header.Value))
                    {
                        enabled = false;
                        break;
                    }

                    var filtered = page.Headers.Where(x => x != header);
                    if (filtered.Where(x =>
                    {
                        return x.Key == header.Key;
                    }).Any())
                    {
                        enabled = false;
                        break;
                    }
                }

                dialog.IsPrimaryButtonEnabled = enabled;
            };

            task.Completed += (IAsyncOperation<ContentDialogResult> s, AsyncStatus e) =>
            {
                if (s.GetResults() == ContentDialogResult.Primary)
                {
                    Headers = page.Headers.ToList();
                    _root.Headers = Headers;
                }
            };
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _root = (RootPage)e.Parameter;

            InitWithRoot();
        }

        /// <summary>
        /// Initializes the OptionsPage when the root page is available in <see cref="_root"/>.
        /// </summary>
        /// <remarks>This method throws a <see cref="InvalidOperationException"/> if <see cref="_root"/> isn't set at the time the method runs.</remarks>
        /// <exception cref="InvalidOperationException"/>
        private void InitWithRoot()
        {
            if (_root == null) throw new InvalidOperationException("InitWithRoot requires _root to be set.");
            Headers = _root.Headers;
            URLBox.Text = _root.Url;
            Body = _root.Body;

            RequestMethodCombo.SelectedItem = null;
            foreach (ComboBoxItem item in RequestMethodCombo.Items)
            {
                if (item.Content.ToString() == _root.Mode)
                {
                    RequestMethodCombo.SelectedItem = item;
                    break;
                }
            }
        }

        private void EditBodyButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Title = "Edit Request Body",
                PrimaryButtonText = "OK",
                SecondaryButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary
            };

            var box = new TextBox
            {
                Width = 450,
                Height = 400,
                AcceptsReturn = true,
                Text = Body
            };

            ScrollViewer.SetVerticalScrollBarVisibility(box, ScrollBarVisibility.Auto);

            dialog.Content = box;

            var task = dialog.ShowAsync();

            task.Completed += (IAsyncOperation<ContentDialogResult> s, AsyncStatus e) =>
            {
                if (s.GetResults() == ContentDialogResult.Primary)
                {
                    Body = box.Text;
                    _root.Body = Body;
                }
            };
        }

        private void URLBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _root.Url = URLBox.Text;
        }

        private void RequestMethodCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RequestMethodCombo.SelectedItem == null) return;
            _root.Mode = (string)((ComboBoxItem)RequestMethodCombo.SelectedItem).Content;
        }

        private async void importCard_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var hWnd = WindowNative.GetWindowHandle(_root._window);

            // Initialize the file picker with the window handle (HWND).
            InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for the file picker
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".hro");

            // Open the picker for the user to pick a file
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                LoadFile(file.Path, out List<HttpHeader> heads, out string url, out string mode, out string body);

                if (_root != null) // Avoid the exception since InitWithRoot will throw an InvalidOperationException if _root is null
                {
                    _root.Headers = heads;
                    _root.Url = url;
                    _root.Mode = mode;
                    _root.Body = body;

                    InitWithRoot(); // Reinitialize everything to sync back the changes to the UI
                }
            }
        }

        public static void LoadFile(string path, out List<HttpHeader> headers, out string url, out string mode, out string body)
        {
            headers = new List<HttpHeader>();
            url = string.Empty;
            mode = string.Empty;
            body = string.Empty;
            using (XmlReader reader = XmlReader.Create(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                reader.ReadStartElement("HttpRequestOptions");
                var lastNode = string.Empty;
                var isReadingHeaders = false;

                while (reader.Read())
                {
                    // Read the next node
                    // If it's an element, store it
                    // If it's the text or attribute of an element, check what element it belongs to by checking the previously stored value
                    // and apply it. This allows the .hro file format to be flexible by making all fields optional
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "HttpRequestOptions") continue;
                        lastNode = reader.Name;
                        if (reader.Name == "Headers")
                        {
                            isReadingHeaders = true;
                        }

                        if (isReadingHeaders && reader.Name == "HttpHeader")
                        {
                            var head = new HttpHeader();
                            head.Key = reader.GetAttribute("Key") ?? "<Header>";
                            head.Value = reader.GetAttribute("Value") ?? "<Header>";

                            headers.Add(head);
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.Text)
                    {
                        switch (lastNode)
                        {
                            case "Url":
                                url = reader.Value;
                                break;
                            case "Method":
                                mode = reader.Value;
                                break;
                            case "Body":
                                body = reader.Value;
                                break;
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (reader.Name == "Headers")
                        {
                            isReadingHeaders = false;
                        }
                    }
                }
            }
        }

        private async void exportCard_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new FileSavePicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var hWnd = WindowNative.GetWindowHandle(_root._window);

            // Initialize the file picker with the window handle (HWND).
            InitializeWithWindow.Initialize(savePicker, hWnd);

            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("HTTP Request Options file", [".hro"]);

            // Open the picker for the user to pick a file
            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                using (XmlWriter writer = XmlWriter.Create(new FileStream(file.Path, FileMode.Create, FileAccess.ReadWrite), new XmlWriterSettings
                {
                    Indent = true
                }))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("HttpRequestOptions");

                    writer.WriteElementString("Url", URLBox.Text);
                    writer.WriteElementString("Method", _root.Mode);

                    writer.WriteStartElement("Headers");
                    foreach (var header in Headers)
                    {
                        writer.WriteStartElement("HttpHeader");
                        writer.WriteAttributeString("Key", header.Key);
                        writer.WriteAttributeString("Value", header.Value);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteElementString("Body", Body);

                    writer.WriteEndElement();
                }
            }
        }
    }
}
