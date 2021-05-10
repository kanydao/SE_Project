import tools

# 目前已经通过的模块:login,register


# 均以json的形式统一进行返回
# 用户登录
# 成功插入好友表，开始进行相应测试---->测试完毕，该模块通过
def login(conn, data):
    if tools.whether_miss(data, ['account', 'password']):
        return {
            'function': 'login',
            'error_code': 3,
            'error_message': 'lose data!',
            'data': {}
        }, False
    if len(data['password']) != 32:
        return {
            'function': 'login',
            'error_code': 202,
            'error_message': 'password format error',
            'data': {}
        }, False
    sql = '''SELECT username ,icon FROM User WHERE account='{}' LIMIT 1'''.format(data['account'])
    cur = conn.cursor()
    cur.execute(sql)
    # tools.sqlite_dict(cur),需要找一个函数将查询结果以一个列表的形式返回
    res = cur.fetchone()  # 去数据库中查询相应的数据
    if res is None:
        cur.close()
        return {
            'function': 'login',
            'error_code': 201,
            'error_message': 'account or password is error!',
            'data': {}
        }, False
    else:  # 登录成功
        # 更改state
        sql2 = '''UPDATE User set state = 1 WHERE account={}'''.format(data['account'])
        cur.execute(sql2)
        conn.commit()
        # 继续查找好友表，以便进一步返回好友信息
        # 查询语句出现问题
        # sql3 = '''SELECT username,account,icon,state FROM User WHERE (account =
        # (SELECT account_1 FROM Friend WHERE account_2 = {} ) or account =
        # (SELECT account_2 FROM Friend WHERE account_1 = {} ) )'''.format(data['account'],data['account'])
        sql3 = '''SELECT username,account,icon,state FROM User WHERE (account in
                (SELECT account_1 FROM Friend WHERE account_2 = {} ) or account in
                (SELECT account_2 FROM Friend WHERE account_1 = {} ) )'''.format(data['account'], data['account'])
        cur.execute(sql3)
        # conn.row_factory = tools.sqlite_dict
        # 注意fetchone()只返回符合条件的查找的第一项，应该使用fetchall()将所有符合条件的全部查询出来
        res2 = cur.fetchall()
        cur.close()
        print(res2)
        res["friend"] = res2
        # 需要匹配好account和对应的ip
        # 'rec_message'表示没在线时，其他用户发过来的消息

        #用来查找历史存留的消息

        sql4 = '''SELECT * FROM  Message WHERE  r_account = {}'''.format(data['account'])
        cur2 = conn.cursor()
        cur2.execute(sql4)
        # conn.row_factory = tools.sqlite_dict
        # 注意fetchone()只返回符合条件的查找的第一项，应该使用fetchall()将所有符合条件的全部查询出来
        res2 = cur2.fetchall()
        cur2.close()


        return {
            'function': 'login',
            'error_code': 0,
            'error_message': '',
            'data':res,
            'rec_message':res2
        },True

# 用户注册----该模块json文件测试完毕，没有问题
def register(conn,data):
    if tools.whether_miss(data,['username','password','mail','icon']):
        return{
            'function': 'register',
            'error_code': 3,
            'error_message': 'lose data!',
            'data': {}
        }
    if len(data['password']) != 32:
        return {
            'function': 'register',
            'error_code': 102,
            'error_message': 'password format error',
            'data': {}
        }
    account = tools.new_account()  # 后台生成一个账号
    # 首先判断邮箱是否重复
    sql = '''SELECT mail FROM User WHERE mail='{}' '''.format(data['mail'])
    cur = conn.cursor()
    cur.execute(sql)
    res = cur.fetchone()  # 去数据库中查询相应的数据
    if res is not  None:
        cur.close()
        return{
            'function': 'register',
            'error_code': 101,
            'error_message': 'The email is used!',
            'data': {}
        }
    else:
        # 测试成功，成功传递过来,并插入到数据库中
        sql2 ='''INSERT INTO User (state,account,password,username,icon,mail) VALUES('{}','{}','{}','{}','{}','{}')'''.format(1,account,data['password'],data['username'],data['icon'],data['mail'])
        cur.execute(sql2)
        conn.commit()
        cur.close()
        return {
            'function': 'register',
            'error_code': 0,
            'error_message': '',
            'data': {"account":account}
        }





