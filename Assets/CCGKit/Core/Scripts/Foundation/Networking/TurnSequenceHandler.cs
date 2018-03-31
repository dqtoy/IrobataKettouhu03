using UnityEngine.Networking;

namespace CCGKit
{
    /// <summary>
    /// This server handler is responsible for managing client requests to end the current game
    /// turn. If a player does not explicitly request the server to end his current turn, then
    /// the turn will naturally end after the pre-configured turn duration.
    /// このサーバハンドラは、現在のゲームターンを終了するためのクライアント要求を管理する責任があります。 
    /// プレーヤーが現在のターンを終了するようにサーバーに明示的に要求していない場合、ターンはあらかじめ設定されたターン時間後に自然に終了します。
    /// </summary>
    public class TurnSequenceHandler : ServerHandler
    {
        public TurnSequenceHandler(Server server) : base(server)
        {
        }

        public override void RegisterNetworkHandlers()
        {
            base.RegisterNetworkHandlers();
            NetworkServer.RegisterHandler(NetworkProtocol.StopTurn, OnStopTurn);
        }

        public override void UnregisterNetworkHandlers()
        {
            base.UnregisterNetworkHandlers();
            NetworkServer.UnregisterHandler(NetworkProtocol.StopTurn);
        }

        protected virtual void OnStopTurn(NetworkMessage netMsg)
        {
            server.StopTurn();
        }
    }
}