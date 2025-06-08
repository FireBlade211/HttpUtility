using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#pragma warning disable CS8602
#pragma warning disable CS8604
#pragma warning disable CS8618
namespace HttpUtility
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConfigPage : Page
    {
        private ConfigManager configMgr;

        public ConfigPage()
        {
            InitializeComponent();
#if DEBUG
            var card = new SettingsCard();
            card.Header = "You are running a debug build of HTTP Utility";
            card.Description = "This message will automatically be hidden in release builds.";
            card.HeaderIcon = new FontIcon
            {
                Glyph = "\uEBE8"
            };
            AboutExpander.Items.Add(card);
#endif
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is object[] data)
            {
                configMgr = new ConfigManager(data[0] as RootPage, data[1] as MainWindow);
                var i = 0;
                foreach (ComboBoxItem item in themeMode.Items)
                {
                    if ((string)item.Tag == Enum.GetName(typeof(ElementTheme), configMgr.Theme))
                    {
                        themeMode.SelectedIndex = i;
                        break;
                    }
                    i++;
                }
                i = 0;
                foreach (ComboBoxItem item in navigationLocation.Items)
                {
                    if ((string)item.Tag == Enum.GetName(typeof(NavigationViewPaneDisplayMode), configMgr.NavBarPaneNavigMode))
                    {
                        navigationLocation.SelectedIndex = i;
                        break;
                    }
                    i++;
                }
                soundToggle.IsOn = configMgr.SoundEnabled;
                spatialSoundBox.IsOn = configMgr.SpatialSoundEnabled;
                SoundVolumeSlider.Value = configMgr.SoundVolume * 100;
                i = 0;
                foreach (ComboBoxItem item in winBg.Items)
                {
                    if ((string)item.Tag == Enum.GetName(typeof(ConfigManager.SysBackdropType), configMgr.WindowBackdrop))
                    {
                        winBg.SelectedIndex = i;
                        break;
                    }
                    i++;
                }
            }
        }

        private void themeMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (configMgr != null)
            {
                configMgr.Theme = (ElementTheme)Enum.Parse(typeof(ElementTheme), (e.AddedItems[0] as ComboBoxItem).Tag as string);
            }
        }

        private void navigationLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (configMgr != null)
            {
                var mode = (NavigationViewPaneDisplayMode)Enum.Parse(typeof(NavigationViewPaneDisplayMode), (e.AddedItems[0] as ComboBoxItem).Tag as string);
                configMgr.NavBarPaneNavigMode = mode;
            }
        }

        private void soundToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (configMgr != null)
            {
                configMgr.SoundEnabled = soundToggle.IsOn;
            }
            SpatialAudioCard.IsEnabled = soundToggle.IsOn;
            spatialSoundBox.IsEnabled = soundToggle.IsOn;
            SoundVolumeCard.IsEnabled = soundToggle.IsOn;
            SoundVolumeSlider.IsEnabled = soundToggle.IsOn;
        }

        private void spatialSoundBox_Toggled(object sender, RoutedEventArgs e)
        {
            if (configMgr != null)
            {
                configMgr.SpatialSoundEnabled = spatialSoundBox.IsOn;
            }
        }

        private void bugRequestCard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void githubCard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SoundVolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (configMgr != null)
            {
                configMgr.SoundVolume = e.NewValue / 100;
            }
        }

        private void AudioTestCard_Click(object sender, RoutedEventArgs e)
        {
            var items = Enum.GetValues(typeof(ElementSoundKind)).Cast<ElementSoundKind>().ToList();
            var idx = Random.Shared.Next(items.Count);
            ElementSoundPlayer.Play(items[idx]);
        }

        private void winBg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var t = (winBg.SelectedItem as ComboBoxItem).Tag as string;

            if (configMgr != null)
            {
                configMgr.WindowBackdrop = Enum.Parse<ConfigManager.SysBackdropType>(t);
            }
        }

        private void AboutExpander_Expanded(object sender, EventArgs e)
        {
            WinAppSdkLink.Content = Globals.WinAppSdkRuntimeDetails;
        }
    }
}
#pragma warning restore