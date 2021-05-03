using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class window_set_chat : MonoBehaviour
{
    public Image quit_button;
    public Image minimize_button;
    public Image chat_up_background;

    private RectTransform[] exclude_area;


    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
#else
        MyWindows.SetWindowsNoFrame();
        MyWindows.SetWindowsSize(1075,800);
        exclude_area = new RectTransform[2];
        exclude_area[0] = quit_button.rectTransform;
        exclude_area[1] = minimize_button.rectTransform;
#endif
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
#else
        MyWindows.WindowsMove(chat_up_background.rectTransform, exclude_area);
#endif
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Minimized()
    {
#if UNITY_EDITOR

#else
        MyWindows.Minimized();
#endif
    }
}
