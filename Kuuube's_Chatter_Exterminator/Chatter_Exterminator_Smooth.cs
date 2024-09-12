using System;
using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Output;

namespace Kuuube_s_Chatter_Exterminator;

[PluginName("Kuuube's CHATTER EXTERMINATOR SMOOTH")]
public class Kuuube_s_CHATTER_EXTERMINATOR_SMOOTH : IPositionedPipelineElement<IDeviceReport> {
    private Vector2 LastPos;
    private readonly float weight = 0.9f;
    private readonly float Y_Offset = 15;
    private readonly float Stretch = 1.96f;
    private readonly float X_Offset = 1.7f;

    public Vector2 Filter(Vector2 input) {
        Vector2 Delta = new Vector2();
        {
            Delta.X = input.X - LastPos.X;
            Delta.Y = input.Y - LastPos.Y;
        }

        float distance = (float)MathF.Sqrt(MathF.Pow(Delta.X, 2) + MathF.Pow(Delta.Y, 2));

        float function = MathF.Pow((distance * -1 + (Chatter_Extermination_Strength - X_Offset)) / Stretch, 19) + Y_Offset;

        float weightModifier = Math.Clamp(function + Y_Offset, 0, float.MaxValue);

        weightModifier = Math.Clamp(weight / weightModifier, 0, 1);

        LastPos.X += Delta.X * weightModifier;
        LastPos.Y += Delta.Y * weightModifier;

        return LastPos;
    }

    public event Action<IDeviceReport> Emit;

    public void Consume(IDeviceReport value) {
        if (value is IAbsolutePositionReport report) {
            report.Position = Filter(report.Position);
            value = report;
        }

        Emit?.Invoke(value);
    }

    public PipelinePosition Position => PipelinePosition.PreTransform;

    [Property("Chatter Extermination Strength"), DefaultPropertyValue(6f), ToolTip
        ("Kuuube's CHATTER EXTERMINATOR SMOOTH:\n\n" +
        "Recommended settings are 6-7 for dragging and 15-16 for hovering.\n" +
        "However, any value above zero is accepted.")]
    public float Chatter_Extermination_Strength { set; get; }
}
