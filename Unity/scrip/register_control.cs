using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.WebSocket;
using LitJson;
using UnityEngine.SceneManagement;
using System;

public class register_control : MonoBehaviour
{
    public InputField name_input;
    public InputField password_input;
    public InputField email_input;
    public GameObject warning;
    public GameObject register_sucess;
    private MyWebSocket myWebSocket;

    private string username; //直接用name会和Object里面有冲突
    private string password;
    private string email;

    // Start is called before the first frame update
    void Start()
    {
        myWebSocket = MyWebSocket.Instance;
        myWebSocket.webSocket.OnMessage = null;
        myWebSocket.webSocket.OnMessage += RegisterOnMessage;
        myWebSocket.webSocket.OnOpen = null;
        myWebSocket.webSocket.OnOpen += RegisterOnOpen;
        myWebSocket.webSocket.OnError = null;
        myWebSocket.webSocket.OnError += RegisterOnError;
        myWebSocket.webSocket.OnError += myWebSocket.OnError;

    }


    public void RegisterOnClick()
    {
        Text warning_text = warning.transform.GetChild(0).GetComponent<Text>();
        if (name_input.text == "")
        {
            warning.SetActive(true);
            warning_text.text = "请输入用户名后注册";
            return;
        }
        else if(password_input.text == "")
        {
            warning.SetActive(true);
            warning_text.text = "请输入密码后注册";
            return;
        }else if(email_input.text == "")
        {
            warning.SetActive(true);
            warning_text.text = "请输入邮箱后注册";
            return;
        }
        else
        {
            warning.SetActive(false);
        }

        username = name_input.text;
        password = password_input.text;
        email = email_input.text;

        if (myWebSocket.IsConnected() == false)
            myWebSocket.Connect();
        else
            myWebSocket.Register(username,password,email);

        /*string test = "{\"function\":\"register\",\"error_code\": 0,\"error_message\":\"\",\"data\":{\"account\":\"100001\"}}";
        RegisterOnMessage(myWebSocket.webSocket,test);*/

    }

    public void RegisterOnOpen(WebSocket web)
    {
        print("连接成功");
        myWebSocket.Register(username, password, email);
    }

    public void RegisterOnMessage(WebSocket web,string message)
    {
        print("收到：" + message);

        JsonData registerJson = JsonMapper.ToObject(message);
        if (registerJson["function"].ToString() != "register")
        {
            print("fucntion对应错误"+ registerJson["function"].ToString());
            return;
        }
        if(registerJson["error_code"].ToString()=="101")
        {
            warning.SetActive(true);
            Text warning_text = warning.transform.GetChild(0).GetComponent<Text>();
            warning_text.text = "该邮箱已被注册";
            return;
        }
        if(registerJson["error_code"].ToString()=="0")
        {
            warning.SetActive(false);
            JsonData dataJson = registerJson["data"];
            register_sucess.SetActive(true);
            Text id_text = register_sucess.transform.Find("id").GetComponent<Text>();
            id_text.text = dataJson["account"].ToString();
        }  
    }

    public void RegisterOnError(WebSocket ws, Exception ex)
    {
        warning.SetActive(true);
        Text warning_text = warning.transform.GetChild(0).GetComponent<Text>();
        warning_text.text = "网络链接失败";
    }

    public void ReturnButtonOnClick()
    {
        SceneManager.LoadScene("Login");
    }
}
