using System.Collections.Generic;
using Riftborne.Core.Model;

namespace Riftborne.Core.Combat.Resolution
{
    public interface IHitQuery
    {
        void QueryHits(
            GameEntityId attacker,
            List<GameEntityId> results);
    }
}
