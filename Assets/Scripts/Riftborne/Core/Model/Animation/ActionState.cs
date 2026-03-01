namespace Riftborne.Core.Model.Animation
{
    public enum ActionState : byte
    {
        None = 0,
        LightAttack = 1,
        HeavyAttack = 2,
        Parry = 3,
        Dodge = 4,
        DodgePerfect = 5,
        Hurt = 6,
        Dead = 7
    }
}