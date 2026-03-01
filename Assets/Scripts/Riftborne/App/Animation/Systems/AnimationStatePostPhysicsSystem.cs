using System;
using Riftborne.App.Animation.Composition;
using Riftborne.App.Animation.Composition.Abstractions;
using Riftborne.App.Animation.Events;
using Riftborne.App.Animation.Provider;
using Riftborne.Core.Config;
using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;
using Riftborne.Core.Physics.Model;
using Riftborne.Core.Stores.Abstractions;
using Riftborne.Core.Systems.PostPhysicsTickSystems;

namespace Riftborne.App.Animation.Systems
{
    public sealed class AnimationStatePostPhysicsSystem : IPostPhysicsTickSystem
    {
        private readonly GameState _state;
        private readonly MotorParams _motor;
        private readonly InputTuning _input;

        private readonly IAttackChargeStore _charge;
        private readonly IAnimationModifiersProvider _animMods;

        private readonly IActionEventStore _events;
        private readonly IAnimationStateComposer _composer;
        
        private readonly IBlockStateStore _block;

        public AnimationStatePostPhysicsSystem(
            GameState state,
            MotorParams motor,
            IAttackChargeStore charge,
            IAnimationModifiersProvider animMods,
            IGameplayTuning tuning,
            IActionEventStore events,
            IAnimationStateComposer composer, 
            IBlockStateStore block)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _motor = motor ?? throw new ArgumentNullException(nameof(motor));
            _charge = charge ?? throw new ArgumentNullException(nameof(charge));
            _animMods = animMods ?? throw new ArgumentNullException(nameof(animMods));
            _input = (tuning ?? throw new ArgumentNullException(nameof(tuning))).Input;
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _composer = composer ?? throw new ArgumentNullException(nameof(composer));
            _block = block ?? throw new ArgumentNullException(nameof(block));
        }

        public void Tick(int tick)
        {
            foreach (var kv in _state.Entities)
            {
                var e = kv.Value;

                AttackChargeSnapshot charge;
                if (_charge.TryGet(e.Id, out var charging, out var charge01))
                    charge = new AttackChargeSnapshot(charging, charge01);
                else
                    charge = AttackChargeSnapshot.None;

                var mods = _animMods.Get(e.Id);

                ActionPayload action;
                if (_events.TryConsume(e.Id, tick, out var ev) && ev.Action != ActionState.None)
                {
                    action = new ActionPayload(ev.Action, tick, ev.DurationTicks);
                }
                else
                {
                    action = ActionPayload.None(tick);
                }

                var blocking = _block.IsBlocking(e.Id);

                var ctx = new AnimationStateComposeContext(
                    tick,
                    e,
                    e.AnimationState,
                    _motor,
                    _input,
                    mods,
                    charge,
                    action,
                    blocking);

                var next = _composer.Compose(in ctx);
                e.SetAnimationState(next);
            }
        }
    }
}