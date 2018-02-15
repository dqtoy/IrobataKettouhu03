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
    /// ポップアップ処理。
    /// <T>はジェネリック。メソッドやプロパティで使用する型をパラメータとして定義し、使用する時に決定する機能。
    /// OpenPopupを呼び出した時、OpenPopup<int>とかで呼び出すとAction<T>の値がintになる
    /// ジェネリック宣言で定義されている型パラメーターの引数として使用できる型に対する制約を指定する場合に where 句を使用します
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="onOpened"></param>
    /// <param name="darkenBackground"></param>
    public void OpenPopup<T>(string name, Action<T> onOpened = null, bool darkenBackground = true) where T : Popup
    {
        //コルーチンの特徴は処理の途中で一旦中断し、あとでその行から再開できたりすること
        //IEnumeratorはコルーチンのインターフェース
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
    /// ↑コルーチンのインターフェイスとして利用することもある。
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


        //request.asset：読み込まれているアセットバンドルを返す
        //AssetBundle は、ヘッダーとデータセグメントのふたつの部分から構成されます
        //ヘッダーには AssetBundle の識別子や、圧縮・非圧縮の識別情報、マニフェストなど、AssetBundle に関する情報が含まれています
        //データセグメントには、AssetBundle 内での Asset のシリアライズによって生成された生データが含まれています。
        ///Instantiateは、PrefabのTransformのままで生成する場合に使用
        ///asは生成したオブジェクトになんらかの処理を行いたい場合に使う。
        ///今回は2,3行目の座標指定、今のScene指定など。
        currentPopup = Instantiate(request.asset) as GameObject;
        // ワールド座標系の位置情報を親オブジェクトのローカル座標系に変換する
        currentPopup.transform.SetParent(canvas.transform, false);
        //オブジェクト生成→GetComponentでオブジェクトにアクセス
        //※parentSceneはPopup内のBaseScene型。Popupが開いてるかどうかの判定に使われる
        currentPopup.GetComponent<Popup>().parentScene = this;
        if (darkenBackground)
        {
            panelCanvasGroup.blocksRaycasts = true;
            panelCanvasGroup.GetComponent<Image>().DOKill();
            panelCanvasGroup.GetComponent<Image>().DOFade(0.5f, 0.5f);
        }

        if (onOpened != null)
        {
            //オブジェクトにアクセス。
            //onOpenedはデリゲート(デリゲートとは、何かの処理を実行させたいときに、処理機能を持つメソッドを直接呼び出すのではなく、代表者に処理をしてもらうこと。onOpenedはその対象。)
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
