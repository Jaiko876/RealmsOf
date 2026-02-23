namespace Riftborne.Core.Model.Animation
{
    public struct AnimationState
    {
        public ActionState Action;
        public sbyte Facing;         // -1/+1

        public bool Grounded;
        public bool JustLanded;
        public bool Moving;


        public float Speed01;        // 0..1 (земля)
        public float AirSpeed01;     // 0..1 (воздух)
        public float AirT;           // 0..1 (вверх->вниз)
        
        public bool HeavyCharging;   // true когда держим и порог пройден
        public float Charge01;       // 0..1 прогресс (опционально)
        
        public float AttackAnimSpeed;
        public float ChargeAnimSpeed;
    }
}