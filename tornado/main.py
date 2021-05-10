import tornado.web
import tornado.websocket
import tornado.httpserver
import tornado.ioloop
import json
import sqlite3
import user
import ms

# 数据库存放的统一路径 ./db/NEUIM.db

"""
User_Message 保存已经登录的用户的信息。用于在单人聊天的时候保存用户的信息，用户给客户端发送消息
保存所有用户的信息
"""
client={}

# 重写 SQLite dict 返回值
# 数据库默认以元组的形式返回，将其改写为以字典的形式返回内容
def dict_factory(cursor, row):
    d = {}
    for index, col in enumerate(cursor.description):
        d[col[0]] = row[index]
    return d


# 二级路由处理
class WebSocketHandler(tornado.websocket.WebSocketHandler):
    conn = None
    #保存所有的在线用户
    client = set()
    jm = None
    acc="1143550127"
    # 打开时就需要连接上数据库
    def open(self):
        self.client.add(self)
        if self.conn == None:
            self.conn = sqlite3.connect('D:/NEUIM.db')
            self.conn.row_factory = dict_factory

    def on_message(self, message):
        global res
        try:
            self.jm = json.loads(message)  # 读出客户端发送的json文件
            function = self.jm['function']  # 找到对应的功能字段

        except:
            self.write_message({
                'function': None,
                'error_code': 4,
                'error_message': 'json format error',
                'data': {}
            })
            return
        if function == 'login':
            res,flag= user.login(self.conn, self.jm['data'])
            self.acc=self.jm['data']['account']
            self.write_message(res)
            if flag==True:
                cur=self.conn.cursor()
                sql="selct *from toOther where account_1={}".format(self.acc)
                cur.execute(sql)
                mess=cur.fetchall() #形式是一条元组
                for per in mess:
                    message={
                            "function": per[2],
                            "error_code": 0,
                            "error_message": "",
                            "data":
                                {
                                    "account": per[1],
                                    "username": per[3],
                                    "icon": per[4],
                                    "state": per[5]
                                }
                            }
                    self.write_message(message)
                    client[self.acc] = {self}
            #key=self.jm['data']['account']
            #只需要存储每一个登录用户的user_id即可
        elif function == 'message':
            res = ms.message(self.conn, self.jm['data'], client)
            rc = self.jm['data']['s_account']
            client[rc].write_message(res)
            self.write_message(res)
        elif function == 'register':
            res = user.register(self.conn, self.jm['data'])
            self.write_message(res)    #想当于给自己推送消息
        elif function=='search_user':
            res=user.search_user(self.conn,self.jm['data'])
            self.write_message(res)
        elif function=='add_friend':
            #加好友首先需要向自己发送消息，然后向对方推送消息，如果对方不在线，消息将被缓存到数据库中，等用户登录自动推送
            #存储朋友的id
            friend_id=self.jm['data']['account']
            res=user.add_friend(self.conn, self.jm['data'], self.acc)
            if(isinstance(res,dict)):
                self.write_message(res)
            #给自己发消息
            else:
                 self.write_message(res[0])
                 # 向朋友端返回，在线情况
                 if(client.get(friend_id)!=None):
                        client[friend_id].write_message(res[1])
                 else:
                        cur=self.conn.cursor()
                        cur.execute("Insert into toOthers values ('{}','{}','{}','{}','{}','{}')".format(friend_id,
                                                                                            res[1]['data']['account'],
                                                                                           'add_friend',
                                                                                           res[1]['data']['username'],
                                                                                           res[1]['data']['icon'],
                                                                                           res[1]['data']['state']))
                        self.conn.commit()

            #需要判断不在线的情况，若是不在线，则直接缓存消息
        elif function=='delete_friend':
             res=user.delete_friend(self.conn, self.jm['data'], self.acc)
             if isinstance(res,dict):
                self.write_message(res)
             else:
                 self.write_message(res[1])
                 if(client[self.jm['data']['account']]==None):
                     client[self.jm['data']['account']].write_message(res[0])
                 else:
                    cur = self.conn.cursor()
                    cur.execute("insert into toOthers values ('{}','{}','{}','{}','{}')".format(self.jm['data']['account'],
                                                                                        res[1]['data'][ 'account'],
                                                                                         'delete_friend',
                                                                                         res[1]['data']['username'],
                                                                                         res[1]['data']['icon'],
                                                                                         res[1]['data']['state']))
                    self.conn.commit()

    def on_close(self):
            self.client.remove(self)
            key=self.acc
            client.pop(key)
            # 下线后，更改数据库状态state = 1
            if self.jm is not None:
                user.exit_s(self.conn, self.jm['data'])

    def check_origin(self, origin):
            return True
            #  允许进行跨域连接

    # 一级路由处理
class Application(tornado.web.Application):
    def __init__(self):
            handlers = [
                ("/main", WebSocketHandler)
            ]
            tornado.web.Application.__init__(self, handlers)

if __name__ == "__main__":
        app = Application()
        server = tornado.httpserver.HTTPServer(app)
        server.listen(8000)
        tornado.ioloop.IOLoop.current().start()

# nohup python3 -u main.py > /NEUIM/test.log 2>&1 &
# ps -aux | grep "python3 -u main.py"