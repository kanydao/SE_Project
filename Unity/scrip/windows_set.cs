using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class windows_set : MonoBehaviour
{

    public Image school_background_image;
    public Image quit_button_image;
    public Image minimized_button_image;
    private RectTransform[] exclude_area;


    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
#else
        MyWindows.SetWindowsNoFrame();
        MyWindows.SetWindowsSize(537, 413);
        exclude_area = new RectTransform[2];
        exclude_area[1] = minimized_button_image.rectTransform;
        exclude_area[0] = quit_button_image.rectTransform;
#endif

    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        
#else
        MyWindows.WindowsMove(school_background_image.rectTransform,exclude_area);
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

    public void LoginOnClick()
    {
        SceneManager.LoadScene("Chat");
    }

    public void RegisterOnClick()
    {
        SceneManager.LoadScene("Register");
    }
}
