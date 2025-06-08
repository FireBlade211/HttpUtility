using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.Foundation.Collections;
using Windows.Storage;
using WinRT;

namespace HttpUtility
{
    /// <summary>
    /// Manages the application configuration.
    /// </summary>
    class ConfigManager
    {
        private RootPage? _rootPage;
        private MainWindow? _mainWindow;
        private SystemBackdropConfiguration? _sysBgAcrylicConfigSrc;
        private DesktopAcrylicController? _sysBgAcrylicController;
        private IPropertySet _localCfg => ApplicationData.Current.LocalSettings.Values; // Shortcut
        
        public ElementTheme Theme
        {
            get
            {
                int val;
                if (_localCfg.TryGetValue("Theme", out object? v))
                {
                    val = (int)v;
                }
                else
                {
                    val = 0; // 0 = ElementTheme.Default
                }

                return (ElementTheme)val;
            }
            set
            {
                _localCfg["Theme"] = (int)value;
                if (_rootPage != null)
                {
                    _rootPage.RequestedTheme = value;
                }
            }
        }

        public NavigationViewPaneDisplayMode NavBarPaneNavigMode
        {
            get
            {
                int val;
                if (_localCfg.TryGetValue("NavBarPaneNavigMode", out object? mode))
                {
                    val = (int)mode;
                }
                else
                {
                    val = 2; // 2 = Top
                }

                return (NavigationViewPaneDisplayMode)val;
            }
            set
            {
                _localCfg["NavBarPaneNavigMode"] = (int)value;
                if (_rootPage != null)
                {
                    _rootPage.NavBar.PaneDisplayMode = value;
                    if (value == NavigationViewPaneDisplayMode.Top)
                    {
                        Grid.SetRow(_rootPage.NavBar, 1);
                    }
                    else
                    {
                        Grid.SetRow(_rootPage.NavBar, 0);
                    }
                }
            }
        }

        public bool SoundEnabled
        {
            get
            {
                bool val;
                if (_localCfg.TryGetValue("SoundEnabled", out object? v))
                {
                    val = (bool)v;
                }
                else
                {
                    val = false;
                }

                return val;
            }
            set
            {
                _localCfg["SoundEnabled"] = value;
                if (_rootPage != null) // Even though this doesn't require a page, this validates that the ConfigManager isn't in save-only mode.
                {
                    ElementSoundPlayer.State = value ? ElementSoundPlayerState.On : ElementSoundPlayerState.Off;
                }
            }
        }

        public bool SpatialSoundEnabled
        {
            get
            {
                bool val;
                if (_localCfg.TryGetValue("SpatialSoundEnabled", out object? v))
                {
                    val = (bool)v;
                }
                else
                {
                    val = false;
                }

                return val;
            }
            set
            {
                _localCfg["SpatialSoundEnabled"] = value;

                if (_rootPage != null) // Even though this doesn't require a page, this validates that the ConfigManager isn't in save-only mode.
                {
                    ElementSoundPlayer.SpatialAudioMode = value ? ElementSpatialAudioMode.On : ElementSpatialAudioMode.Off;
                }
            }
        }

        public double SoundVolume
        {
            get
            {
                double val;
                if (_localCfg.TryGetValue("SoundVolume", out object? v))
                {
                    val = (double)v;
                }
                else
                {
                    val = 1.0;
                }

                return val;
            }
            set
            {
                if (value > 1.0 || value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("SoundVolume", "SoundVolume must be a 0-100% percentage (0.0 - 1.0 in code).");
                }
                _localCfg["SoundVolume"] = value;
                if (_rootPage != null) // Even though this doesn't require a page, this validates that the ConfigManager isn't in save-only mode.
                {
                    ElementSoundPlayer.Volume = value;
                }
            }
        }

