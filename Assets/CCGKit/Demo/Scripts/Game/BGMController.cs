using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 前奏とループが存在するBGMの実装
/// </summary>
public class BGMController : MonoBehaviour
{
	[SerializeField]
	private List<SoundInfo> _infoList;
	[SerializeField]
	private BgmDefine _currentBgmDefine;

	private AudioSource _currentBgmSource;

	/// <summary>
	/// BGMの登録
	/// </summary>
	/// <param name="define">Define.</param>
	/// <param name="clip">Clip.</param>
    public void RegisterBgm (BgmDefine define, AudioClip clip)
	{
		if (_infoList == null) {
			_infoList = new List<SoundInfo> ();
		}

		var info = _infoList.Find (x => x.define.key == define.key);
		if (info != null) {
			Debug.LogError ("already exist key. " + define.key);
			return;
		}

		info = new SoundInfo (define, clip);
		_infoList.Add (info);
	}

	/// <summary>
	/// BGMの再生
	/// </summary>
	/// <param name="key">Key.</param>
	public void PlayBgm (string key)
	{
		var info = _infoList.Find (x => x.clip.name == key);
		if (info == null) {
			return;
		}

		// 複数サウンド同時再生はこのサンプルでは考慮していません
		var source = GetComponent<AudioSource> ();
		if (source == null) {
			source = gameObject.AddComponent<AudioSource> ();
		}
		source.clip = info.clip;
		source.Play ();

		// 再生するBGMのstartTimeのセット
		source.time = info.define.startTime;
		_currentBgmDefine = info.define;
		_currentBgmSource = source;
	}

	void Update ()
	{
		// 再生中のBGMの再生時間を監視する
		if (_currentBgmDefine != null && _currentBgmSource != null && _currentBgmSource.isPlaying) {
			if (_currentBgmSource.time >= _currentBgmDefine.endTime) {
				_currentBgmSource.time = _currentBgmDefine.loopTime;
			}
		}
	}

}

/// <summary>
/// BGM定義ファイルとAudioClipをセットにしたクラス
/// </summary>
[System.Serializable]
public class SoundInfo
{
	public BgmDefine define;
	public AudioClip clip;

	public SoundInfo (BgmDefine define, AudioClip clip)
	{
		this.define = define;
		this.clip = clip;
		// Clip名はdefine.keyに強制的に変更する。オペミスで異なった時のため
		this.clip.name = define.key;
	}
}

/// <summary>
/// BGM定義クラス
/// 最終的にはアセットバンドルと同梱するテキストファイルになる予定
/// </summary>
[System.Serializable]
public class BgmDefine
{
	/// <summary>
	/// ユニークキー
	/// </summary>
	public string key;
	/// <summary>
	/// BGMのスタート時間
	/// </summary>
	public float startTime;
	/// <summary>
	/// ループポイント時間
	/// </summary>
	public float loopTime;
	/// <summary>
	/// BGMの終了時間
	/// </summary>
	public float endTime;
}