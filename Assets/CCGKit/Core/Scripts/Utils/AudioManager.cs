using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Utility class used to play music and sound effects. It uses an object pool of audio sources
/// internally for better performance.
/// 音楽と効果音を再生するためのユーティリティクラス。 パフォーマンスを向上させるため、オーディオソースのオブジェクトプールを内部的に使用します。
/// </summary>
public class AudioManager : MonoBehaviour
{
    
    
     private static AudioManager instance;

    private ObjectPool objectPool;

    public static AudioManager Instance
    {
        get
        {
            return instance ?? new GameObject("AudioManager").AddComponent<AudioManager>();
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        objectPool = GetComponent<ObjectPool>();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (instance == null)
            instance = this;
    }

    public void PlaySound(AudioClip clip, bool loop = false)
    {
        if (clip != null)
            objectPool.GetObject().GetComponent<SoundFX>().Play(clip, loop);
    }
}