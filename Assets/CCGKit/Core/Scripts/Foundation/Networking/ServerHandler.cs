namespace CCGKit
{
    /// <summary>
    /// Base type intended to be subclassed from in order to provide management of a specific subset
    /// of the server's functionality. This separation of concerns helps to avoid having a gigantic
    /// server class and makes the code more maintainable and reusable.
    /// サーバの機能の特定のサブセットの管理を提供するために、サブクラス化されることを意図したベースタイプ。 
    /// この懸念の分離は、巨大なサーバークラスを避け、コードをよりメンテナンスしやすく、再利用的にします。
    /// </summary>
    public class ServerHandler
    {
        /// <summary>
        /// Convenient access to the server itself.
        /// サーバー自体に簡単にアクセスできます。
        /// </summary>
        protected Server server;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="server">Reference to the server.</param>
        public ServerHandler(Server server)
        {
            this.server = server;
        }

        /// <summary>
        /// This method is where subclasses should register to receive the network messages they are
        /// interested in.
        /// このメソッドは、サブクラスが、興味のあるネットワークメッセージを受信するために登録する必要があります。
        /// </summary>
        public virtual void RegisterNetworkHandlers()
        {
        }

        /// <summary>
        /// This method is where subclasses should unregister to stop receiving the network messages they are
        /// interested in.
        /// このメソッドは、サブクラスが登録を解除して、関心のあるネットワークメッセージの受信を停止する必要があります。
        /// </summary>
        public virtual void UnregisterNetworkHandlers()
        {
        }

        /// <summary>
        /// This method provides a convenient entry point for subclasses to perform any turn-start-specific
        /// initialization logic.
        /// このメソッドは、ターン・スタート固有の初期化ロジックを実行するためのサブクラスに便利なエントリ・ポイントを提供します。
        /// </summary>
        public virtual void OnStartTurn()
        {
        }

        /// <summary>
        /// This method provides a convenient entry point for subclasses to perform any turn-end-specific
        /// cleanup logic.
        /// このメソッドは、サブクラスがターンエンド固有のクリーンアップロジックを実行するための便利なエントリポイントを提供します。
        /// </summary>
        public virtual void OnEndTurn()
        {
        }
    }
}
