namespace Riftborne.Core.Stats
{
    public struct StatValue
    {
        public float Base;
        public float Add;
        public float Mul; // 1 = no change

        public StatValue(float @base)
        {
            Base = @base;
            Add = 0f;
            Mul = 1f;
        }

        public float Effective => (Base + Add) * (Mul == 0f ? 1f : Mul);

        public void ClearMods()
        {
            Add = 0f;
            Mul = 1f;
        }
    }
}