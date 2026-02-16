using Game.App.Commands;
using Game.App.Time;
using Game.Core.Commands;
using Game.Core.Model;
using Game.Core.Combat.Abilities;
using Game.Core.Combat.Config;
using Game.Configs;
using UnityEngine;
using VContainer;

namespace Game.Unity.Input
{
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private int controlledEntityId = 0;

        private GameEntityId Controlled => new GameEntityId(controlledEntityId);

        private ICommandQueue _commandQueue;
        private ITickClock _clock;
        private CombatInputTuning _tuning;

        private InputSnapshot _snapshot;

        private bool _prevJumpHeld;
        private bool _prevAttackHeld;
        private bool _prevDefenseHeld;

        // hold tracking (ticks)
        private int _attackHoldStartTick = -1;
        private bool _attackConsumedAsHeavy;

        private int _defenseHoldStartTick = -1;
        private bool _defenseConsumedAsBlock;

        [Inject]
        public void Construct(
            ICommandQueue commandQueue,
            ITickClock clock,
            CombatConfigAsset combatConfigAsset)
        {
            _commandQueue = commandQueue;
            _clock = clock;

            // один ассет — источник всего
            if (combatConfigAsset != null)
                _tuning = combatConfigAsset.ToInputTuning();
            else
                _tuning = new CombatInputTuning(10, 10, 6, 0.35f);
        }

        public void SetMove(float x, float y)
        {
            _snapshot.MoveX = x;
            _snapshot.MoveY = y;
        }

        public void SetJumpHeld(bool held)
        {
            _snapshot.JumpHeld = held;

            if (held && !_prevJumpHeld)
                _snapshot.JumpPressed = true;

            _prevJumpHeld = held;
        }

        // --- Combat input setters ---

        public void SetAttackHeld(bool held)
        {
            _snapshot.AttackHeld = held;

            if (held && !_prevAttackHeld)
            {
                _snapshot.AttackPressed = true;

                _attackHoldStartTick = _clock.CurrentTick;
                _attackConsumedAsHeavy = false;
            }

            // release -> decide Light if not consumed as Heavy
            if (!held && _prevAttackHeld)
            {
                if (!_attackConsumedAsHeavy)
                {
                    UseAbility(AbilitySlot.LightAttack);
                }

                _attackHoldStartTick = -1;
                _attackConsumedAsHeavy = false;
            }

            _prevAttackHeld = held;
        }

        public void SetDefenseHeld(bool held)
        {
            _snapshot.DefenseHeld = held;

            if (held && !_prevDefenseHeld)
            {
                _snapshot.DefensePressed = true;

                _defenseHoldStartTick = _clock.CurrentTick;
                _defenseConsumedAsBlock = false;
            }

            // release -> decide Parry if not consumed as Block
            if (!held && _prevDefenseHeld)
            {
                if (!_defenseConsumedAsBlock)
                {
                    UseAbility(AbilitySlot.Parry);
                }

                _defenseHoldStartTick = -1;
                _defenseConsumedAsBlock = false;
            }

            _prevDefenseHeld = held;
        }

        public void OnEvadePressed()
        {
            _snapshot.EvadePressed = true;
        }

        // --- Commands ---

        private void UseAbility(AbilitySlot slot)
        {
            var tick = _clock.CurrentTick;

            _commandQueue.Enqueue(
                new UseAbilityCommand(
                    tick,
                    Controlled,
                    slot
                )
            );
        }

        public void FlushForTick(int tick)
        {
            // 1) Movement каждую физику
            var s = _snapshot;

            _commandQueue.Enqueue(new MoveCommand(
                tick,
                Controlled,
                s.MoveX,
                s.MoveY,
                s.JumpPressed,
                s.JumpHeld
            ));

            // 2) Hold-threshold triggers (Heavy / Block)
            // Attack held long enough -> Heavy immediately once
            if (s.AttackHeld && !_attackConsumedAsHeavy && _attackHoldStartTick >= 0)
            {
                int heldTicks = tick - _attackHoldStartTick;
                if (heldTicks >= _tuning.AttackHoldTicksForHeavy)
                {
                    _attackConsumedAsHeavy = true;
                    UseAbility(AbilitySlot.HeavyAttack);
                }
            }

            // Defense held long enough -> Block (механика блока будет позже, но команда уже есть)
            if (s.DefenseHeld && !_defenseConsumedAsBlock && _defenseHoldStartTick >= 0)
            {
                int heldTicks = tick - _defenseHoldStartTick;
                if (heldTicks >= _tuning.DefenseHoldTicksForBlock)
                {
                    _defenseConsumedAsBlock = true;
                    UseAbility(AbilitySlot.Block);
                }
            }

            // 3) Evade: Dodge без направления, Dash с направлением
            if (s.EvadePressed)
            {
                float ax = s.MoveX;
                float abs = ax < 0f ? -ax : ax;

                if (abs >= _tuning.EvadeDeadzone)
                    UseAbility(AbilitySlot.Dash);
                else
                    UseAbility(AbilitySlot.Dodge);
            }

            // 4) Consume one-frame edges
            _snapshot.JumpPressed = false;
            _snapshot.AttackPressed = false;
            _snapshot.DefensePressed = false;
            _snapshot.EvadePressed = false;
        }
    }
}
