using System;
using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Output;

namespace Kuuube_s_Chatter_Exterminator
{
    using static MathF;

    [PluginName("Kuuube's CHATTER EXTERMINATOR SMOOTH")]
    public class Kuuube_s_CHATTER_EXTERMINATOR_SMOOTH : AsyncPositionedPipelineElement<IDeviceReport>
    {
        public Kuuube_s_CHATTER_EXTERMINATOR_SMOOTH() {  }

        private bool isReady;
        private Vector2 position;
        private Vector2 targetPos, calcTarget;
        private readonly float threshold = 0.9f;
        private readonly float _AntichatterOffsetY = 15;
        private readonly float _Xoffset = 1.96f;
        private readonly float _FuncStretch = 1.7f;
        private uint pressure;

        public Vector2 Filter(Vector2 calcTarget)
        {
                if (!this.isReady)
                {
                    this.position = calcTarget;
                    this.isReady = true;
                    return calcTarget;
                }

                var delta = calcTarget - this.position;
                var distance = Vector2.Distance(this.position, calcTarget);

                float target = 1 - threshold;
                float weight = (float)(1.0 - (1.0 / Pow((float)(1.0 / target), (float)1 / 1000)));

                var weightModifier = (float)Pow(((distance * -1 + (Chatter_Extermination_Strength - _FuncStretch)) / _Xoffset), 19) + _AntichatterOffsetY;

                // Limit minimum
                if (weightModifier + _AntichatterOffsetY < 0)
                    weightModifier = 0;
                else
                    weightModifier += _AntichatterOffsetY;

                weightModifier = weight / weightModifier;
                weightModifier = Math.Clamp(weightModifier, 0, 1);
                this.position += delta * weightModifier;

                return this.position;
            }

        protected override void ConsumeState()
        {
            if (State is ITabletReport report)
            {
                this.targetPos = report.Position;
                this.pressure = report.Pressure;
                calcTarget = targetPos;
            }
            else
                calcTarget = targetPos;
        }

        protected override void UpdateState()
        {
            if (State is ITabletReport report)
            {
                report.Position = Filter(calcTarget);
                report.Pressure = this.pressure;
                State = report;
            }

            if (PenIsInRange() || State is not ITabletReport)
            {
                OnEmit();
            }
        }
        
        public override PipelinePosition Position => PipelinePosition.PreTransform;

        [Property("Chatter Extermination Strength"), DefaultPropertyValue(6f), ToolTip
            ("Kuuube's CHATTER EXTERMINATOR SMOOTH:\n\n" +
            "Recommended settings are 6-7 for dragging and 15-16 for hovering.\n" +
            "However, any value above zero is accepted.\n" +
            "Frequency is unused and only present due to restrictions on OTD's plugin system.")]
        public float Chatter_Extermination_Strength { set; get; } = 3;
    }
}
