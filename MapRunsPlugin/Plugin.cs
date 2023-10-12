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

namespace MapRuns
{
    public sealed class Plugin : IDalamudPlugin
    {
        private const string CommandName = "/mapruns";

        private readonly ConfigWindow configWindow;
        private readonly MainWindow mainWindow;
        private readonly WindowSystem windowSystem;

        private readonly uint messageTypeGilItemGained = 62;

        public Plugin(DalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<Services>();

            Services.Config = Services.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

            configWindow = new ConfigWindow();
            mainWindow = new MainWindow();
            windowSystem = new WindowSystem("MapRuns");
            windowSystem.AddWindow(configWindow);
            windowSystem.AddWindow(mainWindow);

            Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Toggles the visibility of the tracking panel, can also pass [start|stop|clear] as arguments."
            });

            Services.PluginInterface.UiBuilder.Draw += DrawUI;
            Services.PluginInterface.UiBuilder.Draw += DrawConfigUI;

            Services.ChatGui.ChatMessageHandled += ChatMessageHandled;
        }

        public void Dispose()
        {
            windowSystem.RemoveAllWindows();
            configWindow.Dispose();
            mainWindow.Dispose();
            Services.CommandManager.RemoveHandler(CommandName);

            Services.PluginInterface.UiBuilder.Draw -= DrawUI;
            Services.PluginInterface.UiBuilder.Draw -= DrawConfigUI;

            Services.ChatGui.ChatMessageHandled -= ChatMessageHandled;
        }

        private void ChatMessageHandled(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {

        }

        private void OnCommand(string command, string args)
        {
            switch (args)
            {
                case "":
                    mainWindow.Toggle();
                    break;
                case "start":
                    //todo start tracking
                    break;
                case "pause":
                    //todo pause
                    break;
                case "unpause":
                    //todo unpause
                    break;
                case "stop":
                    //todo stop tracking
                    break;
                case "clear":
                    //todo delete data
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
