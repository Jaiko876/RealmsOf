using Riftborne.Core.Model;
using Riftborne.Unity.View.Presenters.Abstractions;
using UnityEngine;

namespace Riftborne.Unity.View.Presenters
{
    public sealed class EntityTransformPresenter : IEntityTransformPresenter
    {
        public void Present(EntityState e, float alpha01, Transform visualRoot, Transform flipRoot)
        {
            if (e == null) return;
            if (visualRoot == null) return;

            var x = Mathf.Lerp(e.PrevX, e.X, alpha01);
            var y = Mathf.Lerp(e.PrevY, e.Y, alpha01);
            visualRoot.position = new Vector3(x, y, 0f);

            ApplyFacing(e.Facing, flipRoot);
        }

        private static void ApplyFacing(int facing, Transform flipRoot)
        {
            if (flipRoot == null) return;

            var s = flipRoot.localScale;
            var ax = Mathf.Abs(s.x);
            s.x = facing < 0 ? -ax : ax;
            flipRoot.localScale = s;
        }
    }
}