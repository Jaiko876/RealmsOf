namespace Riftborne.Core.Level
{
    public sealed class LevelGenConfig
    {
        public int Width;
        public int BaseGroundY;

        // Границы высот
        public int MinGroundY;
        public int MaxGroundY;

        // Ограничения проходимости (главное!)
        public int MaxStepUp;   // на сколько клеток можно подняться за 1 X
        public int MaxStepDown; // на сколько клеток можно спуститься за 1 X

        // Биомы / “характер ландшафта”
        public int MinSegmentLen;
        public int MaxSegmentLen;

        public int PlainsStep;     // шаг по высоте в равнине (0..1)
        public int HillsStep;      // 1..2
        public int MountainsStep;  // 2..4

        public int PlainsChance;     // веса для выбора сегмента
        public int HillsChance;
        public int MountainsChance;

        // Глубина “земли” вниз (сколько заполняем dirt/stone)
        public int FillDepth;

        // Stone начинается ниже чем Dirt
        public int StoneStartDepth;

        // Объекты
        public int SpawnerCount;
        public int SpawnerMinDistanceFromPlayer;
    }
}
