using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using MapRuns.Windows;
using System.ComponentModel;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using System;
using Dalamud.Logging;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;

namespace MapRuns
{
    public sealed class Plugin : IDalamudPlugin
    {
        private const string CommandName = "/mapruns";

        private readonly ConfigWindow configWindow;
        private readonly MainWindow mainWindow;
        private readonly WindowSystem windowSystem;

        public MapRunLoot? mapRunLoot = null;

        private const int MessageTypeGilItemGained = 62;
        private const int MessageTypeSystem = 57; // "A[n]+<itempayload>+ has been added to the loot list"

        public Plugin(DalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<Services>();
            Services.PluginLog.Information("loading?");

            Services.Config = Services.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

            configWindow = new ConfigWindow();
            mainWindow = new MainWindow(this);
            windowSystem = new WindowSystem("MapRuns");
            windowSystem.AddWindow(configWindow);
            windowSystem.AddWindow(mainWindow);

            Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Toggles the visibility of the tracking panel, can also pass [start|stop|clear] as arguments."
            });

            this.mapRunLoot = new MapRunLoot();

            //test values uwu
            this.mapRunLoot.goodItems.Add("Pixie Wings", "The'letr Fyev, Ray Park");
            this.mapRunLoot.goodItems.Add("Exciting Leather", "Ray Park");
            this.mapRunLoot.goodItems.Add("Malake Karpasos", "Ray Park, Lilah Rae");
            this.mapRunLoot.goodItems.Add("Exciting Fiber", "The'letr Fyev");

            Services.PluginInterface.UiBuilder.Draw += DrawUI;
            Services.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            Services.ChatGui.ChatMessage += ChatMessage;
        }

        private void ChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (!Services.Config.Enabled) return;
            var realtype = (int)type & 0x7F;
            switch (realtype)
            {
                case MessageTypeGilItemGained:
                    if (Services.Config.TrackGil)
                    {
                        foreach (var payload in message.Payloads)
                        {
                            switch (payload)
                            {
                                case TextPayload textPayload:
                                    if (textPayload.Text != null && textPayload.Text.Contains("gil"))
                                    {
                                        Regex reg = new Regex("(\\d{1,3},?\\d{1,3})");
                                        Match m = reg.Match(textPayload.Text);
                                        var s = m.Groups[0].ToString().Trim().Replace(",", "");
                                        var gil = int.Parse(s);

                                        this.mapRunLoot!.gil += gil;
                                    }
                                    break;
                            }
                        }
                    }
                    if (Services.Config.TrackItemsWithRolls && this.mapRunLoot!.tempItems!.Count> 0)
                    {
                        //we're tracking items with rolls and we're waiting on someone to win an item
                        foreach (var payload in message.Payloads)
                        {
                            switch (payload)
                            {
                                case TextPayload textPayload:
                                    if(textPayload.Text != null)
                                    {
                                        Regex reg = new Regex("\\sobtains?\\san?\\s?");
                                        Match m = reg.Match(textPayload.Text);
                                        if (m.Success)
                                        {
                                            var pname = "";
                                            var iname = "";
                                            foreach (var pl in message.Payloads)
                                            {
                                                if(pl.GetType() == typeof(ItemPayload))
                                                {
                                                    ItemPayload p = (ItemPayload)pl;
                                                    iname = p.Item!.Name;
                                                }
                                                if(pl.GetType() == typeof(PlayerPayload))
                                                {
                                                    PlayerPayload p = (PlayerPayload)pl;
                                                    pname = p.PlayerName;
                                                }
                                                if(textPayload.Text.StartsWith("You "))
                                                {
                                                    pname = Services.ClientState.LocalPlayer!.Name.ToString();
                                                }
                                            }
                                            if(this.mapRunLoot!.goodItems!.ContainsKey(iname))
                                            {
                                                Services.PluginLog.Information("Dict contains item already, adding extra player to end");
                                                this.mapRunLoot.goodItems[iname] += ", " + pname;
                                            }
                                            else
                                            {
                                                Services.PluginLog.Information("dict didn't contain item, adding new loot record");
                                                this.mapRunLoot!.goodItems.Add(iname, pname);
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case MessageTypeSystem:
                    foreach (var payload in message.Payloads)
                    {
                        switch (payload)
                        {
                            case TextPayload textPayload:
                                if (textPayload.Text!.Contains("has been added to the loot list.") && Services.Config.TrackItemsWithRolls)
                                {
                                    //indicating we have an item that needs rolling on
                                    var iname = "";

                                    foreach (var pl in message.Payloads)
                                    {
                                        if (pl.GetType() == typeof(ItemPayload))
                                        {
                                            ItemPayload p = (ItemPayload)pl;
                                            iname = p.Item!.Name;
                                        }
                                    }

                                    this.mapRunLoot!.tempItems!.Add(iname);
                                    Services.PluginLog.Information("Added: " + iname + " to the tempitems list");
                                    break;
                                }
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void Dispose()
        {
            windowSystem.RemoveAllWindows();
            configWindow.Dispose();
            mainWindow.Dispose();
            Services.CommandManager.RemoveHandler(CommandName);

            Services.PluginInterface.UiBuilder.Draw -= DrawUI;
            Services.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;

            Services.ChatGui.ChatMessage -= ChatMessage;
        }

        private void OnCommand(string command, string args)
        {
            switch (args)
            {
                case "":
                    mainWindow.Toggle();
                    break;
                case "start":
                    if (Services.Config.Enabled)
                    {
                        Services.ChatGui.Print("[MapRuns]: Tracking already enabled.");
                        break;
                    }
                    else
                    {
                        Services.Config.Enabled = true;
                        Services.ChatGui.Print("[MapRuns]: Tracking enabled.");
                    }
                    break;
                case "stop":
                    if (!Services.Config.Enabled)
                    {
                        Services.ChatGui.Print("[MapRuns]: Tracking already disabled.");
                        break;
                    }
                    else
                    {
                        Services.Config.Enabled = false;
                        Services.ChatGui.Print("[MapRuns]: Tracking disabled.");
                    }
                    break;
                case "clear":
                    if(this.mapRunLoot!.goodItems!.Count > 0)
                    {
                        this.mapRunLoot.goodItems.Clear();
                        this.mapRunLoot.gil = 0;
                        //TODO: Other currencies/tracked stuff
                        Services.ChatGui.Print("[MapRuns]: Loot items cleared.");
                    }
                    break;
                default:
                    Services.ChatGui.PrintError($"Unknown command: '/{command} {args}'");
                    break;
            }
        }

        private void DrawUI()
        {
            windowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            configWindow.IsOpen = !configWindow.IsOpen;
        }
    }
}
