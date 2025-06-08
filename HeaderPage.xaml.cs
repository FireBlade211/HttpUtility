using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Contacts;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HttpUtility
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HeaderPage : Page
    {
        public ObservableCollection<HttpHeader> Headers = new();

        public HeaderPage()
        {
            this.InitializeComponent();
        }

        private void AddCustom(object sender, RoutedEventArgs e)
        {
            Headers.Add(new HttpHeader());
        }

        private void AddHeader(object sender, RoutedEventArgs e)
        {
            var header = new HttpHeader();
            if (sender is MenuFlyoutItem item)
            {
                header.Key = item.Text;
            }
            Headers.Add(header);
        }

        private void DeleteItem_ItemInvoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            Headers.Remove((HttpHeader)args.SwipeControl.DataContext);
        }

        private void HeaderDeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)sender).DataContext;
            var header = (HttpHeader)item;
            Headers.Remove(header);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)sender).DataContext;
            var header = (HttpHeader)item;
            Headers.Remove(header);
        }
    }

    public class HttpHeader : INotifyPropertyChanged
    {
        private string _key = string.Empty;
        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Key)));
            }
        }
        private string _val = string.Empty;
        public string Value
        {
            get
            {
                return _val;
            }
            set
            {
                _val = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public HttpHeader()
        {

        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
