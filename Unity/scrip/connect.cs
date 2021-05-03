using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using System;

public class connect : MonoBehaviour
{
    public InputField hostInput;
    public InputField portInput;
    public Text connectText;
    public Text txtStr;
    public string serverStr;
    public InputField textInput;

    Socket socket;
    const int buff_size = 1024;
    public byte[] readBuff = new byte[buff_size];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        txtStr.text = serverStr;
    }

    //链接服务器并接收
    public void Connection()
    {
        txtStr.text = "";
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        string host = hostInput.text;
        int port = int.Parse(portInput.text);
        socket.Connect(host, port);
        connectText.text = socket.LocalEndPoint.ToString();
        socket.BeginReceive(readBuff, 0, buff_size, SocketFlags.None, ReceiveCb, null);

    }
    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            //接收数据大小
            int count = socket.EndReceive(ar);
            string str = System.Text.Encoding.Default.GetString(readBuff,0,count);
            if (serverStr.Length > 300)
                serverStr = "";
            serverStr += str + "\n";

            //继续接收数据
            socket.BeginReceive(readBuff, 0, buff_size, SocketFlags.None, ReceiveCb, null);

        }
        catch(Exception e)
        {
            txtStr.text = "链接已经断开"+e.Message;
            socket.Close();
        }
    }


    //给服务器发送消息
    public void Send()
    {
        string str = textInput.text;
        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        try
        {
            socket.Send(bytes);
        }
        catch
        {

        }
    }
}
