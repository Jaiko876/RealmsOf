using Riftborne.Core.Config;
using Riftborne.Core.Model;

namespace Riftborne.Core.Gameplay.Combat.Model
{
    public readonly struct AttackInputStepRequest
    {
        public readonly GameEntityId Id;
        public readonly int Tick;
        public readonly bool HeldNow;

        public readonly bool PrevHeld;
        public readonly int HeldTicks;

        public readonly float AttackSpeed; // уже clamped
        public readonly float ChargeSpeed;  // уже clamped

        public readonly CombatInputTuning InputTuning;
        public readonly CombatAnimationTuning AnimTuning;

        public AttackInputStepRequest(
            GameEntityId id, int tick, bool heldNow,
            bool prevHeld, int heldTicks,
            float attackSpeed, float chargeSpeed,
            CombatInputTuning inputTuning, CombatAnimationTuning animTuning)
        {
            Id = id;
            Tick = tick;
            HeldNow = heldNow;
            PrevHeld = prevHeld;
            HeldTicks = heldTicks;
            AttackSpeed = attackSpeed;
            ChargeSpeed = chargeSpeed;
            InputTuning = inputTuning;
            AnimTuning = animTuning;
        }
    }

}