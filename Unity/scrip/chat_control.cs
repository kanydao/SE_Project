using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.WebSocket;
using LitJson;
using System;

public class chat_control : MonoBehaviour
{
    public ScrollRect talk_scroll;
    public ScrollRect message_scroll;
    public ScrollRect friend_scroll;
    public Image chat_button;
    public Image friend_button;
    public Button send_button;
    public InputField message_intput;
    public Image chat_input_background;
    public Text name_text;
    public GameObject friend_information;
    public GameObject add_friend;
    public GameObject _404UI;
    static public chat_control Instance
    {
        get
        {
            return instance;
        }
    }
    static private chat_control instance = null;
    

    public Sprite[] chat_button_sprite;
    public Sprite[] friend_button_sprite;

    //局部变量
    public User.Talk currentTalk = null;
    public string current_friend_id;

    private MyWebSocket myWebSocket;

    //预制体
    public TalkListItem talkListItem;
    public MessageScrollItem message_left;
    public MessageScrollItem message_right;
    public FriendListItem friendListItem;
    public AddScrollItem addListItem;

    // Start is called before the first frame update
    void Start()
    {
        myWebSocket = MyWebSocket.Instance;
        myWebSocket.webSocket.OnOpen = null;
        myWebSocket.webSocket.OnMessage = null;
        myWebSocket.webSocket.OnMessage += ChatOnMessage;
        myWebSocket.webSocket.OnError = null;
        myWebSocket.webSocket.OnError += ChatOnError;
        myWebSocket.webSocket.OnError += myWebSocket.OnError;


        instance = transform.GetComponent<chat_control>();
        Init();
    }


    /// <summary>
    /// 完成界面的初始化
    /// </summary>
    private void Init()
    {
        
        if(User.Instance.talkList!=null)
        {
            Transform talk_scroll_content = talk_scroll.transform.Find("Viewport").transform.Find("Content");
            foreach (User.Talk talk in User.Instance.talkList)
            {
                TalkListItem talk_list_item = Instantiate(talkListItem, talk_scroll_content);
                User.Friend friend = User.Instance.FindFriend(talk.friend_id);
                User.Message message = talk.messagesList[talk.messagesList.Count - 1];
                talk_list_item.Set(talk.friend_id, friend.name, message.content, message.time);
                talk_list_item.SetIcon(friend.icon);
            }
        }

        if(User.Instance.friendList!=null)
        {
            Transform friend_scroll_content = friend_scroll.transform.Find("Viewport").transform.Find("Content");
            foreach (User.Friend friend in User.Instance.friendList)
            {
                FriendListItem friend_list_item = Instantiate(friendListItem, friend_scroll_content);
                friend_list_item.SetIcon(friend.icon);
                friend_list_item.Set(friend.name,friend.id);
            }
        }

        //注意做messagescroll的初始化，就是一开始如果没有talk
        //模仿微信 messagescroll 一开始不会显示 点击talklist之后才会显示
    }

    /// <summary>
    /// 切换聊天框使用，由TalkListItem调用
    /// </summary>
    /// <param name="id">切换到的好友id</param>
    public void UpdateMessageScroll(string id)
    {
        //清空当前聊天框
        Transform message_scroll_content = message_scroll.transform.Find("Viewport").transform.Find("Content");
        for (int i = 0; i < message_scroll_content.childCount; i++)
            Destroy(message_scroll_content.GetChild(i).gameObject);

        User.Friend friend = User.Instance.FindFriend(id);  

        //获得talk
        currentTalk = User.Instance.FindTalk(id);
        if (currentTalk == null)
        {
            //添加新的talk
            currentTalk = new User.Talk();
            currentTalk.friend_id = id;
            currentTalk.messagesList = new List<User.Message>();
            User.Instance.talkList.Add(currentTalk);

            //在talk_scroll中添加新的item
            Transform talk_scroll_content = talk_scroll.transform.Find("Viewport").transform.Find("Content");
            TalkListItem talk_list_item = Instantiate(talkListItem, talk_scroll_content);
            talk_list_item.Set(id, friend == null ? "--" : friend.name);
            talk_list_item.SetIcon(friend == null ? User.Icon.ICON1 : friend.icon);
        }
        

        //更新聊天框
        foreach(User.Message message in currentTalk.messagesList)
        {
            if(message.direction == User.Message.MessageDirection.Send)
            {
                MessageScrollItem message_scroll_item = Instantiate(message_right,message_scroll_content);
                message_scroll_item.SetMessage(message.content);
                message_scroll_item.SetIcon(User.Instance.icon);
            }
            else if(message.direction == User.Message.MessageDirection.Receive)
            {
                MessageScrollItem message_scroll_item = Instantiate(message_left, message_scroll_content);
                message_scroll_item.SetMessage(message.content);
                message_scroll_item.SetIcon(friend == null ? User.Icon.ICON1 : friend.icon);
            }
        }

        //更新名字
        name_text.text = friend == null ? "--" : friend.name;

    }

