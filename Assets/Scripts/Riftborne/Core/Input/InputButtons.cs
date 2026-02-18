using System;

namespace Riftborne.Core.Input
{
    [Flags]
    public enum InputButtons : ushort
    {
        None = 0,

        JumpPressed = 1 << 0,
        JumpHeld = 1 << 1,

        AttackPressed = 1 << 2,
        AttackHeld = 1 << 3,
        AttackHeavyPressed = 1 << 7,

        DefensePressed = 1 << 4,
        DefenseHeld = 1 << 5,

        EvadePressed = 1 << 6
    }
}