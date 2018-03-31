using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Wrapper around Unity's AudioSource that disables the game object after the sound clip
/// has been played (allowing it to be reused in the context of a pool of sound effects, see
/// the SoundManager class).
/// SoundClip���Đ�������ɃQ�[���I�u�W�F�N�g�𖳌��ɂ���Unity��AudioSource�̎�������b�p�[���܂�
/// �i�T�E���h�G�t�F�N�g�̃v�[���̃R���e�L�X�g�ōė��p�ł���悤�ɂ��܂��ASoundManager�N���X���Q�Ɓj�B
/// </summary>
public class SoundFX : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Assert.IsTrue(audioSource != null);
    }

    public void Play(AudioClip clip, bool loop = false)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.Play();
            Invoke("DisableSoundFX", clip.length + 0.1f);
        }
    }

    private void DisableSoundFX()
    {
        gameObject.SetActive(false);
    }
}