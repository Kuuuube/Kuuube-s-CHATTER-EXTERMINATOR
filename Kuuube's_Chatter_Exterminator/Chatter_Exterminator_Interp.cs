using System;
using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Output;

namespace Chatter_Exterminator
{
    using static MathF;

    [PluginName("Kuuube's CHATTER EXTERMINATOR")]
    public class AntiChatter : AsyncPositionedPipelineElement<IDeviceReport>
    {
        public AntiChatter() {  }

        private bool isReady;
        private Vector2 position;
        private Vector2 targetPos, calcTarget;
        private const float threshold = 0.9f;
        private float _AntichatterOffsetY = 15;
        private float _Xoffset = 1.96f;
        private float _FuncStretch = 1.7f;

        protected override void ConsumeState()
        {
            if (State is ITabletReport report)
            {
                this.targetPos = report.Position;
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
                State = report;
            }

            if (PenIsInRange())
            {
                OnEmit();
            }
        }

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

                var weightModifier = (float)Pow(((distance * -1 + (AntichatterStrength - _FuncStretch)) / _Xoffset), 19) + _AntichatterOffsetY;

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

        public override PipelinePosition Position => PipelinePosition.PreTransform;

        [Property("Chatter Extermination Strength"), DefaultPropertyValue(6f), ToolTip
            ("Kuuube's CHATTER EXTERMINATOR:\n\n" +
            "Accepted settings are 1-20 (and 1000hz).\n" +
            "Recommended settings for the interpolator version: 6-7 for drag and 15-16 for hover.\n\n" +
            "For more information: Open the wiki from plugin manager or go to https://github.com/Kuuuube/Kuuube-s-CHATTER-EXTERMINATOR.")]
        public float AntichatterStrength { set; get; } = 3;
    }
}
