using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class FadeScript : MonoBehaviour
{
    [SerializeField]
    public float fadeSpeed;        //透明度が変わるスピードを管理
    [SerializeField]
    public float red, green, blue, alfa;   //パネルの色、不透明度を管理
    [SerializeField]
    public bool isFadeOut = false;  //フェードアウト処理の開始、完了を管理するフラグ
    [SerializeField]
    public bool isFadeIn = false;   //フェードイン処理の開始、完了を管理するフラグ
    [SerializeField]
    public Image fadeImage;                //透明度を変更するパネルのイメージ

    public FadeScript()
    {
    
    }

    void Start()
    {
        if (fadeImage == null)
        {
            fadeImage = GetComponent<Image>();
        }
        /*
        red = fadeImage.color.r;
        green = fadeImage.color.g;
        blue = fadeImage.color.b;
        alfa = fadeImage.color.a;
        */
    }
    void Update()
    {
        if (isFadeIn)
        {
            StartFadeIn();
        }
        if (isFadeOut)
        {
            StartFadeOut();
        }
    }


    public void StartFadeIn()
    {
        alfa -= fadeSpeed;                //a)不透明度を徐々に下げる
        SetAlpha();                      //b)変更した不透明度パネルに反映する
        if (alfa <= 0)
        {                    //c)完全に透明になったら処理を抜ける
            isFadeIn = false;
            fadeImage.enabled = false;    //d)パネルの表示をオフにする
        }
    }

    public void StartFadeOut()
    {
        if (fadeImage == null)
        {
            fadeImage = GetComponent<Image>();
        }

        fadeImage.enabled = true;  // a)パネルの表示をオンにする
        alfa += fadeSpeed;         // b)不透明度を徐々にあげる
        SetAlpha();               // c)変更した透明度をパネルに反映する
        if (alfa >= 1)
        {             // d)完全に不透明になったら処理を抜ける
            isFadeOut = false;
        }
    }

    public void InitOut()
    {
        if (fadeImage == null) {
            fadeImage = GetComponent<Image>();
        }

        if (fadeImage.enabled == false) {
            fadeImage.enabled = true;
        }

        alfa = 0f;
        fadeSpeed = 0.033f;
    }

    public void InitIn() {
        if (fadeImage == null)
        {
            fadeImage = GetComponent<Image>();
        }
        if (fadeImage.enabled == false) {
            fadeImage.enabled = true;
        }

        alfa = 1f;
        fadeSpeed = 0.033f;
    }




    public void SetAlpha()
    {
        fadeImage.color = new Color(red, green, blue, alfa);
    }

}
