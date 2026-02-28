namespace Riftborne.Core.Config
{
    public readonly struct CombatHitTuning
    {
        public readonly int TargetLayerMask;

        public CombatHitTuning(int targetLayerMask)
        {
            TargetLayerMask = targetLayerMask;
        }
    }
}