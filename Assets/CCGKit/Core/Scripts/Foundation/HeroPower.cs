using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections.Generic;

using UnityEngine.Assertions;

/// <summary>
/// Cardクラスのパクリ
/// </summary>
namespace CCGKit
{
    public class HeroPower : Resource
    {
        /// <summary>
        /// The current resource identifier.
        /// 現在のリソース識別子(ヒロパのユニークID)
        /// </summary>
        public static int currentId;

        /// <summary>
        /// The type of this card.
        /// このカードのタイプ。
        /// </summary>
        public int HeroPowerTypeId;

        /// <summary>
        /// The name of this card.
        /// このヒロパの名前。
        /// </summary>
        public string name;

        /// <summary>
        /// The costs of this card.
        /// このヒロパのコスト。
        /// </summary>
        public List<Cost> costs = new List<Cost>();

        /// <summary>
        /// The properties of this card.
        /// このヒロパの特性。
        /// </summary>
        public List<Property> properties = new List<Property>();

        /// <summary>
        /// The stats of this card.
        /// このヒロパのスタッツ。
        /// </summary>
        public List<Stat> stats = new List<Stat>();

         /// <summary>
        /// The abilities of this card.
        /// このヒロパの能力。
        /// </summary>
        public List<Ability> abilities = new List<Ability>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public HeroPower() : base(currentId++)
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

