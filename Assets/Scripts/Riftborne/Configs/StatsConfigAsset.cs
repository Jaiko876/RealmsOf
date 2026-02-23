using Riftborne.Core.Stats;
using UnityEngine;

namespace Riftborne.Configs
{
    [CreateAssetMenu(menuName = "Riftborne/Config/Stats", fileName = "StatsConfig")]
    public sealed class StatsConfigAsset : ScriptableObject
    {
        [Header("Resources")]
        [Min(1)] public int HpMax = 100;
        [Min(1)] public int StaminaMax = 100;
        [Min(1)] public int StaggerMax = 100;

        [Header("Primaries")]
        [Min(0f)] public float Attack = 10f;
        [Min(0f)] public float Defense = 0f;
        [Min(0f)] public float MoveSpeed = 1f;
        [Min(0f)] public float AttackSpeed = 1f;
        [Min(0f)] public float ChargeSpeed = 1f;
        [Min(0f)] public float StaggerResist = 0f;

        [Header("Regen/Decay (per second)")]
        public float HpRegenPerSec = 0f;
        public float StaminaRegenPerSec = 12f;
        public float StaggerDecayPerSec = 25f;

        public StatsDefaults ToDefaults()
        {
            return new StatsDefaults(
                hpMax: HpMax,
                staminaMax: StaminaMax,
                staggerMax: StaggerMax,
                attack: Attack,
                defense: Defense,
                moveSpeed: MoveSpeed,
                attackSpeed: AttackSpeed,
                chargeSpeed: ChargeSpeed,
                staggerResist: StaggerResist,
                hpRegenPerSec: HpRegenPerSec,
                staminaRegenPerSec: StaminaRegenPerSec,
                staggerDecayPerSec: StaggerDecayPerSec
            );
        }
    }
}