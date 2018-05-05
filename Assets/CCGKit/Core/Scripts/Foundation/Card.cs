using System.Collections.Generic;

using UnityEngine.Assertions;

namespace CCGKit
{
    /// <summary>
    /// This class represents a single card in the game.
    /// このクラスは、ゲーム内の1枚のカードを表します。
    /// </summary>
    public class Card : Resource
    {
        /// <summary>
        /// カードの所属陣営(追加)
        /// </summary>
        public static int groupId { get; set; }

        /// <summary>
        /// The current resource identifier.
        /// 現在のリソース識別子(カードのユニークID)
        /// </summary>
//        public static int currentId;
        public static int currentId { get; set; }

        /// <summary>
        /// The type of this card.
        /// このカードのタイプ。
        /// </summary>
//        public int cardTypeId;
        public int cardTypeId { get; set; }

        /// <summary>
        /// The name of this card.
        /// このカードの名前。
        /// </summary>
//        public string name;
        public string name { get; set; }

        /// <summary>
        /// The costs of this card.
        /// このカードのコスト。
        /// </summary>
        public List<Cost> costs = new List<Cost>();

        /// <summary>
        /// The properties of this card.
        /// このカードの特性。
        /// </summary>
        public List<Property> properties = new List<Property>();

        /// <summary>
        /// The stats of this card.
        /// このカードのスタッツ情報。
        /// </summary>
        public List<Stat> stats = new List<Stat>();

        /// <summary>
        /// The keywords of this card.
        /// このカードのキーワード。
        /// </summary>
        public List<RuntimeKeyword> keywords = new List<RuntimeKeyword>();

        /// <summary>
        /// The abilities of this card.
        /// このカードの能力。
        /// </summary>
        public List<Ability> abilities = new List<Ability>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public Card() : base(currentId++)
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
