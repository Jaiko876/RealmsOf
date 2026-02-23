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

            public GroundProbeTuning(float skin, float checkDepth, float widthMultiplier, float probeHeight)
            {
                Skin = skin;
                CheckDepth = checkDepth;
                WidthMultiplier = widthMultiplier;
                ProbeHeight = probeHeight;
            }
        }

        public readonly struct WallProbeTuning
        {
            public readonly float Skin;
            public readonly float CheckDistance;
            public readonly float ProbeThickness;
            public readonly float HeightShrink;
            public readonly float MinWallNormalAbsX;

            public WallProbeTuning(float skin, float checkDistance, float probeThickness, float heightShrink, float minWallNormalAbsX)
            {
                Skin = skin;
                CheckDistance = checkDistance;
                ProbeThickness = probeThickness;
                HeightShrink = heightShrink;
                MinWallNormalAbsX = minWallNormalAbsX;
            }
        }
    }
}