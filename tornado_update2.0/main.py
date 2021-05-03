import tornado.web
import tornado.websocket
import tornado.httpserver
import tornado.ioloop
import json
import sqlite3
import user
import ms

# 数据库存放的统一路径 ./db/NEUIM.db


client = {}


# 保存连接用户，用于后续推送消息，登录后成功后即添加进去

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
    jm = None
    acc = None

    # 打开时就需要连接上数据库
    def open(self):
        if self.conn == None:
            self.conn = sqlite3.connect('./db/NEUIM.db')
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
        # login 成功后，需要将当前用户的保留在字典中
        if function == 'login':
            res, flag = user.login(self.conn, self.jm['data'])
            # flag 用来记录是否登录成功
            if flag == True:
                acc = self.jm['data']['account']
                client[acc] = self
                #将登录的用户传递过去、
        elif function == 'register':
            res = user.register(self.conn, self.jm['data'])
        elif function == 'message':
            res = ms.message(self.conn, self.jm['data'],client)
            rc = self.jm['data']['s_account']
            client[rc].write_message(res)
            # 实现对指定的客户端返回相应的消息
        # res = {
        #     'function': 'login',
        #     'error_code': 201,
        #     'error_message': 'account or password is error!',
        #     'data': {}
        # }
        self.write_message(res)

    def on_close(self):
        # 从client中删除已经下线的用户
        client.pop(self.acc)
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
