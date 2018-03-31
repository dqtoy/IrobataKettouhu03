using UnityEngine.Networking;

namespace CCGKit
{
    /// <summary>
    /// This server handler is responsible for managing client requests to end the current game
    /// turn. If a player does not explicitly request the server to end his current turn, then
    /// the turn will naturally end after the pre-configured turn duration.
    /// ���̃T�[�o�n���h���́A���݂̃Q�[���^�[�����I�����邽�߂̃N���C�A���g�v�����Ǘ�����ӔC������܂��B 
    /// �v���[���[�����݂̃^�[�����I������悤�ɃT�[�o�[�ɖ����I�ɗv�����Ă��Ȃ��ꍇ�A�^�[���͂��炩���ߐݒ肳�ꂽ�^�[�����Ԍ�Ɏ��R�ɏI�����܂��B
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