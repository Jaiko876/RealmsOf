using System;
using Riftborne.Core.Abstractions;
using Riftborne.Core.Model;

namespace Riftborne.Core.Commands
{
    public readonly struct MoveCommand : ICommand, IEquatable<MoveCommand>
    {
        public int Tick { get; }
        public GameEntityId EntityId { get; }
        public float Dx { get; }
        public float Dy { get; }

        public bool JumpPressed { get; }
        public bool JumpHeld { get; }

        public MoveCommand(int tick, GameEntityId entityId, float dx, float dy, bool jumpPressed, bool jumpHeld)
        {
            Tick = tick;
            EntityId = entityId;
            Dx = dx;
            Dy = dy;
            JumpPressed = jumpPressed;
            JumpHeld = jumpHeld;
        }

        public bool Equals(MoveCommand other) =>
            Tick == other.Tick &&
            EntityId.Equals(other.EntityId) &&
            Dx.Equals(other.Dx) &&
            Dy.Equals(other.Dy) &&
            JumpPressed == other.JumpPressed &&
            JumpHeld == other.JumpHeld;

        public override bool Equals(object obj) => obj is MoveCommand other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Tick, EntityId, Dx, Dy, JumpPressed, JumpHeld);

        public static bool operator ==(MoveCommand left, MoveCommand right) => left.Equals(right);
        public static bool operator !=(MoveCommand left, MoveCommand right) => !left.Equals(right);

        public override string ToString() =>
            $"MoveCommand(tick={Tick}, entity={EntityId.Value}, dx={Dx}, dy={Dy}, jumpPressed={JumpPressed}, jumpHeld={JumpHeld})";
    }
}
