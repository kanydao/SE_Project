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

    string address = "ws://8.140.2.201:8000/main";
    public WebSocket webSocket;
    string meg_send = string.Empty;
    string meg_receive = string.Empty;


    private MyWebSocket(string address = "ws://8.140.2.201:8000/main")
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
        //return true;
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
        JsonData loginSendJson = new JsonData();
        loginSendJson["function"] = "login";
        JsonData loginSendDataJson = new JsonData();
        loginSendDataJson["account"] = id;
        loginSendDataJson["password"] = GetMD5(password);
        loginSendJson["data"] = loginSendDataJson;

        Debug.Log("发送："+loginSendJson.ToJson());
        webSocket.Send(loginSendJson.ToJson());
    }

    //注册函数
    public void Register(string name, string password, string email)
    {
        JsonData registerJson = new JsonData();
        registerJson["function"] = "register";
        JsonData dataJson = new JsonData();
        dataJson["username"] = name;
        dataJson["password"] = GetMD5(password);
        dataJson["mail"] = email;
        dataJson["icon"] = 1;
        registerJson["data"] = dataJson;
        Debug.Log("发送：" + registerJson.ToJson());
        webSocket.Send(registerJson.ToJson());
        
    }

    //发送消息
    public void SendMessage(string id,string message)
    {
        JsonData sendJson = new JsonData();
        sendJson["function"] = "message";
        JsonData dataJson = new JsonData();
        dataJson["s_account"] = User.Instance.id;
        dataJson["r_account"] = id;
        dataJson["g_account"] = "";
        dataJson["is_group"] = 0;
        dataJson["content"] = message;
        dataJson["date"] = System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        sendJson["data"] = dataJson;

        Debug.Log("发送：" + sendJson.ToJson());
        webSocket.Send(sendJson.ToJson());

    }

    //添加好友
    public void AddFriend(string id)
    {
        JsonData addJson = new JsonData();
        addJson["function"] = "add_friend";
        JsonData dataJson = new JsonData();
        dataJson["account"] = id;
        addJson["data"] = dataJson;

        Debug.Log("发送：" + addJson.ToJson());
        webSocket.Send(addJson.ToJson());

    }

    //搜索好友
    public void SearchFriend(string id)
    {
        JsonData searchJson = new JsonData();
        searchJson["function"] = "search_user";
        JsonData dataJson = new JsonData();
        dataJson["account"] = id;
        searchJson["data"] = dataJson;

        Debug.Log("发送：" + searchJson.ToJson());
        webSocket.Send(searchJson.ToJson());
    }

    //删除好友
    public void DeleteFriend(string id)
    {
        JsonData deleteJson = new JsonData();
        deleteJson["function"] = "delete_friend";
        JsonData dataJson = new JsonData();
        dataJson["account"] = id;
        deleteJson["data"] = dataJson;

        Debug.Log("发送：" + deleteJson.ToJson());
        webSocket.Send(deleteJson.ToJson());
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
    public void OnError(WebSocket ws, Exception ex)
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