# 退出后修改,将用户的登录状态改为0:
def exit_s(conn, data):
    if tools.whether_miss(data, ['account', 'password']):
        return
    sql = '''SELECT username ,icon FROM User WHERE account={}'''.format(data['account'])
    cur = conn.cursor()
    cur.execute(sql)
    res = cur.fetchone()  # 去数据库中查询相应的数据
    if res is None:
        cur.close()
    else:   # 存在对应的用户，需要将该用户的状态设置为0
        sql2 = '''UPDATE User set state = 0 WHERE account={}'''.format(data['account'])
        cur.execute(sql2)
        conn.commit()
        cur.close()  # 关闭数据库的连接


#conn是对数据库的连接的对象，data为前端传过来的信息
def search_user(conn,data):
    c=conn.cursor()
    #print(data['account'])
    sql="select username,account,icon from User where account like '{}%%%%'".format(data['account'])
    c.execute(sql)
    #将所有的数据据查询出来
    res=c.fetchall()
    print(res)
    #如果返回为空，则直接返回空就行
    if res.__len__()==0:
        response={
    "function":"search_user",
    "error_code": 301,
    "error_message":"查找结果为空",
    "data":None
       }
        return response
    else:
        user_response=list()
        for user in res:
            account=user['username']
            username=user['account']
            icon=user['icon']
            user_message={
                "username": account,
                "account":username ,
                "icon": icon,
            }
            user_response.append(user_message)
        response=    {
        "function":"search_user",
        "error_code": 0,
        "error_message":"",
        "data":
        {
            "result":user_response
        }
    }
        return response
##操作完成以后，需要修改好友表中的相应数据
def add_friend(conn,data,account):
    c=conn.cursor()
    user_request=account   #请求者的id
    print(account)
    user_friend=data['account']    #想要添加的用户的用户id
    print(user_friend)
    #首先对好友表进行查询，如果查询成功，则表明两个人已经是好友关系，则直接向请求端发送消息即可
    #如果查询失败，则表明两个还不是好友关系，则需要继续查询用户表，找到用户表中被添加者的信息，并且修改数据库
    sql="select * from Friend where account_1='{}' and account_2='{}'".format(user_friend,user_request)
    c.execute(sql)
    res=c.fetchone()
    print(res)
    if res!=None:
        return {
            "function": "add_friend",
            "error_code": 401,
            "error_message": "已经是好友",
            "data": None
        }
    else:
        #查询
        sql1="select username,account,icon,state from User where account={}".format(user_friend)
        c.execute(sql1)
        res1=c.fetchone()
        #如果查询不存在，则直接返回用户不存在的信息即可
        if res1==None:
            return {
                "function": "add_friend",
                "error_code": 402,
                "error_message": "账号不存在",
                "data": None
            }
        else:
            sql2 = "select username,account,icon,state from User where account={}".format(user_request)
            c.execute(sql2)
            res2 = c.fetchone()
        #表明已经查询成功，需要重新对数据库进行操作
            c.execute("insert into Friend values ('{}','{}')".format(user_request,user_friend))
            c.execute("insert into Friend values ('{}','{}')".format(user_friend,user_request))
            conn.commit()
            return {
                "function": "add_friend",
                "error_code": 0,
                "error_message": "",
                "data":
                    {
                        "account": res1['account'],
                        "username":res1['username'],
                        "icon": res1['icon'],
                        "state":res1['state']
                 }
            },{
                "function": "add_friend",
                "error_code": 0,
                "error_message": "",
                "data":
                    {
                        "account": res2['account'],
                        "username":res2['username'],
                        "icon": res2['icon'],
                        "state":res2['state']
                 }
            }

def delete_friend(conn,data,account):
    c = conn.cursor()
    user_request = account  # 请求者的id
    user_friend = data['account']  # 被删除的用户的用户id
    # 首先对好友表进行查询，如果查询成功，则进行删除操作
    # 如果查询失败，则表明两个人还不是好友关系，则直接返回错误信息
    sql = "select * from Friend where account_1={} and account_2={}".format(user_friend, user_request)
    c.execute(sql)
    res = c.fetchone()
    if res is None:
        return {
            "function": "delete_friend",
            "error_code": 501,
            "error_message": "该用户不是你的好友",
            "data": None
        }
    else:
            # 表明已经查询成功，需要重新对数据库进行操作
            c.execute("delete from Friend where account_1={} and account_2={}".format(user_request,user_friend))
            c.execute("delete from Friend where account_1={} and account_2={}".format(user_friend,user_request))
            conn.commit()
            return {
                    "function": "delete_friend",
                    "error_code": 0,
                    "error_message": "",
                    "data":
                        {
                            "account": user_request
                        }
                },{
                    "function": "delete_friend",
                    "error_code": 0,
                    "error_message": "",
                    "data":
                        {
                            "account": user_friend
                        }
                }






