using System;
using System.Collections.Generic;

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The modifier of a stat.
    /// statの修飾子。
    /// </summary>
    public class Modifier
    {
        /// <summary>
        /// The constant value to identify a permanent modifier.
        /// 永続修飾子を識別する定数です。
        /// </summary>
        private const int PERMANENT = 0;

        /// <summary>
        /// The value of this modifier.
        /// </summary>
        public int value;

        /// <summary>
        /// The duration of this modifier.
        /// 修飾子の期間
        /// </summary>
        public int duration;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">The value of the modifier.</param>
        /// <param name="duration">The duration of the modifier.</param>
        public Modifier(int value, int duration = PERMANENT)
        {
            this.value = value;
            this.duration = duration;
        }

        /// <summary>
        /// Returns true if this modifier is permanent and false otherwise.
        /// この修飾子が永続的である場合はtrueを返し、そうでない場合はfalseを返します。
        /// </summary>
        /// <returns>True if this modifier is permanent; false otherwise.</returns>
        public bool IsPermanent()
        {
            return duration == PERMANENT;
        }
    }

    /// <summary>
    /// Stats are a fundamental concept in CCG Kit. They represent integer values that can change over
    /// the course of a game and are used in both players and cards. For example, a player could have
    /// life and mana stats and a creature card could have cost, attack and defense stats. Stats are
    /// transmitted over the network, which means you should only use them to represent values that can
    /// actually change over the course of a game in order to save bandwidth.
    /// Statsは、CCGキットの基本概念です。 それらは、ゲームの過程で変化し、プレイヤーとカードの両方で使用される整数値を表します。 
    /// 例えば、プレイヤーはライフとマナ・ステータスを持つことができ、クリーチャー・カードはコスト、攻撃、防御のStatsを持つことができます。 
    /// Statsはネットワークを介して送信されます。つまり、Stats情報を使用して、ゲームの過程で実際に変更できる値を表すだけで、帯域幅を節約できます。
    /// </summary>
    public class Stat
    {
        /// <summary>
        /// The identifier of this stat.
        /// 対象のスタッツを表す固有ID
        /// </summary>
        public int statId;

        /// <summary>
        /// The name of this stat.
        /// スタッツの名前
        /// </summary>
        public string name;

        /// <summary>
        /// The base value of this stat.
        /// スタッツのベースとなる値
        /// </summary>
        private int _baseValue;

        /// <summary>
        /// The base value of this stat.
        /// スタッツの基本値
        /// </summary>
        [SerializeField]
        public int baseValue
        {
            get { return _baseValue; }
            set
            {
                var oldValue = _baseValue;
                _baseValue = value;
                if (onValueChanged != null && oldValue != _baseValue)
                {
                    onValueChanged(oldValue, value);
                }
            }
        }

        /// <summary>
        /// The original value of this stat.
        /// このスタッツの元の値
        /// </summary>
        public int originalValue;

        /// <summary>
        /// The minimum value of this stat.
        /// スタッツの最小値
        /// </summary>
        public int minValue;

        /// <summary>
        /// The maximum value of this stat.
        /// スタッツの最大値
        /// </summary>
        public int maxValue;

        /// <summary>
        /// The modifiers of this stat.
        /// スタッツの修飾語
        /// </summary>
        public List<Modifier> modifiers = new List<Modifier>();

        /// <summary>
        /// The callback that is called when the value of this stat changes.
        /// このstatの値が変更されたときに呼び出されるコールバックです。
        /// </summary>
        public Action<int, int> onValueChanged;

        /// <summary>
        /// The effective value of this stat.
        /// スタッツの実効値
        /// </summary>
        public int effectiveValue
        {
            get
            {
                // Start with the base value.
                //基本値から開始
                var value = baseValue;

                // Apply all the modifiers.
                //全ての修飾子を適用
                foreach (var modifier in modifiers)
                {
                    value += modifier.value;
                }

                // Clamp to [minValue, maxValue] if needed.
                //必要に応じて[minValue、maxValue]に固定します。
                if (value < minValue)
                {
                    value = minValue;
                }
                else if (value > maxValue)
                {
                    value = maxValue;
                }

                // Return the effective value.
                //実効値を返す
                return value;
            }
        }

        /// <summary>
        /// Adds a modifier to this stat.
        /// スタッツに修飾子を追加する(バフデバフする)
        /// </summary>
        /// <param name="modifier">The modifier to add.</param>
        public void AddModifier(Modifier modifier)
        {
            var oldValue = effectiveValue;
            modifiers.Add(modifier);
            if (onValueChanged != null)
            {
                onValueChanged(oldValue, effectiveValue);
            }
        }

        /// <summary>
        /// This method is automatically called when the turn ends.
        /// ターン終了時に呼ばれるメソッド
        /// </summary>
        public void OnEndTurn()
        {
            var oldValue = effectiveValue;

            var modifiersToRemove = new List<Modifier>(modifiers.Count);

            var temporaryModifiers = modifiers.FindAll(x => !x.IsPermanent());
            foreach (var modifier in temporaryModifiers)
            {
                modifier.duration -= 1;
                if (modifier.duration <= 0)
                {
                    modifiersToRemove.Add(modifier);
                }
            }

            foreach (var modifier in modifiersToRemove)
            {
                modifiers.Remove(modifier);
            }
            if (modifiersToRemove.Count > 0 && onValueChanged != null)
            {
                onValueChanged(oldValue, effectiveValue);
            }
        }
    }
}
