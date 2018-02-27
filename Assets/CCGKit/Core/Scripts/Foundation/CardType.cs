using UnityEngine.Assertions;

using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// This class represents a single card type in the game.
    /// このクラスは、ゲーム内の単一のカードタイプを表します。
    /// </summary>
    public class CardType : Resource
    {
        /// <summary>
        /// The current resource identifier.
        /// 現在のリソース識別子。
        /// </summary>
        public static int currentId;

        /// <summary>
        /// The name of this card type.
        /// このカードタイプの名前。
        /// </summary>
        public string name;

        /// <summary>
        /// The properties of this card type.
        /// このカードタイプのプロパティ。
        /// </summary>
        public List<Property> properties = new List<Property>();

        /// <summary>
        /// The stats of this card type.
        /// このカードタイプのスタッツ
        /// </summary>
        public List<DefinitionStat> stats = new List<DefinitionStat>();

        /// <summary>
        /// The destroy conditions of this card type.
        /// このカードタイプの破壊条件。
        /// </summary>
        public List<DestroyCardCondition> destroyConditions = new List<DestroyCardCondition>();

        /// <summary>
        /// True if this card should move to another zone after triggering its effect
        /// このカードが効果を発動した後に別のゾーンに移動する必要がある場合はTrue
        /// (useful for spell-like cards); false otherwise.
        /// （スペルのようなカードに便利です）。 そうでなければfalse。
        /// </summary>
        public bool moveAfterTriggeringEffect;

        /// <summary>
        /// The zone to which this card should move after triggering its effect.
        /// このカードが効果を発動した後に移動するゾーン。
        /// </summary>
        [GameZoneField("Zone")]
        public int zoneId;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CardType() : base(currentId++)
        {
        }

        /// <summary>
        /// Returns the value of the integer property with the specified name.
        /// 指定された名前を持つ整数プロパティの値を返します。
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        public int GetIntProperty(string name)
        {
            var property = properties.Find(x => x.name == name && x is IntProperty);
            Assert.IsNotNull(property);
            return (property as IntProperty).value;
        }

        /// <summary>
        /// Returns the value of the string property with the specified name.
        /// 指定した名前の文字列プロパティの値を返します。
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        public string GetStringProperty(string name)
        {
            var property = properties.Find(x => x.name == name && x is StringProperty);
            Assert.IsNotNull(property);
            return (property as StringProperty).value;
        }
    }
}
