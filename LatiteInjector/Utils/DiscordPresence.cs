#region

using System.Collections.Generic;
using System.IO;
using System.Timers;
using DiscordRPC;

#endregion

namespace LatiteInjector.Utils;
using LatiteInjector.Properties;

public static class DiscordPresence
{
    private static readonly DiscordRpcClient DiscordClient = new("1066896173799047199");

    private record PresenceDetails(
        string Name,
        string LogoKey,
        string LogoTooltip
    );

    private static readonly Dictionary<string, PresenceDetails> presenceDictionary = new()
    {
        { "hivebedrock.network", new PresenceDetails("The Hive", "thehive", "The Hive Logo") },
        { "cubecraft.net", new PresenceDetails("Cubecraft Games", "cubecraft", "Cubecraft Games Logo") },
        { "play.galaxite.net", new PresenceDetails("Galaxite Network", "galaxite", "Galaxite Network Logo") },
        { "zeqa.net", new PresenceDetails("Zeqa Practice", "zeqa", "Zeqa Logo") },
        { "nethergames.org", new PresenceDetails("NetherGames Network", "nethergames", "NetherGames Network Logo") }
    };

    public static void InitializePresence() => DiscordClient.Initialize();
    public static Timestamps CurrentTimestamp = Timestamps.Now;

    public static void DefaultPresence()
    {
        DiscordClient.SetPresence(
            new RichPresence
            {
                State = "Idling in the injector",
                Timestamps = CurrentTimestamp,
                Buttons = new[]
                {
                    new Button { Label = "Download Latite Client", Url = "https://discord.gg/zcJfXxKTA4" }
                },
                Assets = new Assets
                {
                    LargeImageKey = "latite",
                    LargeImageText = "Latite Client Icon"
                }
            }
        );
        Settings.Default.DiscordPresence = true;
    }

    public static void PlayingPresence()
    {
        DiscordClient.UpdateDetails($"Playing Minecraft {Injector.MinecraftVersion}");
        DiscordClient.UpdateLargeAsset("minecraft", "Minecraft Bedrock Logo");
        DiscordClient.UpdateSmallAsset("latite", "Latite Client Icon");

        DiscordClient.UpdateState(Injector.IsCustomDll ? $"with {Injector.CustomDllName}" : "with Latite Client");
    }

    public static void DetailedPlayingPresence(object? sender, ElapsedEventArgs e)
    {
        if (!Settings.Default.DiscordPresence || !Injector.IsMinecraftRunning()) return;

        string serverIP = "none";
        if (File.Exists($@"{Logging.LatiteFolder}\serverip.txt"))
            serverIP = File.ReadAllText($@"{Logging.LatiteFolder}\serverip.txt");
        foreach (KeyValuePair<string, PresenceDetails> server in presenceDictionary)
        {
            // Partial match check on server IP
            if (serverIP.IndexOf(server.Key) == -1) continue;

            DiscordClient.UpdateDetails($"Playing on {server.Value.Name}");
            DiscordClient.UpdateState(Injector.IsCustomDll ? $"with {Injector.CustomDllName}"  : "with Latite Client");
            DiscordClient.UpdateLargeAsset(server.Value.LogoKey, server.Value.LogoTooltip);
            DiscordClient.UpdateSmallAsset("latite", "Latite Client Icon");
        }

        if (serverIP == "none")
            PlayingPresence();
    }

    public static void IdlePresence()
    {
        DiscordClient.SetPresence(new RichPresence
        {
            State = "Idling in the injector",
            Timestamps = CurrentTimestamp,
            Buttons = new[]
            {
                new Button { Label = "Download Latite Client", Url = "https://discord.gg/zcJfXxKTA4" }
            },
            Assets = new Assets
            {
                LargeImageKey = "latite",
                LargeImageText = "Latite Client Icon"
            }
        });
    }

    public static void SettingsPresence() => DiscordClient.UpdateState("Changing settings");
    public static void CreditsPresence() => DiscordClient.UpdateState("Reading the credits");
    public static void LanguagesPresence() => DiscordClient.UpdateState("Changing language");

    public static void StopPresence()
    {
        DiscordClient.ClearPresence();
        Settings.Default.DiscordPresence = false;
    }

    public static void ShutdownPresence()
    {
        if (DiscordClient.IsDisposed) return;
        DiscordClient.ClearPresence();
        DiscordClient.Deinitialize();
        DiscordClient.Dispose();
        Settings.Default.DiscordPresence = false;
    }
}
