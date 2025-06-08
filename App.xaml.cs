using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HttpUtility
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            string[] commandLineArgs = Environment.GetCommandLineArgs();

            if (commandLineArgs.Length > 1)
            {

                if (Path.Exists(commandLineArgs[1]))
                {
                    string filePath = commandLineArgs[1];

                    OptionsPage.LoadFile(filePath, out List<HttpHeader> heads, out string url, out string mode, out string body);

                    var data = await RootPage.SendRequest(mode, url, heads, body);

                    if (data.success && data.result != null)
                    {
                        m_window = new MainWindow();
                        m_window.Activate();

                        m_window.MainFrame.Navigated += async (s, e) =>
                        {
                            if (e.Content is RootPage root)
                            {
                                root.LastResponse = await data.result.Content.ReadAsStringAsync();
                                root.NavBar.SelectedItem = root.ReqRespItem;
                            }
                        };

                        m_window.MainFrame.Navigate(typeof(RootPage), m_window);
                    }
                }
            }
            else
            {
                m_window = new MainWindow();
                m_window.Activate();
            }
        }

        private MainWindow? m_window;
    }
}
