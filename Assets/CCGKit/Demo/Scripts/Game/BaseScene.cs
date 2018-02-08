using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class BaseScene : MonoBehaviour
{
    /// <summary>
    /// GameObjectは全てのデータの入れ物となるベースクラス
    /// </summary>
    public GameObject currentPopup { get; protected set; }

    [SerializeField]
    protected Canvas canvas;

    [SerializeField]
    protected CanvasGroup panelCanvasGroup;

    /// <summary>
    /// <T>はジェネリック。メソッドやプロパティで使用する型をパラメータとして定義し、使用する時に決定する機能。
    /// OpenPopupを呼び出した時、OpenPopup<int>とかで呼び出すとAction<T>の値がintになる
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="onOpened"></param>
    /// <param name="darkenBackground"></param>
    public void OpenPopup<T>(string name, Action<T> onOpened = null, bool darkenBackground = true) where T : Popup
    {
        StartCoroutine(OpenPopupAsync<T>(name, onOpened, darkenBackground));
    }

    public void ClosePopup()
    {
        if (currentPopup != null)
        {
            Destroy(currentPopup);
            currentPopup = null;
            panelCanvasGroup.blocksRaycasts = false;
            panelCanvasGroup.GetComponent<Image>().DOKill();
            panelCanvasGroup.GetComponent<Image>().DOFade(0.0f, 0.2f);
        }
    }

    /// <summary>
    /// IEnumeratorは非同期で処理したい時に使ったりする。
    /// where T :Popupは、コロン以降のクラス名でTが満たすべき条件を指定する。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="onOpened"></param>
    /// <param name="darkenBackground"></param>
    /// <returns></returns>
    protected IEnumerator OpenPopupAsync<T>(string name, Action<T> onOpened, bool darkenBackground) where T : Popup
    {
        var request = Resources.LoadAsync<GameObject>(name);
        while (!request.isDone)
        {
            yield return null;
        }

        currentPopup = Instantiate(request.asset) as GameObject;
        currentPopup.transform.SetParent(canvas.transform, false);
        currentPopup.GetComponent<Popup>().parentScene = this;
        if (darkenBackground)
        {
            panelCanvasGroup.blocksRaycasts = true;
            panelCanvasGroup.GetComponent<Image>().DOKill();
            panelCanvasGroup.GetComponent<Image>().DOFade(0.5f, 0.5f);
        }

        if (onOpened != null)
        {
            onOpened(currentPopup.GetComponent<T>());
        }
    }

    public void OnPopupClosed(Popup popup)
    {
        panelCanvasGroup.blocksRaycasts = false;
        panelCanvasGroup.GetComponent<Image>().DOKill();
        panelCanvasGroup.GetComponent<Image>().DOFade(0.0f, 0.25f);
    }
}
