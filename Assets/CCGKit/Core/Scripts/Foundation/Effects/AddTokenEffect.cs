using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CCGKit
{

    public class AddTokenEffect : CardBaseEffect
    {
        public GameState gameState;
        //移動先
        [GameZoneField("Destination")]
        [Order(2)]
        public int destinationGameZoneId;
        //召喚する対象のトークン
        //        [UniqueToken("Token Name")]
        //        [Order(3)]
        //        public string tokenName;

        public override void Resolve(GameState state, RuntimeCard card)
        {

            // 条件にあった最初の要素を返します。
            // // 要素に4があれば
            // 例：var num = list.Find(x => x.Equals(4));
            ///移動先のゾーンIDを代入
            var destinationZone = state.config.gameZones.Find(x => x.id == destinationGameZoneId);
            state.effectSolver.SummonToken(card.ownerPlayer.netId, card, destinationZone.name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"：ゲームの現在の完全な状態></param>
        /// <param name="sourceCard"：効果を発動するカード></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool AreTargetsAvailable(GameState state, RuntimeCard sourceCard, Target target)
        {
            var cards = new List<RuntimeCard>();
            var tokens = new List<RuntimeCard>();
            //蟲のCardクラス
            var insectCard = gameState.config.GetCardFromName("\u87f2");

            //            List<RuntimeCard> insect = new List<RuntimeCard>();
            //            insect.Add(new RuntimeCard(groupId=1, 1,1)());
            /*
            //トークンプールにトークン一覧を追加
            foreach (var card in state.currentPlayer.zones[originGameZoneId].cards)
            {
                if (card.cardId == 327)
                {
                    TokenPool.AddCard(card);
                }
            }
            */



            switch (target.GetTarget())
            {
                case EffectTarget.ThisCard:
                case EffectTarget.PlayerCard:
                case EffectTarget.AllPlayerCards:
                case EffectTarget.RandomPlayerCard:
                /*
                {
                    //移動元のゾーン
                    foreach (var card in state.currentPlayer.zones[originGameZoneId].cards)
                    {
                        cards.Add(card);
                    }
                    break;
                }
                */
                case EffectTarget.OpponentCard:
                case EffectTarget.AllOpponentCards:
                case EffectTarget.RandomOpponentCard:
                /*
                {
                    foreach (var card in state.currentOpponent.zones[originGameZoneId].cards)
                    {
                        cards.Add(card);
                    }
                    break;
                }
                */
                case EffectTarget.TargetCard:
                case EffectTarget.AllCards:
                case EffectTarget.RandomCard:
                /*
                {
                    foreach (var card in state.currentPlayer.zones[originGameZoneId].cards)
                    {
                        cards.Add(card);
                    }
                    foreach (var card in state.currentOpponent.zones[originGameZoneId].cards)
                    {
                        cards.Add(card);
                    }
                    break;
                }
                */

                case EffectTarget.PlayerOrPlayerCreature:
                case EffectTarget.OpponentOrOpponentCreature:
                case EffectTarget.AnyPlayerOrCreature:
                    return true;
                case EffectTarget.Token_insect:
                    {

                        var gameConfig = GameManager.Instance.config;
                        //                        var TokenName = insectCard.GetStringProperty("name");

                        //                        var libraryCard = GameManager.Instance.config.GetCard(card.cardId);
                        var cost = insectCard.costs.Find(x => x is PayResourceCost);


                        //var libraryCard = gameConfig.GetCard(cardId);
                        //gameConfig.cards.name(x => x.id == libraryCard.cardTypeId);




                        //ハンドの分ループする
                        //                        foreach (var card in state.currentPlayer.zones[4].cards)
                        //                        {
                        //リグルだったら召喚する
                        //   if (card.instanceId == 327) {

                        ///移動先のゾーンIDを代入
                        var destinationZone = state.config.gameZones.Find(x => x.id == destinationGameZoneId);
 //                       state.effectSolver.SummonToken(sourceCard.ownerPlayer.netId, insectCard, destinationZone.name);
                    }
                    //                        }

                    break;

            }

//            default:
                    return false;
        }
        /*
            cards.RemoveAll(x => x.cardType.id != cardTypeId);
            cards.RemoveAll(x =>
            {
                var conditionsFullfilled = true;
                var cardTarget = target as CardTargetBase;
                foreach (var condition in cardTarget.conditions)
                {
                    if (!condition.IsTrue(x))
                    {
                        conditionsFullfilled = false;
                        break;
                    }
                }
                return !conditionsFullfilled;
            });
            return cards.Count > 0;
        }
        */

    }

}
    

