using System.Collections.Generic;
using Riftborne.Core.Model;

namespace Riftborne.Core.Stats
{
    /// <summary>
    /// Источник модификаторов статов для конкретной сущности.
    /// Например: базовые статы монстра, экипировка, бафы, модификатор реалма.
    /// </summary>
    public interface IStatSource
    {
        /// <summary>
        /// Добавляет модификаторы в outModifiers (не очищает список).
        /// Возвращает true, если что-то добавил.
        /// </summary>
        bool TryGetModifiers(GameEntityId entityId, StatId statId, List<StatModifier> outModifiers);
    }
}
