using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialReceive : MonoBehaviour
{
    //https://qiita.com/yjiro0403/items/54e9518b5624c0030531
    //上記URLのSerialHandler.cのクラス
    public SerialHandler serialHandler;
    public Engine engine;
    void Start()
    {
        //信号を受信したときに、そのメッセージの処理を行う
        serialHandler.OnDataReceived += OnDataReceived;
    }
    //受信した信号(message)に対する処理
    void OnDataReceived(string message)
    {
        // 空白除去
        message = message.Trim();
        try
        {
            engine.CreateCard(message);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message); // エラーを表示
        }
    }
}