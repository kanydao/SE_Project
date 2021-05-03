using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chat_control : MonoBehaviour
{
    public ScrollRect talk_scroll;
    public ScrollRect message_scroll;
    public ScrollRect friend_scroll;
    public Image chat_button;
    public Image friend_button;
    public InputField message_intput;
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

    public User.Talk currentTalk = null;

    //预制体
    public TalkListItem talkListItem;
    public MessageScrollItem message_left;
    public MessageScrollItem message_right;
    public FriendListItem friendListItem;

    // Start is called before the first frame update
    void Start()
    {
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
                talkListItem.Set(talk.friend_id, friend.name, message.content, message.time);
                talkListItem.SetIcon(friend.icon);
            }
        }

        if(User.Instance.friendList!=null)
        {
            Transform friend_scroll_content = friend_scroll.transform.Find("Viewport").transform.Find("Content");
            foreach (User.Friend friend in User.Instance.friendList)
            {
                FriendListItem friend_list_item = Instantiate(friendListItem, friend_scroll_content);
                friend_list_item.SetIcon(friend.icon);
                friend_list_item.SetName(friend.name);
            }
        }
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

        //获得talk
        currentTalk = User.Instance.FindTalk(id);
        if (currentTalk == null)
        {
            //添加新的talk
            currentTalk = new User.Talk();
            currentTalk.friend_id = id;
            currentTalk.messagesList = new List<User.Message>();
            User.Instance.talkList.Add(currentTalk);
        }
        

        //更新聊天框
        User.Friend friend = User.Instance.FindFriend(id);
        if (friend == null)
            return;
        foreach(User.Message message in currentTalk.messagesList)
        {
            if(message.direction == User.Message.MessageDirection.Send)
            {
                MessageScrollItem message_scroll_item = Instantiate(message_right,message_scroll_content);
                message_scroll_item.SetMessage(message.content);
                message_scroll_item.SetIcon(friend.icon);
            }
            else if(message.direction == User.Message.MessageDirection.Receive)
            {
                MessageScrollItem message_scroll_item = Instantiate(message_left, message_scroll_content);
                message_scroll_item.SetMessage(message.content);
                message_scroll_item.SetIcon(friend.icon);
            }
        }


    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public void SendMessage()
    {
        if (message_intput.text == "")
            return;

        User.Message message = new User.Message();
        message.direction = User.Message.MessageDirection.Send;
        message.content = message_intput.text;
        message.to_id = currentTalk.friend_id;
        message.from_id = User.Instance.id;
        message.type = User.Message.MessageType.Text;
        message.state = User.Message.MessageState.None;
        currentTalk.messagesList.Add(message);

        Transform message_scroll_content = message_scroll.transform.Find("Viewport").transform.Find("Content");
        MessageScrollItem message_scroll_item = Instantiate(message_right, message_scroll_content);
        message_scroll_item.SetMessage(message_intput.text);
        message_scroll_item.SetIcon(User.Instance.FindFriend(currentTalk.friend_id).icon);

        LayoutRebuilder.ForceRebuildLayoutImmediate(message_scroll_item.gameObject.transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(message_scroll_content.transform as RectTransform);

        //未实现
        MyWebSocket.Instance.SendMessage(message_intput.text);

        message_intput.text = "";

        

        message_intput.ActivateInputField();
    }

    public void ChatButtonOnClick()
    {
        chat_button.sprite = chat_button_sprite[1];
        friend_button.sprite = friend_button_sprite[0];
        talk_scroll.gameObject.SetActive(true);
        friend_scroll.gameObject.SetActive(false);
    }

    public void FriendButtonOnClick()
    {
        friend_button.sprite = friend_button_sprite[1];
        chat_button.sprite = chat_button_sprite[0];
        friend_scroll.gameObject.SetActive(true);
        talk_scroll.gameObject.SetActive(false);

    }
   
}
