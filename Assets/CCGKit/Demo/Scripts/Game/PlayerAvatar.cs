using UnityEngine;

using CCGKit;

/// <summary>
/// This class holds information about a player avatar from the game scene, which can be clicked
/// to select a target player for an effect or during combat (this will send the appropriate
/// information to the server).
/// ���̃N���X�́A�Q�[���V�[������v���C���[�̃A�o�^�[�Ɋւ������ێ����܂��B
/// ���̃A�^�b�J�[�́A�G�t�F�N�g�܂��͐퓬���ɖړI�̃v���C���[��I�����邽�߂ɃN���b�N�ł��܂��i�T�[�o�[�ɓK�؂ȏ��𑗐M���܂��j�B
/// </summary>
public class PlayerAvatar : MonoBehaviour
{
    public PlayerInfo playerInfo;
    public bool IsBottom;
    public int index { get { return IsBottom ? 0 : 1; } }

    private Player GetTargetPlayer()
    {
        var players = FindObjectsOfType<Player>();
        if (IsBottom)
        {
            foreach (var player in players)
            {
                if (player.isLocalPlayer && player.isHuman)
                    return player;
            }
        }
        else
        {
            foreach (var player in players)
            {
                if (!player.isLocalPlayer || (player.isLocalPlayer && !player.isHuman))
                    return player;
            }
        }
        return null;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.transform.parent != null)
        {
            var targetingArrow = collider.transform.parent.GetComponent<TargetingArrow>();
            if (targetingArrow != null)
            {
                targetingArrow.OnPlayerSelected(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.transform.parent != null)
        {
            var targetingArrow = collider.transform.parent.GetComponent<TargetingArrow>();
            if (targetingArrow != null)
            {
                targetingArrow.OnPlayerUnselected(this);
            }
        }
    }
}
