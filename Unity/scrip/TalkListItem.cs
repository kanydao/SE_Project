using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkListItem : MonoBehaviour
{
    public GameObject icon;
    public Text nameText;
    public Text messageText;
    public Text timeText;
    public Image iconImage;

    //用户id
    private string id;

    /// <summary>
    /// 初始化的过程中
    /// </summary>
    /// <param name="name"></param>
    /// <param name="message"></param>
    /// <param name="time"></param>
    public void Set(string id = null,string name = null,string message = null, string time = null)
    {
        if (id != null)
            this.id = id;
        if(name!=null)
            nameText.text = name;
        if (message != null)
            messageText.text = message;
        if (time != null)
            timeText.text = time;
    }

    /// <summary>
    /// 设置头像
    /// </summary>
    /// <param name="icon"></param>
    public void SetIcon(User.Icon icon)
    {
        Sprite iconSprite;
        switch (icon)
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
                return;
        }
        iconImage.overrideSprite = iconSprite;

    }


    /// <summary>
    /// 通过chat_control.Instance完成message_scroll的更新
    /// </summary>
    public void OnClick()
    {
        chat_control.Instance.UpdateMessageScroll(id);
        
    }

}
