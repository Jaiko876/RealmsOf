using Game.Core.Model;

namespace Game.Core.Combat.Rules
{
    public interface ICombatRulesModifier
    {
        int Priority { get; }

        /// <summary>
        /// Возвращает true, если модификатор хочет переопределить/изменить значение правила.
        /// Для bool: value 0/1.
        /// Для int: value как int.
        /// Для float: value как float.
        /// Тип значения определяется самим CombatRuleId и трактуется в резолвере.
        /// </summary>
        bool TryModify(GameEntityId subject, CombatRuleId ruleId, ref float value);
    }
}
