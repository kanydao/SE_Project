using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendListItem : MonoBehaviour
{
    public Image iconImage;
    public Text nameText;

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

    public void SetName(string name)
    {
        nameText.text = name;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
