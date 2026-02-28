using Riftborne.Core.Model;

namespace Riftborne.Core.Entities
{
    public interface IGameEntityIdReceiver
    {
        void SetEntityId(GameEntityId id);
    }

    public interface IPlayerOwnerReceiver
    {
        void SetOwner(PlayerId playerId);
    }
}