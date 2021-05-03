using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User 
{
    public enum Icon { ICON1, ICON2, ICON3, ICON4 }

    /// <summary>
    /// 朋友类
    /// </summary>
    public class Friend
    {
        public string id;
        public string name;
        public Icon icon;
    }

    /// <summary>
    /// 对话类
    /// </summary>
    public class Talk
    {
        public string friend_id;
        public List<Message> messagesList;
    }

    /// <summary>
    /// 消息类
    /// </summary>
    public class Message
    {
        public enum MessageType {Text, Image}
        public enum MessageDirection {Send, Receive}
        public enum MessageState {NoFriend, NoNetWork, None}
        public string from_id;
        public string to_id;
        public MessageType type;
        public MessageState state;
        public MessageDirection direction;
        public string content;
        public string time;
    }


    private static User instance;
    public static User Instance
    {
        get
        {
            if (null == instance)
                instance = new User();
            return instance;
        }
    }

    public string id;
    public string name;
    public Icon icon;
    private bool is_init = false;
    public bool Is_init { get { return is_init; } }
    public List<Friend> friendList;
    public List<Talk> talkList;

    /// <summary>
    /// 在登录界面的Login中调用，完成初始化
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    public void Init(string id,string name, Icon icon)
    {
        this.id = id;
        this.name = name;
        this.icon = icon;
        friendList = new List<Friend>();
        talkList = new List<Talk>();
        //以后应该在本地存 完成talklist初始化
        is_init = true;
    }

    /// <summary>
    /// 获得对应id的本地Talk聊天记录
    /// </summary>
    /// <param name="id">用户id</param>
    /// <returns></returns>
    public Talk FindTalk(string id)
    {
        foreach(Talk talk in talkList)
        {
            if (talk.friend_id == id)
                return talk;
        }
        return null;
    }

    /// <summary>
    /// 获得对应id的朋友
    /// </summary>
    /// <returns></returns>
    public Friend FindFriend(string id)
    {
        foreach(Friend friend in friendList)
        {
            if (friend.id == id)
                return friend;
        }
        return null;
    }



    
    

}
