using System;
using Game.Domain.Abstractions;
using Game.Domain.Model;

namespace Game.Domain.Commands
{
    public readonly struct MoveCommand : ICommand, IEquatable<MoveCommand>
    {
        public int Tick { get; }
        public PlayerId PlayerId { get; }
        public float Dx { get; }
        public float Dy { get; }

        public MoveCommand(int tick, PlayerId playerId, float dx, float dy)
        {
            Tick = tick;
            PlayerId = playerId;
            Dx = dx;
            Dy = dy;
        }

        public bool Equals(MoveCommand other) =>
            Tick == other.Tick &&
            PlayerId.Equals(other.PlayerId) &&
            Dx.Equals(other.Dx) &&
            Dy.Equals(other.Dy);

        public override bool Equals(object obj) => obj is MoveCommand other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Tick, PlayerId, Dx, Dy);

        public static bool operator ==(MoveCommand left, MoveCommand right) => left.Equals(right);
        public static bool operator !=(MoveCommand left, MoveCommand right) => !left.Equals(right);

        public override string ToString() => $"MoveCommand(tick={Tick}, player={PlayerId.Value}, dx={Dx}, dy={Dy})";
    }
}
