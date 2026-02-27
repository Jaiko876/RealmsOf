using System;
using Riftborne.App.Combat.Providers.Abstractions;
using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;

namespace Riftborne.App.Animation.Provider
{
    public sealed class StatsAnimationModifiersProvider : IAnimationModifiersProvider
    {
        private readonly ICombatSpeedProvider _speeds;

        public StatsAnimationModifiersProvider(ICombatSpeedProvider speeds)
        {
            _speeds = speeds ?? throw new ArgumentNullException(nameof(speeds));
        }

        public AnimationModifiers Get(GameEntityId entityId)
        {
            var s = _speeds.Get(entityId);
            return new AnimationModifiers(s.AttackSpeed, s.ChargeSpeed);
        }
    }
}