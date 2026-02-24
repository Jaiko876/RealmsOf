using System;
using Riftborne.Core.Input.Commands.Abstractions;
using Riftborne.Core.Model;

namespace Riftborne.Core.Input.Commands
{
    public readonly struct InputCommand : ICommand, IEquatable<InputCommand>
    {
        public int Tick { get; }
        public GameEntityId EntityId { get; }
        public float Dx { get; }
        public float Dy { get; }
        
        public InputButtons Buttons { get; }

        public InputCommand(int tick, GameEntityId entityId, float dx, float dy, InputButtons buttons)
        {
            Tick = tick;
            EntityId = entityId;
            Dx = dx;
            Dy = dy;
            Buttons = buttons;
        }

        public bool Equals(InputCommand other) =>
            Tick == other.Tick &&
            EntityId.Equals(other.EntityId) &&
            Dx.Equals(other.Dx) &&
            Dy.Equals(other.Dy) &&
            Buttons == other.Buttons;

        public override bool Equals(object obj) => obj is InputCommand other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Tick, EntityId, Dx, Dy, Buttons);

        public static bool operator ==(InputCommand left, InputCommand right) => left.Equals(right);
        public static bool operator !=(InputCommand left, InputCommand right) => !left.Equals(right);

        public override string ToString() =>
            $"MoveCommand(tick={Tick}, entity={EntityId.Value}, dx={Dx}, dy={Dy}, buttons={Buttons})";
    }
}
