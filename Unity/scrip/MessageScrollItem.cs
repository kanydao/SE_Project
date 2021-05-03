using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageScrollItem : MonoBehaviour
{
    public Text messageText;
    public Image iconImage;
    public const int LINELENGTH = 40;

    /// <summary>
    /// 设置消息
    /// </summary>
    /// <param name="message"></param>
    public void SetMessage(string message)
    {
        string tail_message = message;
        message = "";
        while(IsTooLong(tail_message))
        {
            DivideMessage(ref message, ref tail_message);
        }

        if (message == "")
            message = tail_message;
        else if(tail_message !="")
            message = message + "\n" + tail_message;
        messageText.text = message;
    }

    /// <summary>
    /// 设置消息的头像
    /// </summary>
    /// <param name="i"></param>
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
    /// 计算字符串中中文字的数量
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    static public int CountChinese(string message)
    {
        int count = 0;
        for(int i = 0; i<message.Length; i++)
        {
            if(message[i]>=0x4e00 && message[i]<=0x9fa5)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// 判断字符串是否需要分割
    /// </summary>
    /// <returns></returns>
    public bool IsTooLong(string message)
    {
        if (CountChinese(message) + message.Length > 34)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 字符串分割
    /// </summary>
    /// <param name="message"></param>
    /// <param name="tail_message"></param>
    public void DivideMessage(ref string message, ref string tail_message)
    {
        int sub_length = 17;
        string sub_message = tail_message.Substring(0, sub_length);

        while(CountChinese(sub_message)+sub_message.Length<=34)
        {
            sub_length++;
            sub_message = tail_message.Substring(0, sub_length);
        }

        if (sub_length >= tail_message.Length)
            tail_message = "";
        else
            tail_message = tail_message.Substring(sub_length, tail_message.Length-sub_length);

        if (message == "")
            message = sub_message;
        else
            message = message + "\n" + sub_message;
    }
}
