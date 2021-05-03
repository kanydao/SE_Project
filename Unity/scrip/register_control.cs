using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class register_control : MonoBehaviour
{
    public InputField name_input;
    public InputField password_input;
    public InputField email_input;
    public GameObject warning;
    private Text warning_text;
    
    // Start is called before the first frame update
    void Start()
    {
        warning_text = warning.transform.GetChild(0).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterOnClick()
    {
        if(name_input.text == "")
        {
            warning.SetActive(true);
            warning_text.text = "请输入用户名后注册";
            return;
        }
        else if(password_input.text == "")
        {
            warning.SetActive(true);
            warning_text.text = "请输入密码后注册";
            return;
        }else if(email_input.text == "")
        {
            warning.SetActive(true);
            warning_text.text = "请输入邮箱后注册";
            return;
        }
        else
        {
            warning.SetActive(false);
        }

    }
}
