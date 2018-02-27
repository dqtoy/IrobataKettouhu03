using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// The ability types supported by the kit.
    /// キットでサポートされている能力タイプ。
    /// </summary>
    public enum AbilityType
    {
        Triggered,
        Activated
    }

    /// <summary>
    /// The base ability class.
    /// 基礎能力クラス。
    /// </summary>
    public class Ability
    {
        /// <summary>
        /// The name of this ability.
        /// この能力の名前。
        /// </summary>
        public string name;

        /// <summary>
        /// The type of this ability.
        /// この能力のタイプ。
        /// </summary>
        public AbilityType type;

        /// <summary>
        /// The effect of this ability.
        /// この能力の効果。
        /// </summary>
        public Effect effect;

        /// <summary>
        /// The target of this ability.
        /// この能力の目標。
        /// </summary>
        public Target target;
    }

    /// <summary>
    /// Triggered abilities are abilities that get resolved when their
    /// associated trigger takes place.
    /// トリガー能力とは、そのトリガーが発生したときに解決される能力です。
    /// </summary>
    public class TriggeredAbility : Ability
    {
        /// <summary>
        /// The trigger of this ability.
        /// この能力の引き金。
        /// </summary>
        public Trigger trigger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TriggeredAbility()
        {
            type = AbilityType.Triggered;
        }
    }

    /// <summary>
    /// Activated abilities are abilities that get resolved when the player
    /// pays a cost/s.
    /// アクティブ化された能力は、プレイヤーがコストを支払ったときに解決される能力です。
    /// </summary>
    public class ActivatedAbility : Ability
    {
        /// <summary>
        /// The zone of this ability.
        /// この能力のゾーン。
        /// </summary>
        [GameZoneField("Zone")]
        public int zoneId;

        /// <summary>
        /// The costs of this ability.
        /// この能力のコスト
        /// </summary>
        public List<Cost> costs = new List<Cost>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActivatedAbility()
        {
            type = AbilityType.Activated;
        }
    }
}
