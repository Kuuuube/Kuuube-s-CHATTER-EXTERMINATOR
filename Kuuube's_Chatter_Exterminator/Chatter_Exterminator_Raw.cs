using System;
using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Output;

namespace Kuuube_s_Chatter_Exterminator
{
    using static MathF;

    [PluginName("Kuuube's CHATTER EXTERMINATOR RAW")]
    public class Kuuube_s_CHATTER_EXTERMINATOR_RAW : IPositionedPipelineElement<IDeviceReport>
    {
        private Vector2 _lastPos;
        private readonly float _threshold = 0.9f;
        private readonly float _OffsetY = 15;
        private readonly float _Xoffset = 1.96f;
        private readonly float _FuncStretch = 1.7f;

        public Vector2 Filter(Vector2 point)
        {
            Vector2 calcTarget = new Vector2();
            float deltaX, deltaY, distance, weightModifier;
            {
                calcTarget.X = point.X;
                calcTarget.Y = point.Y;
            }

            deltaX = calcTarget.X - _lastPos.X;
            deltaY = calcTarget.Y - _lastPos.Y;
            distance = Sqrt(deltaX * deltaX + deltaY * deltaY);

            float target = 1 - _threshold;
            float weight = (float)(1.0 - (1.0 / (float)(1.0 / target)));

            weightModifier = (float)Pow(((distance * -1 + (Chatter_Extermination_Strength - _FuncStretch)) / _Xoffset), 19) + _OffsetY;

            // Limit minimum
            if (weightModifier + _OffsetY < 0)
                weightModifier = 0;
            else
                weightModifier += _OffsetY;

            weightModifier = weight / weightModifier;
            weightModifier = Math.Clamp(weightModifier, 0, 1);
            _lastPos.X += (float)(deltaX * weightModifier);
            _lastPos.Y += (float)(deltaY * weightModifier);

            return _lastPos;
        }

        public event Action<IDeviceReport> Emit;

        public void Consume(IDeviceReport value)
        {
            if (value is ITabletReport report)
            {
                report.Position = Filter(report.Position);
                value = report;
            }

            Emit?.Invoke(value);
        }

        public PipelinePosition Position => PipelinePosition.PostTransform;

        [Property("Chatter Extermination Strength"), DefaultPropertyValue(2f), ToolTip
            ("Kuuube's CHATTER EXTERMINATOR RAW:\n\n" +
            "Accepted settings are 1-20.\n" +
            "Recommended settings for Kuuube's CHATTER EXTERMINATOR RAW: 2-3 for drag and 5-6 for hover.\n\n" +
            "For more information: Open the wiki from plugin manager or go to https://github.com/Kuuuube/Kuuube-s-CHATTER-EXTERMINATOR.")]
        public float Chatter_Extermination_Strength { set; get; } = 3;
    }
}
