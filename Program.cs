﻿
using System.Runtime.CompilerServices;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

public class JsonConfig
{
    public string token { get; set; } = "";
    public string prefix { get; set; } = "!";
    public string status { get; set; } = "";
    public string game { get; set; } = "";
}
public class Program
{
    public static Task Main(string[] args) => new Program().MainAsync();
    public static string version = "0.0.2 Dev";
    private Discord.WebSocket.DiscordSocketClient? _client;
    public static Discord.WebSocket.DiscordSocketConfig config = new Discord.WebSocket.DiscordSocketConfig();
    public static JsonConfig Config = new JsonConfig();
    public static DateTime startTime = DateTime.Now;
    public static EmbedBuilder DefaultEmbed = new EmbedBuilder();
    public static int Commands = 0;
    public static ulong Warp = 408615875252322305;
    public async Task MainAsync()
    {
        Config = JsonConvert.DeserializeObject<JsonConfig>(File.ReadAllText("config.json"));
        config = new Discord.WebSocket.DiscordSocketConfig
        {
            AlwaysDownloadUsers = true,
            MessageCacheSize = 250,
            LogLevel = LogSeverity.Debug,
            GatewayIntents = GatewayIntents.All
        };
        _client = new Discord.WebSocket.DiscordSocketClient(config);

        _client.Log += Log;

        if (string.IsNullOrEmpty(Config.token))
        {
            l.Critical("Token is empty!", "MainAsync");
            return;
        }
        if (string.IsNullOrEmpty(Config.status))
        {
            l.Info("No status set. Defaulting to online", "MainAsync");
            Config.status = "Online";
        }

        await _client.LoginAsync(TokenType.Bot, Config.token);
        await _client.StartAsync();
        await _client.SetActivityAsync(new Game(Config.game, ActivityType.Watching, details: "https://warp.tf/"));
        // await _client.SetGameAsync(Config.game);
        await _client.SetStatusAsync((UserStatus)Enum.Parse(typeof(UserStatus), Config.status));

        CommandServiceConfig csc = new CommandServiceConfig
        {
            CaseSensitiveCommands = false,
            // DefaultRunMode = RunMode.Async,
            LogLevel = config.LogLevel
        };

        var ch = new echoBot.CommandHandler(_client, new CommandService(csc));
        await ch.InstallCommandsAsync();
        DefaultEmbed = new EmbedBuilder().WithColor(Color.DarkPurple).WithCurrentTimestamp().WithFooter(new EmbedFooterBuilder()
        {
            Text = $"echoBot {version}",
            IconUrl = "https://cdn.discordapp.com/avatars/869399518267969556/7d05a852cbea15a1028540a913ae43b5.png?size=4096"
        });

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
    public Task Log(LogMessage msg)
    {
        l.Log(msg);
        return Task.CompletedTask;
    }
}
public class l
{
    public static void Log(LogMessage msg)
    {
        if (msg.Severity <= Program.config.LogLevel)
            Console.WriteLine($"[{System.DateTime.Now.ToString()}] [{msg.Severity}] [{msg.Source}] {msg.Message}");
    }
    public static void Debug(string msg, string source = "?")
    {
        Log(new LogMessage(LogSeverity.Debug, source, msg));
    }
    public static void Verbose(string msg, string source = "?")
    {
        Log(new LogMessage(LogSeverity.Verbose, source, msg));
    }
    public static void Info(string msg, string source = "?")
    {
        Log(new LogMessage(LogSeverity.Info, source, msg));
    }
    public static void Warning(string msg, string source = "?")
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Log(new LogMessage(LogSeverity.Warning, source, msg));
        Console.ResetColor();
    }
    public static void Error(string msg, string source = "?")
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Log(new LogMessage(LogSeverity.Error, source, msg));
        Console.ResetColor();
    }
    public static void Critical(string msg, string source = "?")
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Log(new LogMessage(LogSeverity.Critical, source, msg));
        Console.ResetColor();
    }
}


