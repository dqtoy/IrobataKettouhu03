// Copyright (C) 2016-2017 David Pol. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace CCGKit
{
    /// <summary>
    /// The available game action targets.
    /// 利用可能なゲームアクションターゲット。
    /// </summary>
    public enum GameActionTarget
    {
        CurrentPlayer,
        CurrentOpponent,
        AllPlayers
    }

    /// <summary>
    /// The base class for game actions.
    /// ゲームアクションの基本クラス。
    /// </summary>
    public abstract class GameAction
    {
        /// <summary>
        /// The name of this action.
        /// このアクションの名前
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// The target of this action.
        /// このアクションのターゲット
        /// </summary>
        public GameActionTarget target;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        public GameAction(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Resolves this action.
        /// 操作を解決する
        /// </summary>
        /// <param name="state">The state of the game.</param>
        /// <param name="player">The player on which to resolve this action.</param>
        public abstract void Resolve(GameState state, PlayerInfo player);
    }
}
