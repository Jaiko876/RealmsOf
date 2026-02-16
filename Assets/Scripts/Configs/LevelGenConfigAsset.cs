using UnityEngine;

namespace Game.Configs
{
    [CreateAssetMenu(menuName = "Game/Config/LevelGenConfig")]
    public sealed class LevelGenConfigAsset : ScriptableObject
    {
        [Header("Size")]
        [Min(1)] public int Width = 220;

        [Header("Ground Height")]
        public int BaseGroundY = 6;
        public int MinGroundY = 2;
        public int MaxGroundY = 20;

        [Header("Walkability constraints")]
        [Min(0)] public int MaxStepUp = 1;
        [Min(0)] public int MaxStepDown = 2;

        [Header("Biome segments")]
        [Min(1)] public int MinSegmentLen = 10;
        [Min(1)] public int MaxSegmentLen = 40;

        [Header("Steps per biome")]
        [Range(0, 2)] public int PlainsStep = 0;
        [Range(1, 3)] public int HillsStep = 2;
        [Range(2, 6)] public int MountainsStep = 4;

        [Header("Biome weights")]
        [Min(0)] public int PlainsChance = 55;
        [Min(0)] public int HillsChance = 30;
        [Min(0)] public int MountainsChance = 15;

        [Header("Fill")]
        [Min(1)] public int FillDepth = 18;
        [Min(1)] public int StoneStartDepth = 8;

        [Header("Objects")]
        [Min(0)] public int SpawnerCount = 3;
        [Min(0)] public int SpawnerMinDistanceFromPlayer = 25;
    }
}
