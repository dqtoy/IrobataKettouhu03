using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// This class is the in-game entry point to the game configuration managed from within
    /// the CCG Kit menu option in Unity.
    /// このクラスは、UnityのCCG Kitメニューオプションから管理されているゲーム設定へのゲーム内エントリポイントです。
    /// </summary>
    public sealed class GameManager
    {
        /// <summary>
        /// The configuration of this game.
        /// このゲームの構成。
        /// </summary>
        public GameConfiguration config = new GameConfiguration();

        /// <summary>
        /// The default deck of this game.
        /// ゲームのデフォルトデッキ
        /// </summary>
        public Deck defaultDeck;

        /// <summary>
        /// The player decks of this game.
        /// プレイヤーが既に構築しているデッキ
        /// </summary>
        public List<Deck> playerDecks = new List<Deck>();

        /// <summary>
        /// The player's nickname of this game.
        /// プレイヤー名
        /// </summary>
        public string playerName;

        /// <summary>
        /// True if the player is logged in; false otherwise (used in Master Server Kit
        /// integration).
        /// プレーヤーがログインしている場合はTrueです。 それ以外の場合はfalseです（Master Server Kitの統合で使用されます）。
        /// </summary>
        public bool isPlayerLoggedIn;

        /// <summary>
        /// Static instance.
        /// 静的インスタンス
        /// </summary>
        private static readonly GameManager instance = new GameManager();

        /// <summary>
        /// Constructor.
        /// コンストラクタ
        /// </summary>
        private GameManager()
        {
            config.LoadGameConfigurationAtRuntime();
        }

        /// <summary>
        /// Static instance.
        /// 静的インスタンス
        /// </summary>
        public static GameManager Instance
        {
            get { return instance; }
        }
    }
}