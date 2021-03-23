using System;
using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;

namespace Chatter_Exterminator
{
    using static MathF;

    [PluginName("Kuuube's CHATTER EXTERMINATOR")]
    public class Chatter_Exterminator_Filter : IFilter
    {
        private Vector2 _lastPos;
        private const float _threshold = 0.9f;
        private float _AntichatterOffsetY = 15;
        private float _Xoffset = 1.96f;
        private float _FuncStretch = 1.7f;
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

            weightModifier = (float)Pow(((distance * -1 + (AntichatterStrength - _FuncStretch)) / _Xoffset), 19) + _AntichatterOffsetY;

            // Limit minimum
            if (weightModifier + _AntichatterOffsetY < 0)
                weightModifier = 0;
            else
                weightModifier += _AntichatterOffsetY;

            weightModifier = weight / weightModifier;
            weightModifier = Math.Clamp(weightModifier, 0, 1);
            _lastPos.X += (float)(deltaX * weightModifier);
            _lastPos.Y += (float)(deltaY * weightModifier);

            return _lastPos;
        }

        public FilterStage FilterStage => FilterStage.PostTranspose;

        [Property("Chatter Extermination Strength"), DefaultPropertyValue(2f), ToolTip
            ("Kuuube's CHATTER EXTERMINATOR:\n\n" +
            "Accepted settings are 1-20.\n" +
            "Recommended settings for the filter version: 2-3 for drag and 5-6 for hover.\n\n" +
            "For more information: Open the wiki from plugin manager or go to https://github.com/Kuuuube/Kuuube-s-CHATTER-EXTERMINATOR.")]
        public float AntichatterStrength { set; get; } = 3;
    }
}