    /// <summary>
    /// 更新朋友信息框UI
    /// </summary>
    public void UpdateFriendInformation(string id)
    {
        current_friend_id = id;

        friend_information.SetActive(true);

        //将上一次有可能提示的错误清除
        GameObject warning = friend_information.transform.Find("warning").gameObject;
        warning.SetActive(false);

        User.Friend friend = User.Instance.FindFriend(id);

        Text name_text = friend_information.transform.Find("name").GetComponent<Text>();
        Text id_text = friend_information.transform.Find("id").GetComponent<Text>();
        Text state_text = friend_information.transform.Find("state").GetComponent<Text>();
        Image icon_image = friend_information.transform.Find("icon").Find("icon image").GetComponent<Image>();

        if(friend!=null)
        {
            name_text.text = friend.name;
            id_text.text = friend.id;
            if (friend.state == 0)
                state_text.text = "在线";
            else
                state_text.text = "下线";
            Sprite iconSprite;
            switch (friend.icon)
            {
                case User.Icon.ICON1:
                    iconSprite = Resources.Load("icon/icon1", typeof(Sprite)) as Sprite;
                    break;
                case User.Icon.ICON2:
                    iconSprite = Resources.Load("icon/icon2", typeof(Sprite)) as Sprite;
                    break;
                case User.Icon.ICON3:
                    iconSprite = Resources.Load("icon/icon3", typeof(Sprite)) as Sprite;
                    break;
                case User.Icon.ICON4:
                    iconSprite = Resources.Load("icon/icon4", typeof(Sprite)) as Sprite;
                    break;
                default:
                    iconSprite = Resources.Load("icon/icon1", typeof(Sprite)) as Sprite;
                    break;
            }
            icon_image.overrideSprite = iconSprite;
        }
        else
        {
            name_text.text = "--";
            id_text.text = "--";
            state_text.text = "--";
        }
        
        
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public void SendButtonOnClick()
    {
        if (message_intput.text == "")
            return;

        /*User.Message message = new User.Message();
        message.direction = User.Message.MessageDirection.Send;
        message.content = message_intput.text;
        message.to_id = currentTalk.friend_id;
        message.from_id = User.Instance.id;
        message.type = User.Message.MessageType.Text;
        message.state = User.Message.MessageState.None;
        currentTalk.messagesList.Add(message);*/

        /*Transform message_scroll_content = message_scroll.transform.Find("Viewport").transform.Find("Content");
        MessageScrollItem message_scroll_item = Instantiate(message_right, message_scroll_content);
        message_scroll_item.SetMessage(message_intput.text);
        message_scroll_item.SetIcon(User.Instance.FindFriend(currentTalk.friend_id).icon);

        LayoutRebuilder.ForceRebuildLayoutImmediate(message_scroll_item.gameObject.transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(message_scroll_content.transform as RectTransform);*/

        myWebSocket.SendMessage(currentTalk.friend_id,message_intput.text);

        message_intput.text = "";

        message_intput.ActivateInputField();
    }

    /// <summary>
    /// 点击左边talk按钮，完成页面切换
    /// </summary>
    public void ChatButtonOnClick()
    {
        chat_button.sprite = chat_button_sprite[1];
        friend_button.sprite = friend_button_sprite[0];

        name_text.gameObject.SetActive(true);
        talk_scroll.gameObject.SetActive(true);
        friend_scroll.gameObject.SetActive(false);
        message_scroll.gameObject.SetActive(true);
        message_intput.gameObject.SetActive(true);
        send_button.gameObject.SetActive(true);
        chat_input_background.gameObject.SetActive(true);
        friend_information.SetActive(false);
    }

    /// <summary>
    /// 点击左边friend按钮，完成页面切换
    /// </summary>
    public void FriendButtonOnClick()
    {
        friend_button.sprite = friend_button_sprite[1];
        chat_button.sprite = chat_button_sprite[0];

        name_text.gameObject.SetActive(false);
        friend_scroll.gameObject.SetActive(true);
        talk_scroll.gameObject.SetActive(false);
        message_scroll.gameObject.SetActive(false);
        message_intput.gameObject.SetActive(false);
        send_button.gameObject.SetActive(false);
        chat_input_background.gameObject.SetActive(false);


    }
   
    /// <summary>
    /// 好友信息UI中点击发送消息
    /// </summary>
    public void FriendSendButtonOnClick()
    {

        //加一个是否是好友的检测

        if(User.Instance.FindFriend(current_friend_id) == null)
        {
            GameObject warning =  friend_information.transform.Find("warning").gameObject;
            warning.SetActive(true);
            Text text = warning.transform.Find("text").GetComponent<Text>();
            text.text = "对方不是您的好友";
            return;
        }

        ChatButtonOnClick();
        UpdateMessageScroll(current_friend_id);
    }

    /// <summary>
    /// 添加好友UI中点击添加好友
    /// </summary>
    public void AddFriendButtonOnClick()
    {
        add_friend.SetActive(true);
    }

    /// <summary>
    /// 添加好友UI中点击关闭
    /// </summary>
    public void AddFriendQuitButtonOnClick()
    {
        add_friend.transform.Find("warning").gameObject.SetActive(false);
        add_friend.SetActive(false);
    }

    /// <summary>
    /// 添加好友UI中点击搜索
    /// </summary>
    public void AddFriendSearchButtonOnClick()
    {
        InputField id_input =  add_friend.transform.Find("search InputField").GetComponent<InputField>();
        string id = id_input.text;
        if (id == "")
            return;
        myWebSocket.SearchFriend(id);
    }

    /// <summary>
    /// 好友信息UI中点击删除好友
    /// </summary>
    public void DeletaFriendButtonOnClick()
    {
        myWebSocket.DeleteFriend(current_friend_id);
        //FriendListItem删除
    }


    public void ChatOnMessage(WebSocket webSocket, string message)
    {
        print("收到：" + message);
        JsonData Json = JsonMapper.ToObject(message);
        string function = Json["function"].ToString();
        switch (function)
        {
            case "add_friend":
                AddFriendOnMessage(webSocket, message);
                break;
            case "delete_friend":
                DeleteFriendOnMessage(webSocket, message);
                break;
            case "search_user":
                SearchFriendOnMessage(webSocket, message);
                break;
            case "message":
                MessageOnMessage(webSocket, message);
                break;
        }

    }

    // AddFriend
    public void AddFriendOnMessage(WebSocket webSocket,string message)
    {
        JsonData addJson = JsonMapper.ToObject(message);
        if (addJson["function"].ToString() != "add_friend")
            return;
        if(addJson["error_code"].ToString()=="401")
        {
            add_friend.transform.Find("warning").gameObject.SetActive(true);
            return;
        }
        if(addJson["error_code"].ToString()=="0")
        {
            GameObject warning = add_friend.transform.Find("warning").gameObject;
            Text text = warning.transform.Find("text").GetComponent<Text>();
            warning.SetActive(true);
            text.text = "添加好友成功";

            JsonData dataJson = addJson["data"];
            string id = dataJson["account"].ToString();
            string name = dataJson["username"].ToString();
            User.Icon icon = (User.Icon)int.Parse(dataJson["icon"].ToString());
            int state = int.Parse(dataJson["state"].ToString());

            User.Friend friend = new User.Friend();
            friend.id = id;
            friend.name = name;
            friend.icon = icon;
            friend.state = state;

            User.Instance.friendList.Add(friend);

            Transform friend_scroll_content = friend_scroll.transform.Find("Viewport").transform.Find("Content");
            FriendListItem friend_list_item = Instantiate(friendListItem, friend_scroll_content);
            friend_list_item.Set(name, id);
            friend_list_item.SetIcon(icon);
            LayoutRebuilder.ForceRebuildLayoutImmediate(friend_scroll_content as RectTransform);
        }
    }
    
    // DelteFriend
    public void DeleteFriendOnMessage(WebSocket webSocket, string message)
    {
        JsonData deleteJson = JsonMapper.ToObject(message);
        if (deleteJson["function"].ToString() != "delete_friend")
            return;
        if (deleteJson["error_code"].ToString() == "501")
        {
            print("对方不是您的好友");
            return;
            //可以考虑更加好的提示方式
        }
        if (deleteJson["error_code"].ToString() == "0")
        {
            JsonData dataJson = deleteJson["data"];
            string id = dataJson["account"].ToString();
            User.Friend friend = User.Instance.FindFriend(id);
            if (friend!=null)
                User.Instance.friendList.Remove(friend);

            Transform friend_scroll_content = friend_scroll.transform.Find("Viewport").transform.Find("Content");
            for (int i = 0; i < friend_scroll_content.childCount; i++)
            {
                FriendListItem friendListItem = friend_scroll_content.GetChild(i).GetComponent<FriendListItem>();
                if(friendListItem.id==id)
                {
                    Destroy(friend_scroll_content.GetChild(i).gameObject);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(friend_scroll_content as RectTransform);
                    break;
                }
            }

            //显示提示
            if(friend_information.activeSelf == true && current_friend_id == id)
            {
                GameObject warning = friend_information.transform.Find("warning").gameObject;
                warning.SetActive(true);
                Text text = warning.transform.Find("text").GetComponent<Text>();
                text.text = "已删除好友";
            }

            

        }


    }

    // SearchFriend
    public void SearchFriendOnMessage(WebSocket webSocket, string message)
    {
        JsonData searchJson = JsonMapper.ToObject(message);
        if (searchJson["function"].ToString() != "search_user")
            return;
       
        //清空
        Transform add_friend_content = add_friend.transform.Find("add friend list Scroll View").Find("Viewport").Find("Content");
        for (int i = 0; i < add_friend_content.childCount; i++)
            Destroy(add_friend_content.GetChild(i).gameObject);

        if (searchJson["error_code"].ToString() == "301")
        {
            GameObject warning = add_friend.transform.Find("warning").gameObject;
            Text text = warning.transform.Find("text").GetComponent<Text>();
            warning.SetActive(true);
            text.text = "搜索结果为空";
            return;
        }

        if (searchJson["error_code"].ToString() == "0")
        {
            JsonData dataJson = searchJson["data"];
            JsonData resultJson = dataJson["result"];
            foreach(JsonData itemJson in resultJson)
            {
                string id = itemJson["account"].ToString();
                string name = itemJson["username"].ToString();
                User.Icon icon = (User.Icon) int.Parse(itemJson["icon"].ToString());
                AddScrollItem addScrollItem = Instantiate(addListItem, add_friend_content);
                addScrollItem.Set(id, name);
                addScrollItem.SetIcon(icon);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(add_friend_content as RectTransform);
        }

    }

    // Message
    public void MessageOnMessage(WebSocket webSocket, string r_message)
    {
        JsonData messageJson = JsonMapper.ToObject(r_message);
        if (messageJson["function"].ToString() != "message")
            return;
        if(messageJson["error_code"].ToString() == "601")
        {
            print("对方不是您的好友");
            return;
        }
        if(messageJson["error_code"].ToString() == "0")
        {
            JsonData dataJson = messageJson["data"];
            string s_id = dataJson["s_account"].ToString();
            string r_id = dataJson["r_account"].ToString();
            string content = dataJson["content"].ToString();
            string time = dataJson["date"].ToString();

            if (s_id != User.Instance.id && r_id != User.Instance.id)
                return;

            bool is_send;
            //己方发送的消息
            if (s_id == User.Instance.id)
                is_send = true;
            else
                is_send = false;

            User.Message message = new User.Message();
            message.direction = is_send?User.Message.MessageDirection.Send:User.Message.MessageDirection.Receive;
            message.content = content;
            message.to_id =  r_id;
            message.from_id = s_id;
            message.type = User.Message.MessageType.Text;
            message.state = User.Message.MessageState.None;
            message.time = time;

            //当前窗口
            if (r_id == currentTalk.friend_id)
            {

                currentTalk.messagesList.Add(message);

                Transform message_scroll_content = message_scroll.transform.Find("Viewport").transform.Find("Content");
                MessageScrollItem message_scroll_item = Instantiate(is_send ? message_right : message_left, message_scroll_content);
                message_scroll_item.SetMessage(message_intput.text);
                User.Friend friend = User.Instance.FindFriend(currentTalk.friend_id);
                message_scroll_item.SetIcon(is_send ? User.Instance.icon : (friend == null ? User.Icon.ICON1 : friend.icon));

                LayoutRebuilder.ForceRebuildLayoutImmediate(message_scroll_item.gameObject.transform as RectTransform);
                LayoutRebuilder.ForceRebuildLayoutImmediate(message_scroll_content.transform as RectTransform);
            }
            //其他窗口
            else
            {
                User.Talk talk = User.Instance.FindTalk(is_send ? r_id : s_id);
                TalkListItem talk_list_item = null;
                Transform talk_scroll_content = talk_scroll.transform.Find("Viewport").transform.Find("Content");

                if (talk == null)
                {
                    talk = new User.Talk();
                    talk.friend_id = is_send ? r_id : s_id;
                    talk.messagesList = new List<User.Message>();
                    User.Instance.talkList.Add(talk);
                    talk_list_item = Instantiate(talkListItem, talk_scroll_content);
                }
                else
                {
                    for (int i = 0; i < talk_scroll_content.childCount; i++)
                    {
                        TalkListItem talkListItem_ = talk_scroll_content.GetChild(i).GetComponent<TalkListItem>();
                        if (talkListItem_.id == (is_send ? r_id : s_id)) 
                        {
                            talk_list_item = talkListItem_;
                            break;
                        }
                    }
                }

                talk.messagesList.Add(message);
                User.Friend friend = User.Instance.FindFriend(talk.friend_id);
                talk_list_item.Set((is_send ? r_id : s_id), (friend == null ? "--" : friend.name), message.content, message.time);
                talk_list_item.SetIcon(friend == null ? User.Icon.ICON1:friend.icon);
                talk_list_item.SetNew(true);

            }
        }

    }
    
    //Error
    public void ChatOnError(WebSocket ws, Exception ex)
    {
        _404UI.SetActive(true);
    }
}
