using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CCGKit;
using System;
using System.IO;

public class TestDebugLog : MonoBehaviour {

    /// <summary>
    /// デバッグ時のログ出し
    /// </summary>
    /// <param name="str"></param>
    public void Print(string str)
    {
        Debug.Log(str);
    }

    /**
     *  エラー調査用ログ出力
     *  @param ex catchしたエラー内容
     */
    static public void OutputErrLog(Exception ex)
    {
        // 書き込み設定
        System.IO.StreamWriter sw = new StreamWriter(
         "C:\\errorlog_" + DateTime.Now.ToString("yyyyMMdd") + ".txt",   // 出力先ファイル名
         true,                                                           // 追加書き込み
         System.Text.Encoding.GetEncoding("Shift_JIS"));                 // 文字コード

        // ログ出力
        Console.SetOut(sw); // 出力先（Outプロパティ）を設定
        Console.WriteLine("[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "] message: " + ex.Message);
        Console.WriteLine("[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "] stack trace: " + ex.StackTrace);
        Console.WriteLine(ex.StackTrace);
        Console.WriteLine();

        // ファイルを閉じる
        sw.Dispose();
    }

    // catchの中に書いて完成！
    // try {
    //     …
    // }catch(Exception ex) {
    //     OutputErrLog(ex);
    // }

        /// <summary>
        /// intをstringにして返す(nullも可)
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
    public String MakeString(int i) {
        String str = Convert.ToString(i);
        return str;
    }

    public void LogMassage() {
        //iは何を指定する…？？
    //    System.IO.File.AppendAllText("C:\\log" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "] message: 変数i=" + i);
    }
}
