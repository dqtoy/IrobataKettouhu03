using UnityEngine;
using System.Collections;

namespace CCGKit
{
    [PlayerTarget]
    public class DrawCardsEffect : PlayerEffect
    {
        [IntField("Amount")]
        [Order(1)]
        public int numCards;

        public override void Resolve(GameState state, PlayerInfo player)
        {
            state.effectSolver.DrawCards(player.netId, numCards);
        }
    }
}
