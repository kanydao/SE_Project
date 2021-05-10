import tools
message_account = 0;
# 用来不断自动更新消息的序号，消息的_id作为消息存储的唯一标识

def message(conn, data,client):
    global message_account
    if tools.whether_miss(data, ['s_account', 'r_account', 'g_account', 'is_group', 'content', 'date']):
        return {
            'function': 'message',
            'error_code': 3,
            'error_message': 'lose data!',
            'data': {}
        }
    # r_account:接收者账号，首先判断这个是否为当前用户的好友
    sql = '''SELECT account FROM User WHERE (account in
                    (SELECT account_1 FROM Friend WHERE account_2 = {} ) or account in
                    (SELECT account_2 FROM Friend WHERE account_1 = {} ) )'''.format(data['s_account'], data['s_account'])
    cur = conn.cursor()
    cur.execute(sql)
    # conn.row_factory = tools.sqlite_dict
    # 注意fetchone()只返回符合条件的查找的第一项，应该使用fetchall()将所有符合条件的全部查询出来
    res = cur.fetchall()
    # print(res)
    # print(data['r_account'])
    flag = tools.check_friend(data['r_account'],res)
    cur.close()
    if not flag:
        return{
            'function': 'message',
            'error_code': 601,
            'error_message': 'He or she is not your friend!',
            'data': {}
        }
    # 是好友，检查是否在线，然后处理
    # 对client进行处理
    if client.has_key(data['r_account']) == True:
        # 当前好友在线直接返回相应的消息
        return{
            'function': 'message',
            'error_code': 0,
            'error_message': '',
            'data': data
        }
    else:
        # 当前用户不在线，需要存储到数据库中，然后在该用户上线的时候，将数据库中对应的消息发送出去
        cur = conn.cursor()
        message_account +=1
        # 每次需要存储一个消息时，对应的位置自动+1
        # 统一存储成非群消息格式
        sql = '''INSERT INTO Message (_id,s_account,r_account,is_group,content,create_date) VALUES('{}','{}','{}','{}','{}','{}')'''.format(
            message_account, data['s_account'], data['r_account'], 0, data['content'], data['date'])
        cur.execute(sql)
        conn.commit()
        cur.close()
        # 存储完成
        # 注意在登录时，需要再读一下，如果又相应的信息，需要再发送给客户端

