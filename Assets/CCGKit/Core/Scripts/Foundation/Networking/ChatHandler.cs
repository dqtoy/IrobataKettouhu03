using UnityEngine.Networking;

namespace CCGKit
{
    /// <summary>
    /// This server handler is responsible for managing the in-game chat.
    /// このサーバハンドラは、インゲームチャットの管理を担当します。
    /// </summary>
    public class ChatHandler : ServerHandler
    {
        public ChatHandler(Server server) : base(server)
        {
        }

        public override void RegisterNetworkHandlers()
        {
            base.RegisterNetworkHandlers();
            NetworkServer.RegisterHandler(NetworkProtocol.SendChatTextMessage, OnSendChatText);
        }

        public override void UnregisterNetworkHandlers()
        {
            base.UnregisterNetworkHandlers();
            NetworkServer.UnregisterHandler(NetworkProtocol.SendChatTextMessage);
        }

        public virtual void OnSendChatText(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<SendChatTextMessage>();
            var player = server.gameState.players.Find(x => x.netId == msg.senderNetId);
            if (player != null)
            {
                var broadcastMsg = new BroadcastChatTextMessage();
                broadcastMsg.senderNetId = msg.senderNetId;
                broadcastMsg.text = player.nickname + ": " + msg.text;
                NetworkServer.SendToAll(NetworkProtocol.BroadcastChatTextMessage, broadcastMsg);
            }
        }
    }
}
