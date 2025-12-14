using LatiteInjector.Properties;
using LatiteInjector.Utils;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LatiteInjector
{
    /// <summary>
    /// Interaction logic for CreditWindow.xaml
    /// </summary>
    public partial class LanguageWindow
    {
        public LanguageWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            if (!Settings.Default.DiscordPresence) return;
            if (!Injector.IsMinecraftRunning())
            {
                DiscordPresence.IdlePresence();
                return;
            }
            DiscordPresence.PlayingPresence();
        }

        private void Window_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private RadioButton _languageSelected = new();

        private void CustomLanguageRadioButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "XAML files (*.xaml)|*.xaml",
                RestoreDirectory = true
            };

            if (!(openFileDialog.ShowDialog() ?? false))
            {
                _languageSelected.IsChecked = true;
                return;
            }

            string selectedFile = openFileDialog.FileName;
            CustomLanguageRadioButton.Content = selectedFile;

            try
            {
                App.ChangeLanguage(new Uri(selectedFile, UriKind.Absolute));
                Settings.Default.SelectedLanguage = selectedFile;
                Settings.Default.Save();
            }
            catch (Exception)
            {
                string defaultLang = "pack://application:,,,/Latite Injector;component//Assets/Translations/English.xaml";
                App.ChangeLanguage(new Uri(defaultLang, UriKind.Absolute));
                Settings.Default.SelectedLanguage = defaultLang;
                Settings.Default.Save();
            }
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            _languageSelected = (RadioButton)sender;

            string langName = ((RadioButton)sender).Content.ToString()!;
            string langUri = $"pack://application:,,,/Latite Injector;component//Assets/Translations/{langName}.xaml";

            try
            {
                App.ChangeLanguage(new Uri(langUri, UriKind.Absolute));
                Settings.Default.SelectedLanguage = langUri;
                Settings.Default.Save();
            }
            catch (Exception)
            {
                string defaultLang = "pack://application:,,,/Latite Injector;component//Assets/Translations/English.xaml";
                App.ChangeLanguage(new Uri(defaultLang, UriKind.Absolute));
                Settings.Default.SelectedLanguage = defaultLang;
                Settings.Default.Save();
            }
        }

        private void LanguageWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (UIElement uiElement in LanguagesStackPanel.Children)
            {
                if (uiElement is not RadioButton radioButton) continue;
                if ((string)radioButton.Content != Settings.Default.SelectedLanguage
                        .Replace("pack://application:,,,/Latite Injector;component//Assets/Translations/", "")
                        .Replace(".xaml", "")) continue;
                radioButton.IsChecked = true;
                return;
            }
            
            CustomLanguageRadioButton.IsChecked = true;
            CustomLanguageRadioButton.Content = Settings.Default.SelectedLanguage;
        }
    }
}
