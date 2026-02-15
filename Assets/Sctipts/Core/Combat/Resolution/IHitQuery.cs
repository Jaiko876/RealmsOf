using System.Collections.Generic;
using Game.Core.Model;

namespace Game.Core.Combat.Resolution
{
    public interface IHitQuery
    {
        void QueryHits(
            GameEntityId attacker,
            List<GameEntityId> results);
    }
}
