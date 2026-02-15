using System;
using Game.Core.Abstractions;
using Game.Core.Model;

namespace Game.Core.Commands
{
    public readonly struct MoveCommand : ICommand, IEquatable<MoveCommand>
    {
        public int Tick { get; }
        public PlayerId PlayerId { get; }
        public float Dx { get; }
        public float Dy { get; }

        public bool JumpPressed { get; }
        public bool JumpHeld { get; }

        public MoveCommand(int tick, PlayerId playerId, float dx, float dy, bool jumpPressed, bool jumpHeld)
        {
            Tick = tick;
            PlayerId = playerId;
            Dx = dx;
            Dy = dy;
            JumpPressed = jumpPressed;
            JumpHeld = jumpHeld;
        }

        public bool Equals(MoveCommand other) =>
            Tick == other.Tick &&
            PlayerId.Equals(other.PlayerId) &&
            Dx.Equals(other.Dx) &&
            Dy.Equals(other.Dy) &&
            JumpPressed == other.JumpPressed &&
            JumpHeld == other.JumpHeld;

        public override bool Equals(object obj) => obj is MoveCommand other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Tick, PlayerId, Dx, Dy, JumpPressed, JumpHeld);

        public static bool operator ==(MoveCommand left, MoveCommand right) => left.Equals(right);
        public static bool operator !=(MoveCommand left, MoveCommand right) => !left.Equals(right);

        public override string ToString() =>
            $"MoveCommand(tick={Tick}, player={PlayerId.Value}, dx={Dx}, dy={Dy}, jumpPressed={JumpPressed}, jumpHeld={JumpHeld})";
    }
}
