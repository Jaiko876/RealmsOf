namespace Riftborne.Core.Config
{
    public readonly struct PhysicsProbesTuning
    {
        public readonly GroundProbeTuning Ground;
        public readonly WallProbeTuning Wall;

        public PhysicsProbesTuning(GroundProbeTuning ground, WallProbeTuning wall)
        {
            Ground = ground;
            Wall = wall;
        }

        public readonly struct GroundProbeTuning
        {
            public readonly float Skin;
            public readonly float CheckDepth;
            public readonly float WidthMultiplier;
            public readonly float ProbeHeight;
            public readonly int GroundLayerMask;

            public GroundProbeTuning(float skin, float checkDepth, float widthMultiplier, float probeHeight, int groundLayerMask)
            {
                Skin = skin;
                CheckDepth = checkDepth;
                WidthMultiplier = widthMultiplier;
                ProbeHeight = probeHeight;
                GroundLayerMask = groundLayerMask;
            }
        }

        public readonly struct WallProbeTuning
        {
            public readonly float Skin;
            public readonly float CheckDistance;
            public readonly float ProbeThickness;
            public readonly float HeightShrink;
            public readonly float MinWallNormalAbsX;
            public readonly int WallLayerMask;

            public WallProbeTuning(float skin, float checkDistance, float probeThickness, float heightShrink, float minWallNormalAbsX, int wallLayerMask)
            {
                Skin = skin;
                CheckDistance = checkDistance;
                ProbeThickness = probeThickness;
                HeightShrink = heightShrink;
                MinWallNormalAbsX = minWallNormalAbsX;
                WallLayerMask = wallLayerMask;
            }
        }
    }
}