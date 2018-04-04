using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// CardSetクラスのパクリ
/// </summary>
namespace CCGKit
{
    /// <summary>
    /// カードセットは名前付きのカードの集合です。 彼らの主な目的は、
    /// 大規模なカードのコレクションを、より小さく、より管理しやすいサブグループに編成することです。
    /// </summary>
    public class HeroPowerSet
    {
        /// <summary>
        /// このカードセットの名前。
        /// </summary>
        public string name;

        /// <summary>
        /// このカードセットのカード。
        /// </summary>
        public List<HeroPower> HeroPowers = new List<HeroPower>();
    }
}