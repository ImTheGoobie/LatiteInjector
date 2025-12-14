using LatiteInjector.Properties;
using LatiteInjector.Utils;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace LatiteInjector;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public static readonly SettingsWindow SettingsWindow = new();
    public static readonly CreditWindow CreditWindow = new();
    public static readonly LanguageWindow LanguageWindow = new();

    private void App_OnStartup(object sender, StartupEventArgs startupEventArgs)
    {
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        if (!Environment.Is64BitOperatingSystem)
        {
            MessageBox.Show(
                "It looks like you're running a 32 bit OS/Computer. Sadly, you cannot use Latite Client with a 32 bit OS/Computer. Please do not report this as a bug, make a ticket, or ask how to switch to 64 bit in the Discord, you cannot use Latite Client AT ALL!!!",
                "32 bit OS/Computer", MessageBoxButton.OK, MessageBoxImage.Error);
            Current.Shutdown();
        }

        DiscordPresence.InitializePresence();
        if (Settings.Default.DiscordPresence)
            DiscordPresence.DefaultPresence();

        // is this probably a bad practice? yes! do i care? no!
        System.Timers.Timer detailedPresenceTimer = new(5000);
        detailedPresenceTimer.AutoReset = true;
        detailedPresenceTimer.Elapsed += DiscordPresence.DetailedPlayingPresence;
        detailedPresenceTimer.Start();

        SettingsWindow.Closing += OnClosing!;
        CreditWindow.Closing += OnClosing!;
        LanguageWindow.Closing += OnClosing!;

        MainWindow = new MainWindow();
        LanguageOnStartup();
        MainWindow.Show();
    }

    public static void ChangeLanguage(Uri uri)
    {
        if (Application.Current == null)
            throw new InvalidOperationException("Application.Current is null. ChangeLanguage must be called after App startup.");

        ResourceDictionary langDict = new ResourceDictionary{ Source = uri };

        var merged = Application.Current.Resources.MergedDictionaries;
        if (merged.Count > 0) merged[0] = langDict;
        else merged.Add(langDict);

        Settings.Default.SelectedLanguage = uri.OriginalString;
        Settings.Default.Save();

        SetStatusLabel.Default();
    }

    public static string GetTranslation(string key, params string[] args)
    {
        try
        {
            var resource = Application.Current?.TryFindResource(key) ?? key;
            string result = resource.ToString() ?? key;

            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    result = result.Replace($"{{{i}}}", args[i]);
                }
            }

            return result.Replace("\\n", "\n");
        }
        catch
        {
            return key.Replace("\\n", "\n");
        }
    }

    private static void LanguageOnStartup()
    {
        if (!string.IsNullOrWhiteSpace(Settings.Default.SelectedLanguage) &&
            Settings.Default.SelectedLanguage !=
            "pack://application:,,,/Latite Injector;component//Assets/Translations/English.xaml")
        {
            try {
                ChangeLanguage(new Uri(Settings.Default.SelectedLanguage, UriKind.Absolute));
                return;
            }
            catch (Exception ex) {
                Logging.ErrorLogging(
                    $"FAILED TO AUTO SWITCH LANGUAGE (SelectedLanguage) ({Settings.Default.SelectedLanguage}) ({ex})");
            }
        }

        string? lang = CultureInfo.CurrentCulture.Name switch
        {
            "ar-SA" => "Arabic",
            "cs-CZ" => "Czech",
            "nl-NL" => "Dutch",
            "fr-FR" => "French",
            "hi-IN" => "Hindi",
            "ja" or "ja-JP" => "Japanese",
            "pt" or "pt-BR" or "pt-PT" => "Portuguese",
            "es" or "es-AR" or "es-BO" or "es-CL" or "es-CR" or "es-DO" or
            "es-EC" or "es-ES" or "es-GT" or "es-HN" or "es-MX" or "es-NI" or
            "es-PA" or "es-PE" or "es-PR" or "es-PY" or "es-SV" or "es-UY" or
            "es-VE" => "Spanish",
            "zh-CN" => "Chinese (Simplified)",
            "zh-HK" or "zh-MO" or "zh-TW" => "Chinese (Traditional)",
            _ => null
        };

        if (lang == null)
            return;

        string langUri =
            $"pack://application:,,,/Latite Injector;component//Assets/Translations/{lang}.xaml";

        try
        {
            ChangeLanguage(new Uri(langUri, UriKind.Absolute));

            Settings.Default.SelectedLanguage = langUri;
            Settings.Default.Save();
        }
        catch (Exception ex)
        {
            Logging.ErrorLogging(
                $"FAILED TO AUTO SWITCH LANGUAGE (langUri) ({langUri}) ({ex})");
        }
    }

    private static void OnUnhandledException(object sender,
        UnhandledExceptionEventArgs ex) =>
        Logging.ExceptionLogging(ex.ExceptionObject as Exception);

    private static void OnClosing(object sender, CancelEventArgs e) => e.Cancel = true;
}