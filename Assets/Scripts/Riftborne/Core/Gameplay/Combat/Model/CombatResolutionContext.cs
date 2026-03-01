namespace Riftborne.Core.Gameplay.Combat.Model
{
    public struct CombatResolutionContext
    {
        public CombatResolveRequest Request;

        // Effective states (modifiers may override)
        public bool EffectiveParry;
        public bool EffectiveDodge;
        public bool EffectiveBlock;

        public CombatRuleFlags Flags;

        public static CombatResolutionContext Create(in CombatResolveRequest req)
        {
            var c = new CombatResolutionContext();
            c.Request = req;
            c.EffectiveParry = req.DefenderParryActive;
            c.EffectiveDodge = req.DefenderDodgeActive;
            c.EffectiveBlock = req.DefenderBlockActive;
            c.Flags = default;
            return c;
        }
    }
}