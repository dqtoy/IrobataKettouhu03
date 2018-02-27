using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Wrapper around Unity's AudioSource that disables the game object after the sound clip
/// has been played (allowing it to be reused in the context of a pool of sound effects, see
/// the SoundManager class).
/// SoundClipを再生した後にゲームオブジェクトを無効にするUnityのAudioSourceの周りをラッパーします
/// （サウンドエフェクトのプールのコンテキストで再利用できるようにします、SoundManagerクラスを参照）。
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