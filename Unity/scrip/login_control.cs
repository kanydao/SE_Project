using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.WebSocket;
using System;

public class login_control : MonoBehaviour
{
    public InputField id_input;
    public InputField password_input;
    public GameObject id_missing;
    public GameObject password_missing;

    private MyWebSocket myWebSocket;
    private string id;
    private string password;

    // Start is called before the first frame update
    void Start()
    {
        myWebSocket = MyWebSocket.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoginOnClick()
    {
        if (id_input.text == "")
        {
            password_missing.SetActive(true);
            return;
        }
        else
        {
            password_missing.SetActive(false);
        }
        if (password_input.text == "")
        {
            password_missing.SetActive(true);
            return;
        }
        else
        {
            password_missing.SetActive(false);
        }

        id = id_input.text;
        password = password_input.text;

        myWebSocket.webSocket.OnOpen = null;
        myWebSocket.webSocket.OnOpen += LoginOnOpen;
        myWebSocket.webSocket.OnMessage = null;
        myWebSocket.webSocket.OnMessage += LoginOnMessage;
        myWebSocket.webSocket.OnError = null;
        myWebSocket.webSocket.OnError += LoginOnError;

        if (!myWebSocket.IsConnected())
            myWebSocket.Connect();
        else
            myWebSocket.Login(id, password);
    }


    public void LoginOnOpen(WebSocket web)
    {
        myWebSocket.Login(id, password);
    }

    public void LoginOnMessage(WebSocket web, string message)
    {
        User user = User.Instance;
        user.Init("10001", "menglong", User.Icon.ICON1);
    }

    public void LoginOnError(WebSocket ws, Exception ex)
    {

    }

}