        public SysBackdropType WindowBackdrop
        {
            get
            {
                SysBackdropType val;
                if (_localCfg.TryGetValue("WindowBackdrop", out object? v))
                {
                    val = (SysBackdropType)v;
                }
                else
                {
                    val = SysBackdropType.None;
                }

                return val;
            }
            set
            {
                _localCfg["WindowBackdrop"] = (int)value;
                if (_rootPage != null && _mainWindow != null)
                {
                    switch (value)
                    {
                        case SysBackdropType.None:
                            _rootPage.Background = (Brush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"];
                            DisposeAcrylicControllers();
                            _mainWindow.SystemBackdrop = null;
                            break;
                        case SysBackdropType.Mica:
                            _rootPage.Background = null;
                            DisposeAcrylicControllers();
                            _mainWindow.SystemBackdrop = new MicaBackdrop();
                            break;
                        case SysBackdropType.Mica2:
                            _rootPage.Background = null;
                            DisposeAcrylicControllers();
                            var bd = new MicaBackdrop();
                            bd.Kind = MicaKind.BaseAlt;
                            _mainWindow.SystemBackdrop = bd;
                            break;
                        case SysBackdropType.Acrylic:
                            _rootPage.Background = null;
                            DisposeAcrylicControllers();
                            _mainWindow.SystemBackdrop = new DesktopAcrylicBackdrop();
                            break;
                        case SysBackdropType.AcrylicThin:
                            _rootPage.Background = null;
                            _mainWindow.SystemBackdrop = new DesktopAcrylicBackdrop();
                            if (_sysBgAcrylicController != null)
                            {
                                _sysBgAcrylicController.Dispose();
                            }
                            _sysBgAcrylicConfigSrc = new();
                            _sysBgAcrylicConfigSrc.IsInputActive = true;
                            _sysBgAcrylicController = new();
                            _sysBgAcrylicController.Kind = DesktopAcrylicKind.Thin;
                            _sysBgAcrylicController.AddSystemBackdropTarget(_mainWindow.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                            _sysBgAcrylicController.SetSystemBackdropConfiguration(_sysBgAcrylicConfigSrc);

                            _mainWindow.Closed -= _mainWindow_Closed;
                            _mainWindow.Closed += _mainWindow_Closed;

                            _mainWindow.Activated -= _mainWindow_Activated;
                            _mainWindow.Activated += _mainWindow_Activated;

                            _rootPage.ActualThemeChanged -= _rootPage_ActualThemeChanged;
                            _rootPage.ActualThemeChanged += _rootPage_ActualThemeChanged;
                            break;
                    }
                }
            }
        }

        private void _rootPage_ActualThemeChanged(FrameworkElement sender, object args)
        {
            if (_rootPage != null && _sysBgAcrylicConfigSrc != null)
            {
                switch (_rootPage.ActualTheme)
                {
                    case ElementTheme.Default:
                        _sysBgAcrylicConfigSrc.Theme = SystemBackdropTheme.Default;
                        break;
                    case ElementTheme.Light:
                        _sysBgAcrylicConfigSrc.Theme = SystemBackdropTheme.Light;
                        break;
                    case ElementTheme.Dark:
                        _sysBgAcrylicConfigSrc.Theme = SystemBackdropTheme.Dark;
                        break;
                }
            }
        }

        private void _mainWindow_Closed(object sender, WindowEventArgs args)
        {
            DisposeAcrylicControllers();
        }

        private void _mainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (_sysBgAcrylicConfigSrc != null)
            {
                _sysBgAcrylicConfigSrc.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ConfigManager"/> class with save-only enabled.
        /// Save-only means that the <see cref="ConfigManager"/> will only save the configuration, and won't update the current state of the application.
        /// </summary>
        public ConfigManager()
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="ConfigManager"/> class with save-only disabled.
        /// </summary>
        /// <param name="rootPage">The <see cref="RootPage"/> to use when changing certain settings.</param>
        /// <param name="mainWin">The <see cref="MainWindow"/> to use when changing certain settings.</param>
        public ConfigManager(RootPage rootPage, MainWindow mainWin)
        {
            _rootPage = rootPage;
            _mainWindow = mainWin;

            Theme = Theme;
            NavBarPaneNavigMode = NavBarPaneNavigMode;
            SoundEnabled = SoundEnabled;
            SoundVolume = SoundVolume;
            SpatialSoundEnabled = SpatialSoundEnabled;
            WindowBackdrop = WindowBackdrop;
        }

        private void DisposeAcrylicControllers()
        {
            _sysBgAcrylicController?.Dispose();
            _sysBgAcrylicConfigSrc = null;
        }

        public enum SysBackdropType
        {
            None,
            Mica,
            Mica2,
            Acrylic,
            AcrylicThin
        }
    }
}
