using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

//登录
public class Login_send_data
{
    public int id;
    public string password;
}
public class Login_send
{
    public string function = "login";
    public Login_send_data data;
}
public class Login_receive_data
{
    public int id; //用户ID，非微信ID
    public string nickname; //昵称，可能为空
    public int age; //年龄，可能为空
    public int gender; //性别，0为女性，1为男性，2为未填写
    public int type; //患病类型
    public string mac; //完全按照客户端发来的mac返回

}
public class Login_receive
{
    public string function;
    public int error_code;
    public string error_message;
    public Login_receive_data data;

}

//注册
public class Register_send_data
{
    public string nickname;
    public string password;
    public int age;
    public int gender;
    public int type;
    public string mac;
}
public class Register_send
{
    public string function = "register";
    public Register_send_data data;
}
public class Register_receive_data
{
    public int id; //用户ID，非微信ID
    public string nickname; //昵称，可能为空
    public int age; //年龄，可能为空
    public int gender; //性别，0为女性，1为男性，2为未填写
    public int type; //患病类型
    public string mac; //完全按照客户端发来的mac返回
}
public class Register_receive
{
    public string function;
    public int error_code;
    public string error_message;
    public Register_receive_data data;
}




//提交训练和测评记录
public class Evaluate
{
    public int[] action;
    public int[] score;
    public float[] time;
}
public class Put_record_send_data
{
    public int user_id;
    public int group_id;
    public JsonData evaluate;
    public float score;
    public string mac;
    public string datetime;
}
public class Put_record_send
{
    public string function = "put_record";
    public JsonData data;
}
public class Put_record_receive
{
    public string function;
    public int error_code;
    public string error_message;
    public JsonData data;
}


//获得训练评测记录
public class Get_record_send
{
    public string function = "get_record";
    public Get_record_send_data data;

}
public class Get_record_send_data
{
    public int user_id;
    public int page_no;
    public int page_size;
}

public class Get_record_receive
{
    public string fucntion;
    public int error_code;
    public string error_message;
    public Get_record_receive_data[] data;
}
public class Get_record_receive_data
{
    public int id; //训练记录ID
    public string user_id;
    public string group_id;
    public string mac;
    public Evaluate evaluate;
    public float score;
    public string datatime;
    public int number;
}

//动作推荐
public class Get_advice_send
{
    public string function = "get_advice";
    public Get_advice_send_data data;
}
public class Get_advice_send_data
{
    public int user_id;
}
public class Get_advice_receive
{
    public string function;
    public int error_code;
    public string error_message;
    public Get_advice_receive_data data;
}
public class Get_advice_receive_data
{
    public int id;
    public string position;
    public int difficulty;
    public int duration;
    public int times;
    public int range;
    public int speed;

}
