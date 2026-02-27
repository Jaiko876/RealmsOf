using Riftborne.Core.Model;
using UnityEngine;

namespace Riftborne.Unity.View.Presenters.Abstractions
{
    public interface IEntityTransformPresenter
    {
        void Present(EntityState e, float alpha01, Transform visualRoot, Transform flipRoot);
    }
}