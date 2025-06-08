using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Windowing;
using System.Collections.Generic;
using System.Net.Http;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HttpUtility
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RootPage : Page
    {
        public MainWindow _window;
        public List<HttpHeader> Headers = [];
        public string Body = string.Empty;
        public string Url;
        public string Mode;
        public string LastResponse;
        private bool canNavigStack = true;

#pragma warning disable CS8618
        public RootPage()
        {
            this.InitializeComponent();
            NavBar.SelectedItem = ReqOptionsItem;
            VisualStateManager.GoToState(this, "Top", true);
        }
#pragma warning restore CS8618

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _window = (MainWindow)e.Parameter;

            _window.Title = Globals.AppTitleName;
            _window.ExtendsContentIntoTitleBar = true;
            _window.SetTitleBar(AppTitleBar);
            _window.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            _window.AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets", "Branding", "Logo.ico"));

            _ = new ConfigManager(this, _window); // Init settings
        }

        private void OnPaneDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            if (sender.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
            {
                VisualStateManager.GoToState(this, "Top", true);
            }
            else
            {
                if (args.DisplayMode == NavigationViewDisplayMode.Minimal)
                {
                    VisualStateManager.GoToState(this, "Compact", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Default", true);
                }
            }
        }

        private void NavBar_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            canNavigStack = false;
            if (args.IsSettingsSelected)
            {
                MainFrame.Navigate(typeof(ConfigPage), new object[] { this, _window }, args.RecommendedNavigationTransitionInfo);
            }
            else
            {
                switch (((NavigationViewItem)NavBar.SelectedItem).Tag as string)
                {
                    case "reqOptions":
                        MainFrame.Navigate(typeof(OptionsPage), this, args.RecommendedNavigationTransitionInfo);
                        break;
                    case "resp":
                        MainFrame.Navigate(typeof(ResponsePage), LastResponse, args.RecommendedNavigationTransitionInfo);
                        break;
                    default:
                        break;
                }
            }
        }

        private async void NavBar_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var item = (NavigationViewItem)args.InvokedItemContainer;
            switch (item.Tag as string)
            {
                case "sendReq":
                    sendReqItem.IsEnabled = false;
                    var data = await SendRequest(Mode, Url, Headers, Body);
                    if (data.success)
                    {
                        var dialog = new ContentDialog
                        {
                            CloseButtonText = "OK",
                            DefaultButton = ContentDialogButton.Close,
                            Title = "Success",
                            Content = new TextBlock
                            {
                                Text = "The HTTP request was successful! Check the Response tab to view the response.",
                                MaxWidth = 320,
                                TextWrapping = TextWrapping.Wrap,
                            },
                            XamlRoot = XamlRoot
                        };

                        _ = dialog.ShowAsync();
                        if (data.result != null)
                        {
                            LastResponse = await data.result.Content.ReadAsStringAsync();
                        }

                    }
                    else
                    {
                        var msg = !string.IsNullOrEmpty(data.statusText) ? data.statusText : "[No message]";
                        var dialog = new ContentDialog
                        {
                            CloseButtonText = "OK",
                            DefaultButton = ContentDialogButton.Close,
                            Title = "Request Failed",
                            Content = new TextBlock
                            {
                                Text = $"The request returned with HTTP Error Code {data.statusCode}: {msg}",
                                MaxWidth = 320,
                                TextWrapping = TextWrapping.Wrap,
                            },
                            XamlRoot = XamlRoot
                        };

                        _ = dialog.ShowAsync();
                    }
                    sendReqItem.IsEnabled = true;
                    break;
            }
        }

        public static async Task<(bool success, int statusCode, string statusText, HttpResponseMessage? result)> SendRequest(string type, string url, List<HttpHeader> headers, string body)
        {
            var success = true;
            HttpResponseMessage? result = null;
            int statusCode = -1;
            string statusText = string.Empty;

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }

                    switch (type)
                    {
                        case "GET":
                        default:
                            result = await client.GetAsync(url);
                            break;
                        case "POST":
                            result = await client.PostAsync(url, new StringContent(body));
                            break;
                        case "PUT":
                            result = await client.PutAsync(url, new StringContent(body));
                            break;
                        case "PATCH":
                            result = await client.PatchAsync(url, new StringContent(body));
                            break;
                        case "DELETE":
                            result = await client.DeleteAsync(url);
                            break;
                    }

                    success = result.IsSuccessStatusCode;
                    statusCode = (int)result.StatusCode;
                    statusText = result?.ReasonPhrase ?? string.Empty;
                }
            }
            catch (HttpRequestException ex)
            {
                success = false;
                statusCode = (int?)ex.StatusCode ?? 0;
                statusText = ex.Message;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetType().Name + ": " + ex.Message);
            }


            return (success, statusCode, statusText, result);
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back && canNavigStack)
            {
                switch (e.Content)
                {
                    case OptionsPage:
                        NavBar.SelectedItem = ReqOptionsItem;
                        break;
                    case ResponsePage:
                        NavBar.SelectedItem = ReqRespItem;
                        break;
                    case ConfigPage:
                        NavBar.SelectedItem = NavBar.SettingsItem;
                        break;
                }
            }

            NavBar.IsBackEnabled = MainFrame.CanGoBack;
            canNavigStack = true;
        }

        private void NavBar_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            MainFrame.GoBack();
        }
    }
}
