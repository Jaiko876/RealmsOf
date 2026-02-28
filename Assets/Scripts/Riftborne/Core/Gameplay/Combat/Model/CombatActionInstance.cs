using System;

namespace Riftborne.Core.Gameplay.Combat.Model
{
    public readonly struct CombatActionInstance
    {
        public readonly CombatActionType Type;
        public readonly int StartTick;

        public readonly int WindupTicks;
        public readonly int ActiveTicks;
        public readonly int RecoveryTicks;

        public readonly int CooldownTicks;

        public readonly sbyte LockedFacing;   // 0 = no lock, else -1/+1
        public readonly bool HitApplied;      // for attacks: only once per action

        public CombatActionInstance(
            CombatActionType type,
            int startTick,
            int windupTicks,
            int activeTicks,
            int recoveryTicks,
            int cooldownTicks,
            sbyte lockedFacing,
            bool hitApplied)
        {
            Type = type;
            StartTick = startTick;
            WindupTicks = Math.Max(0, windupTicks);
            ActiveTicks = Math.Max(0, activeTicks);
            RecoveryTicks = Math.Max(0, recoveryTicks);
            CooldownTicks = Math.Max(0, cooldownTicks);
            LockedFacing = lockedFacing;
            HitApplied = hitApplied;
        }

        public int TotalTicks => WindupTicks + ActiveTicks + RecoveryTicks;

        public bool IsRunningAt(int tick)
        {
            int e = tick - StartTick;
            return e >= 0 && e < TotalTicks;
        }

        public CombatPhase GetPhaseAt(int tick)
        {
            int e = tick - StartTick;
            if (e < 0 || e >= TotalTicks) return CombatPhase.None;

            if (e < WindupTicks) return CombatPhase.Windup;
            if (e < WindupTicks + ActiveTicks) return CombatPhase.Active;
            return CombatPhase.Recovery;
        }

        public bool IsActiveAt(int tick) => GetPhaseAt(tick) == CombatPhase.Active;

        public bool IsFirstActiveTick(int tick)
        {
            int e = tick - StartTick;
            return e == WindupTicks;
        }

        public CombatActionInstance WithHitApplied(bool v)
            => new CombatActionInstance(Type, StartTick, WindupTicks, ActiveTicks, RecoveryTicks, CooldownTicks, LockedFacing, v);
    }
}