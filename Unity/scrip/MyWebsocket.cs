using BestHTTP.WebSocket;
using LitJson;
using System;
using UnityEngine;
using System.Security.Cryptography;

public class MyWebSocket
{
    //单例模式
    private static MyWebSocket instance;
    public static MyWebSocket Instance
    {
        get
        {
            if (null == instance)
            {
                instance = new MyWebSocket();
            }
            return instance;
        }
    }

    string address = "ws://60.205.178.57:8888/main";
    public WebSocket webSocket;
    string meg_send = string.Empty;
    string meg_receive = string.Empty;


    private MyWebSocket(string address = "ws://60.205.178.57:8888/main")
    {
        //创建websocket单例
        webSocket = new WebSocket(new Uri(address));
        webSocket.OnOpen += OnOpen;
        webSocket.OnMessage += OnMessageReceived;
        webSocket.OnClosed += OnClosed;
        webSocket.OnError += OnError;
    }

    //开始链接
    public bool Connect()
    {
        webSocket.Open();
        Debug.Log("开始链接......");
        return webSocket.IsOpen;
    }
    //断开链接
    public void Close()
    {
        webSocket.Close();
        Debug.Log("断开链接");
    }

    //判断是否开始链接
    public bool IsConnected()
    {
        return webSocket.IsOpen;
    }

    //当有消息从服务器发过来时调用
    private void OnMessageReceived(WebSocket ws, string message)
    {
        Debug.Log("received: " + message);
    }

    //登录函数
    public void Login(string id, string password)
    {

    }

    //注册函数
    public void Register(string nickname, string password, string email)
    {

    }

    //发送消息
    public void SendMessage(string message)
    {
        
    }

    //添加好友
    public void AddFriend()
    {

    }

    //搜索好友
    public void SearchFriend()
    {

    }

    //删除好友
    public void DeleteFriend()
    {

    }





    

    //链接成功回调
    void OnOpen(WebSocket webSocket)
    {
        Debug.Log("链接成功！");
    }

    //密码转换为MD5 小写
    public static string GetMD5(string password)
    {
        string password_md5 = string.Empty;
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] data = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(password));
        for (int i = 0; i < data.Length; i++)
        {
            password_md5 += data[i].ToString("x2");
        }
        return password_md5;
    }

    //发生错误回调函数
    void OnError(WebSocket ws, Exception ex)
    {
        string errorMsg = string.Empty;
#if !UNITY_WEBGL || UNITY_EDITOR
        if (ws.InternalRequest.Response != null)
            errorMsg = string.Format("Status Code from Server: {0} and Message: {1}", ws.InternalRequest.Response.StatusCode, ws.InternalRequest.Response.Message);
#endif

        Debug.Log(string.Format("-An error occured: {0}\n", (ex != null ? ex.Message : "Unknown Error " + errorMsg)));

        webSocket = null;
    }

    //链接断开回调函数
    void OnClosed(WebSocket webSocket, UInt16 code, string message)
    {
        Debug.Log(string.Format("-WebSocket closed! Code: {0} Message: {1}\n", code, message));
    }


}
