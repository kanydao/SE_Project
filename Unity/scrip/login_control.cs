using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.WebSocket;
using System;
using LitJson;
using UnityEngine.SceneManagement;

public class login_control : MonoBehaviour
{
    public InputField id_input;
    public InputField password_input;
    public GameObject id_missing;
    public GameObject password_missing;
    public GameObject password_error;

    private MyWebSocket myWebSocket;
    private string id;
    private string password;

    // Start is called before the first frame update
    void Start()
    {
        myWebSocket = MyWebSocket.Instance;
        myWebSocket.webSocket.OnOpen = null;
        myWebSocket.webSocket.OnOpen += LoginOnOpen;
        myWebSocket.webSocket.OnMessage = null;
        myWebSocket.webSocket.OnMessage += LoginOnMessage;
        myWebSocket.webSocket.OnError = null;
        myWebSocket.webSocket.OnError += LoginOnError;
        myWebSocket.webSocket.OnError += myWebSocket.OnError;

    }

    public void LoginOnClick()
    {
        if (id_input.text == "")
        {
            id_missing.SetActive(true);
            return;
        }
        else
        {
            id_missing.SetActive(false);
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



        if (!myWebSocket.IsConnected())
            myWebSocket.Connect();
        else
            myWebSocket.Login(id, password);

        //string test = "{\"function\":\"login\",\"error_code\":0,\"error_message\":\"\",\"data\":{\"username\":\"MengLong\",\"icon\":1,\"friend\":[{\"username\":\"ZhangHanTao\",\"account\":\"100002\",\"icon\":0,\"state\":0},{\"username\":\"ZengZhiQiang\",\"account\":\"100003\",\"icon\":0,\"state\":0},{\"username\":\"LaiYuHua\",\"account\":\"100004\",\"icon\":3,\"state\":1}]}}";
        //LoginOnMessage(myWebSocket.webSocket, test);
    }

    public void LoginOnOpen(WebSocket web)
    {
        print("连接成功");
        myWebSocket.Login(id, password);
    }

    public void LoginOnMessage(WebSocket web, string message)
    {

        print("收到：" + message);

        JsonData loginReceiveJson = JsonMapper.ToObject(message);
        if (loginReceiveJson["error_code"].ToString() == "201")
        {
            password_error.gameObject.SetActive(true);
            return;
        }
        else if(loginReceiveJson["error_code"].ToString() == "0")
        {
            JsonData loginReceiveDataJson = loginReceiveJson["data"];
            User user = User.Instance;
            string name = loginReceiveDataJson["username"].ToString();
            user.Init(id, name, (User.Icon)int.Parse(loginReceiveDataJson["icon"].ToString()));

            JsonData friendJson = loginReceiveDataJson["friend"];
            foreach(JsonData friendItemJson in friendJson)
            {
                User.Friend friend = new User.Friend();
                friend.name = friendItemJson["username"].ToString();
                print(friend.name);
                friend.id = friendItemJson["account"].ToString();
                friend.icon = (User.Icon) int.Parse(friendItemJson["icon"].ToString());
                friend.state = int.Parse(friendItemJson["icon"].ToString());
                user.friendList.Add(friend);
            }


            //出来下线的时候的信息，注意要添加消息是放在里talklist里面，talklist与
            //friendlist并不完全对应，注意添加到对应的talklist里面去
            JsonData MessageJson = loginReceiveJson["rec_message"];
            foreach(JsonData messageItemJson in MessageJson)
            {
                string s_id = messageItemJson["s_account"].ToString();
                string r_id = messageItemJson["r_account"].ToString();
                string content = messageItemJson["content"].ToString();
                string time = messageItemJson["date"].ToString();

                bool is_send;
                if (s_id != user.id && r_id != user.id)
                    continue;
                if (s_id == user.id)
                    is_send = true;
                else
                    is_send = false;

                User.Message message_ = new User.Message();
                message_.direction = is_send ? User.Message.MessageDirection.Send : User.Message.MessageDirection.Receive;
                message_.content = content;
                message_.to_id = r_id;
                message_.from_id = s_id;
                message_.type = User.Message.MessageType.Text;
                message_.state = User.Message.MessageState.None;
                message_.time = time;

                User.Talk talk = user.FindTalk(is_send ? r_id : s_id);
                if(talk == null)
                {
                    talk = new User.Talk();
                    talk.friend_id = is_send ? r_id : s_id;
                    talk.messagesList = new List<User.Message>();
                    user.talkList.Add(talk);
                }

                talk.messagesList.Add(message_);
            }

            print("登录成功");

            SceneManager.LoadScene("Chat");
        }
        else
        {
            print(loginReceiveJson["error_message"].ToString());
        }
            
    }

    public void LoginOnError(WebSocket ws, Exception ex)
    {
        password_error.SetActive(true);
        Text text = password_error.transform.Find("Text").GetComponent<Text>();
        text.text = "网络链接错误！";
    }

}
