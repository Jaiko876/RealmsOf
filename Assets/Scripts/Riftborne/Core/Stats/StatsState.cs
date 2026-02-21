using System;
using Riftborne.Core.Model;

namespace Riftborne.Core.Stats
{
    public sealed class StatsState
    {
        public GameEntityId EntityId { get; }
        public bool IsInitialized { get; private set; }

        public int HpCur { get; private set; }
        public int StaminaCur { get; private set; }
        public int StaggerCur { get; private set; } // starts at 0

        private readonly StatValue[] _values;

        public StatsState(GameEntityId entityId)
        {
            EntityId = entityId;
            _values = new StatValue[(int)StatId.Count];
        }

        public StatValue Get(StatId id) => _values[(int)id];

        public float GetEffective(StatId id) => _values[(int)id].Effective;

        public void SetBase(StatId id, float value)
        {
            var i = (int)id;
            var v = _values[i];
            if (v.Mul == 0f) v.Mul = 1f;
            v.Base = value;
            _values[i] = v;
        }

        public void AddFlat(StatId id, float add)
        {
            var i = (int)id;
            var v = _values[i];
            if (v.Mul == 0f) v.Mul = 1f;
            v.Add += add;
            _values[i] = v;
        }

        public void MulBy(StatId id, float mul)
        {
            var i = (int)id;
            var v = _values[i];
            if (v.Mul == 0f) v.Mul = 1f;
            v.Mul *= mul;
            _values[i] = v;
        }

        public int HpMax => ToIntMin1(GetEffective(StatId.HpMax));
        public int StaminaMax => ToIntMin1(GetEffective(StatId.StaminaMax));
        public int StaggerMax => ToIntMin1(GetEffective(StatId.StaggerMax));

        public void InitializeFromDefaults(in StatsDefaults d)
        {
            SetBase(StatId.HpMax, d.HpMax);
            SetBase(StatId.StaminaMax, d.StaminaMax);
            SetBase(StatId.StaggerMax, d.StaggerMax);

            SetBase(StatId.Attack, d.Attack);
            SetBase(StatId.Defense, d.Defense);
            SetBase(StatId.MoveSpeed, d.MoveSpeed);
            SetBase(StatId.AttackSpeed, d.AttackSpeed);
            SetBase(StatId.StaggerResist, d.StaggerResist);

            SetBase(StatId.HpRegenPerSec, d.HpRegenPerSec);
            SetBase(StatId.StaminaRegenPerSec, d.StaminaRegenPerSec);
            SetBase(StatId.StaggerDecayPerSec, d.StaggerDecayPerSec);

            HpCur = HpMax;
            StaminaCur = StaminaMax;
            StaggerCur = 0;

            IsInitialized = true;
        }

        public void SetHp(int value) => HpCur = Clamp(value, 0, HpMax);
        public void SetStamina(int value) => StaminaCur = Clamp(value, 0, StaminaMax);
        public void SetStagger(int value) => StaggerCur = Clamp(value, 0, StaggerMax);

        public void AddHp(int delta) => SetHp(HpCur + delta);
        public void AddStamina(int delta) => SetStamina(StaminaCur + delta);
        public void AddStagger(int delta) => SetStagger(StaggerCur + delta);
        
        public void ClearAllMods()
        {
            for (int i = 0; i < _values.Length; i++)
            {
                var v = _values[i];
                v.Add = 0f;
                v.Mul = 1f;
                _values[i] = v;
            }
        }

        private static int ToIntMin1(float v)
        {
            if (v < 1f) v = 1f;
            return (int)MathF.Round(v);
        }

        private static int Clamp(int v, int min, int max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
    }
}