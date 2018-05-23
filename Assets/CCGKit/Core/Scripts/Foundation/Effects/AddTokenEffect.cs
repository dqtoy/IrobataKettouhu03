using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CCGKit
{

    public class AddTokenEffect : CardBaseEffect
    {
        //移動先
        [GameZoneField("Destination")]
        [Order(2)]
        public int destinationGameZoneId;
        //召喚する対象のトークン
        [CardTypeField("Token Name")]
        [Order(3)]
        public string tokenName;

    }
}
    

