using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using BestHTTP.WebSocket;
using System;
using UnityEngine.UI;


public class websocket_test : MonoBehaviour
{
    public InputField hostInput;
    public InputField portInput;
    public Text connectText;
    public Text txtStr;
    private string serverStr = string.Empty;
    public InputField textInput;

    //public string address = "ws://172.22.107.84/test";
    WebSocket webSocket;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        txtStr.text = serverStr;
    }

    void OnOpen(WebSocket web)
    {
        Debug.Log("链接成功！");
        connectText.text = "链接成功！";
    }

    void OnMessage(WebSocket web,string message)
    {
        if (serverStr.Length > 300)
            serverStr = "";
        serverStr += message + "\n";
    }

    void OnClose(WebSocket web)
    {
        connectText.text = "链接断开";
    }

    public void Connect()
    {
        txtStr.text = "";
        string host = "ws://";
        host = host + hostInput.text + ":";
        host = host + portInput.text + "/" + "test";
        print(host);
        webSocket = new WebSocket(new Uri(host));
        webSocket.OnOpen += OnOpen;
        webSocket.OnMessage += OnMessage;
        webSocket.Open();
    }

    public void Send()
    {
        if (textInput.text == string.Empty)
            return;
        webSocket.Send(textInput.text);
        textInput.text = null;
    }


}
