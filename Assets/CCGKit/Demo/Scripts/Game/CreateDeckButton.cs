using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateDeckButton : MonoBehaviour
{
    [HideInInspector]
    public DeckBuilderScene scene;


//    [HideInInspector]
//    public KoumaDeckBuilderScene kScene;
 //   [HideInInspector]
 //   public HakugyokuDeckBuilderScene hScene;
 //   [HideInInspector]
 //   public EinenDeckBuilderScene eScene;


 /// <summary>
 /// 新規デッキ作成ボタン(プラスボタン)が押下された時。
 /// SelectTeam画面に遷移する。
 /// </summary>
    public void OnButtonPressed()
    {
        //陣営選択が終わったらデッキ＋の欄を増やす。このボタンは＋を押したときの処理
        SceneManager.LoadScene("SelectTeam");
    }
}