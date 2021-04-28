import sqlite3

conn = sqlite3.connect("./db/NEUIM.db")  # 不存在的话，默认创建一个数据库
print("open successfully!")
c = conn.cursor()
# 1.用户表
c.execute('''create table User
    (
        account message_text  primary  key,  
        password  message_text  not null ,
        username message_text not null , 
        icon message_text not null,
        mail message_text not null,
        state int not null );''')
# 2.朋友表
# 注意朋友表，两者的id共同作为主键
c.execute('''create table Friend
    (
        account_1 message_text not null , 
        account_2 message_text not null,
        primary key (account_1,account_2)
);''')
# 3.消息表
c.execute('''create table Message
    (
        _id int primary  key not null ,
        owner message_text not null,
        direct int not null ,
        account message_text not null ,
        type int not null ,
        content text not null,
        state int not null,
        read int not null,
        create_time int not null);''')
# 4.会话表，显示的好友聊表那片
c.execute('''create table Conversation(
        _id int primary  key not null ,
        owner   text not null ,
        account text not null,
        icon text not null ,
        name text not null,
        content text not null,
        update_count int not null,
        update_time int not null);''')
print("Table create successfully!")
# 数据生成文件，只运行一次，目的是在指定目录下生成所要使用的数据库文件
# nohup python3 -u main.py > /NEUIM/test.log 2>&1 &
#45
# ps -aux | grep "python3 -u test.py"