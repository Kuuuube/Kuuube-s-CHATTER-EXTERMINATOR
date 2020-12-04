using System;
using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Tablet.Interpolator;
using OpenTabletDriver.Plugin.Timers;

namespace TabletDriverFilters.Chatter_Exterminator
{
    using static MathF;

    [PluginName("Kuuube's CHATTER EXTERMINATOR")]
    public class AntiChatter : Interpolator
    {
        public AntiChatter(ITimer scheduler) : base(scheduler) {  }

        private bool isReady;
        private Vector2 position;
        private Vector2 targetPos, calcTarget;
        private SyntheticTabletReport report;
        private const float threshold = 0.9f;
        private float _AntichatterOffsetY = 15;

        public override void UpdateState(SyntheticTabletReport report)
        {
            this.targetPos = report.Position;
                calcTarget = targetPos;

            this.report = report;
        }

        public override SyntheticTabletReport Interpolate()
        {
            this.report.Position = Filter(this.calcTarget);
            return this.report;
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

            var weightModifier = (float)Pow(((distance * -1 + (AntichatterStrength - 100)) / 100), 999) + _AntichatterOffsetY;

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

        public static FilterStage FilterStage => FilterStage.PostTranspose;

        [Property("Chatter Extermination Strength")]
        public float AntichatterStrength { set; get; } = 3;
    }
}