using Riftborne.Core.Physics.Model;
using Riftborne.Core.TIme;
using UnityEngine;

namespace Riftborne.Configs
{
    [CreateAssetMenu(menuName = "Riftborne/Config/Motor", fileName = "MotorConfig")]
    public sealed class MotorConfigAsset : ScriptableObject
    {
        [Header("Horizontal")]
        [Min(0f)] public float MaxSpeedX = 8f;
        [Min(0f)] public float AccelX = 60f;
        [Min(0f)] public float DecelX = 70f;

        [Header("Jump")]
        public float JumpVelocity = 12f;
        [Min(0f)] public float CoyoteTimeSeconds = 0.08f;
        [Min(0f)] public float JumpBufferSeconds = 0.10f;
        [Header("Air")]
        [Min(0f)] public float MaxFallSpeed = 20f;

        public MotorParams ToMotorParams(SimulationParameters sim)
        {
            int SecToTicks(float seconds)
            {
                if (seconds <= 0f) return 0;
                return Mathf.CeilToInt(seconds / sim.TickDeltaTime);
            }

            return new MotorParams(
                MaxSpeedX,
                AccelX,
                DecelX,
                JumpVelocity,
                SecToTicks(CoyoteTimeSeconds),
                SecToTicks(JumpBufferSeconds),
                maxFallSpeed: MaxFallSpeed
            );
        }
    }
}