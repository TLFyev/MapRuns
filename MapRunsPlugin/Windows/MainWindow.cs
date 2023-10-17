using System;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace MapRuns.Windows;

public class MainWindow : Window, IDisposable
{
    Plugin plugin;
    internal MainWindow(Plugin plugin) : base(
        "MapRuns Tracker")
    {
        this.plugin = plugin;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Draw()
    {
        ImGui.SetNextWindowSize(new Vector2(400, 400), ImGuiCond.FirstUseEver);
        var tracking = Services.Config.Enabled;
        if(ImGui.Checkbox("Enabled", ref tracking))
        {
            Services.Config.Enabled = tracking;
            Services.Config.Save();
        }
        var gilCurrent = plugin.mapRunLoot!.gil;
        ImGui.Text("Gil earned: " + this.plugin.mapRunLoot.GetPrettyPrintGil());
        var chestsCurrent = plugin.mapRunLoot!.chests;
        ImGui.Text("Chests found: " + this.plugin.mapRunLoot.chests.ToString());
        var portalsCurrent = plugin.mapRunLoot!.portals;
        ImGui.Text("Portals found: " + this.plugin.mapRunLoot.portals.ToString());
        var buttonHeld = ImGui.GetIO().KeyCtrl;
        if (!buttonHeld) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
        if (ImGui.Button("Clear") && buttonHeld)
        {
            if (this.plugin.mapRunLoot.goodItems!.Count > 0)
            {
                this.plugin.mapRunLoot.Clear();
            }
        }
        if (!buttonHeld) ImGui.PopStyleVar();
        ImGui.SameLine(); ImGui.Text("(Hold CTRL to clear items.)");
        //item - winner(s)
        int cols = 2;
        ImGui.Columns(cols, "MapRuns", true);
        ImGui.Separator();
        ImGui.Text("Item Name"); ImGui.NextColumn();
        ImGui.Text("Winner"); ImGui.NextColumn();
        ImGui.Separator();

        foreach (var e in this.plugin.mapRunLoot!.goodItems!)
        {
            ImGui.Text(e.Key); ImGui.NextColumn();
            ImGui.Text(e.Value); ImGui.Separator(); ImGui.NextColumn();
        }
        ImGui.Columns(1);
        ImGui.Separator();
        if (ImGui.Button("Export"))
        {
            if (this.plugin.mapRunLoot.goodItems.Count > 0)
            {
                var text = "```\n";
                text += string.Format("{0}~ gil\n", this.plugin.mapRunLoot.GetPrettyPrintGil());
                text += string.Format("{0} chests opened.\n", this.plugin.mapRunLoot.chests.ToString());
                text += string.Format("{0} portals found.\n", this.plugin.mapRunLoot.portals.ToString());
                foreach (var e in this.plugin.mapRunLoot.goodItems)
                {
                    var count = e.Value.ToString().Split(", ").Length;
                    text += string.Format("{0}x {1} - {2}\n", count, e.Key, e.Value);
                }
                text += "```";
                ImGui.SetClipboardText(text);
                Services.ChatGui.Print("[MapRuns]: Loot copied to clipboard.");
            }
        }
        ImGui.SameLine();
        ImGui.Text("(Defaults to Discord ```<text>``` code formatting.)");
    }
}
