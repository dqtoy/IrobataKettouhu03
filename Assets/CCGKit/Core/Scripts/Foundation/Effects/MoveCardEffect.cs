using System.Collections.Generic;

/// <summary>
/// カード移動時の効果
/// battlecryやdeathrattleなど。
/// </summary>
namespace CCGKit
{
    [CardTarget]
    public class MoveCardEffect : CardBaseEffect
    {
        //移動元
        [GameZoneField("Origin")]
        [Order(1)]
        public int originGameZoneId;

        //移動先
        [GameZoneField("Destination")]
        [Order(2)]
        public int destinationGameZoneId;

        [CardTypeField("Card type")]
        [Order(3)]
        public int cardTypeId;

        public override void Resolve(GameState state, RuntimeCard card)
        {
            // 条件にあった最初の要素を返します。
            // // 要素に4があれば
            // var num = list.Find(x => x.Equals(4));
            ///移動元のゾーンIDを代入
            var originZone = state.config.gameZones.Find(x => x.id == originGameZoneId);
            ///移動先のゾーンIDを代入
            var destinationZone = state.config.gameZones.Find(x => x.id == destinationGameZoneId);
            state.effectSolver.MoveCard(card.ownerPlayer.netId, card, originZone.name, destinationZone.name);
        }

        public override bool AreTargetsAvailable(GameState state, RuntimeCard sourceCard, Target target)
        {
            var cards = new List<RuntimeCard>();
            switch (target.GetTarget())
            {
                case EffectTarget.ThisCard:
                    cards.Add(sourceCard);
                    break;

                case EffectTarget.PlayerCard:
                case EffectTarget.AllPlayerCards:
                case EffectTarget.RandomPlayerCard:
                    {
                        foreach (var card in state.currentPlayer.zones[originGameZoneId].cards)
                        {
                            cards.Add(card);
                        }
                        break;
                    }
                case EffectTarget.OpponentCard:
                case EffectTarget.AllOpponentCards:
                case EffectTarget.RandomOpponentCard:
                    {
                        foreach (var card in state.currentOpponent.zones[originGameZoneId].cards)
                        {
                            cards.Add(card);
                        }
                        break;
                    }
                case EffectTarget.TargetCard:
                case EffectTarget.AllCards:
                case EffectTarget.RandomCard:
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

                case EffectTarget.PlayerOrPlayerCreature:
                case EffectTarget.OpponentOrOpponentCreature:
                case EffectTarget.AnyPlayerOrCreature:
                    return true;

                default:
                    return false;
            }

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
    }
}