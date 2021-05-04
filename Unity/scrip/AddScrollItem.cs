using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddScrollItem : MonoBehaviour
{
    public Text name_text;
    public Text id_text;
    public Image icon_image;
    private string id;
    

    public void Set(string id,string name)
    {
        this.id = id;
        this.name = name;
        name_text.text = name;
        id_text.text = "id:"+id;
    }

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
        icon_image.overrideSprite = iconSprite;
    }

    public void AddButtonOnClick()
    {
        MyWebSocket myWebSocket = MyWebSocket.Instance;
        myWebSocket.AddFriend(id);
    }
}
