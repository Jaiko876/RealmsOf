// Assets/Scripts/Riftborne/App/Combat/Rules/CombatRulesResolver.cs
using System;
using System.Collections.Generic;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Gameplay.Combat.Rules.Abstractions;

namespace Riftborne.App.Combat.Rules
{
    public sealed class CombatRulesResolver : ICombatRulesResolver
    {
        private readonly ICombatRulesEngine _engine;
        private readonly ICombatRulesModifier[] _mods;

        public CombatRulesResolver(ICombatRulesEngine engine, IEnumerable<ICombatRulesModifier> mods)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            _mods = Order(mods);
        }

        public CombatHitResult Resolve(in CombatResolveRequest req)
        {
            var ctx = CombatResolutionContext.Create(in req);

            for (int i = 0; i < _mods.Length; i++)
                _mods[i].Apply(ref ctx);

            return _engine.Resolve(in ctx);
        }

        private static ICombatRulesModifier[] Order(IEnumerable<ICombatRulesModifier> mods)
        {
            if (mods == null) return Array.Empty<ICombatRulesModifier>();

            var list = new List<ICombatRulesModifier>();
            foreach (var m in mods)
                if (m != null) list.Add(m);

            list.Sort((a, b) =>
            {
                int c = a.Order.CompareTo(b.Order);
                if (c != 0) return c;
                return string.CompareOrdinal(a.GetType().FullName, b.GetType().FullName);
            });

            return list.ToArray();
        }
    }
}