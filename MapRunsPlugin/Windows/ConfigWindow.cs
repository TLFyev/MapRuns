using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace MapRuns.Windows;

public class ConfigWindow : Window, IDisposable
{
    internal ConfigWindow() : base(
        "MapRuns Settings", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size = new Vector2(300, 250);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Draw()
    {
        ImGui.Text("Tick boxes below to enable tracking of various loot");
        var trackGil = Services.Config.TrackGil;
        if (ImGui.Checkbox("Track Gil", ref trackGil))
        {
            Services.Config.TrackGil = trackGil;
            Services.Config.Save();
        }
        ImGui.Separator();
        var trackWithRolls = Services.Config.TrackItemsWithRolls;
        if (ImGui.Checkbox("Track items that require rolls", ref trackWithRolls))
        {
            Services.Config.TrackItemsWithRolls = trackWithRolls;
            Services.Config.Save();
        }
        var trackWithoutRolls = Services.Config.TrackItemsWithoutRolls;
        if (ImGui.Checkbox("Track all misc items", ref trackWithoutRolls))
        {
            Services.Config.TrackItemsWithoutRolls = trackWithoutRolls;
            Services.Config.Save();
        }
    }
}
