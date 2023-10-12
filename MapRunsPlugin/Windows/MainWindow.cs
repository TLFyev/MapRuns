using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace MapRuns.Windows;

public class MainWindow : Window, IDisposable
{
    internal MainWindow() : base(
        "MapRuns Tracker")
    {
        Size = new Vector2(400, 400);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Draw()
    {
        
    }
}
