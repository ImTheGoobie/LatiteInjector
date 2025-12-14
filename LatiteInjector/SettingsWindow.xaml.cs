using LatiteInjector.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LatiteInjector.Properties;

namespace LatiteInjector;

/// <summary>
/// Interaction logic for SettingsWindow.xaml
/// </summary>
public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();

        DiscordPresenceCheckBox.IsChecked = Settings.Default.DiscordPresence;
        CloseAfterInjectedCheckBox.IsChecked = Settings.Default.CloseAfterInjected;
        LatiteBetaCheckBox.IsChecked = Settings.Default.Nightly;
        LatiteDebugCheckBox.IsChecked = Settings.Default.Debug;
        CustomDLLInput.Text = Settings.Default.CustomDllUrl;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Hide();
        if (!Settings.Default.DiscordPresence) return;

        if (!Injector.IsMinecraftRunning()) DiscordPresence.IdlePresence();
        else DiscordPresence.PlayingPresence();
    }

    private void Window_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

    private void DiscordPresenceCheckBox_OnClick(object sender, RoutedEventArgs e)
    {
        Settings.Default.DiscordPresence = DiscordPresenceCheckBox.IsChecked == true;

        if (Settings.Default.DiscordPresence) DiscordPresence.DefaultPresence();
        else DiscordPresence.StopPresence();

        Settings.Default.Save();
    }

    private void CloseAfterInjectedCheckBox_OnClick(object sender, RoutedEventArgs e)
    {
        Settings.Default.CloseAfterInjected = CloseAfterInjectedCheckBox.IsChecked == true;
        Settings.Default.Save();
    }

    private void LatiteBetaCheckBox_OnClick(object sender, RoutedEventArgs e)
    {
        Settings.Default.Nightly = LatiteBetaCheckBox.IsChecked == true;

        if (Settings.Default.Nightly && Settings.Default.Debug)
        {
            Settings.Default.Debug = false;
            LatiteDebugCheckBox.IsChecked = false;
        }

        if (Settings.Default.Nightly)
        {
            MessageBoxResult result = MessageBox.Show(
                App.GetTranslation(@"WARNING: This option lets you use experimental builds of Latite Client that have a high chance of containing bugs or crashes\nDo you still want to use Latite Nightly?"),
                App.GetTranslation("Latite Nightly disclaimer"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                Settings.Default.Nightly = false;
                LatiteBetaCheckBox.IsChecked = false;
            }
        }

        Settings.Default.Save();
    }

    private void LatiteDebugCheckBox_OnClick(object sender, RoutedEventArgs e)
    {
        Settings.Default.Debug = LatiteDebugCheckBox.IsChecked == true;

        if (Settings.Default.Debug && Settings.Default.Nightly)
        {
            Settings.Default.Nightly = false;
            LatiteBetaCheckBox.IsChecked = false;
        }

        if (Settings.Default.Debug)
        {
            MessageBoxResult result = MessageBox.Show(
                App.GetTranslation(@"WARNING: This option is only meant for detailed reporting of bugs in Latite Client.\nThis version may run slower than production builds.\nDo you still want to use Latite Debug?"),
                App.GetTranslation("Latite Debug disclaimer"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                Settings.Default.Debug = false;
                LatiteDebugCheckBox.IsChecked = false;
            }
        }

        Settings.Default.Save();
    }

    private void CustomDLLInput_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (Uri.TryCreate(CustomDLLInput.Text, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
            uri.AbsolutePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
            Settings.Default.CustomDllUrl = CustomDLLInput.Text;
            Settings.Default.Save();
        }
    }

    private void SwitchLanguageButton_OnClick(object sender, RoutedEventArgs e)
    {
        App.LanguageWindow.Show();
        if (Settings.Default.DiscordPresence)
            DiscordPresence.LanguagesPresence();
    }
}