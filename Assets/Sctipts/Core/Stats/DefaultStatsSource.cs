using System.Collections.Generic;
using Game.Core.Model;

namespace Game.Core.Stats
{
    public sealed class DefaultStatsSource : IStatSource
    {
        // Приоритет очень низкий: любые другие источники могут переопределить/нарастить.
        private const int Priority = -1000;

        private readonly Dictionary<StatId, float> _defaults = new Dictionary<StatId, float>();

        public DefaultStatsSource()
        {
            // Минимум, чтобы всё не было 0.
            _defaults[StatId.MaxHp] = 10f;
            _defaults[StatId.HpRegenPerSec] = 0f;

            _defaults[StatId.Defense] = 0f;
            _defaults[StatId.ArmorPenetration] = 0f;
            _defaults[StatId.DamageTakenMultiplier] = 1f;
            _defaults[StatId.DamageDealtMultiplier] = 1f;

            _defaults[StatId.MaxStamina] = 10f;
            _defaults[StatId.StaminaRegenPerSec] = 5f;
            _defaults[StatId.StaminaRegenDelaySec] = 0.35f;

            _defaults[StatId.MaxStagger] = 10f;
            _defaults[StatId.StaggerDecayPerSec] = 3f;
            _defaults[StatId.StaggerVulnerableBonus] = 1.5f;

            _defaults[StatId.MoveSpeed] = 5f;
            _defaults[StatId.JumpImpulse] = 8f;

            _defaults[StatId.ParryWindowSec] = 0.12f;
            _defaults[StatId.DodgeIFramesSec] = 0.18f;
            _defaults[StatId.BlockDamageTakenMultiplier] = 0.6f;
            _defaults[StatId.BlockStaminaCostPerSec] = 2.0f;

            _defaults[StatId.LightDamageHp] = 2f;
            _defaults[StatId.LightDamageStamina] = 1f;
            _defaults[StatId.LightBuildStagger] = 2f;

            _defaults[StatId.HeavyDamageHp] = 4f;
            _defaults[StatId.HeavyDamageStamina] = 2f;
            _defaults[StatId.HeavyBuildStagger] = 4f;
        }

        public bool TryGetModifiers(GameEntityId entityId, StatId statId, List<StatModifier> outModifiers)
        {
            float v;
            if (!_defaults.TryGetValue(statId, out v))
                return false;

            // Дефолт как Add: можно сверху добавлять, умножать и т.д.
            outModifiers.Add(StatModifier.Add(v, Priority));
            return true;
        }
    }
}
