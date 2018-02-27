// Copyright (C) 2016-2017 David Pol. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Reflection;

namespace CCGKit
{
    /// <summary>
    /// The base class for the custom attributes used in the kit.
    /// キットで使用されるカスタム属性の基本クラスです。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FieldAttribute : Attribute
    {
        /// <summary>
        /// The prefix of this field.
        /// このフィールドの接頭辞。
        /// </summary>
        public string prefix { get; private set; }

        /// <summary>
        /// The width of this field.
        /// このフィールドの幅。
        /// </summary>
        public int width { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="prefix">Prefix.</param>
        public FieldAttribute(string prefix)
        {
            this.prefix = prefix;
            width = 30;
        }

        /// <summary>
        /// Draws this attribute.
        /// この属性を描画します。
        /// </summary>
        /// <param name="gameConfig">The configuration of the game.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="field">The field information.</param>
        public virtual void Draw(GameConfiguration gameConfig, object instance, ref FieldInfo field)
        {
        }
    }
}
