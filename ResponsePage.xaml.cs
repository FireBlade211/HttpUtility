using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using System;
using Microsoft.Web.WebView2.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HttpUtility
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ResponsePage : Page
    {
        private string _response = string.Empty;

        public ResponsePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _response = (string)e.Parameter;
            SelectBar.SelectedItem = RawItem;
        }

        private async void SelectBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            switch (sender.SelectedItem.Tag)
            {
                case "raw":
                    RawBox.Visibility = Visibility.Visible;
                    RenderView.Visibility = Visibility.Collapsed;
                    RawBox.Text = _response;
                    break;
                case "render":
                    RawBox.Visibility = Visibility.Collapsed;
                    RenderView.Visibility = Visibility.Visible;
                    var isInited = RenderView.CoreWebView2 != null;
                    await RenderView.EnsureCoreWebView2Async();
#pragma warning disable CS8602
                    if (!isInited)
                    {
                        RenderView.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
"(KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36 Edg/114.0.1823.43";
                        RenderView.NavigateToString(_response);
                    }
                    break;
#pragma warning restore
            }
        }
    }
}
