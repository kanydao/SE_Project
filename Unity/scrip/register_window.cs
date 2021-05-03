using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class register_window : MonoBehaviour
{
    public Image quit_button;
    public Image minimize_button;
    public GameObject move_area;

    private RectTransform[] exclude_area;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR

#else
        MyWindows.SetWindowsNoFrame();
        MyWindows.SetWindowsSize(537,613);
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
        MyWindows.WindowsMove(move_area.transform as RectTransform, exclude_area);
#endif
    }

    public void Quit()
    {
        SceneManager.LoadScene("Login");
    }

    public void Minimized()
    {
#if UNITY_EDITOR

#else
        MyWindows.Minimized();
#endif
    }
}
